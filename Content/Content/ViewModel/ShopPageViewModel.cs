using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Content.Domain;
using Content.Service;
using Content.User;
using Content.ViewModel.Interface;
using Microsoft.UI.Xaml;
namespace Content.ViewModel
{
    public class ShopPageViewModel : IShopPageViewModel
    {
        private readonly MainService service;
        private readonly UserSession session;
        private List<Shop> allShops;

        public ObservableCollection<Shop> Shops { get; } = new ObservableCollection<Shop>();

        public bool IsAdmin => session.IsAdmin;
        public Visibility AddShopVisibility => session.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        public bool IsCartEnabled => !session.IsAdmin;
        public double CartOpacity => session.IsAdmin ? 0.4 : 1.0;

        public ShopPageViewModel(MainService service, UserSession session)
        {
            this.service = service;
            this.session = session;
            LoadItems();
        }

        public void LoadItems()
        {
            var shops = service.shopService.GetAllAvailableShops();
            allShops = shops.ToList();
            Shops.Clear();
            foreach (var s in allShops)
            {
                Shops.Add(s);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void AddShop(string name, string type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            service.shopService.AddShop(new Shop(name, type, session.UserId));
            LoadItems();
        }

        public void EditShop(Shop shop, string newName, string newType)
        {
            service.shopService.UpdateShop(new Shop(shop.Id, newName, newType, session.UserId));
            shop.Name = newName;
            shop.Type = newType;
            LoadItems();
        }

        public void DeleteShop(Shop shop)
        {
            service.shopService.DeleteShop(shop.Id);
            LoadItems();
        }
        public void Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                LoadItems();
                return;
            }

            var filtered = service.shopService.SearchByName(query).ToList();
            allShops = filtered;
            Shops.Clear();
            foreach (var item in filtered)
            {
                Shops.Add(item);
            }
        }

        public void SortByReviews()
        {
            if (allShops == null)
            {
                return;
            }

            var sorted = service.GetShopsSortedByTickets().ToList();

            allShops = sorted;

            Shops.Clear();
            foreach (var shop in sorted)
            {
                Shops.Add(shop);
            }
        }

        public void SortAlphabetically()
        {
            if (allShops == null)
            {
                return;
            }

            var sorted = service.shopService.SortAlphabetically(allShops).ToList();
            allShops = sorted;
            Shops.Clear();
            foreach (var shop in sorted)
            {
                Shops.Add(shop);
            }
        }
    }
}