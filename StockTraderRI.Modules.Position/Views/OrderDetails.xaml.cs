using Microsoft.Toolkit.Wpf.UI.XamlHost;
using StockTraderRI.Modules.Position.ViewModels;
using System.Windows;
using System.Windows.Controls;


namespace StockTraderRI.Modules.Position.Views
{
    public partial class OrderDetails : UserControl
    {

        public OrderDetails()
        {
            InitializeComponent();
        }

        WindowsXAMLControls.SignatureView _xamlControl;
        private void WindowsXamlHost_ChildChanged(object sender, System.EventArgs e)
        {
            if (sender is WindowsXamlHost host)
            {
                if (host.Child is WindowsXAMLControls.SignatureView xamlControl)
                {
                    if (_xamlControl == null)
                    {
                        _xamlControl = xamlControl;
                        xamlControl.AcceptBtn.Click += (s, a) =>
                        {
                            SignaturePopup.IsOpen = false;
                            if (this.DataContext is OrderDetailsViewModel viewModel)
                            {
                                viewModel.IsSignatureRequired = false;
                                viewModel.SubmitOrder();
                            }
                        };  /*TODO: Call Accept Command */
                        xamlControl.CancelBtn.Click += (s, a) =>
                        {
                            SignaturePopup.IsOpen = false;
                            if (this.DataContext is OrderDetailsViewModel viewModel)
                            {
                                viewModel.IsSignatureRequired = false;
                                viewModel.CancelCommand.Execute(null);
                            }
                        };  /*TODO: Call Cancel Command */
                    }
                }
            }
        }
    }
}
