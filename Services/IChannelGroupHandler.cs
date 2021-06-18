using SqliteTestBed.Model;

namespace SqliteTestBed.Services
{
    public interface IChannelGroupHandler
    {
        void Process(TvChannelFile currentChannelTV);
    }
}