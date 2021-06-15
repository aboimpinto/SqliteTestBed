using System;

namespace SqliteTestBed.DbServices
{
    public class ChannelsDbService
    {
        private ChannelsContext _dbContext;

        public ChannelsDbService()
        {
            this._dbContext = new ChannelsContext();
        }
    }
}
