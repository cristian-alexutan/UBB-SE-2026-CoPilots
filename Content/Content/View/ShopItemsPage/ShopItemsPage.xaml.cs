using Content.Domain;
using Content.Service;
using Content.User;
using Content.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.StartScreen;
using WinRT.Interop;

namespace Content
{

    public class PathToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;

            string path = value.ToString();

            try
            {
                if (path.StartsWith("Assets/"))
                    return new BitmapImage(new Uri($"ms-appx:///{path}"));
                else
                    return new BitmapImage(new Uri(path));
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public sealed partial class ShopItemsPage : Window
    {

        private Button _activeSortButton;
        public ShopItemsViewModel ViewModel { get; }
        private readonly UserSession _session;
        private Shop _currentShop;
        private readonly MainService _service;
        private Cart _cart;

        public ShopItemsPage(MainService service, UserSession session, Shop currentShop)
        {
            this.InitializeComponent();
            _service = service;
            _session = session;
            _currentShop = currentShop;
            _cart = _service.cartService.GetCartById(_session.UserId);

            ViewModel = new ShopItemsViewModel(_service.ShopItemService, _service.cartService, session, currentShop);
            RootGrid.DataContext = ViewModel;

            // Set default active sort button
            SetActiveSortButton(SortAlphabeticButton);

            if (_session.IsAdmin)
            {
                AddButton.Visibility = Visibility.Visible;
            }
        }


        public bool IsAdmin => _session.IsAdmin;

        private void ShowError(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = RootGrid.XamlRoot
            };

            _ = dialog.ShowAsync();
        }

        private void SetActiveSortButton(Button activeButton)
        {
            // Reset all buttons to default style
            ResetButtonStyle(SortAlphabeticButton);
            ResetButtonStyle(SortPriceButton);

            // Set the active button style
            activeButton.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 43, 184, 192)); // #2bb8c0
            activeButton.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));

            _activeSortButton = activeButton;
        }

        private void ResetButtonStyle(Button button)
        {
            if (button != null)
            {
                button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 211, 211, 211)); // LightGray
                button.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); // Black
            }
        }

        // Sort button click handlers
        private void SortAlphabetic_Click(object sender, RoutedEventArgs e)
        {

            SetActiveSortButton(SortAlphabeticButton);
            ViewModel.SortByName();
        }

        private void SortPrice_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSortButton(SortPriceButton);
            ViewModel.SortByPrice();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_session.IsAdmin) return;

            string selectedImagePath = null;
            var button = sender as Button;

            var imagePreview = new Image
            {
                Width = 120,
                Height = 120,
                Stretch = Microsoft.UI.Xaml.Media.Stretch.UniformToFill,
                Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
        new Uri("ms-appx:///Assets/placeholder.png"))
            };

            var nameBox = new TextBox { PlaceholderText = "Enter item name" };
            var descBox = new TextBox { PlaceholderText = "Enter description" };
            var dropZone = new Border
            {
                Height = 140,
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Windows.UI.Color.FromArgb(30, 0, 0, 0)),
                Child = new TextBlock
                {
                    Text = "Drag image here or click to select",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            var priceBox = new TextBox { PlaceholderText = "Enter price" };
            var quantityBox = new TextBox { PlaceholderText = "Enter quantity" };

            dropZone.AllowDrop = true;

            dropZone.Drop += async (s, e) =>
            {
                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    var items = await e.DataView.GetStorageItemsAsync();
                    var file = items.FirstOrDefault() as StorageFile;

                    if (file != null)
                    {
                        using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                        {
                            var bitmap = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage();
                            await bitmap.SetSourceAsync(stream);
                            imagePreview.Source = bitmap;
                        }

                        // store REAL path
                        selectedImagePath = file.Path;
                    }
                }
            };

            dropZone.Tapped += async (s, e) =>
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();

                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".jpg");

                var file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                    {
                        var bitmap = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage();
                        await bitmap.SetSourceAsync(stream);
                        imagePreview.Source = bitmap;
                    }

                    selectedImagePath = file.Path;
                }
            };

            var dialog = new ContentDialog
            {
                Title = "Add Shop Item",
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Save",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = button.XamlRoot,
                Content = new ScrollViewer
                {
                    MaxHeight = 500,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollMode = ScrollMode.Auto,

                    Content = new StackPanel
                    {
                        Spacing = 15,
                        Children =
                        {
                            
                            new TextBlock { Text = "Item Name", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            nameBox,

                            new TextBlock { Text = "Description", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            descBox,

                            dropZone,
                            imagePreview,

                            new TextBlock { Text = "Price", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            priceBox,

                            new TextBlock { Text = "Quantity", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            quantityBox
                        }
                    }
                }
            };

            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(nameBox.Text) ||
                string.IsNullOrWhiteSpace(priceBox.Text) ||
                string.IsNullOrWhiteSpace(quantityBox.Text))
            {
                ShowError("All fields must be filled!");
                return;
            }
            if (result == ContentDialogResult.Primary)
            {
                if (!string.IsNullOrWhiteSpace(nameBox.Text) &&
                    float.TryParse(priceBox.Text, out float price) &&
                    int.TryParse(quantityBox.Text, out int quantity))
                {
                    var newItem = new ShopItem(
                        0,
                        quantity,
                        price,
                        _currentShop.Id,
                        selectedImagePath ?? "Assets/PlaceHolder.png",
                        nameBox.Text,
                        descBox.Text
                    );

                    try { ViewModel.AddItem(newItem); }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message);
                    }


                    // Reapply current sort
                    if (_activeSortButton == SortAlphabeticButton)
                        SortAlphabetic_Click(null, null);
                    else if (_activeSortButton == SortPriceButton)
                        SortPrice_Click(null, null);
                }
            }
        }

        // Edit button click handler
        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_session.IsAdmin) return;

            var button = sender as Button;
            var item = button?.Tag as ShopItem;

            if (item != null)
            {
                var nameBox = new TextBox { Text = item.Name };
                var descBox = new TextBox { Text = item.Description };
                var priceBox = new TextBox { Text = item.Price.ToString() };
                var quantityBox = new TextBox { Text = item.Quantity.ToString() };
                string selectedImagePath = item.Photo;

                var imagePreview = new Image
                {
                    Width = 120,
                    Height = 120,
                    Stretch = Stretch.UniformToFill
                };

                // load existing image
                try
                {
                    if (!string.IsNullOrEmpty(item.Photo) && item.Photo.StartsWith("Assets/"))
                        imagePreview.Source = new BitmapImage(new Uri($"ms-appx:///{item.Photo}"));
                    else if (!string.IsNullOrEmpty(item.Photo))
                        imagePreview.Source = new BitmapImage(new Uri(item.Photo));
                }
                catch
                {
                    imagePreview.Source = null;
                }

                var dropZone = new Border
                {
                    Height = 140,
                    Background = new SolidColorBrush(Windows.UI.Color.FromArgb(30, 0, 0, 0)),
                    Child = new TextBlock
                    {
                        Text = "Drag image here or click to change",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };

                dropZone.Drop += async (s, e) =>
                {
                    if (e.DataView.Contains(StandardDataFormats.StorageItems))
                    {
                        var items = await e.DataView.GetStorageItemsAsync();
                        var file = items.FirstOrDefault() as Windows.Storage.StorageFile;

                        if (file != null)
                        {
                            using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                            {
                                var bitmap = new BitmapImage();
                                await bitmap.SetSourceAsync(stream);
                                imagePreview.Source = bitmap;
                            }

                            selectedImagePath = file.Path;
                        }
                    }
                };

                dropZone.Tapped += async (s, e) =>
                {
                    var picker = new Windows.Storage.Pickers.FileOpenPicker();

                    var hwnd = WindowNative.GetWindowHandle(this);
                    InitializeWithWindow.Initialize(picker, hwnd);

                    picker.FileTypeFilter.Add(".png");
                    picker.FileTypeFilter.Add(".jpg");

                    var file = await picker.PickSingleFileAsync();

                    if (file != null)
                    {
                        using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                        {
                            var bitmap = new BitmapImage();
                            await bitmap.SetSourceAsync(stream);
                            imagePreview.Source = bitmap;
                        }

                        selectedImagePath = file.Path;
                    }
                };

                dropZone.AllowDrop = true;

                var dialog = new ContentDialog
                {
                    Title = "Edit Shop Item",
                    CloseButtonText = "Cancel",
                    PrimaryButtonText = "Save",
                    RequestedTheme = ElementTheme.Light,
                    XamlRoot = button.XamlRoot,
                    Content = new ScrollViewer
                    {
                        MaxHeight = 500,
                        Content = new StackPanel
                        {
                            Spacing = 15,
                            Children =
                        {
                            new TextBlock { Text = "Item Name", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            nameBox,
                            new TextBlock { Text = "Description", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            descBox,

                            dropZone,
                            imagePreview,

                            new TextBlock { Text = "Price", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            priceBox,
                            new TextBlock { Text = "Quantity", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            quantityBox
                        }
                        }
                    }
                };

                var result = await dialog.ShowAsync();

                if (result != ContentDialogResult.Primary)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(nameBox.Text) ||
                string.IsNullOrWhiteSpace(priceBox.Text) ||
                string.IsNullOrWhiteSpace(quantityBox.Text))
                {
                    ShowError("All fields must be filled!");
                    return;
                }
                if (result == ContentDialogResult.Primary)
                {
                    if (float.TryParse(priceBox.Text, out float price) &&
                        int.TryParse(quantityBox.Text, out int quantity))
                    {
                        var updatedItem = new ShopItem(
                            item.Id,
                            quantity,
                            price,
                            item.ShopId,
                            selectedImagePath ?? item.Photo,
                            nameBox.Text,
                            descBox.Text
                        );

                        try { ViewModel.UpdateItem(updatedItem); }
                        catch (Exception ex)
                        {
                            ShowError(ex.Message);
                        }

                        // Reapply current sort
                        if (_activeSortButton == SortAlphabeticButton)
                            SortAlphabetic_Click(null, null);
                        else if (_activeSortButton == SortPriceButton)
                            SortPrice_Click(null, null);
                    }
                }
            }
        }

        // Delete button click handler
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_session.IsAdmin) return;

            var button = sender as Button;
            var item = button?.Tag as ShopItem;

            if (item != null)
            {
                var dialog = new ContentDialog
                {
                    Title = "Delete Item",
                    Content = $"Are you sure you want to delete \"{item.Name}\"?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "Cancel",
                    RequestedTheme = ElementTheme.Light,
                    XamlRoot = button.XamlRoot
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    ViewModel.DeleteItem(item);

                    // Reapply current sort
                    if (_activeSortButton == SortAlphabeticButton)
                        SortAlphabetic_Click(null, null);
                    else if (_activeSortButton == SortPriceButton)
                        SortPrice_Click(null, null);
                }
            }
        }

        private void OpenCartButton_Click(object sender, RoutedEventArgs e)
        {
            var cartPage = new CartPage(_service, _session);
            cartPage.Activate();

            this.Close();
        }

        private void AddItemToCartButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.Tag as ShopItem;

            if (item == null) return;
            var cart = _service.cartService.GetCartById(_session.UserId);
            if (cart == null)
            {
                cart = new Cart(_session.UserId, new Client(_session.UserId, "Current Client"), new System.Collections.Generic.Dictionary<int, CartItem>());
                _service.cartService.AddCart(cart);
            }
            ViewModel.AddToCart(item, 1);

        }

        private void BackToShops_Click(object sender, RoutedEventArgs e)
        {
            var shopPage = new ShopPage(_service, _session);
            shopPage.Activate();

            this.Close();
        }

        private void BackToLandingPage_Click(object sender, RoutedEventArgs e)
        {
            var landingPage = new LandingPage(_service, _session);
            landingPage.Activate();

            this.Close();
        }

        private void SearchBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var query = ((TextBox)sender).Text;
                ViewModel.Search(query);
            }
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ShopItem;
            if (item == null) return;

            var detailPage = new ItemDetailsPage(_service, _session, item, _cart);
            detailPage.Activate();

            this.Close();
        }
    }
}