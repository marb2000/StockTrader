using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.Market.Services;
using StockTraderRI.Modules.Market.Views;

namespace StockTraderRI.Modules.Market
{
    public class MarketModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.ResearchRegion, typeof(TrendLine));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMarketFeedService, MarketFeedService>();
            containerRegistry.RegisterSingleton<IMarketHistoryService, MarketHistoryService>();
        }
    }
}
