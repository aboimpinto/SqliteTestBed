namespace SqliteTestBed.Model
{
    public class TvChannelFile
    {
        private string _name;

        public string Id { get; set; }

        public string Name 
        { 
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;

                if (this._name.Contains("[HD]"))
                {
                    this.ChannelQuality = ChannelQuality.HD;
                }
                else if(this.Name.Contains("[FHD]"))
                {
                    this.ChannelQuality = ChannelQuality.FHD;
                }
                else if(this._name.Contains("[H265/HEVC]"))
                {
                    this.ChannelQuality = ChannelQuality.H265;
                }
                else if(this._name.Contains("[LOW]"))
                {
                    this.ChannelQuality = ChannelQuality.Low;
                }
                else
                {
                    this.ChannelQuality = ChannelQuality.Normal;
                }
            }
        }

        public string Logo { get; set; }

        public string GroupTitle { get; set; }

        public string Path { get; set; }

        public string VideoParameters { get; set; }

        public ChannelQuality ChannelQuality { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
