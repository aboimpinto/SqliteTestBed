using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using SqliteTestBed.DbServices;
using SqliteTestBed.Extensions;
using SqliteTestBed.Model;
using SqliteTestBed.Services;

namespace SqliteTestBed
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var readerStream = File.OpenText("tv_channels_pauloaboim_plus.m3u"))
            {
                var lineStream = readerStream
                    .ToObservableUntilEndOfStream()
                    .Subscribe(ProcessM3ULine);
            }

            // using (var db = new ChannelsContext())
            // {
                

                // db.Add(new Category { Name="Desporto" });
                // db.Add(new Category { Name="Documentários" });

                // db.SaveChanges();

                // var channels = db.Categories
                //     .OrderBy(x => x.Name);
                
                // foreach(var channel in channels)
                // {
                //     Console.WriteLine($"-> {channel.Name}");
                // }
            // }
        }

        private static List<TvChannelFile> Channels = new List<TvChannelFile>();
        private static List<string> Categories = new List<string>();
        private static List<Channel> ChannelList = new List<Channel>();

        private static IDictionary<string, IChannelGroupHandler> _handlers = new Dictionary<string, IChannelGroupHandler>
        {
            { "PORTUGAL", new PortugalChannelsService(ChannelsDbService.Instance) }
        };

        private static void ProcessM3ULine(string m3uLine)
        {
            if (m3uLine.StartsWith("#EXTINF"))
            {
                var currentChannelTV = ExtractExtendedChannelInformation(m3uLine);
                if (currentChannelTV == null)
                {
                    return;
                }

                Channels.Add(currentChannelTV);

                // Check Category
                if(!Categories.Any(x => x.ToUpper() == currentChannelTV.GroupTitle.ToUpper()))
                {
                    Categories.Add(currentChannelTV.GroupTitle.ToUpper());
                    Console.WriteLine($"--> {currentChannelTV.GroupTitle.ToUpper()}");
                }

                if (currentChannelTV.Name.Contains("*"))
                {
                    return;
                }

                Console.WriteLine($"Processing: {currentChannelTV.GroupTitle}:{currentChannelTV.Id}:{currentChannelTV.ChannelQuality}");

                var channelGroupHandler = _handlers.SingleOrDefault(x => x.Key == currentChannelTV.GroupTitle).Value;
                if (channelGroupHandler == null)
                {
                    return;
                }

                channelGroupHandler.Process(currentChannelTV);

                // // Check if Channel added
                // var isNewChannel = false;
                // var currentChannel = ChannelList.SingleOrDefault(x => x.Name == currentChannelTV.Id);
                // if (currentChannel == null)
                // {
                //     currentChannel = new Channel();
                //     currentChannel.Name = currentChannelTV.Id;
                //     currentChannel.Logo = currentChannelTV.Logo;
                //     currentChannel.Category = currentChannelTV.GroupTitle.ToUpper();
                //     isNewChannel = true;
                // }
                
                // var channelUrl = new ChannelUrl();
                // channelUrl.ChannelQuality = currentChannelTV.ChannelQuality.ToString();
                // channelUrl.Url = currentChannelTV.Path;
                // currentChannel.Url.Add(channelUrl);

                // if (isNewChannel)
                // {
                //     ChannelList.Add(currentChannel);
                // }
                
                // if (currentChannelTV.Name.Contains('*'))
                // {
                    Console.WriteLine(m3uLine);
                // }
            }

            
        }

        private static TvChannelFile ExtractExtendedChannelInformation(string line)
        {
            var auxInformation = line.Substring(11, line.Length - 11);

            var channel = new TvChannelFile();

            var tvgId_Index = auxInformation.IndexOf("tvg-id");
            var tvgName_Index = auxInformation.IndexOf("tvg-name");
            var tvgLogo_Index = auxInformation.IndexOf("tvg-logo");
            var groupTitle_Index = auxInformation.IndexOf("group-title");

            var tvgId = auxInformation.Substring(tvgId_Index, tvgId_Index + tvgName_Index).Trim();
            channel.Id = ExtractAndSanitizeValue(tvgId);

            var tvgName = auxInformation.Substring(tvgName_Index, tvgLogo_Index - tvgName_Index).Trim();

            var tvgNameArray = tvgName.Split(':');
            if (tvgNameArray.Length == 1)
            {
                channel.Name = ExtractAndSanitizeValue(tvgName);
            }
            else
            {
                channel.Name = tvgNameArray[0].Trim();
                channel.VideoParameters = tvgNameArray[1].Trim();
            }

            if (string.IsNullOrEmpty(channel.Id))
            {
                channel.Id = channel.Name;
            }

            var tvgLogo = auxInformation.Substring(tvgLogo_Index, groupTitle_Index - tvgLogo_Index).Trim();
            channel.Logo = ExtractAndSanitizeValue(tvgLogo);

            var groupTitle = auxInformation.Substring(groupTitle_Index, auxInformation.Length - groupTitle_Index).Split(',')[0].Trim();
            channel.GroupTitle = ExtractAndSanitizeValue(groupTitle);

            return channel;
        }

        private static string ExtractAndSanitizeValue(string value)
        {
            return value.Split('=')[1].Replace("\"", "");
        }
    }
}
