using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Events;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Infrastructure.Models;
using System;

namespace StockTraderRI.Modules.Position
{
    public class ObservablePosition : IObservablePosition
    {
        private IAccountPositionService _accountPositionService;
        private IMarketFeedService _marketFeedService;

        public ObservableCollection<PositionSummaryItem> Items { get; private set; }

        public ObservablePosition(IAccountPositionService accountPositionService, IMarketFeedService marketFeedService, IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }

            Items = new ObservableCollection<PositionSummaryItem>();

            _accountPositionService = accountPositionService;
            _marketFeedService = marketFeedService;

            eventAggregator.GetEvent<MarketPricesUpdatedEvent>().Subscribe(MarketPricesUpdated, ThreadOption.UIThread);

            PopulateItems();

            _accountPositionService.Updated += PositionSummaryItems_Updated;
        }

        public void MarketPricesUpdated(IDictionary<string, decimal> tickerSymbolsPrice)
        {
            if (tickerSymbolsPrice == null)
            {
                throw new ArgumentNullException(nameof(tickerSymbolsPrice));
            }

            foreach (PositionSummaryItem position in Items)
            {
                if (tickerSymbolsPrice.ContainsKey(position.TickerSymbol))
                {
                    position.CurrentPrice = tickerSymbolsPrice[position.TickerSymbol];
                }
            }
        }

        private void PositionSummaryItems_Updated(object sender, AccountPositionModelEventArgs e)
        {
            if (e.AcctPosition != null)
            {
                PositionSummaryItem positionSummaryItem = Items.First(p => p.TickerSymbol == e.AcctPosition.TickerSymbol);

                if (positionSummaryItem != null)
                {
                    positionSummaryItem.Shares = e.AcctPosition.Shares;
                    positionSummaryItem.CostBasis = e.AcctPosition.CostBasis;
                }
            }
        }

        private void PopulateItems()
        {
            PositionSummaryItem positionSummaryItem;
            foreach (AccountPosition accountPosition in _accountPositionService.GetAccountPositions())
            {
                positionSummaryItem = new PositionSummaryItem(accountPosition.TickerSymbol, accountPosition.CostBasis, accountPosition.Shares, _marketFeedService.GetPrice(accountPosition.TickerSymbol));
                Items.Add(positionSummaryItem);
            }
        }
    }
}
