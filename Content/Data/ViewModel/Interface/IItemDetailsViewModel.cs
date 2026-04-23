using System.Windows.Input;

namespace Content.ViewModel.Interface
{
    public interface IItemDetailsViewModel
    {
        ICommand AddToCartCommand { get; }
        ICommand UpdateItemCommand { get; }
    }
}
