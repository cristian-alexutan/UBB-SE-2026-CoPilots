using System;
using System.Linq;
using System.Threading.Tasks;
using Content.Domain;
using Content.ViewModel.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using WinRT.Interop;
using Microsoft.UI.Xaml.Input;
using Windows.System;
namespace Content
{
    public sealed partial class ShopItemsPage : Page
    {
        public IShopItemsViewModel ViewModel { get; private set; }

        private Shop selectedShop;

        public ShopItemsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            selectedShop = (Shop)e.Parameter;
            this.ShopNameTextBlock.Text = selectedShop.Name;

            var viewModelFactory = App.Services.GetRequiredService<Func<Shop, IShopItemsViewModel>>();
            this.ViewModel = viewModelFactory(selectedShop);
            this.ItemsGridView.ItemsSource = this.ViewModel.Items;

            this.AddButton.Visibility = this.ViewModel.CanAddItem ? Visibility.Visible : Visibility.Collapsed;
            this.CartButton.IsEnabled = this.ViewModel.IsCartEnabled;

            this.SortComboBox.SelectedIndex = 0;
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ViewModel == null)
            {
                return;
            }

            if (sender is ComboBox combo && combo.SelectedItem is ComboBoxItem item)
            {
                switch (item.Content.ToString())
                {
                    case "Name":
                        this.ViewModel.SortByName();
                        break;
                    case "Price":
                        this.ViewModel.SortByPrice();
                        break;
                    default:
                        this.ViewModel.LoadItems();
                        break;
                }
            }
        }

        private void SearchBox_KeyDown(object sender, KeyRoutedEventArgs eventArguments)
        {
            if (eventArguments.Key == VirtualKey.Enter && sender is TextBox textBox)
            {
                ViewModel.Search(textBox.Text);
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs eventArguments)
        {
            var button = sender as Button;

            var nameBox = new TextBox { PlaceholderText = "Enter item name" };
            var descriptionBox = new TextBox { PlaceholderText = "Enter description" };
            var priceBox = new TextBox { PlaceholderText = "Enter price" };
            var quantityBox = new TextBox { PlaceholderText = "Enter quantity" };

            var imagePreview = new Image
            {
                Width = 120,
                Height = 120,
                Stretch = Stretch.UniformToFill,
                Source = new BitmapImage(new Uri("ms-appx:///Assets/placeholder.png")),
            };

            var imagePath = new string?[] { null };
            var dropZone = this.BuildDropZone("Drag image here or click to select", imagePreview, imagePath);

            var dialog = BuildItemDialog("Add Shop Item", button, new StackPanel
            {
                Spacing = 15,
                Children =
                {
                    new TextBlock { Text = "Item Name", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                    nameBox,
                    new TextBlock { Text = "Description", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                    descriptionBox,
                    dropZone,
                    imagePreview,
                    new TextBlock { Text = "Price", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                    priceBox,
                    new TextBlock { Text = "Quantity", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                    quantityBox,
                },
            });

            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
            {
                return;
            }

            try
            {
                this.ViewModel.AddItem(nameBox.Text, descriptionBox.Text, priceBox.Text, quantityBox.Text, imagePath[0] ?? "Assets/PlaceHolder.png");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(button.XamlRoot, ex.Message);
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs eeventArguments)
        {
            if (sender is not Button button || button.Tag is not ShopItem item)
            {
                return;
            }

            var nameBox = new TextBox { Text = item.Name };
            var descriptionBox = new TextBox { Text = item.Description };
            var priceBox = new TextBox { Text = item.Price.ToString() };
            var quantityBox = new TextBox { Text = item.Quantity.ToString() };

            var imagePreview = new Image
            {
                Width = 120,
                Height = 120,
                Stretch = Stretch.UniformToFill,
                Source = LoadImageSource(item.Photo),
            };

            var imagePath = new string?[] { item.Photo };
            var dropZone = this.BuildDropZone("Drag image here or click to change", imagePreview, imagePath);

            var dialog = BuildItemDialog("Edit Shop Item", button, new StackPanel
            {
                Spacing = 15,
                Children =
                {
                    new TextBlock { Text = "Item Name", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                    nameBox,
                    new TextBlock { Text = "Description", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                    descriptionBox,
                    dropZone,
                    imagePreview,
                    new TextBlock { Text = "Price", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                    priceBox,
                    new TextBlock { Text = "Quantity", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                    quantityBox,
                },
            });

            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
            {
                return;
            }

            try
            {
                this.ViewModel.UpdateItem(item, nameBox.Text, descriptionBox.Text, priceBox.Text, quantityBox.Text, imagePath[0] ?? item.Photo);
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(button.XamlRoot, ex.Message);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs evemtArguments)
        {
            if (sender is not Button button || button.Tag is not ShopItem item)
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
                XamlRoot = button.XamlRoot,
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                this.ViewModel.DeleteItem(item);
            }
        }

        private async void AddItemToCartButton_Click(object sender, RoutedEventArgs eventArguments)
        {
            if ((sender as Button)?.Tag is not ShopItem item)
            {
                return;
            }

            try
            {
                this.ViewModel.AddToCart(item, 1);
            }
            catch (InvalidOperationException ex)
            {
                await ShowErrorAsync((sender as Button).XamlRoot, ex.Message);
            }
        }

        private void CartButton_Click(object sender, RoutedEventArgs eventArguments)
        {
            this.Frame.Navigate(typeof(CartPage));
        }

        private void BackToShops_Click(object sender, RoutedEventArgs eventArguments)
        {
            this.Frame.Navigate(typeof(ShopPage));
        }

        private void BackToLandingPage_Click(object sender, RoutedEventArgs eventArguments)
        {
            this.Frame.Navigate(typeof(LandingPage));
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs eventArguments)
        {
            if (eventArguments.ClickedItem is not ShopItem item)
            {
                return;
            }

            this.Frame.Navigate(typeof(ItemDetailsPage), new ItemDetailsNavArgs(item, selectedShop));
        }

        private static ImageSource LoadImageSource(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            try
            {
                if (path.StartsWith("Assets/"))
                {
                    return new BitmapImage(new Uri($"ms-appx:///{path}"));
                }

                return new BitmapImage(new Uri(path));
            }
            catch
            {
                return null;
            }
        }

        private Border BuildDropZone(string label, Image imagePreview, string?[] imagePath)
        {
            var dropZone = new Border
            {
                Height = 140,
                Background = new SolidColorBrush(Windows.UI.Color.FromArgb(30, 0, 0, 0)),
                AllowDrop = true,
                Child = new TextBlock
                {
                    Text = label,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
            };

            dropZone.Drop += async (dropSender, dropArgs) =>
            {
                if (!dropArgs.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    return;
                }

                var items = await dropArgs.DataView.GetStorageItemsAsync();
                if (items.FirstOrDefault() is StorageFile file)
                {
                    await ApplyImageFile(file, imagePreview, imagePath);
                }
            };

            dropZone.Tapped += async (tapSender, tapArgs) =>
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(App.MainWindow));
                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".jpg");

                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    await ApplyImageFile(file, imagePreview, imagePath);
                }
            };

            return dropZone;
        }

        private static async Task ApplyImageFile(StorageFile file, Image imagePreview, string?[] imagePath)
        {
            using var stream = await file.OpenAsync(FileAccessMode.Read);
            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(stream);
            imagePreview.Source = bitmap;
            imagePath[0] = file.Path;
        }

        private static ContentDialog BuildItemDialog(string title, Button anchor, StackPanel content)
        {
            return new ContentDialog
            {
                Title = title,
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Save",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = anchor.XamlRoot,
                Content = new ScrollViewer
                {
                    MaxHeight = 500,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollMode = ScrollMode.Auto,
                    Content = content,
                },
            };
        }

        private static async Task ShowErrorAsync(XamlRoot xamlRoot, string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = xamlRoot,
            };
            await dialog.ShowAsync();
        }
    }

    public class ItemDetailsNavArgs
    {
        public ShopItem Item { get; }

        public Shop Shop { get; }

        public ItemDetailsNavArgs(ShopItem item, Shop shop)
        {
            this.Item = item;
            this.Shop = shop;
        }
    }
}