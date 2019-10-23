using System;
using System.Collections.Generic;
using StockTraderRI.Infrastructure.Models;

namespace StockTraderRI.Infrastructure.Interfaces
{
    public interface IAccountPositionService
    {
        event EventHandler<AccountPositionModelEventArgs> Updated;
        IList<AccountPosition> GetAccountPositions();
    }
}
