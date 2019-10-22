using System;
using System.Windows.Controls;
using Prism.Events;

namespace StockTraderRI.Modules.Position.Views
{
    public partial class PositionPieChart : UserControl
    {
        public event EventHandler<DataEventArgs<string>> PositionSelected = delegate { };

        public PositionPieChart()
        {
            InitializeComponent();
        }
    }
}
