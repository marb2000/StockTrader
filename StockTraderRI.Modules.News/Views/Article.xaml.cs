using System.Windows.Controls;
using System.Windows.Media.Animation;
using StockTraderRI.Modules.News.Controllers;

namespace StockTraderRI.Modules.News.Views
{
    public partial class Article : UserControl
    {
        private INewsController NewsController { get; set; }

        public Article(INewsController newsController)
        {
            InitializeComponent();
            NewsController = newsController;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NewsList.SelectedItem != null)
            {
                var storyboard = (Storyboard)Resources["Details"];
                storyboard.Begin();
            }
            else
            {
                var storyboard = (Storyboard)Resources["List"];
                storyboard.Begin();
            }
        }
    }
}
