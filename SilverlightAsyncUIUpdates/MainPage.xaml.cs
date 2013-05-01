using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightAsyncUIUpdates
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            this.DataContext = new ViewModel();
            this.Loaded += MainPage_Loaded;
        }

        ViewModel ViewModel { get { return (ViewModel)DataContext; } }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            readyTextBlock.Visibility = System.Windows.Visibility.Visible;

            Test1();
            //Test2();
        }

        // This test shows what happens if Busy indication is interrupted and returned back in an event handler
        // (without returning the control to the SL)
        private void Test1()
        {
            ViewModel.IsBusy = true;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(5000);

                this.Dispatcher.BeginInvoke(() =>
                {
                    ViewModel.IsBusy = false;

                    Thread.Sleep(5000);

                    ViewModel.IsBusy = true;

                    doneTextBlock.Visibility = System.Windows.Visibility.Visible;
                });
            });
        }

        // This test shows what happens if Busy indication is turned on and off inside a single event handler
        private void Test2()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(100);

                this.Dispatcher.BeginInvoke(() =>
                {
                    ViewModel.IsBusy = true;

                    Thread.Sleep(5000);

                    ViewModel.IsBusy = false;

                    doneTextBlock.Visibility = System.Windows.Visibility.Visible;

                });
            });
        }

    }

    public class ViewModel : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;

        bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy == value) return;

                isBusy = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsBusy"));
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) 
        {
            var h = this.PropertyChanged;
            if (h == null) return;

            h(this, e);
        }
    }
}
