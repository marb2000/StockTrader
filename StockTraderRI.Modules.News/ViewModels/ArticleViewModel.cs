

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using Prism.Regions;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Infrastructure.Models;

namespace StockTraderRI.Modules.News.ViewModels
{
    public class ArticleViewModel : BindableBase
    {
        private string _companySymbol;
        private IList<NewsArticle> _articles;
        private NewsArticle _selectedArticle;
        private readonly INewsFeedService _newsFeedService;
        private readonly IRegionManager _regionManager;
        private readonly ICommand _showArticleListCommand;
        private readonly ICommand _showNewsReaderViewCommand;

        public ArticleViewModel(INewsFeedService newsFeedService, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            if (newsFeedService == null)
            {
                throw new ArgumentNullException(nameof(newsFeedService));
            }

            if (regionManager == null)
            {
                throw new ArgumentNullException(nameof(regionManager));
            }

            if (eventAggregator == null)
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }

            _newsFeedService = newsFeedService;
            _regionManager = regionManager;

            _showArticleListCommand = new DelegateCommand(ShowArticleList);
            _showNewsReaderViewCommand = new DelegateCommand(ShowNewsReaderView);

            eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Subscribe(OnTickerSymbolSelected, ThreadOption.UIThread);
        }

        public string CompanySymbol
        {
            get => _companySymbol;
            set
            {
                if (SetProperty(ref _companySymbol, value))
                {
                    OnCompanySymbolChanged();
                }
            }
        }

        public NewsArticle SelectedArticle
        {
            get => _selectedArticle;
            set => SetProperty(ref _selectedArticle, value);
        }

        public IList<NewsArticle> Articles
        {
            get => _articles;
            private set => SetProperty(ref _articles, value);
        }

        public ICommand ShowNewsReaderCommand => _showNewsReaderViewCommand;

        public ICommand ShowArticleListCommand => _showArticleListCommand;

        private void OnTickerSymbolSelected(string companySymbol)
        {
            CompanySymbol = companySymbol;
        }

        private void OnCompanySymbolChanged()
        {
            Articles = _newsFeedService.GetNews(_companySymbol);
        }

        private void ShowArticleList()
        {
            SelectedArticle = null;
        }

        private void ShowNewsReaderView()
        {
            _regionManager.RequestNavigate(RegionNames.SecondaryRegion, new Uri("/NewsReaderView", UriKind.Relative));
        }
    }
}
