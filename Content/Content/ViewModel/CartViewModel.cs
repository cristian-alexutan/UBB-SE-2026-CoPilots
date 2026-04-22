using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Content.Domain;
using Content.Service;
using Content.User;
using Content.ViewModel.Interface;

namespace Content.ViewModel
{
    public class CartViewModel : INotifyPropertyChanged, ICartViewModel
    {
        private readonly MainService service;
        private readonly UserSession session;
        private int currentReservationId;

        private bool isReserved;

        private double overallTotal;

        public ObservableCollection<CartShopItem> CartShopItems { get; set; }

        public bool IsAdmin => this.session.IsAdmin;

        public bool IsReserved
        {
            get
            {
                return this.isReserved;
            }

            set
            {
                this.isReserved = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.IsReserveButtonEnabled));
                this.OnPropertyChanged(nameof(this.CancelButtonVisibility));
            }
        }

        public bool IsReserveButtonEnabled => !this.IsReserved;

        public Visibility CancelButtonVisibility => this.IsReserved ? Visibility.Visible : Visibility.Collapsed;

        public string OverallTotal => $"${this.overallTotal:0.00}";

        public event PropertyChangedEventHandler PropertyChanged;

        public CartViewModel(MainService service, UserSession session)
        {
            this.service = service;
            this.session = session;
            this.CartShopItems = new ObservableCollection<CartShopItem>();
            this.LoadCartItems();
            this.CheckExistingReservation();
        }

        public void Reload()
        {
            this.LoadCartItems();
        }

        public void ChangeQuantity(CartShopItem item, int newQuantity)
        {
            this.service.CartService.UpdateItemQuantity(this.session.UserId, item.CartItemId, newQuantity);
            item.Quantity = newQuantity;
            this.CalculateOverallTotal();
        }

        public void RemoveShopItem(CartShopItem item)
        {
            this.service.CartService.RemoveItemFromCart(this.session.UserId, item.CartItemId);
            this.CartShopItems.Remove(item);
            this.CalculateOverallTotal();
        }

        public void EmptyCart()
        {
            this.service.CartService.ClearCart(this.session.UserId);
            this.CartShopItems.Clear();
            this.CalculateOverallTotal();
        }

        public void ReserveCart()
        {
            var cart = this.service.CartService.GetCartById(this.session.UserId);
            var newReservation = new Reservation(cart, true, DateTime.Now);
            this.service.ReservationService.ReserveCart(newReservation);
            this.currentReservationId = newReservation.Id;
            this.IsReserved = true;
        }

        public void CancelReservation()
        {
            this.service.ReservationService.CancelReservation(this.currentReservationId);
            this.IsReserved = false;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CheckExistingReservation()
        {
            var cart = this.service.CartService.GetCartById(this.session.UserId);
            if (cart == null)
            {
                return;
            }

            var allReservations = this.service.ReservationService.GetAllReservations();
            var activeReservation = allReservations.FirstOrDefault(r =>
                r.ReservationCart.Id == cart.Id && r.Active);

            if (activeReservation != null)
            {
                this.currentReservationId = activeReservation.Id;
                this.IsReserved = true;
            }
        }

        private void LoadCartItems()
        {
            this.CartShopItems.Clear();
            var cart = this.service.CartService.GetCartById(this.session.UserId);

            if (cart != null && cart.CartItems != null)
            {
                foreach (var dbCartItem in cart.CartItems.Values)
                {
                    this.CartShopItems.Add(new CartShopItem
                    {
                        CartItemId = dbCartItem.Id,
                        ShopItem = dbCartItem.ShopItem,
                        Quantity = dbCartItem.Quantity,
                    });
                }
            }

            this.CalculateOverallTotal();
        }

        private void CalculateOverallTotal()
        {
            if (this.CartShopItems == null || !this.CartShopItems.Any())
            {
                this.overallTotal = 0;
            }
            else
            {
                this.overallTotal = this.CartShopItems.Sum(i => i.Quantity * (i.ShopItem?.Price ?? 0));
            }

            this.OnPropertyChanged(nameof(this.OverallTotal));
        }
    }

    public class CartShopItem : INotifyPropertyChanged
    {
        private int quantity;

        public int CartItemId { get; set; }

        public ShopItem ShopItem { get; set; }

        public string DisplayPrice => this.ShopItem != null ? $"${this.ShopItem.Price:0.00}" : "$0.00";

        public string ItemTotalPrice => this.ShopItem != null ? $"${(this.Quantity * this.ShopItem.Price):0.00}" : "$0.00";

        public int Quantity
        {
            get
            {
                return this.quantity;
            }

            set
            {
                this.quantity = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.ItemTotalPrice));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}