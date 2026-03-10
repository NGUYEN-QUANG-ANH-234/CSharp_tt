using EF_core.Data;
using EF_core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EF_core.Repositories 
{
    class ProductsRepo
    {
        public static void InsertProduct(int id, string productName,string productDescription)
        {
            using var _dbContext = new MyDbContext();

            var p1 = new Products();
            p1.ProductId = id;
            p1.ProductName = productName;
            p1.ProductDescription = productDescription;

            _dbContext.Add(p1);

            //var products = new object[] {
            //    new Products(){ ProductName = "Iphone", ProductDescription = "Description_1" },
            //    new Products(){ ProductName = "SamSung", ProductDescription = "Description_2" },
            //    new Products(){ ProductName = "Oppo", ProductDescription = "Description_3" },
            //}; 

            //_dbContext.AddRange(products);

            // Tra ve so dong bi tac dong
            int number_rows = _dbContext.SaveChanges();
            Console.WriteLine($"Da co {number_rows} dong thay doi");
        }

        public static void ReadProduct()
        {
            using var _dbcontext = new MyDbContext();

            //var products = _dbcontext.products.ToList();
            //products.ForEach(product => product.PrintInfo());

            //var _products = from p in _dbcontext.products
            //               where p.ProductId % 2 == 0
            //               select p;

            var _products = _dbcontext.products.Where(p => p.ProductDescription.Contains("Description"))
                .OrderByDescending(p => p.ProductId);

            if (_products != null) 
            { 
                _products.ToList().ForEach(p => p.PrintInfo());
                
                Console.WriteLine("Da truy xuat doc noi dung bang Products thanh cong");
            }
        }

        public static void RenameProduct(int id, string newName)
        {
            using var _dbContext = new MyDbContext();

            var _renamedProductId = _dbContext.products.Where(p => p.ProductId == id).FirstOrDefault();

            Console.WriteLine(_renamedProductId); //=> tra ve true false neu dung truy van bang .select, .where thi se tra ve 1 entity
            if (_renamedProductId != null)
            {
                // EntityEntry<Product> => Theo doi chinh su thay doi cua model
                // product -> DbContext
                // Ta co the goi EntityEntry nay de truc quan theo doi cu the cho Model Products (ban than entities cung chinh la 1 Model) 
                EntityEntry<Products> entry = _dbContext.Entry(_renamedProductId);
                
                // Co the coi day la 1 cau lenh de chu dong tat Tracking (bien doi ve thanh Non-Tracking)
                //entry.State = EntityState.Detached;


                _renamedProductId.ProductName = newName;

                // Cap nhat lai toan bo model
                int _number_row = _dbContext.SaveChanges();
                Console.WriteLine($"Da co {_number_row} dong thay doi");
            }

            else
            {
                Console.WriteLine("Khong tim thay productID hop le!");
            }

            //if (_renameProductId != null) 
            //{
            //    _renameProductId.
            //}
            

        }
        public static void DeleteProduct(int id) 
        { 
            using var _dbContext = new MyDbContext();

            Products _deletedProductId = _dbContext.products.Where(p => p.ProductId == id).FirstOrDefault();

            if (_deletedProductId != null) 
            {
                _dbContext.Remove(_deletedProductId);

                int _number_row = _dbContext.SaveChanges();
                Console.WriteLine($"Da co {_number_row} dong thay doi");
            }
        }

    }
}