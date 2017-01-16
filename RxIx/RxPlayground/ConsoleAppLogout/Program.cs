using System;
using System.Reactive;
using Examples;

namespace ConsoleAppLogout {
    internal class Program {
        private static void Main(string[] args) {
            LogoutExample();
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
    }
}