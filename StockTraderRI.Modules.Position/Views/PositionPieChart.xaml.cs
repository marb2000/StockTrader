using System;
using System.Windows.Controls;
using Prism.Events;
using StockTraderRI.Modules.Position.ViewModels;

namespace StockTraderRI.Modules.Position.Views
{
    public partial class PositionPieChart : UserControl
    {
        public event EventHandler<DataEventArgs<string>> PositionSelected = delegate { };

        public PositionPieChart()
        {
            InitializeComponent();
        }

        public IPositionPieChartViewModel Model
        {
            get => DataContext as IPositionPieChartViewModel;
            set => DataContext = value;
        }
    }
}
