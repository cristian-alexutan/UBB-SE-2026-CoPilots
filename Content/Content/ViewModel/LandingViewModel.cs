using Content.Domain;
using Content.Helper;
using Content.Service;
using Content.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Content.ViewModel
{
    public class LandingViewModel
    {
        private readonly MainService _service;
        private readonly UserSession _session;

        public bool IsRoleSelected { get; private set; }

        public string ErrorMessage { get; private set; }

        public ICommand SelectAdminCommand { get; }
        public ICommand SelectClientCommand { get; }

        public LandingViewModel(MainService service, UserSession session)
        {
            _service = service;
            _session = session;

            SelectAdminCommand = new RelayCommand(SetAdmin);
            SelectClientCommand = new RelayCommand(SetClient);
        }

        private void SetAdmin()
        {
            var manager = _service.managerService.GetAnyManager();
            if (manager == null)
            {
                ErrorMessage = "No admin found.";
                return;
            }
            _session.SetAdmin(manager.Id);
            IsRoleSelected = true;
        }

        private void SetClient()
        {
            var client = _service.clientService.GetAnyClient();
            if (client == null)
            {
                ErrorMessage = "No client found.";
                return;
            }
            _session.SetClient(client.Id);
            IsRoleSelected = true;
        }
    }
}
