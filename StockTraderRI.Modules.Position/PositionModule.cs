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
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();

            //MEF: [ViewExport(RegionName = RegionNames.MainRegion)][PartCreationPolicy(CreationPolicy.NonShared)]
            regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(PositionSummary));

            //MEF: [ViewExport(RegionName = RegionNames.ResearchRegion)][PartCreationPolicy(CreationPolicy.NonShared)]
            regionManager.RegisterViewWithRegion(RegionNames.ResearchRegion, typeof(PositionPieChart));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAccountPositionService, AccountPositionService>();
            containerRegistry.RegisterSingleton<IOrdersService, XmlOrdersService>();

            //MEF: [Export(typeof(IOrdersView))][PartCreationPolicy(CreationPolicy.NonShared)]
            containerRegistry.Register<IOrders, Views.Orders>();

            //MEF: [Export(typeof(IOrdersController))] [PartCreationPolicy(CreationPolicy.Shared)]
            containerRegistry.RegisterSingleton<IOrdersController, OrdersController>();

            //MEF: [Export(typeof(IObservablePosition))] [PartCreationPolicy(CreationPolicy.NonShared)]
            containerRegistry.Register<IObservablePosition, ObservablePosition>();

            //MEF: [Export(typeof(IOrderCompositeViewModel))] [PartCreationPolicy(CreationPolicy.NonShared)]
            containerRegistry.Register<IOrderCompositeViewModel, OrderCompositeViewModel>();

            //MEF: [Export(typeof(IOrderDetailsViewModel))] [PartCreationPolicy(CreationPolicy.NonShared)]
            containerRegistry.Register<IOrderDetailsViewModel, OrderDetailsViewModel>();

            //MEF: [Export(typeof(IOrdersViewModel))][PartCreationPolicy(CreationPolicy.NonShared)]
            containerRegistry.Register<IOrdersViewModel, OrdersViewModel>();

            //MEF: [Export(typeof(IPositionPieChartViewModel))][PartCreationPolicy(CreationPolicy.NonShared)]
            containerRegistry.Register<IPositionPieChartViewModel, PositionPieChartViewModel>();

            //MEF: [Export(typeof(IPositionSummaryViewModel))][PartCreationPolicy(CreationPolicy.NonShared)]
            containerRegistry.Register<IPositionSummaryViewModel, PositionSummaryViewModel>();

            ViewModelLocationProvider.Register(typeof(OrderComposite).ToString(), typeof(OrderCompositeViewModel));
            ViewModelLocationProvider.Register(typeof(Orders).ToString(), typeof(OrdersViewModel));
        }
    }
}