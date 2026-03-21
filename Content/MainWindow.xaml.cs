using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Content.Service; // make sure your services namespace is included
using Content.Domain;

namespace Content
{
    public sealed partial class MainWindow : Window
    {
        private readonly ManagerService _managerService;
        private readonly TicketService _ticketService;

        public MainWindow(ManagerService managerService, TicketService ticketService)
        {
            this.InitializeComponent();

            _managerService = managerService;
            _ticketService = ticketService;
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }
    }
}