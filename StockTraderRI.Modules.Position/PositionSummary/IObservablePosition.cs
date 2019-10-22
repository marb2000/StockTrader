using System.Collections.ObjectModel;

namespace StockTraderRI.Modules.Position
{
    public interface IObservablePosition
    {
        ObservableCollection<PositionSummaryItem> Items { get; }
    }
}