using System;
using System.Windows.Input;
using Prism.Mvvm;
using StockTraderRI.Modules.Watch.Services;

namespace StockTraderRI.Modules.Watch.ViewModels
{
    public class AddWatchViewModel : BindableBase
    {
        private string stockSymbol;
        private IWatchListService _watchListService;
        public ICommand AddWatchCommand { get { return _watchListService.AddWatchCommand; } }

        public AddWatchViewModel(IWatchListService watchListService)
        {
            if (watchListService == null)
            {
                throw new ArgumentNullException("watchListService");
            }

            _watchListService = watchListService;
        }

        public string StockSymbol
        {
            get { return stockSymbol; }
            set
            {
                SetProperty(ref stockSymbol, value);
            }
        }
    }
}
