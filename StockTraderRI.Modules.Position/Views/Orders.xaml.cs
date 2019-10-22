using System.Windows.Controls;
using StockTraderRI.Modules.Position.Interfaces;

namespace StockTraderRI.Modules.Position.Views
{
    public partial class Orders : UserControl, IOrders
    {
        public Orders()
        {
            InitializeComponent();
        }
    }
}
