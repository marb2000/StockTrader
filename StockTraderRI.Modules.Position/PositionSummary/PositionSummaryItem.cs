using Prism.Mvvm;

namespace StockTraderRI.Modules.Position
{
    public class PositionSummaryItem : BindableBase
    {
        public PositionSummaryItem(string tickerSymbol, decimal costBasis, long shares, decimal currentPrice)
        {
            TickerSymbol = tickerSymbol;
            CostBasis = costBasis;
            Shares = shares;
            CurrentPrice = currentPrice;
        }

        private string _tickerSymbol;
        public string TickerSymbol
        {
            get => _tickerSymbol;
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                SetProperty(ref _tickerSymbol, value);
            }
        }


        private decimal _costBasis;
        public decimal CostBasis
        {
            get => _costBasis;
            set
            {
                if (SetProperty(ref _costBasis, value))
                {
                    RaisePropertyChanged("GainLossPercent");
                }
            }
        }

        private long _shares;

        public long Shares
        {
            get => _shares;
            set
            {
                if (SetProperty(ref _shares, value))
                {
                    RaisePropertyChanged("MarketValue");
                    RaisePropertyChanged("GainLossPercent");
                }
            }
        }


        private decimal _currentPrice;

        public decimal CurrentPrice
        {
            get => _currentPrice;
            set
            {
                if (SetProperty(ref _currentPrice, value))
                {
                    RaisePropertyChanged("MarketValue");
                    RaisePropertyChanged("GainLossPercent");
                }
            }
        }

        public decimal MarketValue
        {
            get => _shares * _currentPrice;
        }

        public decimal GainLossPercent
        {
            get => (CurrentPrice * Shares - CostBasis) * 100 / CostBasis;
        }
    }
}