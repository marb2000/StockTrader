using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.Position.Controllers;
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.Services;
using StockTraderRI.Modules.Position.Views;

namespace StockTraderRI.Modules.Position
{
    public class PositionModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(PositionSummary));
            regionManager.RegisterViewWithRegion(RegionNames.ResearchRegion, typeof(PositionPieChart));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAccountPositionService, AccountPositionService>();
            containerRegistry.RegisterSingleton<IOrdersService, XmlOrdersService>();
            containerRegistry.RegisterSingleton<IOrdersController, OrdersController>();
            containerRegistry.Register<IObservablePosition, ObservablePosition>();


        }
    }
}