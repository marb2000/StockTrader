using System;
using Prism.Commands;
using StockTraderRI.Infrastructure;
using StockTraderRI.Modules.Position.Models;

namespace StockTraderRI.Modules.Position.ViewModels
{
    public interface IOrderDetailsViewModel
    {
        event EventHandler CloseViewRequested;  // TODO consider interaction request

        TransactionInfo TransactionInfo { get; set; }

        TransactionType TransactionType { get; }

        string TickerSymbol { get; }

        int? Shares { get; }

        decimal? StopLimitPrice { get; }

        DelegateCommand SubmitCommand { get; }

        DelegateCommand CancelCommand { get; }
    }
}
