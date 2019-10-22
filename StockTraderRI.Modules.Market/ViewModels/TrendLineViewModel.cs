using System;
using Prism.Mvvm;
using Prism.Events;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Infrastructure.Models;
using StockTraderRI.Infrastructure;

namespace StockTraderRI.Modules.Market.ViewModels
{
    public class TrendLineViewModel : BindableBase
    {
        private readonly IMarketHistoryService _marketHistoryService;

        private MarketHistoryCollection _historyCollection;
        public MarketHistoryCollection HistoryCollection { get => _historyCollection; set => SetProperty(ref _historyCollection, value); }

        private string _tickerSymbol;
        public string TickerSymbol { get => _tickerSymbol; set => SetProperty(ref _tickerSymbol, value); }


        public TrendLineViewModel(IMarketHistoryService marketHistoryService, IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }

            this._marketHistoryService = marketHistoryService;
            eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Subscribe(this.TickerSymbolChanged);
        }

        public void TickerSymbolChanged(string newTickerSymbol)
        {
            MarketHistoryCollection newHistoryCollection = this._marketHistoryService.GetPriceHistory(newTickerSymbol);

            this.TickerSymbol = newTickerSymbol;
            this.HistoryCollection = newHistoryCollection;
        }
    }
}
