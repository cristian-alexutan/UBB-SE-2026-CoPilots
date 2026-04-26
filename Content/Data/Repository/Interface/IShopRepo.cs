namespace Content.Repository.Interface
{
    using System.Collections.Generic;
    using Content.Domain;

    public interface IShopRepo
    {
        IEnumerable<Shop> GetAll();

        Shop? GetById(int shopId);

        void Add(Shop shop);

        Shop? Delete(int shopId);

        Shop? Update(Shop shop);
    }
}
