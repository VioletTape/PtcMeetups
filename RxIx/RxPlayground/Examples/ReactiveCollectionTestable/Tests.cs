using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace Examples.ReactiveCollectionTestable {
    [TestFixture]
    public class Tests {
        [Test]
        public void OnFilter_FilterView() {
            var testScheduler = new TestScheduler();
            var collection = new ReactiveCollection<string>(
                                                            (x, y) => x.Contains(y), testScheduler) {
                                                                                                        Source = new[] {"client", "bad  client", "item", "second  item"}
                                                                                                    };
            collection.Filter = "item";
            testScheduler.Start();
            Assert.IsTrue(new[] {"item", "second  item"}.SequenceEqual(collection.View));
        }

        [Test]
        public void OnFilter_RunFiltrationInAnotherThread() {
            var testScheduler = new TestScheduler();
            var filterThreadId = 0;
            var collection = new ReactiveCollection<int>((x, y) => {
                filterThreadId = Thread.CurrentThread.ManagedThreadId;
                return true;
            }, NewThreadScheduler.Default, testScheduler, testScheduler);

            collection.Source = new int[10];
            testScheduler.Start();

            Assert.AreNotEqual(Thread.CurrentThread.ManagedThreadId, filterThreadId);
        }

        [Test]
        public void OnSlowFilter_BufferResults() {
            var testScheduler = new TestScheduler();
            var filterCounter = 0;
            var collection = new ReactiveCollection<int>((x, y) => {
                if (++filterCounter % 3 == 0)
                    testScheduler.Sleep(TimeSpan.FromMilliseconds(250).Ticks);
                return true;
            }, testScheduler);

            collection.Source = new int[10];
            collection.PropertyChangedAsObservable(x => x.View)
                      .Take(3)
                      .Subscribe(x => Assert.IsTrue(collection.View.Length % 3 == 0));

            testScheduler.Start();

            Assert.AreEqual(10, collection.View.Length);
        }

        [Test]
        public void OnSourceChanged_CallFilter_EvenWithinThreshold() {
            var testScheduler = new TestScheduler();
            var filterCounter = 0;
            var collection = new ReactiveCollection<int>((x, y) => {
                filterCounter++;
                return true;
            }, testScheduler);

            for (var i = 0; i < 10; i++) {
                collection.Source = new int[1];
                testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100).Ticks);
            }
            Assert.AreEqual(10, filterCounter);
        }

        [Test]
        public void OnSameFilter_DoNotCallFilter() {
            var testScheduler = new TestScheduler();
            var filterCounter = 0;

            var collection = new ReactiveCollection<int>((x, y) => {
                filterCounter++;
                return true;
            }, testScheduler);

            collection.Source = new int[1];
            collection.Filter = "firstFilter";
            testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(250).Ticks);
            filterCounter = 0;

            for (var i = 0; i < 10; i++) {
                collection.Filter = "secondFilter";
                testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(150).Ticks);
                collection.Filter = "firstFilter";
                testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(250).Ticks);
            }
            Assert.AreEqual(0, filterCounter);
        }

        [TestCase(100, false)]
        [TestCase(150, false)]
        [TestCase(250, true)]
        [TestCase(500, true)]
        public void OnMultipleChangesWithinThreshold_DoNotCallFilter(
            int threshold, bool runFilter) {
            var testScheduler = new TestScheduler();
            var filterCounter = 0;
            var collection = new ReactiveCollection<int>((x, y) => {
                                                                        filterCounter++;
                                                                        return true;
                                                                    }, testScheduler);

            collection.Source = new int[1];
            testScheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);
            filterCounter = 0;
            for (var i = 0; i < 10; i++) {
                collection.Filter += "filter";
                testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(threshold).Ticks);
            }
            if (runFilter)
                Assert.AreNotEqual(0, filterCounter);
            else
                Assert.AreEqual(0, filterCounter);
        }
    }
}