using Prism.Commands;

namespace StockTraderRI.Infrastructure
{

    public static class StockTraderRiCommands
    {
        private static CompositeCommand _submitOrderCommand = new CompositeCommand(true);
        private static CompositeCommand _cancelOrderCommand = new CompositeCommand(true);
        private static CompositeCommand _submitAllOrdersCommand = new CompositeCommand();
        private static CompositeCommand _cancelAllOrdersCommand = new CompositeCommand();

        public static CompositeCommand SubmitOrderCommand
        {
            get => _submitOrderCommand;
            set => _submitOrderCommand = value;
        }

        public static CompositeCommand CancelOrderCommand
        {
            get => _cancelOrderCommand;
            set => _cancelOrderCommand = value;
        }

        public static CompositeCommand SubmitAllOrdersCommand
        {
            get => _submitAllOrdersCommand;
            set => _submitAllOrdersCommand = value;
        }

        public static CompositeCommand CancelAllOrdersCommand
        {
            get => _cancelAllOrdersCommand;
            set => _cancelAllOrdersCommand = value;
        }
    }

    public class StockTraderRiCommandProxy
    {
        virtual public CompositeCommand SubmitOrderCommand => StockTraderRiCommands.SubmitOrderCommand;

        virtual public CompositeCommand CancelOrderCommand => StockTraderRiCommands.CancelOrderCommand;

        virtual public CompositeCommand SubmitAllOrdersCommand => StockTraderRiCommands.SubmitAllOrdersCommand;

        virtual public CompositeCommand CancelAllOrdersCommand => StockTraderRiCommands.CancelAllOrdersCommand;
    }
}
