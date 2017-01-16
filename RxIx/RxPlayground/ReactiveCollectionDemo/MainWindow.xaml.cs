using System.Collections.Generic;
using System.Windows;
using Examples;
using Examples.ReactiveCollection_;

namespace ReactiveCollectionDemo {
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private string filter;
        private string[] source;
        private string[] view;
        public ReactiveCollection<string> RC { get; set; }

        public string Filter {
            get { return filter; }
            set {
                filter = value;
                RC.Filter = value;
            }
        }

        public string[] Source {
            get { return source; }
            set {
                source = value;
                RC.Source = value;
            }
        }

        public string[] Items => RC.View;


        public MainWindow() {
            InitializeComponent();
            RC = new ReactiveCollection<string>((element, filter) => element.StartsWith(filter));
            Source = new string[0];
            Filter = "";
            

            Source = new List<string> {
                                 "Aaron",
                                 "Andrey",
                                 "Bartosz",
                                 "Barnie",
                                 "Bastard",
                             }.ToArray();

            DataContext = this;
        }
    }
}