using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace StockTraderRI.CustomControls
{
    public class AnimatedTabControl : TabControl
    {
        public static readonly RoutedEvent SelectionChangingEvent = EventManager.RegisterRoutedEvent(
            "SelectionChanging", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(AnimatedTabControl));

        private DispatcherTimer _timer;

        public AnimatedTabControl()
        {
            DefaultStyleKey = typeof(AnimatedTabControl);
        }

        public event RoutedEventHandler SelectionChanging
        {
            add => AddHandler(SelectionChangingEvent, value);
            remove => RemoveHandler(SelectionChangingEvent, value);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(
                (Action)delegate
                {
                    RaiseSelectionChangingEvent();

                    StopTimer();

                    _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 500) };

                    EventHandler handler = null;
                    handler = (sender, args) =>
                        {
                            StopTimer();
                            base.OnSelectionChanged(e);
                        };
                    _timer.Tick += handler;
                    _timer.Start();
                });
        }

        // This method raises the Tap event
        private void RaiseSelectionChangingEvent()
        {
            var args = new RoutedEventArgs(SelectionChangingEvent);
            RaiseEvent(args);
        }

        private void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }
    }
}