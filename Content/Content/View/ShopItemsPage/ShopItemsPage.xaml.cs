using Content.Domain;
using Content.Service;
using Content.User;
using Content.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Content
{

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
        private ShopItemsViewModel ViewModel;
        private readonly UserSession _session;
        private readonly Shop _currentShop;

        public ShopItemsPage(MainService service, UserSession session, Shop currentShop)
        {
            this.InitializeComponent();
            _session = session;
            _currentShop = currentShop;

            ViewModel = new ShopItemsViewModel(service, session, currentShop);
            RootGrid.DataContext = ViewModel;

            // Set default active sort button
            SetActiveSortButton(SortAlphabeticButton);
            _currentShop = currentShop;

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
                XamlRoot=RootGrid.XamlRoot
            };

            _ = dialog.ShowAsync();
        }

        private void SetActiveSortButton(Button activeButton)
        {
            // Reset all buttons to default style
            ResetButtonStyle(SortAlphabeticButton);
            ResetButtonStyle(SortComplaintsButton);
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

        private void SortComplaints_Click(object sender, RoutedEventArgs e)
        {
            /*SetActiveSortButton(SortComplaintsButton);
            var sortedItems = _allItems.OrderBy(item => item.Complaints).ToList();
            ItemsGridView.ItemsSource = sortedItems;*/
        }

        private void SortPrice_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSortButton(SortPriceButton);
            ViewModel.SortByPrice();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_session.IsAdmin) return;

            var button = sender as Button;

            var nameBox = new TextBox { PlaceholderText = "Enter item name" };
            var descBox = new TextBox { PlaceholderText = "Enter description" };
            var priceBox = new TextBox { PlaceholderText = "Enter price" };
            var quantityBox = new TextBox { PlaceholderText = "Enter quantity" };

            var dialog = new ContentDialog
            {
                Title = "Add Shop Item",
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Save",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = button.XamlRoot,
                Content = new StackPanel
                {
                    Spacing = 15,
                    Children =
                    {
                        new TextBlock { Text = "Item Name", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                        nameBox,
                        new TextBlock { Text = "Description", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                        descBox,
                        new TextBlock { Text = "Price", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                        priceBox,
                        new TextBlock { Text = "Quantity", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                        quantityBox
                    }
                }
            };

            var result = await dialog.ShowAsync();
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
                    var newItem = new ShopItem();

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
                    else if (_activeSortButton == SortComplaintsButton)
                        SortComplaints_Click(null, null);
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

                var dialog = new ContentDialog
                {
                    Title = "Edit Shop Item",
                    CloseButtonText = "Cancel",
                    PrimaryButtonText = "Save",
                    RequestedTheme = ElementTheme.Light,
                    XamlRoot = button.XamlRoot,
                    Content = new StackPanel
                    {
                        Spacing = 15,
                        Children =
                        {
                            new TextBlock { Text = "Item Name", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            nameBox,
                            new TextBlock { Text = "Description", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            descBox,
                            new TextBlock { Text = "Price", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            priceBox,
                            new TextBlock { Text = "Quantity", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            quantityBox
                        }
                    }
                };

                var result = await dialog.ShowAsync();
                if (string.IsNullOrWhiteSpace(nameBox.Text) ||
                string.IsNullOrWhiteSpace(priceBox.Text) ||
                string.IsNullOrWhiteSpace(quantityBox.Text))
                {
                    ShowError("All fields must be filled!");
                    return;
                }
                if (result == ContentDialogResult.Primary)
                {
                    item.Name = nameBox.Text;
                    item.Description = descBox.Text;

                    if (float.TryParse(priceBox.Text, out float price))
                        item.Price = price;

                    if (int.TryParse(quantityBox.Text, out int quantity))
                        item.Quantity = quantity;

                    try { ViewModel.UpdateItem(item); }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message);
                    }

                    // Reapply current sort
                    if (_activeSortButton == SortAlphabeticButton)
                        SortAlphabetic_Click(null, null);
                    else if (_activeSortButton == SortPriceButton)
                        SortPrice_Click(null, null);
                    else if (_activeSortButton == SortComplaintsButton)
                        SortComplaints_Click(null, null);
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
                    else if (_activeSortButton == SortComplaintsButton)
                        SortComplaints_Click(null, null);
                }
            }
        }
    }
}