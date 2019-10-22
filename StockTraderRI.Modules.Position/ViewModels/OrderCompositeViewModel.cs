using System;
using System.Windows;
using System.Windows.Input;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.Position.Models;

namespace StockTraderRI.Modules.Position.ViewModels
{
    public partial class OrderCompositeViewModel : DependencyObject, IOrderCompositeViewModel, IHeaderInfoProvider<string>
    {
        private readonly IOrderDetailsViewModel orderDetailsViewModel;

        public static readonly DependencyProperty HeaderInfoProperty =
            DependencyProperty.Register("HeaderInfo", typeof(string), typeof(OrderCompositeViewModel), null);

        public OrderCompositeViewModel(IOrderDetailsViewModel orderDetailsViewModel)
        {
            if (orderDetailsViewModel == null)
            {
                throw new ArgumentNullException("orderDetailsViewModel");
            }

            this.orderDetailsViewModel = orderDetailsViewModel;
            this.orderDetailsViewModel.CloseViewRequested += (sender, e) => CloseViewRequested(sender, e);
        }

        partial void SetTransactionInfo(TransactionInfo transactionInfo);

        public event EventHandler CloseViewRequested = delegate { };

        public TransactionInfo TransactionInfo
        {
            get => orderDetailsViewModel.TransactionInfo; 
            set => SetTransactionInfo(value);
        }

        public ICommand SubmitCommand
        {
            get => orderDetailsViewModel.SubmitCommand; 
        }

        public ICommand CancelCommand
        {
            get  => orderDetailsViewModel.CancelCommand;
        }

        public int Shares
        {
            get  => orderDetailsViewModel.Shares ?? 0; 
        }

        public object OrderDetails
        {
            get  => orderDetailsViewModel;
        }

    }
}