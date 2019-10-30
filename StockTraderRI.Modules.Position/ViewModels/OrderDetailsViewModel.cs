using Prism.Commands;
using Prism.Mvvm;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Infrastructure.Models;
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.Models;
using StockTraderRI.Modules.Position.Properties;
using StockTraderRI.Modules.Position.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace StockTraderRI.Modules.Position.ViewModels
{
    public class OrderDetailsViewModel : BindableBase
    {
        private readonly IAccountPositionService _accountPositionService;
        private readonly IOrdersService _ordersService;
        private TransactionInfo _transactionInfo;
        private int? _shares;
        private OrderType _orderType = OrderType.Market;
        private decimal? _stopLimitPrice;
        private TimeInForce _timeInForce;

        private readonly List<string> _errors = new List<string>();

        public OrderDetailsViewModel(IAccountPositionService accountPositionService, IOrdersService ordersService)
        {
            _accountPositionService = accountPositionService;
            _ordersService = ordersService;

            _transactionInfo = new TransactionInfo();

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

            SubmitCommand = new DelegateCommand<object>(Submit, CanSubmit);
            CancelCommand = new DelegateCommand<object>(Cancel);

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
                ValidateShares(value, true);
                ValidateHasEnoughSharesToSell(value, TransactionType, true);

                SetProperty(ref _shares, value);
            }
        }

        public OrderType OrderType
        {
            get => _orderType;
            set => SetProperty(ref _orderType, value);
        }

        public decimal? StopLimitPrice
        {
            get => _stopLimitPrice;
            set
            {
                ValidateStopLimitPrice(value, true);
                SetProperty(ref _stopLimitPrice, value);
            }
        }

        public TimeInForce TimeInForce
        {
            get => _timeInForce;
            set => SetProperty(ref _timeInForce, value);
        }

        public DelegateCommand<object> SubmitCommand { get; private set; }

        public DelegateCommand<object> CancelCommand { get; private set; }

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
            if (transactionType == TransactionType.Sell && !HoldsEnoughShares(TickerSymbol, sharesToSell))
            {
                AddError("NotEnoughSharesToSell");
                if (throwException)
                {
                    throw new InputValidationException(String.Format(CultureInfo.InvariantCulture, Resources.NotEnoughSharesToSell, sharesToSell));
                }
            }
            else
            {
                RemoveError("NotEnoughSharesToSell");
            }
        }

        private void AddError(string ruleName)
        {
            if (!_errors.Contains(ruleName))
            {
                _errors.Add(ruleName);
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        private void RemoveError(string ruleName)
        {
            if (_errors.Contains(ruleName))
            {
                _errors.Remove(ruleName);
                if (_errors.Count == 0)
                {
                    SubmitCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool CanSubmit(object parameter)
        {
            return _errors.Count == 0;
        }

        private bool HoldsEnoughShares(string symbol, int? sharesToSell)
        {
            if (!sharesToSell.HasValue)
            {
                return false;
            }

            foreach (AccountPosition accountPosition in _accountPositionService.GetAccountPositions())
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

        bool _isSignatureRequired;
        public bool IsSignatureRequired
        {
            get => _isSignatureRequired;
            set
            {
                SetProperty(ref _isSignatureRequired, value);
            }
        }

        private void Submit(object parameter)
        {
            if (!CanSubmit(parameter))
            {
                throw new InvalidOperationException();
            }

            if (Shares != null || StopLimitPrice != null)
            {
                if (Shares.Value >= 10)
                {
                    IsSignatureRequired = true;
                }
                else
                {
                    SubmitOrder();
                }
            }
        }

        public void SubmitOrder()
        {
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

        private void Cancel(object parameter)
        {
            CloseViewRequested(this, EventArgs.Empty);
        }
    }
}