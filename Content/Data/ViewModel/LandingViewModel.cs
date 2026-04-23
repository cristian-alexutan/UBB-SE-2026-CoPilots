using System.Windows.Input;
using Content.Data.Service.Interface;
using Content.Helper;
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
            var manager = managerService.GetAnyManager();
            if (manager == null)
            {
                ErrorMessage = "No admin found.";
                return;
            }
            session.SetAdmin(manager.Id);
            IsRoleSelected = true;
        }

        private void SetClient()
        {
            var client = clientService.GetAnyClient();
            if (client == null)
            {
                ErrorMessage = "No client found.";
                return;
            }
            session.SetClient(client.Id);
            IsRoleSelected = true;
        }
    }
}
