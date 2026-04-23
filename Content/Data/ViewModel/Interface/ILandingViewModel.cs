using System.Windows.Input;

namespace Content.ViewModel.Interface
{
    public interface ILandingViewModel
    {
        bool IsRoleSelected { get; }
        string ErrorMessage { get; }
        ICommand SelectAdminCommand { get; }
        ICommand SelectClientCommand { get; }
    }
}
