using System;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace SqliteTestBed.Extensions
{
    public static class StreamReaderExtensions 
    {
        public static IObservable<string> ToObservableUntilEndOfStream(this StreamReader reader)
        {
            return Observable.Create<string>(line => 
            {
                try
                {
                    while (true)
                    {
                        var lineRead = reader.ReadLine();
                        if (lineRead == null)
                        {
                            break;
                        }

                        line.OnNext(lineRead);
                    }
                }
                catch(Exception ex)
                {
                    line.OnError(ex);
                }
                finally
                {
                    line.OnCompleted();
                }

                return Disposable.Empty;
            });
        }
    }
}
