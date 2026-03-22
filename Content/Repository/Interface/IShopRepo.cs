using Content.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository.Interface
{
    public interface IShopRepo
    {
        IEnumerable<Shop> GetAll();
        Shop GetById(int Id);
        void Add(Shop Shop);
        void Delete(int Id);
        void Update(Shop Shop);
    }
}
