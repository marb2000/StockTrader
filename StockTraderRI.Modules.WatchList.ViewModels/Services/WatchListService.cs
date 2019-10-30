using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Prism.Commands;
using StockTraderRI.Infrastructure.Interfaces;

namespace StockTraderRI.Modules.Watch.Services
{
    public class WatchListService : IWatchListService
    {
        private readonly IMarketFeedService _marketFeedService;

        private ObservableCollection<string> WatchItems { get; set; }
        public ICommand AddWatchCommand { get; set; }

        public WatchListService(IMarketFeedService marketFeedService)
        {
            _marketFeedService = marketFeedService;
            WatchItems = new ObservableCollection<string>();

            AddWatchCommand = new DelegateCommand<string>(AddWatch);
        }

        public ObservableCollection<string> RetrieveWatchList()
        {
            return WatchItems;
        }

        private void AddWatch(string tickerSymbol)
        {
            if (!string.IsNullOrEmpty(tickerSymbol))
            {
                string upperCasedTrimmedSymbol = tickerSymbol.ToUpper(CultureInfo.InvariantCulture).Trim();
                if (!WatchItems.Contains(upperCasedTrimmedSymbol))
                {
                    if (_marketFeedService.SymbolExists(upperCasedTrimmedSymbol))
                    {
                        WatchItems.Add(upperCasedTrimmedSymbol);
                    }
                }
            }
        }
    }
}
