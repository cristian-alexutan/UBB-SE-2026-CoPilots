using System.Windows.Input;
using Content.Data.Service.Interface;
using Content.Helper;
using Content.Service;
using Content.User;

namespace Content.ViewModel
{
    public class LandingViewModel
    {
        private readonly IManagerService managerService;
        private readonly IClientService clientService;
        private readonly UserSession session;

        public bool IsRoleSelected { get; private set; }

        public string ErrorMessage { get; private set; }

        public ICommand SelectAdminCommand { get; }
        public ICommand SelectClientCommand { get; }

        public LandingViewModel(IManagerService managerService, IClientService clientService, UserSession session)
        {
            this.managerService = managerService;
            this.clientService = clientService;
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
