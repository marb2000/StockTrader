using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.Position.Controllers;
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.Services;
using StockTraderRI.Modules.Position.ViewModels;
using StockTraderRI.Modules.Position.Views;

namespace StockTraderRI.Modules.Position
{
    public class PositionModule : IModule
    {
        //private OrdersController _ordersController;

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.MainRegion,typeof(PositionSummary));
            regionManager.RegisterViewWithRegion(RegionNames.ResearchRegion, typeof(PositionPieChart));

            //_ordersController = containerProvider.Resolve<OrdersController>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAccountPositionService, AccountPositionService>();
            containerRegistry.RegisterSingleton<IOrdersService, XmlOrdersService>();
            
            containerRegistry.Register<IOrders, Views.Orders>();
            containerRegistry.RegisterSingleton<IOrdersController, OrdersController>();
            containerRegistry.Register<IObservablePosition, ObservablePosition>();

            containerRegistry.Register<IOrderCompositeViewModel, OrderCompositeViewModel>();
            containerRegistry.Register<IOrderDetailsViewModel, OrderDetailsViewModel>();
            containerRegistry.Register<IOrdersViewModel, OrdersViewModel>();

            containerRegistry.Register<IPositionSummaryViewModel, PositionSummaryViewModel>();
            containerRegistry.Register<IPositionPieChartViewModel, PositionPieChartViewModel>();

            ViewModelLocationProvider.Register(typeof(OrderComposite).ToString(), typeof(OrderCompositeViewModel));
            ViewModelLocationProvider.Register(typeof(Orders).ToString(), typeof(OrdersViewModel));
        }
    }
}
