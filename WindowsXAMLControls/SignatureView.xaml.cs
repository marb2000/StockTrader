using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace WindowsXAMLControls
{
    public sealed partial class SignatureView : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private InkPresenter _inkPresenter;

        public SignatureView()
        {
            this.InitializeComponent();
            _inkPresenter = inkCanvas.InkPresenter;
            _inkPresenter.StrokesCollected += _inkPresenter_StrokesCollected;
            _inkPresenter.InputDeviceTypes =
                CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch;
        }

        private void _inkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            this.OnPropertyChanged(nameof(IsAgreedAndSigned));
        }

        bool IsAgreedAndSigned
        {
            get => (agreedCheckBox.IsChecked.GetValueOrDefault() && _inkPresenter.StrokeContainer.GetStrokes().Count > 0);
        }

        private void agreedCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.OnPropertyChanged(nameof(IsAgreedAndSigned));
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _inkPresenter.StrokeContainer.Clear();
            this.OnPropertyChanged(nameof(IsAgreedAndSigned));
        }
    }
}
