using System;
using System.Linq;
using SqliteTestBed.DbServices;
using SqliteTestBed.Model;

namespace SqliteTestBed.Services
{
    public class PortugalLowChannelsService : IChannelGroupHandler
    {
        private readonly ChannelsContext _dbContext;

        public PortugalLowChannelsService(ChannelsContext dbContext)
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

            var currentChannelTVName = currentChannelTV.Id.ToUpper().Trim();
            var currentChannelTvQuality = currentChannelTV.ChannelQuality.ToString().Trim();

            if (currentChannelTV.Id.ToUpper().Trim().EndsWith("HD"))
            {
                var rawName = currentChannelTV.Id.ToUpper().Trim();
                var withoutHDName = currentChannelTV.Id.ToUpper().Trim().Substring(0, currentChannelTV.Id.ToUpper().Trim().Length - 2);

                Console.WriteLine($"{rawName} : {withoutHDName}");

                currentChannelTVName = withoutHDName.Trim();
            }

            var currentChannel = this._dbContext.Channels.SingleOrDefault(x => x.Name.ToUpper().Trim() == currentChannelTVName);

            if (currentChannel == null)
            {
                currentChannel = new Channel();
                currentChannel.Name = currentChannelTVName;
                currentChannel.Logo = currentChannelTV.Logo.Trim();
                currentChannel.Category = currentChannelTV.GroupTitle.ToUpper();
                isNewChannel = true;
            }
            
            var channelUrl = new ChannelUrl();
            channelUrl.ChannelQuality = currentChannelTvQuality;
            channelUrl.Url = currentChannelTV.Path.Trim();
            currentChannel.Url.Add(channelUrl);

            if (isNewChannel)
            {
                this._dbContext.Add<Channel>(currentChannel);
            }

            this._dbContext.SaveChanges();

            Console.WriteLine($"Processing PORTUGAL LOW: {currentChannel.Name} : {channelUrl.ChannelQuality}");
        }
    }
}
