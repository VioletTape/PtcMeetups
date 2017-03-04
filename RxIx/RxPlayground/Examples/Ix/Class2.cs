using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Examples.Ix {
    public class Class2 {
        public void BackgroundWorker() {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;

            backgroundWorker.ProgressChanged 
                += (_, a) => Console.WriteLine(a.ProgressPercentage);

            backgroundWorker.DoWork += (_, __) => {
                for (var i = 0; i <= 100; i++) {
                    //do something
                    Task.Delay(10);
                    backgroundWorker.ReportProgress(i);
                }
            };
            backgroundWorker.RunWorkerAsync();
            
        }
    }
}