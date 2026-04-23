using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.User
{
    public class UserSession
    {
        public int UserId { get; private set; }

        public bool IsAdmin { get; private set; }

        public void SetAdmin(int managerId)
        {
            UserId = managerId;
            IsAdmin = true;
        }

        public void SetClient(int clientId)
        {
            UserId = clientId;
            IsAdmin = false;
        }
    }
}
