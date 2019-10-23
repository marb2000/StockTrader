

using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System;
using System.Windows.Data;
using System.Collections.Specialized;

namespace StockTraderRI.ChartControls
{
    public class ContinuousAxisPanel : Panel
    {
        public ContinuousAxisPanel()
        {
            _largestLabelSize = new Size();
            SetValue(ItemsSourceKey, new ObservableCollection<String>());
            YValues = new ObservableCollection<double>();
            SetValue(TickPositionsKey, new ObservableCollection<double>());
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            ParentControl = ((ContinuousAxis)((FrameworkElement)VisualTreeHelper.GetParent(this)).TemplatedParent);

            if (ParentControl != null)
            {
                Binding valueBinding = new Binding();
                valueBinding.Source = ParentControl;
                valueBinding.Path = new PropertyPath(ContinuousAxis.SourceValuesProperty);
                SetBinding(DataValuesProperty, valueBinding);

                Binding itemsBinding = new Binding();
                itemsBinding.Source = this;
                itemsBinding.Path = new PropertyPath(ItemsSourceProperty);
                ParentControl.SetBinding(ItemsControl.ItemsSourceProperty, itemsBinding);

                Binding refLineBinding = new Binding();
                refLineBinding.Source = ParentControl;
                refLineBinding.Path = new PropertyPath(ContinuousAxis.ReferenceLineSeperationProperty);
                SetBinding(ReferenceLineSeperationProperty, refLineBinding);

                Binding outputBinding = new Binding();
                outputBinding.Source = this;
                outputBinding.Path = new PropertyPath(YValuesProperty);
                ParentControl.SetBinding(ContinuousAxis.ValuesProperty, outputBinding);

                Binding tickPositionBinding = new Binding();
                tickPositionBinding.Source = this;
                tickPositionBinding.Path = new PropertyPath(TickPositionsProperty);
                ParentControl.SetBinding(ContinuousAxis.TickPositionsProperty, tickPositionBinding);

                Binding zerobinding = new Binding();
                zerobinding.Source = this;
                zerobinding.Path = new PropertyPath(OriginProperty);
                ParentControl.SetBinding(ContinuousAxis.OriginProperty, zerobinding);
            }
        }

        public static void OnDataValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ContinuousAxisPanel p = sender as ContinuousAxisPanel;
            if (p != null && p.DataValues != null)
            {
                ((INotifyCollectionChanged)p.DataValues).CollectionChanged += new NotifyCollectionChangedEventHandler(p.Axis2Panel_CollectionChanged);
                p.GenerateItemsSource();
            }
        }

        public void Axis2Panel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GenerateItemsSource();
        }

        private void GenerateItemsSource()
        {
            if (DataValues==null || DataValues.Count==0)
            {
                return;
            }
            CalculateValueIncrement(_arrangeSize);
            
            ObservableCollection<String> tempItemsSource = ItemsSource;
            tempItemsSource.Clear();
            int referenceLinesCreated = 0;
            while (referenceLinesCreated != _numReferenceLines)
            {
                if (Orientation.Equals(Orientation.Vertical))
                    tempItemsSource.Add(((double)(_startingIncrement + referenceLinesCreated * _valueIncrement)).ToString());
                else
                    tempItemsSource.Add(((double)(_startingIncrement + (_numReferenceLines - 1 - referenceLinesCreated) * _valueIncrement)).ToString());
                referenceLinesCreated++;
            }
            _highValue = _startingIncrement + (_numReferenceLines - 1) * _valueIncrement;
            _lowValue = _startingIncrement;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _largestLabelSize.Height = 0.0;
            _largestLabelSize.Width = 0.0;
            UIElementCollection tempInternalChildren = InternalChildren;
            for (int i = 0; i < tempInternalChildren.Count; i++)
            {
                tempInternalChildren[i].Measure(availableSize);
                _largestLabelSize.Height = _largestLabelSize.Height > tempInternalChildren[i].DesiredSize.Height
                    ? _largestLabelSize.Height : tempInternalChildren[i].DesiredSize.Height;
                _largestLabelSize.Width = _largestLabelSize.Width > tempInternalChildren[i].DesiredSize.Width
                    ? _largestLabelSize.Width : tempInternalChildren[i].DesiredSize.Width;
            }
            if (Orientation.Equals(Orientation.Vertical))
            {
                double fitAllLabelSize = _largestLabelSize.Height * InternalChildren.Count;
                availableSize.Height = fitAllLabelSize < availableSize.Height ? fitAllLabelSize : availableSize.Height;
                availableSize.Width = _largestLabelSize.Width;
            }
            else
            {
                double fitAllLabelsSize = _largestLabelSize.Width * InternalChildren.Count;
                availableSize.Width = fitAllLabelsSize < availableSize.Width ? fitAllLabelsSize : availableSize.Width;
                availableSize.Height = _largestLabelSize.Height;
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!_arrangeSize.Equals(finalSize))
            {
                _arrangeSize = finalSize;
                GenerateItemsSource();
            }
            _arrangeSize = finalSize;
            if (InternalChildren.Count > 0)
            {
                if (Orientation.Equals(Orientation.Vertical))
                {
                    ArrangeVerticalLabels(finalSize);
                    CalculateYOutputValues(finalSize);
                }
                else
                {
                    ArrangeHorizontalLabels(finalSize);
                    CalculateXOutputValues(finalSize);
                }

            }
            return base.ArrangeOverride(finalSize);
        }

        private void ArrangeHorizontalLabels(Size constraint)
        {
            double rectWidth = _largestLabelSize.Width;
            double rectHeight = _largestLabelSize.Height;
            double increments = CalculatePixelIncrements(constraint, _largestLabelSize);
            double startWidth = constraint.Width - _largestLabelSize.Width / 2;
            double endWidth = startWidth - (InternalChildren.Count - 1) * increments;
            ObservableCollection<double> tempTickPositions = TickPositions;

            if (startWidth > endWidth)
            {
                tempTickPositions.Clear();
                Rect r = new Rect(startWidth - rectWidth / 2, 0, rectWidth, rectHeight);
                InternalChildren[0].Arrange(r);
                tempTickPositions.Add(startWidth);
                int count = InternalChildren.Count - 1;
                r = new Rect(startWidth - count * increments - rectWidth / 2, 0, rectWidth, rectHeight);
                InternalChildren[count].Arrange(r);
                tempTickPositions.Add(startWidth - count * increments);

                if (constraint.Width > 3 * rectWidth)
                {
                    _skipFactor = (int)Math.Ceiling((InternalChildren.Count - 2) / Math.Floor((constraint.Width - 2 * rectWidth) / rectWidth));
                    if ((InternalChildren.Count - 2) != 2.0)
                        _skipFactor = Math.Min(_skipFactor, (int)Math.Ceiling((double)(InternalChildren.Count - 2.0) / 2.0));
                    _canDisplayAllLabels = true;
                    if (_skipFactor > 1)
                    {
                        _canDisplayAllLabels = false;
                    }

                    for (int i = 2; i <= InternalChildren.Count - 1; i++)
                    {
                        tempTickPositions.Add(startWidth - (i - 1) * increments);
                        if (_canDisplayAllLabels || (i + 1) % _skipFactor == 0)
                        {
                            r = new Rect(startWidth - (i - 1) * increments - rectWidth / 2, 0, rectWidth, rectHeight);
                            InternalChildren[i-1].Arrange(r);
                        }
                        else
                        {
                            InternalChildren[i-1].Arrange(new Rect(0, 0, 0, 0));
                        }
                    }
                }
            }
        }

        private void ArrangeVerticalLabels(Size constraint)
        {
            double rectWidth = _largestLabelSize.Width;
            double rectHeight = _largestLabelSize.Height;
            double increments = CalculatePixelIncrements(constraint, _largestLabelSize);
            double startHeight = constraint.Height - _largestLabelSize.Height / 2;
            double endHeight = startHeight - (InternalChildren.Count - 1) * increments;
            ObservableCollection<double> tempTickPositions = TickPositions;

            if(startHeight > endHeight)
            {
                tempTickPositions.Clear();
                Rect r = new Rect(constraint.Width - rectWidth, (startHeight - rectHeight / 2), rectWidth, rectHeight);
                InternalChildren[0].Arrange(r);
                tempTickPositions.Add(startHeight);
                int count = InternalChildren.Count-1;
                r = new Rect(constraint.Width - rectWidth, (startHeight - count*increments - rectHeight / 2), rectWidth, rectHeight);
                InternalChildren[count].Arrange(r);
                tempTickPositions.Add(startHeight - count * increments);

                if (constraint.Height > 3 * rectHeight)
                {
                    _skipFactor = (int)Math.Ceiling((InternalChildren.Count - 2) / Math.Floor((constraint.Height - 2 * rectHeight) / rectHeight));
                    if ((InternalChildren.Count - 2) != 2.0)
                        _skipFactor = Math.Min(_skipFactor, (int)Math.Ceiling((double)(InternalChildren.Count - 2.0) / 2.0));
                    _canDisplayAllLabels = true;
                    if (_skipFactor > 1)
                    {
                        _canDisplayAllLabels = false;
                    }
                    
                    for (int i = 2; i <= InternalChildren.Count-1; i++)
                    {
                        tempTickPositions.Add(startHeight - (i - 1) * increments);
                        if (_canDisplayAllLabels || (i + 1) % _skipFactor == 0 )
                        {
                            r = new Rect(constraint.Width - rectWidth, (startHeight - (i - 1) * increments - rectHeight / 2), rectWidth, rectHeight);
                            InternalChildren[i - 1].Arrange(r);
                        }
                        else
                        {
                            InternalChildren[i - 1].Arrange(new Rect(0, 0, 0, 0));
                        }
                    }
                }
            }
        }

        private void CalculateYOutputValues(Size constraint)
        {
            YValues.Clear();
            double startVal, lowPixel, highPixel;
            double pixelIncrement = CalculatePixelIncrements(constraint, _largestLabelSize);
            if (Orientation.Equals(Orientation.Vertical))
            {
                startVal = constraint.Height - _largestLabelSize.Height / 2;
                lowPixel = startVal - (InternalChildren.Count - 1) * pixelIncrement;
                highPixel = startVal;
            }
            else
            {
                startVal = constraint.Width - _largestLabelSize.Width / 2;
                lowPixel = startVal - (InternalChildren.Count - 1) * pixelIncrement;
                highPixel = startVal;
            }
            if (highPixel < lowPixel)
                return;
            for (int i = 0; i < DataValues.Count; i++)
            {
                double outVal = highPixel - ((highPixel - lowPixel) / (_highValue - _lowValue)) * (DataValues[i] - _lowValue);
                YValues.Add(outVal);
            }
            if (_startsAtZero || (!_allNegativeValues && !_allPositiveValues))
                Origin = highPixel - ((highPixel - lowPixel) / (_highValue - _lowValue)) * (0.0 - _lowValue);
            else if (!_startsAtZero && _allPositiveValues)
                Origin = highPixel;
            else
                Origin = lowPixel;
        }

        private void CalculateXOutputValues(Size constraint)
        {
            YValues.Clear();
            double startWidth = constraint.Width - _largestLabelSize.Width / 2;
            double pixelIncrement = CalculatePixelIncrements(constraint, _largestLabelSize);
            double lowPixel = startWidth - (InternalChildren.Count - 1) * pixelIncrement;
            double highPixel = startWidth;
            if (highPixel < lowPixel)
                return;
            for (int i = 0; i < DataValues.Count; i++)
            {
                double output = lowPixel + ((highPixel - lowPixel) / (_highValue - _lowValue)) * (DataValues[i] - _lowValue);
                YValues.Add(output);
            }
            if (_startsAtZero || (!_allNegativeValues && !_allPositiveValues))
                Origin = lowPixel + ((highPixel - lowPixel) / (_highValue - _lowValue)) * (0.0 - _lowValue);
            else if (!_startsAtZero && _allPositiveValues)
                Origin = lowPixel;
            else
                Origin = highPixel;
        }

        /// <summary>
        /// Calculate the pixel distance between each tick mark on the vertical axis
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        private double CalculatePixelIncrements(Size constraint, Size labelSize)
        {
            if(Orientation.Equals(Orientation.Vertical))
                return (constraint.Height - _largestLabelSize.Height) / (_numReferenceLines - 1);
            else
                return (constraint.Width - _largestLabelSize.Width) / (_numReferenceLines - 1);
        }

        private double CalculateValueIncrement(Size size)
        {
            // Determine if the starting value is 0 or not
            bool startsAtZero = false;
            bool allPositiveValues = true;
            bool allNegativeValues = true;
            double incrementValue = 0;
            int multiplier = 1;
            if (DataValues.Count == 0)
                return 0.0;
            //double low = ((DoubleHolder)DataValues[0]).DoubleValue;
            //double high = ((DoubleHolder)DataValues[0]).DoubleValue;

            double low = DataValues[0];
            double high = DataValues[0];

            for (int i = 0; i < DataValues.Count; i++)
            {
                //double temp = ((DoubleHolder)DataValues[i]).DoubleValue;
                double temp = DataValues[i];

                // Check for positive and negative values
                if (temp > 0)
                {
                    allNegativeValues = false;
                }
                else if (temp < 0)
                {
                    allPositiveValues = false;
                }

                // Reset low and high if necessary
                if (temp < low)
                {
                    low = temp;
                }
                else if (temp > high)
                {
                    high = temp;
                }
            }

            // Determine whether or not the increments will start at zero
            if (allPositiveValues && (low < (high / 2)) ||
                (allNegativeValues && high > (low / 2)))
            {
                _startsAtZero = true;
                startsAtZero = true;
            }

            // If all values in dataset are 0, draw one reference line and label it 0
            if (high == 0 && low == 0)
            {
                _valueIncrement = 0;
                _startingIncrement = 0;
                _numReferenceLines = 1;
                _startsAtZero = startsAtZero;
                return incrementValue;
            }

            // Find an increment value that is in the set {1*10^x, 2*10^x, 5*10^x, where x is an integer 
            //  (positive, negative, or zero)}

            if (!allNegativeValues)
            {
                if (startsAtZero)
                {
                    int exp = 0;
                    while (true)
                    {
                        multiplier = IsWithinRange(high, exp, size);
                        if (multiplier != -1)
                        {
                            break;
                        }
                        multiplier = IsWithinRange(high, (-1 * exp), size);
                        if (multiplier != -1)
                        {
                            exp = -1 * exp;
                            break;
                        }
                        exp++;
                    }
                    incrementValue = multiplier * Math.Pow(10, exp);
                }
                else
                {
                    int exp = 0;
                    while (true)
                    {
                        multiplier = IsWithinRange((high - low), exp, size);
                        if (multiplier != -1)
                        {
                            break;
                        }
                        multiplier = IsWithinRange((high - low), (-1 * exp), size);
                        if (multiplier != -1)
                        {
                            exp = -1 * exp;
                            break;
                        }
                        if (high == low)
                        {
                            incrementValue = high;
                            _valueIncrement = incrementValue;
                            _numReferenceLines = 1;
                            break;
                        }

                        exp++;
                    }
                    if (incrementValue == 0)
                    {
                        incrementValue = multiplier * Math.Pow(10, exp);
                    }
                }
            }
            else
            {
                if (startsAtZero)
                {
                    int exp = 0;
                    while (true)
                    {
                        multiplier = IsWithinRange(low, exp, size);
                        if (multiplier != -1)
                        {
                            break;
                        }
                        multiplier = IsWithinRange(low, (-1 * exp), size);
                        if (multiplier != -1)
                        {
                            exp = -1 * exp;
                            break;
                        }
                        exp++;
                    }
                    incrementValue = multiplier * Math.Pow(10, exp);
                }
                else
                {
                    int exp = 0;
                    if (low - high == 0.0)
                        incrementValue = 1.0;
                    else
                    {
                        while (true)
                        {
                            multiplier = IsWithinRange((low - high), exp, size);
                            if (multiplier != -1)
                            {
                                break;
                            }
                            multiplier = IsWithinRange((low - high), (-1 * exp), size);
                            if (multiplier != -1)
                            {
                                exp = -1 * exp;
                                break;
                            }
                            exp++;
                        }
                        incrementValue = multiplier * Math.Pow(10, exp);
                    }
                }
            }



            double startingValue = 0;

            // Determine starting value if it is nonzero
            if (!startsAtZero)
            {
                if (allPositiveValues)
                {
                    if (low % incrementValue == 0)
                    {
                        startingValue = low;
                    }
                    else
                    {
                        startingValue = (int)(low / incrementValue) * incrementValue;
                    }
                }
                else
                {
                    if (low % incrementValue == 0)
                    {
                        startingValue = low;
                    }
                    else
                    {
                        startingValue = (int)((low - incrementValue) / incrementValue) * incrementValue;
                    }
                }
            }
            else if (startsAtZero && allNegativeValues)
            {
                if (low % incrementValue == 0)
                {
                    startingValue = low;
                }
                else
                {
                    startingValue = (int)((low - incrementValue) / incrementValue) * incrementValue;
                }
            }

            // Determine the number of reference lines
            //int numRefLines = 0;
            int numRefLines = (int)Math.Ceiling((high - startingValue) / incrementValue) + 1;

            _valueIncrement = incrementValue;
            _startingIncrement = startingValue;
            _numReferenceLines = numRefLines;
            _startsAtZero = startsAtZero;
            _allPositiveValues = allPositiveValues;
            _allNegativeValues = allNegativeValues;
            return incrementValue;
        }

        /// <summary>
        /// Checks to see if the calculated increment value is between the low and high passed in, 
        /// then returns the multiplier used
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="exponent"></param>
        /// <param name="lowRange"></param>
        /// <param name="highRange"></param>
        /// <returns></returns>
        private int IsWithinRange(double numerator, int exponent, Size size)
        {
            int highRange, lowRange;
           // highRange = (int)Math.Min(10, (int)(size.Height / labelSize.Height)) -2;
            if(Orientation.Equals(Orientation.Vertical))
                highRange = (int)(size.Height / ReferenceLineSeperation);
            else
                highRange = (int)(size.Width / ReferenceLineSeperation);

            lowRange = 1;
            highRange = (int)Math.Max(highRange, 3);
            
            if ((Math.Abs(numerator) / (1 * Math.Pow(10, exponent))) >= lowRange && (Math.Abs(numerator) / (1 * Math.Pow(10, exponent))) <= highRange)
            {
                return 1;
            }
            if ((Math.Abs(numerator) / (2 * Math.Pow(10, exponent))) >= lowRange && (Math.Abs(numerator) / (2 * Math.Pow(10, exponent))) <= highRange)
            {
                return 2;
            }
            if ((Math.Abs(numerator) / (5 * Math.Pow(10, exponent))) >= lowRange && (Math.Abs(numerator) / (5 * Math.Pow(10, exponent))) <= highRange)
            {
                return 5;
            }
            return -1;
        }


        public ObservableCollection<double> YValues
        {
            get => (ObservableCollection<double>)GetValue(YValuesProperty);
            set => SetValue(YValuesProperty, value);
        }

        // Using a DependencyProperty as the backing store for YValues.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YValuesProperty =
            DependencyProperty.Register("YValues", typeof(ObservableCollection<double>), typeof(ContinuousAxisPanel), new UIPropertyMetadata(null));



        public ObservableCollection<String> ItemsSource => (ObservableCollection<String>)GetValue(ItemsSourceProperty);

        // Using a DependencyProperty as the backing store for Axis2Panel.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey ItemsSourceKey =
            DependencyProperty.RegisterReadOnly("ItemsSource", typeof(ObservableCollection<String>), typeof(ContinuousAxisPanel), new UIPropertyMetadata());

        public static readonly DependencyProperty ItemsSourceProperty = ItemsSourceKey.DependencyProperty;
        

        private ObservableCollection<double> DataValues
        {
            get => (ObservableCollection<double>)GetValue(DataValuesProperty);
            set => SetValue(DataValuesProperty, value);
        }

        // Using a DependencyProperty as the backing store for DataValues.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty DataValuesProperty =
            DependencyProperty.Register("DataValues", typeof(ObservableCollection<double>), typeof(ContinuousAxisPanel), new FrameworkPropertyMetadata(OnDataValuesChanged));


        public ObservableCollection<double> TickPositions => (ObservableCollection<double>)GetValue(TickPositionsProperty);

        // Using a DependencyProperty as the backing store for TickPositions.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey TickPositionsKey =
            DependencyProperty.RegisterReadOnly("TickPositions", typeof(ObservableCollection<double>), typeof(ContinuousAxisPanel), new UIPropertyMetadata(null));

        public static readonly DependencyProperty TickPositionsProperty = TickPositionsKey.DependencyProperty;

        public double Origin
        {
            get => (double)GetValue(OriginProperty);
            set => SetValue(OriginProperty, value);
        }

        // Using a DependencyProperty as the backing store for ZeroReferenceLinePosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginProperty =
            DependencyProperty.Register("Origin", typeof(double), typeof(ContinuousAxisPanel), new UIPropertyMetadata(0.0));


        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ContinuousAxisPanel), new UIPropertyMetadata(Orientation.Vertical));




        public double ReferenceLineSeperation
        {
            get => (double)GetValue(ReferenceLineSeperationProperty);
            set => SetValue(ReferenceLineSeperationProperty, value);
        }

        // Using a DependencyProperty as the backing store for ReferenceLineSeperation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReferenceLineSeperationProperty =
            DependencyProperty.Register("ReferenceLineSeperation", typeof(double), typeof(ContinuousAxisPanel), new UIPropertyMetadata(null));



        private Size _largestLabelSize;
        private bool _canDisplayAllLabels;
        private int _skipFactor;
        private double _lowValue, _highValue;
        private Size _arrangeSize;
        public ItemsControl ParentControl;
        private bool _startsAtZero;
        private double _startingIncrement;
        private double _valueIncrement;
        private int _numReferenceLines;
        private bool _allPositiveValues;
        private bool _allNegativeValues;
    }
}