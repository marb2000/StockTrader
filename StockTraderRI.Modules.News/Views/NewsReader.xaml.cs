using System.Windows.Controls;

namespace StockTraderRI.Modules.News.Views
{
    public partial class NewsReader : UserControl
    {
        public NewsReader()
        {
            InitializeComponent();
        }

        public static string Title => Properties.Resources.NewsReaderViewTitle;
    }
}
