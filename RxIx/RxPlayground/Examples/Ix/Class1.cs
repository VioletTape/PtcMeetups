using System;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.Ix {
    public class TemplateProgressReader {
        public static void Solve(IProgress<int> progress) {
            for (var i = 0; i < 100; i++) {
                Thread.Sleep(100);
                progress.Report(i);
            }
        }

        public static async Task ReadFile(string url, IProgress<double> progressReporter) {
            double totalBytes = new FileInfo(url).Length;
            var bufferSize = 1024 * 4; //4k;
            var buffer = new byte[bufferSize];
            var totalBytesRead = 0L;

            using (var fs = File.OpenRead(url)) {
                var bytesRead = 0;
                do {
                    bytesRead = await fs.ReadAsync(buffer, 0, bufferSize);
                    //Do something here with the data that was just read.
                    totalBytesRead += bytesRead;
                    var fractionDone = totalBytesRead / totalBytes;
                    progressReporter.Report(fractionDone);
                } while (bytesRead > 0);
            }
        }
    }


    public static class ObservableProgress {
        public static IObservable<T> Create<T>(this Action<IProgress<T>> action) {
            return Observable.Create<T>(obs => {
                action(new DelegateProgress<T>(obs.OnNext));
                obs.OnCompleted();
                //No apparent cancellation support.
                return Disposable.Empty;
            });
        }

        public static IObservable<T> CreateAsync<T>(Func<IProgress<T>, Task> action) {
            return Observable.Create<T>(async obs => {
                                             await action(new DelegateProgress<T>(obs.OnNext));
                                             obs.OnCompleted();
                                             //No apparent cancellation support. Add an overload that accepts a CancellationToken instead
                                             return Disposable.Empty;
            });
        }

        private sealed class DelegateProgress<T> : IProgress<T> {
            private readonly Action<T> _report;

            public DelegateProgress(Action<T> report) {
                if (report == null) throw new ArgumentNullException();
                _report = report;
            }

            public void Report(T value) {
                _report(value);
            }
        }
    }
}