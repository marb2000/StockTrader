using System;
using System.Windows.Input;
using Prism.Mvvm;
using StockTraderRI.Modules.Watch.Services;

namespace StockTraderRI.Modules.Watch.ViewModels
{
    public class AddWatchViewModel : BindableBase
    {
        private string _stockSymbol;
        private IWatchListService _watchListService;
        public ICommand AddWatchCommand => _watchListService.AddWatchCommand;

        public AddWatchViewModel(IWatchListService watchListService)
        {
            if (watchListService == null)
            {
                throw new ArgumentNullException(nameof(watchListService));
            }

            _watchListService = watchListService;
        }

        public string StockSymbol
        {
            get => _stockSymbol;
            set => SetProperty(ref _stockSymbol, value);
        }
    }
}
