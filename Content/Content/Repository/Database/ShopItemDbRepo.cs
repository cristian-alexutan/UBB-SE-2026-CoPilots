using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository.Database
{
    public class ShopItemDbRepo : IShopItemRepo
    {
        private string ConnectionString;
        private IShopRepo _shopRepo;

        public ShopItemDbRepo(string ConnectionString, IShopRepo shopRepo)
        {
            this.ConnectionString = ConnectionString;
            this._shopRepo = shopRepo;
        }

        public IEnumerable<ShopItem> GetAll()
        {
            var ShopItems = new List<ShopItem>();

            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("SELECT * FROM Item", Conn);
                var Reader = Cmd.ExecuteReader();

                while (Reader.Read())
                {
                    int ShopId = (int)Reader["shop_id"];
                    var ShopItem = new ShopItem(
                        (int)Reader["item_id"],
                        (int)Reader["stock"],
                        (float)Reader["price"],
                        _shopRepo.GetById(ShopId),
                        (string)Reader["img"],
                        (string)Reader["name"],
                        (string)Reader["description"]
                    );
                    ShopItems.Add(ShopItem);
                }
            }

            return ShopItems;
        }

        public ShopItem GetById(int Id)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("SELECT * FROM Item WHERE item_id=@Id", Conn);
                Cmd.Parameters.AddWithValue("@Id", Id);

                var Reader = Cmd.ExecuteReader();
                if (Reader.Read())
                {
                    int ShopId = (int)Reader["shop_id"];
                    return new ShopItem(
                        (int)Reader["item_id"],
                        (int)Reader["stock"],
                        (float)Reader["price"],
                        _shopRepo.GetById(ShopId),
                        (string)Reader["img"],
                        (string)Reader["name"],
                        (string)Reader["description"]
                    );
                }
            }

            return null;
        }

        public void Add(ShopItem ShopItem)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand(
                    "INSERT INTO Item (shop_id, stock, price, img, name, description) VALUES (@ShopId, @Stock, @Price, @Img, @Name, @Description)",
                    Conn
                );
                Cmd.Parameters.AddWithValue("@ShopId", ShopItem.Shop.Id);
                Cmd.Parameters.AddWithValue("@Stock", ShopItem.Quantity);
                Cmd.Parameters.AddWithValue("@Price", ShopItem.Price);
                Cmd.Parameters.AddWithValue("@Img", ShopItem.Photo);
                Cmd.Parameters.AddWithValue("@Name", ShopItem.Name);
                Cmd.Parameters.AddWithValue("@Description", ShopItem.Description);
                Cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int Id)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("DELETE FROM Item WHERE item_id=@Id", Conn);
                Cmd.Parameters.AddWithValue("@Id", Id);
                Cmd.ExecuteNonQuery();
            }
        }

        public void Update(ShopItem ShopItem)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand(
                    "UPDATE Item SET shop_id=@ShopId, stock=@Stock, price=@Price, img=@Img, name=@Name, description=@Description WHERE item_id=@Id",
                    Conn
                );
                Cmd.Parameters.AddWithValue("@ShopId", ShopItem.Shop.Id);
                Cmd.Parameters.AddWithValue("@Stock", ShopItem.Quantity);
                Cmd.Parameters.AddWithValue("@Price", ShopItem.Price);
                Cmd.Parameters.AddWithValue("@Img", ShopItem.Photo);
                Cmd.Parameters.AddWithValue("@Name", ShopItem.Name);
                Cmd.Parameters.AddWithValue("@Description", ShopItem.Description);
                Cmd.Parameters.AddWithValue("@Id", ShopItem.Id);
                Cmd.ExecuteNonQuery();
            }
        }
    }
}
