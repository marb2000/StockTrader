using System;
using System.Windows;
using System.Windows.Input;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.Position.Models;

namespace StockTraderRI.Modules.Position.ViewModels
{
    public partial class OrderCompositeViewModel : DependencyObject, IHeaderInfoProvider<string> 
    {
        private readonly OrderDetailsViewModel _orderDetailsViewModel;

        public static readonly DependencyProperty HeaderInfoProperty =
            DependencyProperty.Register("HeaderInfo", typeof(string), typeof(OrderCompositeViewModel), null);

        public OrderCompositeViewModel(OrderDetailsViewModel orderDetailsViewModel)
        {
            if (orderDetailsViewModel == null)
            {
                throw new ArgumentNullException(nameof(orderDetailsViewModel));
            }

            _orderDetailsViewModel = orderDetailsViewModel;
            _orderDetailsViewModel.CloseViewRequested += OnCloseViewRequested;
        }
  
        partial void SetTransactionInfo(TransactionInfo transactionInfo);

        private void OnCloseViewRequested(object sender, EventArgs e)
        {
            CloseViewRequested(sender, e);
        }

        public event EventHandler CloseViewRequested = delegate { };

        public TransactionInfo TransactionInfo
        {
            get => _orderDetailsViewModel.TransactionInfo; 
            set => SetTransactionInfo(value);
        }

        public ICommand SubmitCommand => _orderDetailsViewModel.SubmitCommand;

        public ICommand CancelCommand => _orderDetailsViewModel.CancelCommand;

        public int Shares => _orderDetailsViewModel.Shares ?? 0;

        public object OrderDetails => _orderDetailsViewModel;
    }
}