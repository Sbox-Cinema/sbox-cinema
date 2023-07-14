using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;
using CinemaTeam.Plugins.Media;

namespace Cinema;

public partial class MediaQueue : EntityComponent<CinemaZone>, ISingletonComponent
{
    public partial class ScoredItem : BaseNetworkable
    {
        [Net]
        public MediaRequest Item { get; set; }
        [Net]
        public IDictionary<IClient, bool> PriorityVotes { get; set; }
    }

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

    [ConCmd.Server]
    public async static void Push(int zoneId, int clientId, int providerId, string query)
    {
        var queue = FindByZoneId(zoneId);
        var client = ClientHelper.FindById(clientId);
        var provider = VideoProviderManager.Instance[providerId];
        var request = await provider.CreateRequest(client, query);
        queue.Push(request);
    }

    public void Push(MediaRequest request)
    {
        Items.Add(new ScoredItem { Item = request });
    }

    public void AddPriorityVote(MediaRequest request, IClient client, bool isUpvote)
    {
        var item = Items.FirstOrDefault(i => i.Item == request);
        if (item == null)
            return;

        // If this client already voted, don't 
        if (item.PriorityVotes.ContainsKey(client) && item.PriorityVotes[client] == isUpvote)
            return;
        item.PriorityVotes[client] = isUpvote;
        var index = Items.IndexOf(item);
        // If we remove a true vote:
        // - Index goes down
        // - Priority goes up
        SwapItem(index, isUpvote ? -1 : 1);
    }

    public void RemovePriorityVote(MediaRequest request, IClient client)
    {
        var item = Items.FirstOrDefault(i => i.Item == request);
        // If no priority votes were cast, there's nothing to remove. 
        if (item != null && !item.PriorityVotes.ContainsKey(client))
            return;

        var voteValue = item.PriorityVotes[client];
        var index = Items.IndexOf(item);
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
        var offsetDirection = Math.Sign(offset);
        // Swap the specified item with the adjacent item in the direction
        // of the offset. Repeat until we've arrived at the desired offset.
        for (int i = 0; i != offset; i += offsetDirection)
        {
            // Find the index of the adjacent item we will swap with.
            var swapIndex = index + i * offsetDirection;
            // If we've reached the end of the queue, no need to swap further.
            if (swapIndex < 0 || swapIndex >= Items.Count)
                return;
            // Swap the items.
            (Items[swapIndex], Items[index]) = (Items[index], Items[swapIndex]);
        }
    }
}
