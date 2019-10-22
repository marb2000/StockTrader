using Prism.Commands;
using Prism.Mvvm;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Infrastructure.Models;
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.Models;
using StockTraderRI.Modules.Position.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace StockTraderRI.Modules.Position.ViewModels
{
    public class OrderDetailsViewModel : BindableBase, IOrderDetailsViewModel
    {
        private readonly IAccountPositionService _accountPositionService;
        private readonly IOrdersService _ordersService;
        private TransactionInfo _transactionInfo;
        private int? _shares;
        private OrderType _orderType = OrderType.Market;
        private decimal? _stopLimitPrice;
        private TimeInForce _timeInForce;

        private readonly List<string> errors = new List<string>();

        public DelegateCommand SubmitCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        public OrderDetailsViewModel(IAccountPositionService accountPositionService, IOrdersService ordersService)
        {
            _accountPositionService = accountPositionService;
            _ordersService = ordersService;
            _transactionInfo = new TransactionInfo();
            SubmitCommand = new DelegateCommand(Submit, CanSubmit);
            CancelCommand = new DelegateCommand(Cancel);

            //use localizable enum descriptions
            AvailableOrderTypes = new ValueDescriptionList<OrderType>
                                        {
                                            new ValueDescription<OrderType>(OrderType.Limit, Resources.OrderType_Limit),
                                            new ValueDescription<OrderType>(OrderType.Market, Resources.OrderType_Market),
                                            new ValueDescription<OrderType>(OrderType.Stop, Resources.OrderType_Stop)
                                        };
            AvailableTimesInForce = new ValueDescriptionList<TimeInForce>
                                          {
                                              new ValueDescription<TimeInForce>(TimeInForce.EndOfDay, Resources.TimeInForce_EndOfDay),
                                              new ValueDescription<TimeInForce>(TimeInForce.ThirtyDays, Resources.TimeInForce_ThirtyDays)
                                          };
            SetInitialValidState();
        }


        public event EventHandler CloseViewRequested = delegate { };

        public IValueDescriptionList<OrderType> AvailableOrderTypes { get; private set; }

        public IValueDescriptionList<TimeInForce> AvailableTimesInForce { get; private set; }

        public TransactionInfo TransactionInfo
        {
            get => _transactionInfo;
            set
            {
                SetProperty(ref _transactionInfo, value);
                RaisePropertyChanged("TickerSymbol");
            }
        }

        public TransactionType TransactionType
        {
            get => _transactionInfo.TransactionType;
            set
            {
                ValidateHasEnoughSharesToSell(Shares, value, false);
                if (_transactionInfo.TransactionType != value)
                {
                    _transactionInfo.TransactionType = value;
                    RaisePropertyChanged("TransactionType");
                }
            }
        }
        public string TickerSymbol
        {
            get => _transactionInfo.TickerSymbol;
            set
            {
                if (_transactionInfo.TickerSymbol != value)
                {
                    _transactionInfo.TickerSymbol = value;
                    RaisePropertyChanged("TickerSymbol");
                }
            }
        }

        public int? Shares
        {
            get => _shares;
            set
            {
                SetProperty(ref _shares, value);
                ValidateShares(value, true);
                ValidateHasEnoughSharesToSell(value, TransactionType, true);
                IsValidOrder = errors.Count == 0;
            }
        }

        private bool _isValidOrder = false;
        public bool IsValidOrder
        {
            get => _isValidOrder;
            set
            {
                SetProperty(ref _isValidOrder, value);
            }
        }

        public OrderType OrderType
        {
            get => _orderType;
            set
            {
                SetProperty(ref _orderType, value);
            }
        }

        public decimal? StopLimitPrice
        {
            get => _stopLimitPrice;
            set
            {
                SetProperty(ref _stopLimitPrice, value);
                ValidateStopLimitPrice(value, true);
                IsValidOrder = errors.Count == 0;
            }
        }

        public TimeInForce TimeInForce
        {
            get => _timeInForce;
            set
            {
                SetProperty(ref _timeInForce, value);
            }
        }

        private void SetInitialValidState()
        {
            ValidateShares(Shares, false);
            ValidateStopLimitPrice(StopLimitPrice, false);
        }

        private void ValidateShares(int? newSharesValue, bool throwException)
        {
            if (!newSharesValue.HasValue || newSharesValue.Value <= 0)
            {
                AddError("InvalidSharesRange");
                if (throwException)
                {
                    throw new InputValidationException(Resources.InvalidSharesRange);
                }
            }
            else
            {
                RemoveError("InvalidSharesRange");
            }
        }

        private void ValidateStopLimitPrice(decimal? price, bool throwException)
        {
            if (!price.HasValue || price.Value <= 0)
            {
                AddError("InvalidStopLimitPrice");
                if (throwException)
                {
                    throw new InputValidationException(Resources.InvalidStopLimitPrice);
                }
            }
            else
            {
                RemoveError("InvalidStopLimitPrice");
            }
        }

        private void ValidateHasEnoughSharesToSell(int? sharesToSell, TransactionType transactionType, bool throwException)
        {
            if (transactionType == TransactionType.Sell && !this.HoldsEnoughShares(this.TickerSymbol, sharesToSell))
            {
                this.AddError("NotEnoughSharesToSell");
                if (throwException)
                {
                    throw new InputValidationException(String.Format(CultureInfo.InvariantCulture, Resources.NotEnoughSharesToSell, sharesToSell));
                }
            }
            else
            {
                this.RemoveError("NotEnoughSharesToSell");
            }
        }

        private void AddError(string ruleName)
        {
            if (!errors.Contains(ruleName))
            {
                errors.Add(ruleName);
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        private void RemoveError(string ruleName)
        {
            if (errors.Contains(ruleName))
            {
                errors.Remove(ruleName);
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        private bool HoldsEnoughShares(string symbol, int? sharesToSell)
        {
            if (!sharesToSell.HasValue)
            {
                return false;
            }

            foreach (AccountPosition accountPosition in this._accountPositionService.GetAccountPositions())
            {
                if (accountPosition.TickerSymbol.Equals(symbol, StringComparison.OrdinalIgnoreCase))
                {
                    if (accountPosition.Shares >= sharesToSell)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        private bool CanSubmit()
        {
            return true; IsValidOrder = errors.Count == 0;
        }

        private void Submit()
        {
            if (!CanSubmit())
            {
                throw new InvalidOperationException();
            }

            var order = new Order
            {
                TransactionType = TransactionType,
                OrderType = OrderType,
                Shares = Shares.Value,
                StopLimitPrice = StopLimitPrice.Value,
                TickerSymbol = TickerSymbol,
                TimeInForce = TimeInForce
            };

            _ordersService.Submit(order);

            CloseViewRequested(this, EventArgs.Empty);
        }

        private void Cancel()
        {
            CloseViewRequested(this, EventArgs.Empty);
        }
    }
}
