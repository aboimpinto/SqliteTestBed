using System;
using System.Collections.Generic;
using System.Linq;
using SqliteTestBed.DbServices;
using SqliteTestBed.Model;

namespace SqliteTestBed.Services
{
    public class PortugalChannelsService : IChannelGroupHandler
    {
        private readonly ChannelsContext _dbContext;
        private List<Channel> _channelList = new List<Channel>();

        public PortugalChannelsService(ChannelsContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public void Process(TvChannelFile currentChannelTV)
        {
            var isNewChannel = false;
            // var currentChannel = this._channelList.SingleOrDefault(x => x.Name == currentChannelTV.Id);
            var currentChannel = this._dbContext.Channels.SingleOrDefault(x => x.Name == currentChannelTV.Id);

            if (currentChannel == null)
            {
                currentChannel = new Channel();
                currentChannel.Name = currentChannelTV.Id;
                currentChannel.Logo = currentChannelTV.Logo;
                currentChannel.Category = currentChannelTV.GroupTitle.ToUpper();
                isNewChannel = true;
            }
            
            var channelUrl = new ChannelUrl();
            channelUrl.ChannelQuality = currentChannelTV.ChannelQuality.ToString();
            channelUrl.Url = currentChannelTV.Path;
            currentChannel.Url.Add(channelUrl);

            if (isNewChannel)
            {
                this._channelList.Add(currentChannel);
                
                this._dbContext.Add<Channel>(currentChannel);
            }

            this._dbContext.SaveChanges();

            Console.WriteLine($"Processing: {currentChannel.Name} : {channelUrl.ChannelQuality}");
        }
    }
}
