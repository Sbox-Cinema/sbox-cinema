﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;
using CinemaTeam.Plugins.Media;
using System.Collections.ObjectModel;

namespace Cinema;

public partial class MediaQueue : EntityComponent<CinemaZone>, ISingletonComponent, INetworkSerializer
{
    private List<ScoredItem> Items { get; init; } = new();
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

    public int Count => Items.Count;
    public IEnumerable<ScoredItem> GetItems() => Items;
    public ScoredItem GetItem(int index) => Items[index];
    public int IndexOf(MediaRequest request)
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
        WriteNetworkData();
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
        var queue = FindByZoneId(zoneId);
        var item = queue.Items.First(r => r.RequestId == requestId);
        var client = ClientHelper.FindById(clientId);
        queue.RemoveItem(item, client);
    }

    /// <summary>
    /// Remove the specified media request from the queue on behalf of the
    /// specified client. If called on the client, a request will be sent to
    /// the server to remove the item.
    /// </summary>
    /// <param name="queueItem">The queued media that should be removed.</param>
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
        WriteNetworkData();
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
        WriteNetworkData();
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
        queueItem.WriteNetworkData();
        var index = Items.IndexOf(queueItem);
        // If we remove a true vote:
        // - Index goes down
        // - Priority goes up
        SwapItem(index, isUpvote ? -1 : 1);
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
        var offsetDirection = Math.Sign(offset);
        Log.Info("Print previous list:");
        foreach(var item in Items)
        {
            Log.Info($"\t{item.RequestId} - {item.Item.GenericInfo.Title}");
        }
        // Swap the specified item with the adjacent item in the direction
        // of the offset. Repeat until we've arrived at the desired offset.
        for (int i = 0; i != offset; i += offsetDirection)
        {
            // Find the index of the adjacent item we will swap with.
            var swapIndex = index + offsetDirection + i * offsetDirection;
            Log.Info($"Swapping indices from {index} to {swapIndex}");
            // If we've reached the end of the queue, no need to swap further.
            if (swapIndex < 0 || swapIndex >= Items.Count)
                return;
            // Swap the items.
            var temp = Items[index];
            Items[index] = Items[swapIndex];
            Items[swapIndex] = temp;
        }
        Log.Info("Print new list:");
        foreach(var item in Items)
        {
            Log.Info($"\t{item.RequestId} - {item.Item.GenericInfo.Title}");
        }
        WriteNetworkData();
    }

    public void Read(ref NetRead read)
    {
        Items.Clear();
        var count = read.Read<int>();
        for (int i = 0; i < count; i++)
        {
            var item = read.ReadClass<ScoredItem>();
            Items.Add(item);
        }
    }

    public void Write(NetWrite write)
    {
        write.Write(Items.Count);
        foreach(var item in Items)
        {
            write.Write(item);
        }
    }
}
