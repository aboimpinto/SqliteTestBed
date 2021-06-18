using System;

namespace SqliteTestBed.DbServices
{
    public class ChannelsDbService
    {
        private ChannelsContext _dbContext;

        private static ChannelsDbService _instance;

        public static ChannelsDbService Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new ChannelsDbService();
                }

                return _instance;
            }
        }

        public ChannelsDbService()
        {
            this._dbContext = new ChannelsContext();
        }
    }
}
