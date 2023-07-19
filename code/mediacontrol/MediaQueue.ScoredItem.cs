using CinemaTeam.Plugins.Media;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public partial class MediaQueue
{
    public partial class ScoredItem : BaseNetworkable, INetworkSerializer
    {
        public ScoredItem()
        {
            RequestId = _NextId++;
        }

        private static int _NextId = 0;

        [Net]
        public int RequestId { get; set; }
        [Net]
        public MediaRequest Item { get; set; }
        [Net]
        public IDictionary<IClient, bool> PriorityVotes { get; set; }

        public void Read(ref NetRead read)
        {
            RequestId = read.Read<int>();
            Item = read.ReadClass<MediaRequest>();
            var count = read.Read<int>();
            PriorityVotes = new Dictionary<IClient, bool>(count);
            for (var i = 0; i < count; i++)
            {
                var client = read.ReadClass<IClient>();
                var vote = read.Read<bool>();
                PriorityVotes.Add(client, vote);
            }
        }

        public void Write(NetWrite write)
        {
            write.Write(RequestId);
            write.Write(Item);
            write.Write(PriorityVotes.Count);
            foreach (var kvp in PriorityVotes)
            {
                write.Write(kvp.Key);
                write.Write(kvp.Value);
            }
        }
    }
}
