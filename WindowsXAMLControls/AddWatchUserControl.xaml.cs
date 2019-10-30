using StockTraderRI.Modules.Watch.Services;
using StockTraderRI.Modules.Watch.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace WindowsXAMLControls
{
    public sealed partial class AddWatchUserControl : UserControl
    {

        public AddWatchViewModel ViewModel { get; set; }

        public AddWatchUserControl()
        {
            InitializeComponent();
        }

        private void OnKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ViewModel.AddWatchCommand.Execute(AddWatchTextBox.Text);
            }
        }
    }
}
