using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;
using CinemaTeam.Plugins.Video;

namespace Cinema;

public partial class MediaQueue : EntityComponent<CinemaZone>, ISingletonComponent
{
    public partial class ScoredItem : BaseNetworkable, IComparable<ScoredItem>
    {
        [Net]
        public MediaRequest Item { get; set; }
        [Net]
        public int Id { get; set; }
        [Net]
        public int PriorityOffset { get; set; }
        [Net]
        public IDictionary<IClient, bool> PriorityVotes { get; set; }

        public void Vote(IClient client, bool isUpvote)
        {
            if (PriorityVotes.TryGetValue(client, out var existingVote))
            {
                if (existingVote == isUpvote)
                {
                    return;
                }
                PriorityVotes[client] = isUpvote;
                PriorityOffset += isUpvote ? 1 : -1;
            }
            else
            {
                PriorityVotes.Add(client, isUpvote);
                PriorityOffset += isUpvote ? 1 : -1;
            }
        }

        public int CompareTo(ScoredItem other)
        {
            return (PriorityOffset + Id).CompareTo(other.PriorityOffset + other.Id);
        }
    }

    [Net]
    public IList<ScoredItem> Items { get; set; }
    public MediaController Controller => Entity.MediaController;
    public IEnumerable<ScoredItem> GetAll() => Items.OrderBy(i => i);

    private static int NextId { get; set; } = 0;


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

    public MediaRequest Pop()
    {
        var item = Items.OrderBy(i => i).FirstOrDefault();

        if (item == null)
            return null;

        Items.Remove(item);
        return item.Item;
    }

    private static MediaQueue FindByZoneId(int zoneId)
    {
        var zone = Sandbox.Entity.FindByIndex(zoneId) as CinemaZone;
        return zone?.MediaQueue;
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
        Items.Add(new ScoredItem { Item = request, Id = NextId++ });
    }
}
