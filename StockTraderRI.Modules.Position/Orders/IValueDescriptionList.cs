

using System.Collections.Generic;

namespace StockTraderRI.Modules.Position.ViewModels
{
    public interface IValueDescriptionList<T> : IList<ValueDescription<T>> where T : struct
    {

    }
}
