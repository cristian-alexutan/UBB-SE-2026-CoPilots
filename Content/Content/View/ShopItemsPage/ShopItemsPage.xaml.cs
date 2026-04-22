using System;
using System.Linq;
using Content.Domain;
using Content.Service;
using Content.User;
using Content.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using WinRT.Interop;

namespace Content
{
    public class PathToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            string path = value.ToString() ?? string.Empty;

            try
            {
                if (path.StartsWith("Assets/"))
                {
                    return new BitmapImage(new Uri($"ms-appx:///{path}"));
                }
                else
                {
                    return new BitmapImage(new Uri(path));
                }
            }
            catch
            {
                return DependencyProperty.UnsetValue;
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
        private Button? selectedSortButton;
        private readonly UserSession userSession;
        private Shop selectedShop;
        private readonly MainService mainService;
        private Cart currentCart;

        public ShopItemsViewModel ViewModel { get; }

        public ShopItemsPage(MainService mainService, UserSession userSession, Shop selectedShop)
        {
            this.InitializeComponent();
            this.mainService = mainService;
            this.userSession = userSession;
            this.selectedShop = selectedShop;
            this.currentCart = this.mainService.cartService.GetCartById(this.userSession.UserId);

            this.ViewModel = new ShopItemsViewModel(this.mainService.ShopItemService, this.mainService.cartService, this.userSession, this.selectedShop);
            this.RootGrid.DataContext = this.ViewModel;

            this.SetActiveSortButton(this.SortAlphabeticButton);

            if (this.userSession.IsAdmin)
            {
                this.AddButton.Visibility = Visibility.Visible;
            }
        }

        public bool IsAdmin => this.userSession.IsAdmin;

        private void ShowError(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.RootGrid.XamlRoot
            };

            _ = dialog.ShowAsync();
        }

        private void SetActiveSortButton(Button activeButton)
        {
            this.ResetButtonStyle(this.SortAlphabeticButton);
            this.ResetButtonStyle(this.SortPriceButton);

            activeButton.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 43, 184, 192));
            activeButton.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));

            this.selectedSortButton = activeButton;
        }

        private void ResetButtonStyle(Button button)
        {
            if (button != null)
            {
                button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 211, 211, 211));
                button.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
            }
        }

        private void SortAlphabetic_Click(object sender, RoutedEventArgs e)
        {
            this.SetActiveSortButton(this.SortAlphabeticButton);
            this.ViewModel.SortByName();
        }

        private void SortPrice_Click(object sender, RoutedEventArgs e)
        {
            this.SetActiveSortButton(this.SortPriceButton);
            this.ViewModel.SortByPrice();
        }

        private void ReapplyCurrentSort()
        {
            if (this.selectedSortButton == this.SortAlphabeticButton)
            {
                this.ViewModel.SortByName();
            }
            else if (this.selectedSortButton == this.SortPriceButton)
            {
                this.ViewModel.SortByPrice();
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.userSession.IsAdmin)
            {
                return;
            }

            string? selectedImagePath = null;
            var sourceButton = sender as Button;
            if (sourceButton == null)
            {
                return;
            }

            var imagePreview = new Image
            {
                Width = 120,
                Height = 120,
                Stretch = Stretch.UniformToFill,
                Source = new BitmapImage(new Uri("ms-appx:///Assets/placeholder.png")),
            };

            var nameBox = new TextBox { PlaceholderText = "Enter item name" };
            var descBox = new TextBox { PlaceholderText = "Enter description" };
            var priceBox = new TextBox { PlaceholderText = "Enter price" };
            var quantityBox = new TextBox { PlaceholderText = "Enter quantity" };

            var dropZone = new Border
            {
                Height = 140,
                Background = new SolidColorBrush(Windows.UI.Color.FromArgb(30, 0, 0, 0)),
                Child = new TextBlock
                {
                    Text = "Drag image here or click to select",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                AllowDrop = true,
            };

            dropZone.Drop += async (s, e) =>
            {
                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    var items = await e.DataView.GetStorageItemsAsync();
                    var file = items.FirstOrDefault() as StorageFile;

                    if (file != null)
                    {
                        using (var stream = await file.OpenAsync(FileAccessMode.Read))
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
                    using (var stream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        var bitmap = new BitmapImage();
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
                XamlRoot = sourceButton.XamlRoot,
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
                            quantityBox,
                        },
                    },
                },
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
                this.ShowError("All fields must be filled!");
                return;
            }

            if (!float.TryParse(priceBox.Text, out float price) || !int.TryParse(quantityBox.Text, out int quantity))
            {
                this.ShowError("Price or quantity is invalid.");
                return;
            }

            var newItem = new ShopItem(0, quantity, price, this.selectedShop.Id, selectedImagePath ?? "Assets/PlaceHolder.png", nameBox.Text, descBox.Text);

            try
            {
                this.ViewModel.AddItem(newItem);
            }
            catch (Exception ex)
            {
                this.ShowError(ex.Message);
            }

            this.ReapplyCurrentSort();
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.userSession.IsAdmin)
            {
                return;
            }

            var sourceButton = sender as Button;
            var item = sourceButton?.Tag as ShopItem;
            if (sourceButton == null || item == null)
            {
                return;
            }

            var nameBox = new TextBox { Text = item.Name };
            var descBox = new TextBox { Text = item.Description };
            var priceBox = new TextBox { Text = item.Price.ToString() };
            var quantityBox = new TextBox { Text = item.Quantity.ToString() };
            string selectedImagePath = item.Photo;

            var imagePreview = new Image
            {
                Width = 120,
                Height = 120,
                Stretch = Stretch.UniformToFill,
            };

            try
            {
                if (!string.IsNullOrEmpty(item.Photo) && item.Photo.StartsWith("Assets/"))
                {
                    imagePreview.Source = new BitmapImage(new Uri($"ms-appx:///{item.Photo}"));
                }
                else if (!string.IsNullOrEmpty(item.Photo))
                {
                    imagePreview.Source = new BitmapImage(new Uri(item.Photo));
                }
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
                    VerticalAlignment = VerticalAlignment.Center,
                },
                AllowDrop = true,
            };

            dropZone.Drop += async (s, e) =>
            {
                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    var items = await e.DataView.GetStorageItemsAsync();
                    var file = items.FirstOrDefault() as StorageFile;

                    if (file != null)
                    {
                        using (var stream = await file.OpenAsync(FileAccessMode.Read))
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
                    using (var stream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        var bitmap = new BitmapImage();
                        await bitmap.SetSourceAsync(stream);
                        imagePreview.Source = bitmap;
                    }

                    selectedImagePath = file.Path;
                }
            };

            var dialog = new ContentDialog
            {
                Title = "Edit Shop Item",
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Save",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = sourceButton.XamlRoot,
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
                            quantityBox,
                        },
                    },
                },
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
                this.ShowError("All fields must be filled!");
                return;
            }

            if (!float.TryParse(priceBox.Text, out float price) || !int.TryParse(quantityBox.Text, out int quantity))
            {
                this.ShowError("Price or quantity is invalid.");
                return;
            }

            var updatedItem = new ShopItem(item.Id, quantity, price, item.ShopId, selectedImagePath ?? item.Photo, nameBox.Text, descBox.Text);

            try
            {
                this.ViewModel.UpdateItem(updatedItem);
            }
            catch (Exception ex)
            {
                this.ShowError(ex.Message);
            }

            this.ReapplyCurrentSort();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.userSession.IsAdmin)
            {
                return;
            }

            var sourceButton = sender as Button;
            var item = sourceButton?.Tag as ShopItem;
            if (sourceButton == null || item == null)
            {
                return;
            }

            var dialog = new ContentDialog
            {
                Title = "Delete Item",
                Content = $"Are you sure you want to delete \"{item.Name}\"?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = sourceButton.XamlRoot,
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                this.ViewModel.DeleteItem(item);
                this.ReapplyCurrentSort();
            }
        }

        private void OpenCartButton_Click(object sender, RoutedEventArgs e)
        {
            var cartPage = new CartPage(this.mainService, this.userSession);
            cartPage.Activate();
            this.Close();
        }

        private async void AddItemToCartButton_Click(object sender, RoutedEventArgs e)
        {
            var sourceButton = sender as Button;
            var item = sourceButton?.Tag as ShopItem;
            if (item == null)
            {
                return;
            }

            var currentCart = this.mainService.cartService.GetCartById(this.userSession.UserId);
            if (currentCart == null)
            {
                currentCart = new Cart(this.userSession.UserId,
                    new Client(this.userSession.UserId, "Current Client"),
                    new System.Collections.Generic.Dictionary<int, CartItem>());
                this.mainService.cartService.AddCart(currentCart);
            }

            try
            {
                this.ViewModel.AddToCart(item, 1);
            }
            catch (InvalidOperationException ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Cannot add to cart",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot,
                };
                await dialog.ShowAsync();
            }
        }

        private void BackToShops_Click(object sender, RoutedEventArgs e)
        {
            var shopPage = new ShopPage(this.mainService, this.userSession);
            shopPage.Activate();
            this.Close();
        }

        private void BackToLandingPage_Click(object sender, RoutedEventArgs e)
        {
            var landingPage = new LandingPage(this.mainService, this.userSession);
            landingPage.Activate();
            this.Close();
        }

        private void SearchBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var query = ((TextBox)sender).Text;
                this.ViewModel.Search(query);
            }
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ShopItem;
            if (item == null)
            {
                return;
            }

            var detailPage = new ItemDetailsPage(this.mainService, this.userSession, item, this.currentCart);
            detailPage.Activate();
            this.Close();
        }
    }
}