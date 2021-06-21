using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SqliteTestBed.DbServices;
using SqliteTestBed.Extensions;
using SqliteTestBed.Model;
using SqliteTestBed.Services;

namespace SqliteTestBed.Handlers
{
    public class M3UFileHandler
    {
        private const string fileName = "tv_channels_pauloaboim_plus.m3u";

        private List<TvChannelFile> Channels = new List<TvChannelFile>();
        private List<string> Categories = new List<string>();

        private IDictionary<string, IChannelGroupHandler> _handlers;

        public M3UFileHandler()
         {
             var dbContext = new ChannelsContext();

            this._handlers = new Dictionary<string, IChannelGroupHandler>
            {
                { "PORTUGAL", new PortugalChannelsService(dbContext) },
                { "PORTUGAL [LOW]", new PortugalLowChannelsService(dbContext) }
            }; 
         }

        public void ProcessFile()
        {
            using(var readerStream = File.OpenText("tv_channels_pauloaboim_plus.m3u"))
            {
                var lineStream = readerStream
                    .ToObservableUntilEndOfStream()
                    .Subscribe(ProcessM3ULine);
            }
        }

        private TvChannelFile _currentChannelTV;

        private void ProcessM3ULine(string m3uLine)
        {
            if (m3uLine.StartsWith("#EXTINF"))
            {
                this._currentChannelTV = ExtractExtendedChannelInformation(m3uLine);
                if (this._currentChannelTV == null)
                {
                    return;
                }

                Channels.Add(this._currentChannelTV);

                // Check Category
                if(!Categories.Any(x => x.ToUpper() == this._currentChannelTV.GroupTitle.ToUpper()))
                {
                    Categories.Add(this._currentChannelTV.GroupTitle.ToUpper());
                    Console.WriteLine($"--> {this._currentChannelTV.GroupTitle.ToUpper()}");
                }

                if (this._currentChannelTV.Name.Contains("*"))
                {
                    return;
                }
            }
            else if (m3uLine.StartsWith("https://"))
            {
                this._currentChannelTV.Path = m3uLine;

                var channelGroupHandler = _handlers.SingleOrDefault(x => x.Key == this._currentChannelTV.GroupTitle).Value;
                if (channelGroupHandler == null)
                {
                    return;
                }

                channelGroupHandler.Process(this._currentChannelTV);
            }
        }

        private TvChannelFile ExtractExtendedChannelInformation(string line)
        {
            var auxInformation = line.Substring(11, line.Length - 11);

            var channel = new TvChannelFile();

            var tvgId_Index = auxInformation.IndexOf("tvg-id");
            var tvgName_Index = auxInformation.IndexOf("tvg-name");
            var tvgLogo_Index = auxInformation.IndexOf("tvg-logo");
            var groupTitle_Index = auxInformation.IndexOf("group-title");

            var tvgId = auxInformation.Substring(tvgId_Index, tvgId_Index + tvgName_Index).Trim();
            channel.Id = this.ExtractAndSanitizeValue(tvgId);

            var tvgName = auxInformation.Substring(tvgName_Index, tvgLogo_Index - tvgName_Index).Trim();

            channel.Name = this.ExtractAndSanitizeValue(tvgName);

            if (string.IsNullOrEmpty(channel.Id))
            {
                channel.Id = channel.Name;
            }

            var tvgLogo = auxInformation.Substring(tvgLogo_Index, groupTitle_Index - tvgLogo_Index).Trim();
            channel.Logo = this.ExtractAndSanitizeValue(tvgLogo);

            var groupTitle = auxInformation.Substring(groupTitle_Index, auxInformation.Length - groupTitle_Index).Split(',')[0].Trim();
            channel.GroupTitle = this.ExtractAndSanitizeValue(groupTitle);

            return channel;
        }

        private string ExtractAndSanitizeValue(string value)
        {
            return value.Split('=')[1].Replace("\"", "");
        }
    }
}
