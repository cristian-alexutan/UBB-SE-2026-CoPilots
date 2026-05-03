using System.Windows.Input;
using Content.Data.Service.Interface;
using CommunityToolkit.Mvvm.Input;
using Content.User;
using Content.ViewModel.Interface;

namespace Content.ViewModel
{
    public class LandingViewModel : ILandingViewModel
    {
        private readonly IClientService clientService;
        private readonly IManagerService managerService;
        private readonly UserSession session;

        public bool IsRoleSelected { get; private set; }

        public string ErrorMessage { get; private set; }

        public ICommand SelectAdminCommand { get; }
        public ICommand SelectClientCommand { get; }

        public LandingViewModel(IClientService clientService, IManagerService managerService, UserSession session)
        {
            this.clientService = clientService;
            this.managerService = managerService;
            this.session = session;

            SelectAdminCommand = new RelayCommand(SetAdmin);
            SelectClientCommand = new RelayCommand(SetClient);
        }

        private void SetAdmin()
        {
            try
            {
                var manager = managerService.GetAnyManager();
                session.SetAdmin(manager.Id);
                IsRoleSelected = true;
            }
            catch (Exception)
            {
                ErrorMessage = "No admin found.";
            }
        }

        private void SetClient()
        {
            try
            {
                var client = clientService.GetAnyClient();
                session.SetClient(client.Id);
                IsRoleSelected = true;
            }
            catch
            {
                ErrorMessage = "No client found.";
            }
        }
    }
}
