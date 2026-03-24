using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Content.Service; 
using Content.Domain;

namespace Content
{
    public sealed partial class MainWindow : Window
    {
        private readonly MainService _mainService;

        public MainWindow(MainService mainService)
        {
            this.InitializeComponent();

            _mainService = mainService;
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }
    }
}