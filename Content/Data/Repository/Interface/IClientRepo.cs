using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository.Interface
{
    public interface IClientRepo
    {
        IEnumerable<Domain.Client> GetAll();
        Domain.Client GetById(int Id);
        void Add(Domain.Client Client);
        void Delete(int Id);

    }
}
