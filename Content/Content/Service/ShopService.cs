using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Content.Domain;
using Content.Repository.Interface;

namespace Content.Service;

public class ShopService
{
    private readonly IShopRepo _shopRepo;

    public ShopService(IShopRepo shopRepo)
    {
        _shopRepo = shopRepo;
    }

    public IEnumerable<Shop> GetAllAvailableShops()
    {
        return _shopRepo.GetAll();
    }

    public Shop SelectShopById(int id)
    {
        if (id < 0)
        {
            throw new ArgumentException("ID must not be negative");
        }

        var shop = _shopRepo.GetById(id);

        if (shop == null)
        {
            throw new KeyNotFoundException("Shop with given ID not found");
        }

        return shop;
    }

    public void AddShop(Shop shop)
    {

        var nameExists = _shopRepo.GetAll().Any(s => string.Equals(s.Name, shop.Name));

        if (nameExists)
        {
            throw new ArgumentException("Shop name already exists");
        }


        _shopRepo.Add(shop);
    }

    public void DeleteShop(int id)
    {

        _shopRepo.Delete(id);
    }

    // Sorted by name
    public IEnumerable<Shop> GetShopsSorted()
    {
        return _shopRepo.GetAll().OrderBy(shop => shop.Name); 
    }

    public Shop FindByName(string name)
    {
        return _shopRepo.GetAll().FirstOrDefault(shop => string.Equals(shop.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public void Update(Shop shop)
    {
        if (string.IsNullOrWhiteSpace(shop.Name))
        {
            throw new ArgumentException("Name field must not be empty");
        }

        var isDuplicate = _shopRepo.GetAll().Any(s =>
            s.Id != shop.Id && string.Equals(s.Name, shop.Name));

        if (isDuplicate)
        {
            throw new ArgumentException("Shop with given name already exists");
        }

        _shopRepo.Update(shop);
    }

}