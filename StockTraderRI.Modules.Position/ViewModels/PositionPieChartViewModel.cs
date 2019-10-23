namespace StockTraderRI.Modules.Position.ViewModels
{
    public class PositionPieChartViewModel : IPositionPieChartViewModel
    {
        public IObservablePosition Position { get; private set; }

        public PositionPieChartViewModel(IObservablePosition observablePosition)
        {
            this.Position = observablePosition;
        }
    }
}
