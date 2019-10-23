using System;
using System.Windows.Input;
using Prism.Events;
using Prism.Mvvm;
using StockTraderRI.Infrastructure;
using StockTraderRI.Modules.Position.Controllers;

namespace StockTraderRI.Modules.Position.ViewModels
{
    public class PositionSummaryViewModel : BindableBase
    {
        private PositionSummaryItem _currentPositionSummaryItem;

        private readonly IEventAggregator _eventAggregator;

        public IObservablePosition Position { get; private set; }

        public PositionSummaryViewModel(IOrdersController ordersController, IEventAggregator eventAggregator, IObservablePosition observablePosition)
        {
            if (ordersController == null)
            {
                throw new ArgumentNullException(nameof(ordersController));
            }

            _eventAggregator = eventAggregator;
            Position = observablePosition;

            BuyCommand = ordersController.BuyCommand;
            SellCommand = ordersController.SellCommand;

            CurrentPositionSummaryItem = new PositionSummaryItem("FAKEINDEX", 0, 0, 0);
        }

        public ICommand BuyCommand { get; private set; }

        public ICommand SellCommand { get; private set; }

        public string HeaderInfo => "POSITION";

        public PositionSummaryItem CurrentPositionSummaryItem
        {
            get => _currentPositionSummaryItem; 
            set
            {
                if (SetProperty(ref _currentPositionSummaryItem, value))
                {
                    if (_currentPositionSummaryItem != null)
                    {
                        _eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Publish(
                            CurrentPositionSummaryItem.TickerSymbol);
                    }
                }
            }
        }
    }
}
