using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Examples {
    public class LogoutManager {
        public IObservable<Unit> Logout { get; private set; }
        public IObserver<Unit> UserActionsObserver { get; private set; }
        public IObserver<Unit> LogoutCommandsObserver { get; private set; }

        public LogoutManager(TimeSpan timeout) {
            var userActionsSubject = new Subject<Unit>();
            var logoutSubject = new Subject<Unit>();

            UserActionsObserver = userActionsSubject.AsObserver();
            LogoutCommandsObserver = logoutSubject.AsObserver();

            Logout = userActionsSubject
                    .StartWith(Unit.Default)
                    .Throttle(timeout)
                    .Merge(logoutSubject);
        }
    }
}