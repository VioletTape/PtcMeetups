using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Examples.ReactiveCollection_;
using XDel.Annotations;

namespace Examples.ReactiveCollectionTestable {
    public interface IReactiveCollection<TItem> : IDisposable, INotifyPropertyChanged {
        //  Data source  (full data set ) 
        TItem[] Source { get; set; }
        //  Data to display  (filtered data set) 
        TItem[] View { get; }

        string Filter { get; set; }
    }

    public class ReactiveCollection<TItem> : IReactiveCollection<TItem> {
        private readonly IDisposable subscription;
        private TItem[] source;
        private TItem[] view;
        private string filter;

        public ReactiveCollection(Func<TItem, string, bool> filterFunc)
            : this(filterFunc, NewThreadScheduler.Default, DispatcherScheduler.Current, Scheduler.Default) {}

        public ReactiveCollection(Func<TItem, string, bool> filterFunc, IScheduler scheduler)
            : this(filterFunc, scheduler, scheduler, scheduler) {}

        public ReactiveCollection(Func<TItem, string, bool> filterFunc, IScheduler filterScheduler, IScheduler observeOn, IScheduler timeScheduler) {
            view = new TItem[0];

            var buffer = new ObservableCollection<TItem>();

            subscription = PropertyChangedAsObservable(x => x.Filter)
                    .Throttle(TimeSpan.FromMilliseconds(200), timeScheduler)
                    .Where(_ => string.IsNullOrWhiteSpace(Filter) || Filter.Length > 2)
                    .DistinctUntilChanged(_ => Filter)
                    .Merge(PropertyChangedAsObservable(x => x.Source))
                    .Do(_ => buffer.Clear())
                    .Select(_ => Source.EmptyIfNull()
                                       .ToObservable(filterScheduler)
                                       .Where(x => filterFunc(x, Filter))
                                       .Buffer(TimeSpan.FromMilliseconds(200), timeScheduler)
                                       .Do(x => x.ToList().ForEach(buffer.Add)))
                    .Switch()
                    .ObserveOn(observeOn)
                    .Subscribe(_ => View = buffer.ToArray());
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose() {
            subscription.Dispose();
        }

        public TItem[] Source {
            get { return source; }
            set {
                source = value;
                OnPropertyChanged();
            }
        }

        public TItem[] View {
            get { return view; }
            set {
                view = value;
                OnPropertyChanged();
            }
        }

        public string Filter {
            get { return filter; }
            set {
                filter = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IObservable<Unit> PropertyChangedAsObservable<TProperty>(Expression<Func<IReactiveCollection<TItem>, TProperty>> property) {
            var member = ((MemberExpression) property.Body).Member;

            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                                                                                      h => PropertyChanged += h,
                                                                                                      h => PropertyChanged -= h)
                             .Where(_ => string.CompareOrdinal(_.EventArgs.PropertyName, member.Name) == 0)
                             .Select(_ => Unit.Default);
        }
    }
}