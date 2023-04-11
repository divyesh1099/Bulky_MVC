using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
       
        public void Update(Product obj)
        {
            // _db.Products.Update(obj); (Commented this because we needed the custom updation because of null or not null image URL
            Product? productFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if(productFromDb != null)
            {
                productFromDb.Title = obj.Title;
                productFromDb.Description = obj.Description;
                productFromDb.ISBN = obj.ISBN;
                productFromDb.Author = obj.Author;
                productFromDb.ListPrice = obj.ListPrice;
                productFromDb.Price = obj.Price;
                productFromDb.Price50 = obj.Price50;
                productFromDb.Price100 = obj.Price100;
                productFromDb.CategoryId = obj.CategoryId;
                if(obj.ImageURL != null)
                {
                    productFromDb.ImageURL = obj.ImageURL;
                }
            }
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
