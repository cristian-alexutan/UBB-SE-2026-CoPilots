using System;
using System.IO;
using System.Linq;
using Content.Domain;
using Content.ViewModel;
using Content.ViewModel.Interface;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Content
{
    public sealed partial class ItemDetailsPage : Page
    {
        private ShopItem item;
        private Cart cart;

        public IItemDetailsViewModel ViewModel { get; private set; }

        private int qty = 1;

        private string productName;
        private string productDesc;
        private string productPrice;
        private int stock;

        public ItemDetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var args = (ItemDetailsNavArgs)e.Parameter;
            this.item = args.Item;
            this.cart = args.Cart;

            ViewModel = new ItemDetailsViewModel(App.CartService, App.ShopItemService, App.Session, item, cart);
            productName = this.item.Name;
            productDesc = this.item.Description;
            productPrice = $"{this.item.Price:C}";
            stock = this.item.Quantity;

            ApplyProductModelToUI();
            LoadAdminFieldsFromModel();
            UpdateQuantityUI();

            SetMode(isAdmin: App.Session.IsAdmin);
        }

        private void UpdateQuantityUI()
        {
            if (qty < 1)
            {
                qty = 1;
            }
            if (qty > 99)
            {
                qty = 99;
            }

            QuantityBox.Text = qty.ToString();
        }

        private void SyncQuantityFromTextBox()
        {
            if (int.TryParse(QuantityBox.Text, out var typed))
            {
                qty = typed;
            }
            UpdateQuantityUI();
        }

        private void QuantityMinus_Click(object sender, RoutedEventArgs e)
        {
            SyncQuantityFromTextBox();
            qty--;
            UpdateQuantityUI();
        }

        private void QuantityPlus_Click(object sender, RoutedEventArgs e)
        {
            SyncQuantityFromTextBox();
            qty++;
            UpdateQuantityUI();
        }

        private async void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            SyncQuantityFromTextBox();

            if (qty > stock)
            {
                var err = new ContentDialog
                {
                    Title = "Not enough stock",
                    Content = $"You requested {qty} item(s), but only {stock} are available.",
                    CloseButtonText = "OK",
                    XamlRoot = Root.XamlRoot,
                };
                await err.ShowAsync();
                return;
            }

            var existingCart = App.CartService.GetCartById(App.Session.UserId);
            if (existingCart == null)
            {
                existingCart = new Cart(App.Session.UserId, new Client(App.Session.UserId, "Current Client"),
                    new System.Collections.Generic.Dictionary<int, CartItem>());
                App.CartService.AddCart(existingCart);
            }

            try
            {
                ViewModel.AddToCartCommand.Execute(qty);
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
                Content = $"Quantity: {qty}",
                CloseButtonText = "OK",
                XamlRoot = Root.XamlRoot,
            };
            await dlg.ShowAsync();

            var shop = App.ShopService.GetAllAvailableShops().FirstOrDefault(s => s.Id == item.ShopId);
            if (shop != null)
            {
                this.Frame.Navigate(typeof(ShopItemsPage), shop);
            }
        }

        private void ViewCart_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CartPage));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var shop = App.ShopService.GetAllAvailableShops().FirstOrDefault(s => s.Id == item.ShopId);
            if (shop != null)
            {
                this.Frame.Navigate(typeof(ShopItemsPage), shop);
            }
        }

        private void SetMode(bool isAdmin)
        {
            ViewCartButton.IsEnabled = !isAdmin;

            var fg = isAdmin ? Colors.Gray : ColorHelper.FromArgb(255, 17, 24, 39);
            var brush = new SolidColorBrush(fg);

            ViewCartIcon.Foreground = brush;
            ViewCartText.Foreground = brush;

            ClientActionsPanel.Visibility = isAdmin ? Visibility.Collapsed : Visibility.Visible;
            AdminActionsPanel.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

            if (isAdmin)
            {
                LoadAdminFieldsFromModel();
                AdminStatusText.Text = string.Empty;
            }
        }

        private void LoadAdminFieldsFromModel()
        {
            AdminNameBox.Text = productName;
            AdminDescBox.Text = productDesc;
            AdminPriceBox.Text = productPrice;
            AdminStockBox.Text = stock.ToString();
        }

        private void ApplyProductModelToUI()
        {
            TitleText.Text = productName;
            DescText.Text = productDesc;
            PriceText.Text = productPrice;

            StockUnitsRun.Text = $"{stock} units";

            SetProductImageFromItem();

            if (qty > stock)
            {
                qty = stock;
            }
            if (qty < 1)
            {
                qty = 1;
            }
            UpdateQuantityUI();
        }

        public async void SetProductImageFromItem()
        {
            var photo = (item.Photo ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(photo))
            {
                ProductImage.Source = null;
                return;
            }

            if (Uri.TryCreate(photo, UriKind.Absolute, out var webUri) &&
                (webUri.Scheme == "http" || webUri.Scheme == "https"))
            {
                ProductImage.Source = new BitmapImage(webUri);
                return;
            }

            if (Uri.TryCreate(photo, UriKind.Absolute, out var appUri) &&
                (appUri.Scheme == "ms-appx" || appUri.Scheme == "ms-appdata"))
            {
                ProductImage.Source = new BitmapImage(appUri);
                return;
            }

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
            productName = (AdminNameBox.Text ?? string.Empty).Trim();
            productDesc = AdminDescBox.Text ?? string.Empty;
            productPrice = (AdminPriceBox.Text ?? string.Empty).Trim();

            if (!int.TryParse(AdminStockBox.Text, out var parsedStock) || parsedStock < 0)
            {
                AdminStatusText.Text = "Error: Stock must be a non-negative number.";
                return;
            }

            stock = parsedStock;

            if (!float.TryParse(productPrice, System.Globalization.NumberStyles.Currency, null, out var price))
            {
                price = item.Price;
            }
            var updatedItem = new ShopItem(
                item.Id,
                stock,
                price,
                item.ShopId,
                item.Photo,
                productName,
                productDesc);

            ViewModel.UpdateItemCommand.Execute(updatedItem);

            ApplyProductModelToUI();
            AdminStatusText.Text = "Saved";
        }
    }
}
