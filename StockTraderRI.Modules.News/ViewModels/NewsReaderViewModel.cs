﻿using Prism.Mvvm;
using StockTraderRI.Infrastructure.Models;

namespace StockTraderRI.Modules.News.ViewModels
{
    public class NewsReaderViewModel : BindableBase
    {
        private NewsArticle newsArticle;
        public NewsArticle NewsArticle
        {
            get
            {
                return this.newsArticle;
            }
            set
            {
                SetProperty(ref this.newsArticle, value);
            }
        }

    }
}
