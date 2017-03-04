using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using XDel.Annotations;

namespace Examples.ReactiveCollection_ {
    public interface IReactiveCollection<TItem> : IDisposable, INotifyPropertyChanged {
        //  Data source  (full data set ) 
        TItem[] Source { get; set; }
        //  Data to display  (filtered data set) 
        TItem[] View { get; }

        string Filter { get; set; }
    }





















    public class ReactiveCollection<TItem> : IReactiveCollection<TItem> {
        private TItem[] source;
        private TItem[] view;
        private string filter;
        private readonly IDisposable subscription;

        public ReactiveCollection(Func<TItem, string, bool> filterFunc) {
            view = new TItem[0];

            var buffer = new ObservableCollection<TItem>();

            subscription = PropertyChangedAsObservable(x => x.Filter)
                    .Throttle(TimeSpan.FromMilliseconds(200))
                    .Where(_ => string.IsNullOrWhiteSpace(Filter) || Filter.Length > 2)
                    .DistinctUntilChanged(_ => Filter)
                    .Merge(PropertyChangedAsObservable(x => x.Source))
                    .Do(_ => buffer.Clear())
                    .Select(_ => Source.EmptyIfNull()
                                       .ToObservable()
                                       .Where(x => filterFunc(x, Filter))
                                       .Buffer(TimeSpan.FromMilliseconds(200))
                                       .Do(x => x.ToList().ForEach(buffer.Add)))
                    .Switch()
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

    public static class ArrayExtensions {
        public static T[] EmptyIfNull<T>(this T[] array) {
            return array ?? new T[0];
        }
    }
}