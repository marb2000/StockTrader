using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommonServiceLocator;
using Prism.Commands;
using Prism.Regions;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Infrastructure.Models;
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.Models;
using StockTraderRI.Modules.Position.ViewModels;
using StockTraderRI.Modules.Position.Properties;

namespace StockTraderRI.Modules.Position.Controllers
{
    public class OrdersController : IOrdersController
    {
        private IRegionManager _regionManager;
        private readonly StockTraderRICommandProxy _commandProxy;
        private IAccountPositionService _accountPositionService;

        public OrdersController(IRegionManager regionManager, 
                                StockTraderRICommandProxy commandProxy, 
                                IAccountPositionService accountPositionService)
        {
            if (commandProxy == null)
            {
                throw new ArgumentNullException("commandProxy");
            }

            _regionManager = regionManager;
            _accountPositionService = accountPositionService;
            _commandProxy = commandProxy;

            BuyCommand = new DelegateCommand<string>((a)=> StartOrder(a, TransactionType.Buy));
            SellCommand = new DelegateCommand<string>((a)=> StartOrder(a, TransactionType.Sell));
            SubmitAllVoteOnlyCommand = new DelegateCommand(() => { }, SubmitAllCanExecute);

            OrderModels = new List<IOrderCompositeViewModel>();
            commandProxy.SubmitAllOrdersCommand.RegisterCommand(SubmitAllVoteOnlyCommand);
        }

        virtual protected bool SubmitAllCanExecute()
        {
            Dictionary<string, long> sellOrderShares = new Dictionary<string, long>();

            if (OrderModels.Count == 0) return false;

            foreach (var order in OrderModels)
            {
                if (order.TransactionInfo.TransactionType == TransactionType.Sell)
                {
                    string tickerSymbol = order.TransactionInfo.TickerSymbol.ToUpper(CultureInfo.CurrentCulture);
                    if (!sellOrderShares.ContainsKey(tickerSymbol))
                        sellOrderShares.Add(tickerSymbol, 0);

                    //populate dictionary with total shares bought or sold by tickersymbol
                    sellOrderShares[tickerSymbol] += order.Shares;
                }
            }

            IList<AccountPosition> positions = _accountPositionService.GetAccountPositions();

            foreach (string key in sellOrderShares.Keys)
            {
                AccountPosition position =
                    positions.FirstOrDefault(
                        x => String.Compare(x.TickerSymbol, key, StringComparison.CurrentCultureIgnoreCase) == 0);
                if (position == null || position.Shares < sellOrderShares[key])
                {
                    //trying to sell more shares than we own
                    return false;
                }
            }

            return true;
        }

        virtual protected void StartOrder(string tickerSymbol, TransactionType transactionType)
        {
            if (string.IsNullOrEmpty(tickerSymbol))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeNullOrEmpty, "tickerSymbol"));
            }
            ShowOrdersView();

            IRegion ordersRegion = _regionManager.Regions[RegionNames.OrdersRegion];

            var orderCompositeViewModel = ServiceLocator.Current.GetInstance<IOrderCompositeViewModel>();

            orderCompositeViewModel.TransactionInfo = new TransactionInfo(tickerSymbol, transactionType);
            orderCompositeViewModel.CloseViewRequested += delegate
            {
                OrderModels.Remove(orderCompositeViewModel);
                _commandProxy.SubmitAllOrdersCommand.UnregisterCommand(orderCompositeViewModel.SubmitCommand);
                _commandProxy.CancelAllOrdersCommand.UnregisterCommand(orderCompositeViewModel.CancelCommand);
                _commandProxy.SubmitOrderCommand.UnregisterCommand(orderCompositeViewModel.SubmitCommand);
                _commandProxy.CancelOrderCommand.UnregisterCommand(orderCompositeViewModel.CancelCommand);
                ordersRegion.Remove(orderCompositeViewModel);
                if (ordersRegion.Views.Count() == 0)
                {
                    RemoveOrdersView();
                }
            };

            ordersRegion.Add(orderCompositeViewModel);
            OrderModels.Add(orderCompositeViewModel);

            _commandProxy.SubmitAllOrdersCommand.RegisterCommand(orderCompositeViewModel.SubmitCommand);
            _commandProxy.CancelAllOrdersCommand.RegisterCommand(orderCompositeViewModel.CancelCommand);
            _commandProxy.SubmitOrderCommand.RegisterCommand(orderCompositeViewModel.SubmitCommand);
            _commandProxy.CancelOrderCommand.RegisterCommand(orderCompositeViewModel.CancelCommand);

            ordersRegion.Activate(orderCompositeViewModel);
        }

        private void RemoveOrdersView()
        {
            IRegion region = this._regionManager.Regions[RegionNames.ActionRegion];

            object ordersView = region.GetView("Orders");
            if (ordersView != null)
            {
                region.Remove(ordersView);
            }
        }

        private void ShowOrdersView()
        {
            IRegion region = this._regionManager.Regions[RegionNames.ActionRegion];

            object ordersView = region.GetView("Orders");
            if (ordersView == null)
            {
                ordersView = ServiceLocator.Current.GetInstance<IOrders>();
                region.Add(ordersView, "Orders");
            }

            region.Activate(ordersView);
        }

        #region IOrdersController Members

        public DelegateCommand<string> BuyCommand { get; private set; }
        public DelegateCommand<string> SellCommand { get; private set; }
        public DelegateCommand SubmitAllVoteOnlyCommand { get; private set; }

        private List<IOrderCompositeViewModel> OrderModels { get; set; }

        #endregion
    }
}
