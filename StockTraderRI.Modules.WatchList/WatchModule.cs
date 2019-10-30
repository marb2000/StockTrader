using Prism.Modularity;
using Prism.Regions;
using StockTraderRI.Infrastructure;
using StockTraderRI.Modules.Watch.Services;
using Prism.Ioc;
using Prism.Mvvm;

namespace StockTraderRI.Modules.Watch
{
    public class WatchModule : IModule
    {
        void IModule.OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion(RegionNames.MainToolBarRegion, typeof(Views.AddWatch));
            regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(Views.WatchList));
        }

        void IModule.RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IWatchListService, WatchListService>();

            //If the View and the ViewModel are in different assemblies, the convention doesn't work
            //you have to register the View within the ViewModel manually
            ViewModelLocationProvider.Register<Views.AddWatch, ViewModels.AddWatchViewModel>();
        }
    }
}
