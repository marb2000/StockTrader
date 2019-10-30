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
using Prism.Mvvm;
using StockTraderRI.Modules.Watch.ViewModels;
using StockTraderRI.Modules.Watch.Services;
using StockTraderRI.Modules.Market.Services;

namespace WindowsXAMLControls
{
    public sealed partial class AutoSuggestStocks : UserControl
    {
        public AddWatchViewModel ViewModel { get; set; }
        public MarketFeedService MarketFeedService { get; set; }

        public AutoSuggestStocks()
        {
            this.InitializeComponent();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {

                var result = from str in MarketFeedService.TickerSymbols
                             where (str.Contains(sender.Text.ToUpper()))
                             select str;

                autoSuggestBox.ItemsSource = result;
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
    
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ViewModel.AddWatchCommand.Execute(args.SelectedItem.ToString()); ;
        }
    }
}
