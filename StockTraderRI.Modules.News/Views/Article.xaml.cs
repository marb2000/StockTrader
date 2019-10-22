using System.Windows.Controls;
using System.Windows.Media.Animation;
using StockTraderRI.Modules.News.Controllers;

namespace StockTraderRI.Modules.News.Views
{
    public partial class Article : UserControl
    {
        private INewsController _newsController { get; set; }

        public Article(INewsController newsController)
        {
            InitializeComponent();
            _newsController = newsController;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.NewsList.SelectedItem != null)
            {
                var storyboard = (Storyboard)this.Resources["Details"];
                storyboard.Begin();
            }
            else
            {
                var storyboard = (Storyboard)this.Resources["List"];
                storyboard.Begin();
            }
        }
    }
}
