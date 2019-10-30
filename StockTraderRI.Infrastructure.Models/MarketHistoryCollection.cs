﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StockTraderRI.Infrastructure.Models
{
    public class MarketHistoryCollection : ObservableCollection<MarketHistoryItem>
    {
        public MarketHistoryCollection()
        {
        }

        public MarketHistoryCollection(IEnumerable<MarketHistoryItem> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            foreach (MarketHistoryItem marketHistoryItem in list)
            {
                Add(marketHistoryItem);
            }
        }
    }

    public class MarketHistoryItem
    {
        public DateTime DateTimeMarker { get; set; }

        public decimal Value { get; set; }
    }
}