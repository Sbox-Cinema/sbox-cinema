using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;
using CinemaTeam.Plugins.Media;

namespace Cinema;

public partial class MediaQueue : EntityComponent<CinemaZone>, ISingletonComponent
{
    [Net]
    public IList<ScoredItem> Items { get; set; }
    public MediaController Controller => Entity.MediaController;

    [GameEvent.Tick.Server]
    public void OnServerTick()
    {
        if (Controller == null)
            return;

        if (Controller.CurrentMedia == null)
        {
            var item = Pop();
            if (item != null)
            {
                Controller.PlayMedia(item);
            }
        }
    }

    protected int IndexOf(MediaRequest request)
        => Items.IndexOf(Items.FirstOrDefault(i => i.Item == request));

    private static MediaQueue FindByZoneId(int zoneId)
    {
        var zone = Sandbox.Entity.FindByIndex(zoneId) as CinemaZone;
        return zone?.MediaQueue;
    }

    public MediaRequest Pop()
    {
        var item = Items.FirstOrDefault();

        if (item == null)
            return null;

        Items.Remove(item);
        return item.Item;
    }

    public bool CanRemove(ScoredItem queueItem, IClient client)
    {
        // TODO: Make this a privileged action.
        return true;
    }

    [ConCmd.Server]
    public static void RemoveItem(int zoneId, int requestId, int clientId)
    {
        var zone = FindByZoneId(zoneId);
        var item = zone.Items.First(r => r.RequestId == requestId);
        var client = ClientHelper.FindById(clientId);
        zone.RemoveItem(item, client);
    }

    /// <summary>
    /// Remove the specified media request from the queue on behalf of the
    /// specified client. If called on the client, a request will be sent to
    /// the server to remove the item.
    /// </summary>
    /// <param name="request">The queued media that should be removed.</param>
    /// <param name="client">The client who requested the removal, of the media,
    /// or null if removal was requested by the server or some other absolute authority.</param>
    public void RemoveItem(ScoredItem queueItem, IClient client)
    {
        if (!CanRemove(queueItem, client))
        {
            Log.Info($"{Entity.Name}: Client {client} not authorized to remove item: {queueItem.RequestId}");
            return;
        }
        if (Game.IsClient)
        {
            RemoveItem(Entity.NetworkIdent, queueItem.RequestId, Game.LocalClient.NetworkIdent);
            return;
        }
        Items.RemoveAt(IndexOf(queueItem.Item));
    }

    [ConCmd.Server]
    public async static void Push(int zoneId, int clientId, int providerId, string query)
    {
        var queue = FindByZoneId(zoneId);
        var client = ClientHelper.FindById(clientId);
        Log.Info($"{client} - Push to zone {zoneId} queue by media provider {providerId}: {query}");
        var provider = VideoProviderManager.Instance[providerId];
        var request = await provider.CreateRequest(client, query);
        if (request.GenericInfo != null && request.GenericInfo.Thumbnail == null)
        {
            request.GenericInfo.Thumbnail = provider.ThumbnailPath;
        }
        queue.Push(request);
    }

    public void Push(MediaRequest request)
    {
        Items.Add(new ScoredItem { Item = request });
    }

    public bool CanAddPriorityVote(ScoredItem queueItem, IClient client, bool isUpvote)
    {
        if (queueItem == null || !Items.Contains(queueItem))
        {
            Log.Info($"{Entity.Name}: No item found matching request.");
            return false;
        }
        bool hasVotedOnItem = queueItem.PriorityVotes.ContainsKey(client);
        // If the client has no active priority vote for this request, they can add one.
        if (!hasVotedOnItem)
            return true;
        // If the client has already cast a priority vote for this request, they cannot
        // vote again with the same value.
        return queueItem.PriorityVotes[client] != isUpvote;
    }

    [ConCmd.Server]
    public static void AddPriorityVote(int zoneId, int clientId, int requestId, bool isUpvote)
    {
        var zone = FindByZoneId(zoneId);
        var client = ClientHelper.FindById(clientId);
        var item = zone.Items.First(r => r.RequestId == requestId);
        zone.AddPriorityVote(item, client, isUpvote);
    }

    public void AddPriorityVote(ScoredItem queueItem, IClient client, bool isUpvote)
    {
        if (!CanAddPriorityVote(queueItem, client, isUpvote))
            return;

        if (Game.IsClient)
        {
            AddPriorityVote(Entity.NetworkIdent, Game.LocalClient.NetworkIdent, queueItem.RequestId, isUpvote);
            return;
        }

        queueItem.PriorityVotes[client] = isUpvote;
        var index = Items.IndexOf(queueItem);
        // If we remove a true vote:
        // - Index goes down
        // - Priority goes up
        SwapItem(index, isUpvote ? -1 : 1);
        WriteNetworkData();
    }

    public void RemovePriorityVote(ScoredItem queueItem, IClient client)
    {
        // If no priority votes were cast, there's nothing to remove. 
        if (queueItem == null || !queueItem.PriorityVotes.ContainsKey(client))
            return;

        var voteValue = queueItem.PriorityVotes[client];
        var index = Items.IndexOf(queueItem);
        // If we remove a true vote:
        // - Index goes down
        // - Priority goes up
        SwapItem(index, voteValue ? -1 : 1);
    }

    /// <summary>
    /// Change index (and inversely, the priority) of queued media by the
    /// specified amount. Swaps the item at the specified index with
    /// adjacent items until it has been swapped by the desired amount.
    /// </summary>
    /// <param name="index">The index of the item to swap.</param>
    /// <param name="offset">The amount of places by which this item should be swapped.</param>
    private void SwapItem(int index, int offset)
    {
        if (index < 0 || index >= Items.Count)
        {
            Log.Error($"Invalid media queue index: {index}");
            return;
        }
        var newList = new List<ScoredItem>(Items);
        var offsetDirection = Math.Sign(offset);
        // Swap the specified item with the adjacent item in the direction
        // of the offset. Repeat until we've arrived at the desired offset.
        for (int i = 0; i != offset; i += offsetDirection)
        {
            // Find the index of the adjacent item we will swap with.
            var swapIndex = index + i * offsetDirection;
            // If we've reached the end of the queue, no need to swap further.
            if (swapIndex < 0 || swapIndex >= newList.Count)
                return;
            // Swap the items.
            (newList[index], newList[swapIndex]) = (newList[swapIndex], newList[index]);
        }
        Items = newList;
    }
}
