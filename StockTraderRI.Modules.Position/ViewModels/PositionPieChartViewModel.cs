namespace StockTraderRI.Modules.Position.ViewModels
{
    public class PositionPieChartViewModel
    {
        public IObservablePosition Position { get; private set; }

        public PositionPieChartViewModel(IObservablePosition observablePosition)
        {
            Position = observablePosition;
        }
    }
}
