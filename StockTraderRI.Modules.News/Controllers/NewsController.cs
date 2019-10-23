using StockTraderRI.Modules.News.ViewModels;
using System.ComponentModel;

namespace StockTraderRI.Modules.News.Controllers
{
    public class NewsController : INewsController
    {
        private readonly ArticleViewModel _articleViewModel;
        private readonly NewsReaderViewModel _newsReaderViewModel;

        public NewsController(ArticleViewModel articleViewModel, NewsReaderViewModel newsReaderViewModel)
        {
            _articleViewModel = articleViewModel;
            _newsReaderViewModel = newsReaderViewModel;
            if (articleViewModel != null)
            {
                _articleViewModel.PropertyChanged += ArticleViewModel_PropertyChanged;
            }
        }

        private void ArticleViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedArticle":
                    _newsReaderViewModel.NewsArticle = _articleViewModel.SelectedArticle;
                    break;
            }
        }
    }
}
