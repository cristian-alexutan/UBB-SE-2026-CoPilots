using System;
﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Content.Domain;

namespace Content.Data.ViewModel.Interface
{
    public interface IShopPageViewModel : INotifyPropertyChanged
    {
        bool CanAddShop { get; }
        double CartOpacity { get; }
        bool IsAdmin { get; }
        bool IsCartEnabled { get; }
        ObservableCollection<Shop> Shops { get; }

        IRelayCommand<Shop> DeleteShopCommand { get; }
        IRelayCommand<string> SearchCommand { get; }
        IRelayCommand SortByReviewsCommand { get; }
        IRelayCommand SortAlphabeticallyCommand { get; }
        IRelayCommand LoadShopsCommand { get; }

        event PropertyChangedEventHandler? PropertyChanged;

        void AddShop(string name, string type);
        void DeleteShop(Shop shop);
        void EditShop(Shop shop, string newName, string newType);
        void Search(string query);
        void SortAlphabetically();
        void SortByReviews();
        void LoadShops();
    }
}