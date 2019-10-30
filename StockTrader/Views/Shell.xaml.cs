using Microsoft.Toolkit.Wpf.UI.XamlHost;
using Prism.Unity;
using StockTraderRI.Modules.Market.Services;
using StockTraderRI.Modules.Watch.ViewModels;
using System.Windows;
using System;

namespace StockTraderRI.Views
{
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();
        }

        
        private void OnASBChanged(object sender, System.EventArgs e)
        {
            if (sender is WindowsXamlHost host)
            {
                if (host.Child is WindowsXAMLControls.AutoSuggestStocks xamlControl)
                {
                    var viewModel = (App.Current as PrismApplication).Container.Resolve(typeof(AddWatchViewModel));
                    xamlControl.ViewModel = viewModel as AddWatchViewModel;
                    xamlControl.MarketFeedService = (App.Current as PrismApplication).Container.Resolve(typeof(MarketFeedService)) as MarketFeedService;
                }
            }
        }
    }
}

