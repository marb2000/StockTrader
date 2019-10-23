using Prism.Events;
using System;
using System.Windows.Controls;

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
