using System;
using System.Reactive;
using System.Reactive.Linq;
using Examples;
using Examples.Ix;

namespace ConsoleAppLogout {
    internal class Program {
        private static void Main(string[] args) {
            ProgressFile();

            Console.ReadLine();
        }


        private static void LogoutExample() {
            var logoutManager = new LogoutManager(TimeSpan.FromSeconds(20));

            Console.WriteLine("> User logged in");

            var logout = false;
            logoutManager.Logout.Subscribe(_ => {
                logout = true;
                Console.WriteLine("> User loged out");
            });

            while (!logout) {
                Console.Write("Write something:");
                Console.ReadLine();
                logoutManager.UserActionsObserver.OnNext(Unit.Default);
            }
        }

        private static void BackgroundWorker() {
            new Class2().BackgroundWorker();
        }

        private static void ProgressSimple() {
            TemplateProgressReader.Solve(new Progress<int>(i => Console.Write(".")));
            Console.WriteLine("Done");
        }

        private static void ProgressFile() {
            var veryLargeFile = @"C:\Users\agord\Downloads\en_sql_server_2016_developer_x64_dvd_8777069.iso";
            ObservableProgress.CreateAsync<double>(
                progressReporter => TemplateProgressReader.ReadFile(veryLargeFile, progressReporter))
                              .Sample(TimeSpan.FromMilliseconds(250))
                              .Select(i => i.ToString("p2"))
                              .Subscribe(
                                         Console.WriteLine,
                                         () => Console.WriteLine("Done")
                                        );
        }
    }
}