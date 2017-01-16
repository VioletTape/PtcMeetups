using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Examples.LogoutManagerTestable {
    public class LogoutManager {
        public IObservable<Unit> Logout { get; private set; }
        public IObserver<Unit> UserActionsObserver { get; private set; }
        public IObserver<Unit> LogoutCommandsObserver { get; private set; }

        public LogoutManager(TimeSpan timeout, IScheduler scheduler) {
            var userActionsSubject = new Subject<Unit>();
            var logoutSubject = new Subject<Unit>();

            UserActionsObserver = userActionsSubject.AsObserver();
            LogoutCommandsObserver = logoutSubject.AsObserver();

            Logout = userActionsSubject.StartWith(Unit.Default)
                                       .Throttle(timeout, scheduler)
                                       .Merge(logoutSubject);
        }

        public LogoutManager(TimeSpan timeout) : this(timeout, Scheduler.Default) {}
    }
}