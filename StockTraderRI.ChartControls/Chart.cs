

using System.Windows.Controls;
using System.Windows;

namespace StockTraderRI.ChartControls
{
    public class Chart : ItemsControl
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ChartItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ChartItem();
        }

        public object Title
        {
            get => (object)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(Chart), new UIPropertyMetadata(null));

        public DataTemplate TitleTemplate
        {
            get => (DataTemplate)GetValue(TitleTemplateProperty);
            set => SetValue(TitleTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for TitleTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleTemplateProperty =
            DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(Chart), new UIPropertyMetadata(null));



        public DataTemplateSelector TitleTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(TitleTemplateSelectorProperty);
            set => SetValue(TitleTemplateSelectorProperty, value);
        }

        // Using a DependencyProperty as the backing store for TitleTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleTemplateSelectorProperty =
            DependencyProperty.Register("TitleTemplateSelector", typeof(DataTemplateSelector), typeof(Chart), new UIPropertyMetadata(null));
    }
}