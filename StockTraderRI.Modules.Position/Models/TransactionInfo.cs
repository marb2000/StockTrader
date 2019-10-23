using Prism.Mvvm;
using StockTraderRI.Infrastructure;

namespace StockTraderRI.Modules.Position.Models
{
    public class TransactionInfo : BindableBase
    {
        private string _tickerSymbol;
        private TransactionType _transactionType;

        public TransactionInfo() { }

        public TransactionInfo(string tickerSymbol, TransactionType transactionType)
        {
            _tickerSymbol = tickerSymbol;
            _transactionType = transactionType;
        }

        public string TickerSymbol
        {
            get => _tickerSymbol;
            set => SetProperty(ref _tickerSymbol, value);
        }

        public TransactionType TransactionType
        {
            get => _transactionType;
            set => SetProperty(ref _transactionType, value);
        }
    }
}