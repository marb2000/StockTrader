using System.Windows.Controls;
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.ViewModels;

namespace StockTraderRI.Modules.Position.Views
{
    public partial class Orders : UserControl, IOrders
    {
        public Orders()
        {
            InitializeComponent();
        }

        public IOrdersViewModel ViewModel
        {
            set { DataContext = value; }
        }
    }
}
