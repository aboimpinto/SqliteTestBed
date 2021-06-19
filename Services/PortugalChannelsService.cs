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
            if (currentChannelTV.Name.Contains("*"))
            {
                return;
            }

            var isNewChannel = false;
            var currentChannel = this._dbContext.Channels.SingleOrDefault(x => x.Name.ToUpper().Trim() == currentChannelTV.Id.ToUpper().Trim());

            if (currentChannel == null)
            {
                currentChannel = new Channel();
                currentChannel.Name = currentChannelTV.Id.ToUpper().Trim();
                currentChannel.Logo = currentChannelTV.Logo.Trim();
                currentChannel.Category = currentChannelTV.GroupTitle.ToUpper();
                isNewChannel = true;
            }
            
            var channelUrl = new ChannelUrl();
            channelUrl.ChannelQuality = currentChannelTV.ChannelQuality.ToString().Trim();
            channelUrl.Url = currentChannelTV.Path.Trim();
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
