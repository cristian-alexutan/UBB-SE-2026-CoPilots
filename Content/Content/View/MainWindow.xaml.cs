using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Content
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.RootFrame.Navigate(typeof(LandingPage));
        }

        public Frame Frame => this.RootFrame;
    }
}
