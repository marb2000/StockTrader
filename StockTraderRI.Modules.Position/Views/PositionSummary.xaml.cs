using StockTraderRI.Modules.Position.ViewModels;
using System.Windows.Controls;

namespace StockTraderRI.Modules.Position.Views
{
    public partial class PositionSummary : UserControl
    {
        public PositionSummary()
        {
            InitializeComponent();
        }

        public IPositionSummaryViewModel Model
        {
            get => DataContext as IPositionSummaryViewModel;
            set => DataContext = value;
        }
    }
}
