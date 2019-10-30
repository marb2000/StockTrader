// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System;

namespace StockTraderRI.CustomControls
{
    /// <summary>
    /// Custom ItemsControl with Header.
    /// </summary>
    public class ResearchControl : ItemsControl
    {
        public static readonly DependencyProperty HeadersProperty =
            DependencyProperty.Register("Headers", typeof(ObservableCollection<object>), typeof(ResearchControl), null);

        public ResearchControl()
        {
            Headers = new ObservableCollection<object>();
        }

        public ObservableCollection<object> Headers
        {
            get => (ObservableCollection<object>)GetValue(HeadersProperty);
            private set => SetValue(HeadersProperty, value);
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            base.OnItemsChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                object newItem = e.NewItems[0];
                DependencyObject header = GetHeader(newItem as FrameworkElement);
                Headers.Insert(e.NewStartingIndex, header);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                Headers.RemoveAt(e.OldStartingIndex);
            }
        }

        private static DependencyObject GetHeader(FrameworkElement view)
        {
            if (view != null)
            {
                DataTemplate template = view.Resources["HeaderIcon"] as DataTemplate;
                if (template != null)
                {
                    return template.LoadContent();
                }
            }

            return null;
        }
    }
}
