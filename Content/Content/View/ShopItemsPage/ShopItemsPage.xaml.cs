using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Content
{
    public sealed partial class ShopItemsPage : Window
    {
        public ShopItemsPage()
        {
            InitializeComponent();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var dialog = new ContentDialog
            {
                Title = "Add Item",
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Save",
                XamlRoot = button.XamlRoot,
                PrimaryButtonStyle = new Style(typeof(Button))
                {
                    Setters =
                        {
                            new Setter(Button.BackgroundProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(
                                Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x2B, 0xB8, 0xC0))),
                            new Setter(Button.CornerRadiusProperty, new CornerRadius(5)),
                            new Setter(Button.ForegroundProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White))
                        }
                },

                Content = new ScrollViewer
                {
                    Content = new StackPanel
                    {
                        Spacing = 25,
                        Children =
                        {
                             new Microsoft.UI.Xaml.Shapes.Rectangle
                            {
                                Height = 1,
                                Fill = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGray),
                                Margin = new Microsoft.UI.Xaml.Thickness(0, 10, 0, 0)
                            },
                            new TextBlock { Text = "Item Name", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            new TextBox { PlaceholderText = "Enter item name" },

                            new TextBlock{ Text = "Image", FontWeight = Microsoft.UI.Text.FontWeights.Bold},

                            new StackPanel
                            {
                                Spacing = 10,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Padding = new Microsoft.UI.Xaml.Thickness(12, 8, 12, 8),
                                BorderThickness = new Microsoft.UI.Xaml.Thickness(1),
                                CornerRadius = new Microsoft.UI.Xaml.CornerRadius(10),
                                BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGray),
                                Children =
                                {

                                    new FontIcon
                                    {
                                        Glyph = "\uE7C5",
                                        FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe Fluent Icons"),
                                        FontSize = 20
                                    },
                                    new TextBlock
                                    {
                                        Text = "Click to upload or drag and drop"
                                    },
                                    new TextBlock
                                    {
                                        Text = "PNG, JPG up to 10MB",
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGray)
                                    }
                                }
                            },

                            new TextBlock { Text = "Description", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            new TextBox { PlaceholderText = "Enter description", AcceptsReturn = true, Height = 80 },

                            new Grid
                            {
                                ColumnSpacing = 10,

                                Children =
                                {
                                    new Microsoft.UI.Xaml.Controls.NumberBox
                                    {
                                        Header = "Stock",
                                        SmallChange = 1,
                                        LargeChange = 10,
                                        SpinButtonPlacementMode = Microsoft.UI.Xaml.Controls.NumberBoxSpinButtonPlacementMode.Inline,
                                        Value = 0,
                                        Minimum = 0,
                                        HorizontalAlignment = HorizontalAlignment.Left
                                    },

                                    new Microsoft.UI.Xaml.Controls.NumberBox
                                    {
                                        Header = "Price ($)",
                                        SmallChange = 1,
                                        LargeChange = 10,
                                        SpinButtonPlacementMode = Microsoft.UI.Xaml.Controls.NumberBoxSpinButtonPlacementMode.Inline,
                                        Value = 0,
                                        Minimum = 0,
                                        HorizontalAlignment = HorizontalAlignment.Right
                                    }
                                }
                            }

                        }
                    }
                }

            };

            await dialog.ShowAsync();
        }
        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var dialog = new ContentDialog
            {
                Title = "Edit Item",
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Save",
                XamlRoot = button.XamlRoot,
                PrimaryButtonStyle = new Style(typeof(Button))
                {
                    Setters =
                        {
                            new Setter(Button.BackgroundProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(
                                Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x2B, 0xB8, 0xC0))),
                            new Setter(Button.CornerRadiusProperty, new CornerRadius(5)),
                            new Setter(Button.ForegroundProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White))
                        }
                },

                Content = new ScrollViewer
                {
                    Content = new StackPanel
                    {
                        Spacing = 25,
                        Children =
                        {
                             new Microsoft.UI.Xaml.Shapes.Rectangle
                            {
                                Height = 1,
                                Fill = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGray),
                                Margin = new Microsoft.UI.Xaml.Thickness(0, 10, 0, 0)
                            },
                            new TextBlock { Text = "Item Name", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            new TextBox { PlaceholderText = "Enter item name" },

                            new TextBlock{ Text = "Image", FontWeight = Microsoft.UI.Text.FontWeights.Bold},

                            new StackPanel
                            {
                                Spacing = 10,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Padding = new Microsoft.UI.Xaml.Thickness(12, 8, 12, 8),
                                BorderThickness = new Microsoft.UI.Xaml.Thickness(1),
                                CornerRadius = new Microsoft.UI.Xaml.CornerRadius(10),
                                BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGray),
                                Children =
                                {

                                    new FontIcon
                                    {
                                        Glyph = "\uE7C5",
                                        FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe Fluent Icons"),
                                        FontSize = 20
                                    },
                                    new TextBlock
                                    {
                                        Text = "Click to upload or drag and drop"
                                    },
                                    new TextBlock
                                    {
                                        Text = "PNG, JPG up to 10MB",
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGray)
                                    }
                                }
                            },

                            new TextBlock { Text = "Description", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            new TextBox { PlaceholderText = "Enter description", AcceptsReturn = true, Height = 80 },

                            new Grid
                            {
                                ColumnSpacing = 10,

                                Children =
                                {
                                    new Microsoft.UI.Xaml.Controls.NumberBox
                                    {
                                        Header = "Stock",
                                        SmallChange = 1,
                                        LargeChange = 10,
                                        SpinButtonPlacementMode = Microsoft.UI.Xaml.Controls.NumberBoxSpinButtonPlacementMode.Inline,
                                        Value = 0,
                                        Minimum = 0,
                                        HorizontalAlignment = HorizontalAlignment.Left
                                    },

                                    new Microsoft.UI.Xaml.Controls.NumberBox
                                    {
                                        Header = "Price ($)",
                                        SmallChange = 1,
                                        LargeChange = 10,
                                        SpinButtonPlacementMode = Microsoft.UI.Xaml.Controls.NumberBoxSpinButtonPlacementMode.Inline,
                                        Value = 0,
                                        Minimum = 0,
                                        HorizontalAlignment = HorizontalAlignment.Right
                                    }
                                }
                            }

                        }
                    }
                }

            };

            await dialog.ShowAsync();
        }

    }
}