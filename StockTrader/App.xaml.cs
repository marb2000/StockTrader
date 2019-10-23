using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using StockTraderRI.Views;
using System.Windows;

namespace StockTraderRI
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {

            moduleCatalog.AddModule<Modules.Market.MarketModule>("MarketModule");
            moduleCatalog.AddModule<Modules.Position.PositionModule>(dependsOn: "MarketModule");
            moduleCatalog.AddModule<Modules.Watch.WatchModule>(dependsOn: "MarketModule");
            moduleCatalog.AddModule<Modules.News.NewsModule>();

        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
        }
    }
}
