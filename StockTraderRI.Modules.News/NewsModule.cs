using Prism.Modularity;
using Prism.Regions;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.News.Controllers;
using StockTraderRI.Modules.News.Services;
using Prism.Ioc;
using StockTraderRI.Modules.News.Views;

namespace StockTraderRI.Modules.News
{
    public class NewsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            //MEF:  [ViewExport(RegionName = RegionNames.ResearchRegion)] [PartCreationPolicy(CreationPolicy.Shared)]
            regionManager.RegisterViewWithRegion(RegionNames.ResearchRegion, typeof(Article));
            regionManager.RegisterViewWithRegion(RegionNames.SecondaryRegion, typeof(NewsReader));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //MEF:  [Export(typeof(INewsController))][PartCreationPolicy(CreationPolicy.Shared)]
            containerRegistry.RegisterSingleton<INewsController, NewsController>();

            // [Export(typeof(INewsFeedService))][PartCreationPolicy(CreationPolicy.Shared)]
            containerRegistry.RegisterSingleton<INewsFeedService, NewsFeedService>();
        }
    }
}
