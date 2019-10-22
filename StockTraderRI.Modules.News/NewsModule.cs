using Prism.Unity;
using Prism.Modularity;
using Prism.Regions;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.News.Controllers;
using StockTraderRI.Modules.News.Services;
using Prism.Ioc;
using Unity;
using StockTraderRI.Modules.News.Views;
using StockTraderRI.Modules.News.ViewModels;
using Prism.Events;

namespace StockTraderRI.Modules.News
{
    public class NewsModule : IModule
    {
        IRegionManager _regionManager;

        public void OnInitialized(IContainerProvider containerProvider)
        {
   
            _regionManager.RegisterViewWithRegion(RegionNames.ResearchRegion, typeof(Article));
            _regionManager.RegisterViewWithRegion(RegionNames.SecondaryRegion, typeof(NewsReader));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

            var containerProvider = containerRegistry.GetContainer();
            var eventAggregator = containerProvider.Resolve<IEventAggregator>();
            _regionManager = containerProvider.Resolve<IRegionManager>();

            INewsFeedService newsFeedService = new NewsFeedService();
            containerRegistry.RegisterInstance(typeof(INewsFeedService), newsFeedService);

            ArticleViewModel articleViewModel = new ArticleViewModel(newsFeedService,_regionManager, eventAggregator);
            containerRegistry.RegisterInstance(typeof(ArticleViewModel), articleViewModel);
            
            NewsReaderViewModel newsReaderViewModel = new NewsReaderViewModel();
            containerRegistry.RegisterInstance(typeof(NewsReaderViewModel), newsReaderViewModel);

            containerRegistry.RegisterInstance(typeof(INewsController), new NewsController(articleViewModel, newsReaderViewModel));
        }
    }
}
