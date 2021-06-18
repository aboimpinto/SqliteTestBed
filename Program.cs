using SqliteTestBed.Handlers;

namespace SqliteTestBed
{
    class Program
    {
        static void Main(string[] args)
        {
            new M3UFileHandler().ProcessFile();
        }
    }
}
