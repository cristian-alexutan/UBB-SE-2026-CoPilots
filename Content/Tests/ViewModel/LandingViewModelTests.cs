using System;
using Content.Data.Service.Interface;
using Content.Domain;
using Content.User;
using Content.ViewModel;
using NSubstitute;
using NUnit.Framework;

namespace Tests.ViewModel
{
    [TestFixture]
    public class LandingViewModelTests
    {
        private IClientService clientService = null!;
        private IManagerService managerService = null!;
        private UserSession session = null!;

        [SetUp]
        public void Setup()
        {
            clientService = Substitute.For<IClientService>();
            managerService = Substitute.For<IManagerService>();
            session = new UserSession();
        }

        [Test]
        public void SetAdminTestSuccessful()
        {
            Manager manager = new Manager(7, "John", "john@email.com", "0712345678");
            managerService.GetAnyManager().Returns(manager);
            LandingViewModel vm = new LandingViewModel(clientService, managerService, session);

            vm.SelectAdminCommand.Execute(null);

            Assert.That(vm.IsRoleSelected, Is.True);
            Assert.That(session.IsAdmin, Is.True);
        }

        [Test]
        public void SetAdminTestUnsuccessful_AdminIsNull()
        {
            managerService.GetAnyManager().Returns((Manager)null!);
            LandingViewModel vm = new LandingViewModel(clientService, managerService, session);

            vm.SelectAdminCommand.Execute(null);

            Assert.That(vm.IsRoleSelected, Is.False);
            Assert.That(vm.ErrorMessage, Is.EqualTo("No admin found."));
        }

        [Test]
        public void SetClientTestSuccessful()
        {
            Client client = new Client(3, "Jane");
            clientService.GetAnyClient().Returns(client);
            LandingViewModel vm = new LandingViewModel(clientService, managerService, session);

            vm.SelectClientCommand.Execute(null);

            Assert.That(vm.IsRoleSelected, Is.True);
            Assert.That(session.IsAdmin, Is.False);
        }

        [Test]
        public void SetClientTestUnsuccessful_ClientIsNull()
        {
            clientService.GetAnyClient().Returns((Client)null!);
            LandingViewModel vm = new LandingViewModel(clientService, managerService, session);

            vm.SelectClientCommand.Execute(null);

            Assert.That(vm.IsRoleSelected, Is.False);
            Assert.That(vm.ErrorMessage, Is.EqualTo("No client found."));
        }
    }
}