using System;
using System.Collections.Generic;
using Content.Data.Service.Interface;
using Content.Domain;
using Content.Repository.Interface;

namespace Content.Service
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepo reservationRepo;
        private readonly IShopItemService shopItemService;
        private readonly ICartService cartService;

        public ReservationService(IReservationRepo reservationRepo, IShopItemService shopItemService, ICartService cartService)
        {
            this.reservationRepo = reservationRepo;
            this.shopItemService = shopItemService;
            this.cartService = cartService;
        }
        public IEnumerable<Reservation> GetAllReservations()
        {
            return reservationRepo.GetAll();
        }
        public Reservation GetReservationById(int reservationId)
        {
            return reservationRepo.GetById(reservationId);
        }
        public void ReserveCart(Reservation reservation)
        {
            var reservationCartItems = reservation.ReservationCart.CartItems;

            foreach (var cartItem in reservationCartItems.Values)
            {
                var shopItem = shopItemService.GetById(cartItem.ShopItem.Id);
                if (shopItem.Quantity < cartItem.Quantity)
                {
                    throw new Exception($"Not enough stock for '{shopItem.Name}'. " +
                        $"Requested: {cartItem.Quantity}, Available: {shopItem.Quantity}");
                }
            }

            foreach (var cartItem in reservationCartItems.Values)
            {
                var shopItem = shopItemService.GetById(cartItem.ShopItem.Id);
                shopItem.Quantity -= cartItem.Quantity;
                shopItemService.UpdateShopItem(shopItem);
            }

            reservationRepo.Add(reservation);
        }

        public Reservation GetActiveReservationForCart(int cartId)
        {
            foreach (var reservation in this.reservationRepo.GetAll())
            {
                if (reservation.ReservationCart.Id == cartId && reservation.Active)
                {
                    return reservation;
                }
            }

            return null;
        }

        public void DeleteReservation(int reservationId)
        {
            reservationRepo.Delete(reservationId);
        }

        public void CancelReservation(int reservationId)
        {
            Reservation reservation = reservationRepo.GetById(reservationId);

            if (!reservation.Active)
            {
                return;
            }

            if (reservation.ReservationCart?.CartItems != null)
            {
                foreach (var cartItem in reservation.ReservationCart.CartItems.Values)
                {
                    var shopItem = shopItemService.GetById(cartItem.ShopItem.Id);
                    shopItem.Quantity += cartItem.Quantity;
                    shopItemService.UpdateShopItem(shopItem);
                }
            }

            cartService.ClearCart(reservation.ReservationCart.Id);
            reservation.Active = false;

            reservationRepo.Update(reservation);
        }
    }
}