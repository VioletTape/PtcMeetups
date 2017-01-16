using System;
using System.Reactive;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace Examples.LogoutManagerTestable {
    [TestFixture]
    public class Tests {
        [Test]
        public void OnInactivity_Logout() {
            // arrange 
            var testScheduler = new TestScheduler();
            var manager = new LogoutManager(TimeSpan.FromMinutes(5), testScheduler);

            // assert 
            manager.Logout.Subscribe(_ => Assert.Pass());

            // act 
            testScheduler.AdvanceBy(TimeSpan.FromMinutes(6).Ticks);

            // fallback 
            Assert.Fail();
        }

        [Test]
        public void OnActivityWithinThreshold_DoNotLogout() {
            // arrange 
            var testScheduler = new TestScheduler();
            var manager = new LogoutManager(TimeSpan.FromMinutes(5), testScheduler);

            // assert 
            manager.Logout.Subscribe(_ => Assert.Fail());

            // act
            for (var i = 0; i < 10; i++) {
                manager.UserActionsObserver.OnNext(Unit.Default);
                testScheduler.AdvanceBy(TimeSpan.FromMinutes(3).Ticks);
            }
        }

        [Test]
        public void OnActivityCommand_LogoutImmediately() {
            // arrange 
            var manager = new LogoutManager(TimeSpan.FromMinutes(5));

            // assert 
            manager.Logout.Subscribe(_ => Assert.Pass());

            // act 
            manager.LogoutCommandsObserver.OnNext(Unit.Default);

            // fallback
            Assert.Fail();
        }
    }
}