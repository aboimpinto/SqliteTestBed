using System.Collections.Generic;

namespace SqliteTestBed.Model
{
    public class Channel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Logo { get; set; }

        public string Category { get; set; }

        public IList<ChannelUrl> Url { get; set; }

        public Channel()
        {
            this.Url = new List<ChannelUrl>();
        }
    }
}
