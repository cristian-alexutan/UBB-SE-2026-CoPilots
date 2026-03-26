using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Content.Service;
using Content.User;
using Content.Domain;

namespace Content.ViewModel
{
    public class CartViewModel : INotifyPropertyChanged
    {
        private readonly MainService _service;
        private readonly UserSession _session;
        private int _currentReservationId; // Stores the active reservation ID for cancellations


        public ObservableCollection<CartShopItem> CartShopItems { get; set; }

        public bool IsAdmin => _session.IsAdmin;

        private bool _isReserved;
        public bool IsReserved
        {
            get => _isReserved;
            set
            {
                _isReserved = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsReserveButtonEnabled));
                OnPropertyChanged(nameof(CancelButtonVisibility));
            }
        }

        public bool IsReserveButtonEnabled => !IsReserved;
        public Visibility CancelButtonVisibility => IsReserved ? Visibility.Visible : Visibility.Collapsed;

        private double _overallTotal;
        public string OverallTotal => $"${_overallTotal:0.00}";

        public CartViewModel(MainService service, UserSession session)
        {
            _service = service;
            _session = session;

            CartShopItems = new ObservableCollection<CartShopItem>();

            LoadCartItems();
        }

        private void LoadCartItems()
        {
            CartShopItems.Clear();

            // Fetch the cart using the current user's ID
            var cart = _service.cartService.GetCartById(_session.UserId);

            if (cart != null && cart.CartItems != null)
            {

                foreach (var dbCartItem in cart.CartItems.Values)
                {
                    CartShopItems.Add(new CartShopItem
                    {
                        CartItemId = dbCartItem.Id,
                        ShopItem = dbCartItem.ShopItem,
                        Quantity = dbCartItem.Quantity
                    });
                }
            }

            CalculateOverallTotal();
        }


        public void Reload()
        {
            LoadCartItems();
        }
        public void ChangeQuantity(CartShopItem item, int newQuantity)
        {
            item.Quantity = newQuantity;

            // Hooked to CartService
            _service.cartService.UpdateItemQuantity(_session.UserId, item.CartItemId, newQuantity);

            CalculateOverallTotal();
        }

        public void RemoveShopItem(CartShopItem item)
        {
            CartShopItems.Remove(item);

            // Hooked to CartService
            _service.cartService.RemoveItemFromCart(_session.UserId, item.CartItemId);

            CalculateOverallTotal();
        }

        public void EmptyCart()
        {
            CartShopItems.Clear();

            // Hooked to CartService
            _service.cartService.ClearCart(_session.UserId);

            CalculateOverallTotal();
        }

        public void ReserveCart()
        {
            var cart = _service.cartService.GetCartById(_session.UserId);

            // Creating a new Reservation based on the commented constructor in ReservationService.cs
            // Passing 0 for ID assuming the database auto-increments it upon Add
            var newReservation = new Reservation(0, cart, true, DateTime.Now);

            _service.reservationService.reserveCart(newReservation);

            // Storing the ID locally so we can cancel it during this session if needed
            _currentReservationId = newReservation.Id;
            IsReserved = true;
        }

        public void CancelReservation()
        {
            // Hooked to ReservationService
            _service.reservationService.cancelReservation(_currentReservationId);

            IsReserved = false;
        }

        private void CalculateOverallTotal()
        {

            if (CartShopItems == null || !CartShopItems.Any())
            {
                _overallTotal = 0;
            }
            else
            {
                _overallTotal = CartShopItems.Sum(i => i.Quantity * (i.ShopItem?.Price ?? 0));
            }

            OnPropertyChanged(nameof(OverallTotal));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Wrapper class for the UI
    public class CartShopItem : INotifyPropertyChanged
    {
        // Added to map back to the CartItem in the database
        public int CartItemId { get; set; }
        public string DisplayPrice => ShopItem != null ? $"${ShopItem.Price:0.00}" : "$0.00";

        public ShopItem ShopItem { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ItemTotalPrice));
            }
        }

        // Includes a null check on ShopItem to prevent crashes during bindings
        public string ItemTotalPrice => ShopItem != null ? $"${(Quantity * ShopItem.Price):0.00}" : "$0.00";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}