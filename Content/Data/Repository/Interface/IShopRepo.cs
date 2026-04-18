namespace Content.Repository.Interface
{
    using System.Collections.Generic;
    using Content.Domain;

    public interface IShopRepo
    {
        IEnumerable<Shop> GetAll();

        Shop GetById(int id);

        void Add(Shop shop);

        Shop? Delete(int id);

        Shop? Update(Shop shop);
    }
}
