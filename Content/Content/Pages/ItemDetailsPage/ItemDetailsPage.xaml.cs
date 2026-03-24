using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;

namespace Content
{
    public sealed partial class ItemDetailsPage : Window
    {
        private int _qty = 1;

        // Demo product model
        private string _productName = "Premium Gold Watch";
        private string _productDesc =
            "An exquisite timepiece crafted with precision and elegance. This premium gold watch features a sophisticated design with a durable stainless steel case, sapphire crystal glass, and a luxurious gold-tone finish. Perfect for any occasion, combining timeless style with modern functionality.";
        private string _productPrice = "$2,499.00";
        private int _stock = 15;

        public ItemDetailsPage()
        {
            InitializeComponent();

            ApplyProductModelToUI();
            LoadAdminFieldsFromModel();
            UpdateQuantityUI();

            SetMode(isClient: true);
        }

        private void UpdateQuantityUI()
        {
            if (_qty < 1) _qty = 1;
            if (_qty > 99) _qty = 99;

            QuantityBox.Text = _qty.ToString();
        }

        private void SyncQuantityFromTextBox()
        {
            if (int.TryParse(QuantityBox.Text, out var typed))
                _qty = typed;

            UpdateQuantityUI();
        }

        private void QuantityMinus_Click(object sender, RoutedEventArgs e)
        {
            SyncQuantityFromTextBox();
            _qty--;
            UpdateQuantityUI();
        }

        private void QuantityPlus_Click(object sender, RoutedEventArgs e)
        {
            SyncQuantityFromTextBox();
            _qty++;
            UpdateQuantityUI();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            SyncQuantityFromTextBox();

            if (_qty > _stock)
            {
                var err = new ContentDialog
                {
                    Title = "Not enough stock",
                    Content = $"You requested {_qty} item(s), but only {_stock} are available.",
                    CloseButtonText = "OK",
                    XamlRoot = Root.XamlRoot
                };

                _ = err.ShowAsync();
                return;
            }

            var dlg = new ContentDialog
            {
                Title = "Added to cart",
                Content = $"Quantity: {_qty}",
                CloseButtonText = "OK",
                XamlRoot = Root.XamlRoot
            };

            _ = dlg.ShowAsync();
        }

        private void ViewCart_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ContentDialog
            {
                Title = "Cart",
                Content = "View Cart clicked (cart page not implemented yet).",
                CloseButtonText = "OK",
                XamlRoot = Root.XamlRoot
            };

            _ = dlg.ShowAsync();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder for navigation
        }

        private void ClientToggle_Click(object sender, RoutedEventArgs e) => SetMode(isClient: true);
        private void AdminToggle_Click(object sender, RoutedEventArgs e) => SetMode(isClient: false);

        private void SetMode(bool isClient)
        {
            ClientToggle.IsChecked = isClient;
            AdminToggle.IsChecked = !isClient;

            ClientActionsPanel.Visibility = isClient ? Visibility.Visible : Visibility.Collapsed;
            AdminActionsPanel.Visibility = isClient ? Visibility.Collapsed : Visibility.Visible;

            var accent = (Brush)Root.Resources["AccentTeal"];
            if (isClient)
            {
                ClientToggle.Background = accent;
                ClientToggle.Foreground = new SolidColorBrush(Colors.White);
                ClientToggle.BorderThickness = new Thickness(0);

                AdminToggle.Background = new SolidColorBrush(Colors.Transparent);
                AdminToggle.Foreground = new SolidColorBrush(Colors.Black);
                AdminToggle.BorderThickness = new Thickness(1);
            }
            else
            {
                AdminToggle.Background = accent;
                AdminToggle.Foreground = new SolidColorBrush(Colors.White);
                AdminToggle.BorderThickness = new Thickness(0);

                ClientToggle.Background = new SolidColorBrush(Colors.Transparent);
                ClientToggle.Foreground = new SolidColorBrush(Colors.Black);
                ClientToggle.BorderThickness = new Thickness(1);

                LoadAdminFieldsFromModel();
                AdminStatusText.Text = "";
            }
        }

        private void LoadAdminFieldsFromModel()
        {
            AdminNameBox.Text = _productName;
            AdminDescBox.Text = _productDesc;
            AdminPriceBox.Text = _productPrice;
            AdminStockBox.Text = _stock.ToString();
        }

        private void ApplyProductModelToUI()
        {
            TitleText.Text = _productName;
            DescText.Text = _productDesc;
            PriceText.Text = _productPrice;

            StockUnitsRun.Text = $"{_stock} units";

            if (_qty > _stock) _qty = _stock;
            if (_qty < 1) _qty = 1;
            UpdateQuantityUI();
        }

        private void AdminSave_Click(object sender, RoutedEventArgs e)
        {
            _productName = (AdminNameBox.Text ?? "").Trim();
            _productDesc = AdminDescBox.Text ?? "";
            _productPrice = (AdminPriceBox.Text ?? "").Trim();

            if (!int.TryParse(AdminStockBox.Text, out var parsedStock) || parsedStock < 0)
            {
                AdminStatusText.Text = "Error: Stock must be a non-negative number.";
                return;
            }

            _stock = parsedStock;

            ApplyProductModelToUI();
            AdminStatusText.Text = "Saved (demo only).";
        }

        private void AdminReset_Click(object sender, RoutedEventArgs e)
        {
            LoadAdminFieldsFromModel();
            AdminStatusText.Text = "Reset.";
        }
    }
}