using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using StockTraderRI.Modules.Watch.Services;
using StockTraderRI.Infrastructure;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using Prism.Regions;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.WatchList.Properties;

namespace StockTraderRI.Modules.Watch.ViewModels
{
    public class WatchListViewModel : BindableBase
    {
        private readonly IMarketFeedService _marketFeedService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly ObservableCollection<string> _watchList;
        public ICommand RemoveWatchCommand { get; }
        private ObservableCollection<WatchItem> _watchListItems;
        public ObservableCollection<WatchItem> WatchListItems { get => _watchListItems; set => SetProperty(ref _watchListItems, value); }


        private WatchItem _currentWatchItem;
        public string HeaderInfo { get; set; }

        public WatchListViewModel(IWatchListService watchListService, IMarketFeedService marketFeedService, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            if (watchListService == null)
            {
                throw new ArgumentNullException(nameof(watchListService));
            }

            if (eventAggregator == null)
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }
            _marketFeedService = marketFeedService;
            _regionManager = regionManager;

            HeaderInfo = Resources.WatchListTitle;
            WatchListItems = new ObservableCollection<WatchItem>();
            
            _watchList = watchListService.RetrieveWatchList();
            _watchList.CollectionChanged += delegate { PopulateWatchItemsList(_watchList); };
            PopulateWatchItemsList(_watchList);

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<MarketPricesUpdatedEvent>().Subscribe(MarketPricesUpdated, ThreadOption.UIThread);

            RemoveWatchCommand = new DelegateCommand<string>(RemoveWatch);

            _watchListItems.CollectionChanged += WatchListItems_CollectionChanged;
        }


        public WatchItem CurrentWatchItem
        {
            get => _currentWatchItem;
            set
            {
                if (value != null)
                {
                    SetProperty(ref _currentWatchItem, value);
                    _eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Publish(_currentWatchItem.TickerSymbol);
                }
            }
        }
               
        private void MarketPricesUpdated(IDictionary<string, decimal> updatedPrices)
        {
            if (updatedPrices == null)
            {
                throw new ArgumentNullException(nameof(updatedPrices));
            }

            foreach (WatchItem watchItem in WatchListItems)
            {
                if (updatedPrices.ContainsKey(watchItem.TickerSymbol))
                {
                    watchItem.CurrentPrice = updatedPrices[watchItem.TickerSymbol];
                }
            }
        }

        private void RemoveWatch(string tickerSymbol)
        {
            _watchList.Remove(tickerSymbol);
        }

        private void PopulateWatchItemsList(IEnumerable<string> watchItemsList)
        {
            WatchListItems.Clear();
            foreach (string tickerSymbol in watchItemsList)
            {
                decimal? currentPrice;
                try
                {
                    currentPrice = _marketFeedService.GetPrice(tickerSymbol);
                }
                catch (ArgumentException)
                {
                    currentPrice = null;
                }

                WatchListItems.Add(new WatchItem(tickerSymbol, currentPrice));
            }
        }

        private void WatchListItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                _regionManager.Regions[RegionNames.MainRegion].RequestNavigate("WatchList");
            }
        }
    }
}
