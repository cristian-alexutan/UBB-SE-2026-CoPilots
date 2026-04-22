using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.UI.Xaml;
namespace Content.ViewModel.Interface
{
    public interface ICartViewModel
    {
        Visibility CancelButtonVisibility { get; }
        ObservableCollection<CartShopItem> CartShopItems { get; set; }
        bool IsAdmin { get; }
        bool IsReserveButtonEnabled { get; }
        bool IsReserved { get; set; }
        string OverallTotal { get; }

        event PropertyChangedEventHandler PropertyChanged;

        void CancelReservation();
        void ChangeQuantity(CartShopItem item, int newQuantity);
        void EmptyCart();
        void Reload();
        void RemoveShopItem(CartShopItem item);
        void ReserveCart();
    }
}