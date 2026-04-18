using Content.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository.Interface
{
    public interface IManagerRepo
    {
        IEnumerable<Manager> GetAll();
        Manager GetById(int Id);
        void Add(Manager Manager);
        void Delete(int Id);
    }
}
