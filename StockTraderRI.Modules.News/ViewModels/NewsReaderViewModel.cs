using Prism.Mvvm;
using StockTraderRI.Infrastructure.Models;

namespace StockTraderRI.Modules.News.ViewModels
{
    public class NewsReaderViewModel : BindableBase
    {
        private NewsArticle _newsArticle;
        public NewsArticle NewsArticle
        {
            get => _newsArticle;
            set => SetProperty(ref _newsArticle, value);
        }

    }
}
