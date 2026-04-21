using Content;
using Content.Domain;
using Content.Helper;
using Content.Service;
using Content.User;
using Content.ViewModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Streams;


namespace Content
{
    public sealed partial class ItemDetailsPage : Window
    {
        private readonly MainService _service;
        private readonly UserSession _session;
        private readonly ShopItem _item;
        private readonly Cart _cart;

        public ItemDetailsViewModel ViewModel { get; }

        private int _qty = 1;

        private string _productName;
        private string _productDesc;
        private string _productPrice;
        private int _stock;


        public ItemDetailsPage(MainService service, UserSession session, ShopItem item, Cart cart)
        {

            InitializeComponent();

            _service = service;
            _session = session;
            _item = item;
            _cart = cart;
            ViewModel = new ItemDetailsViewModel(service, session, item, cart);
            _productName = _item.Name;
            _productDesc = _item.Description;
            _productPrice = $"{_item.Price:C}";
            _stock = _item.Quantity;




            ApplyProductModelToUI();
            LoadAdminFieldsFromModel();
            UpdateQuantityUI();

            SetMode(isAdmin: session.IsAdmin);
            _cart = cart;
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

        private async void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            SyncQuantityFromTextBox();

            if (_qty > _stock)
            {
                var err = new ContentDialog
                {
                    Title = "Not enough stock",
                    Content = $"You requested {_qty} item(s), but only {_stock} are available.",
                    CloseButtonText = "OK",
                    XamlRoot = Root.XamlRoot,
                };
                await err.ShowAsync();
                return;
            }

            var cart = _service.cartService.GetCartById(_session.UserId);
            if (cart == null)
            {
                cart = new Cart(_session.UserId, new Client(_session.UserId, "Current Client"),
                    new System.Collections.Generic.Dictionary<int, CartItem>());
                _service.cartService.AddCart(cart);
            }

            try
            {
                ViewModel.AddToCartCommand.Execute(_qty);
            }
            catch (InvalidOperationException ex)
            {
                var err = new ContentDialog
                {
                    Title = "Cannot add to cart",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = Root.XamlRoot,
                };
                await err.ShowAsync();
                return;
            }

            var dlg = new ContentDialog
            {
                Title = "Added to cart",
                Content = $"Quantity: {_qty}",
                CloseButtonText = "OK",
                XamlRoot = Root.XamlRoot,
            };
            await dlg.ShowAsync();

            var shop = _service.shopService.GetAllAvailableShops().FirstOrDefault(s => s.Id == _item.ShopId);
            if (shop != null)
            {
                var prevPage = new ShopItemsPage(_service, _session, shop);
                prevPage.Activate();
            }

            this.Close();
        }

        private void ViewCart_Click(object sender, RoutedEventArgs e)
        {
            //modify once CartPage is implemented
            var cartPage = new CartPage(_service, _session);
            cartPage.Activate();
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var shop = _service.shopService.GetAllAvailableShops().FirstOrDefault(s => s.Id == _item.ShopId);
            if (shop != null)
            {
                var prevPage = new ShopItemsPage(_service, _session, shop);
                prevPage.Activate();
            }
            this.Close();
        }

        private void SetMode(bool isAdmin)
        {
            // Disable for admin
            ViewCartButton.IsEnabled = !isAdmin;

            // Keep icon/text clearly visible in client; greyed in admin
            var fg = isAdmin ? Colors.Gray : ColorHelper.FromArgb(255, 17, 24, 39); // #111827
            var brush = new SolidColorBrush(fg);

            ViewCartIcon.Foreground = brush;
            ViewCartText.Foreground = brush;

            ClientActionsPanel.Visibility = isAdmin ? Visibility.Collapsed : Visibility.Visible;
            AdminActionsPanel.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

            if (isAdmin)
            {
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

            SetProductImageFromItem();

            if (_qty > _stock) _qty = _stock;
            if (_qty < 1) _qty = 1;
            UpdateQuantityUI();
        }

        async void SetProductImageFromItem()
        {
            var photo = (_item.Photo ?? "").Trim();
            if (string.IsNullOrEmpty(photo))
            {
                ProductImage.Source = null;
                return;
            }

            // 1) Web URL
            if (Uri.TryCreate(photo, UriKind.Absolute, out var webUri) &&
                (webUri.Scheme == "http" || webUri.Scheme == "https"))
            {
                ProductImage.Source = new BitmapImage(webUri);
                return;
            }

            // 2) ms-appx/ms-appdata URI
            if (Uri.TryCreate(photo, UriKind.Absolute, out var appUri) &&
                (appUri.Scheme == "ms-appx" || appUri.Scheme == "ms-appdata"))
            {
                ProductImage.Source = new BitmapImage(appUri);
                return;
            }

            // 3) Local absolute file path like C:\...
            if (Path.IsPathRooted(photo))
            {
                try
                {
                    var file = await StorageFile.GetFileFromPathAsync(photo);
                    using IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);

                    var bmp = new BitmapImage();
                    await bmp.SetSourceAsync(stream);
                    ProductImage.Source = bmp;
                    return;
                }
                catch
                {
                    ProductImage.Source = null;
                    return;
                }
            }

            ProductImage.Source = new BitmapImage(new Uri($"ms-appx:///{photo}"));
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

            if (!float.TryParse(_productPrice, System.Globalization.NumberStyles.Currency, null, out var price))
                price = _item.Price;

            var updatedItem = new ShopItem(
                _item.Id,
                _stock,
                price,
                _item.ShopId,
                _item.Photo,
                _productName,
                _productDesc
            );

            ViewModel.UpdateItemCommand.Execute(updatedItem);

            ApplyProductModelToUI();
            AdminStatusText.Text = "Saved";
        }

    }
}