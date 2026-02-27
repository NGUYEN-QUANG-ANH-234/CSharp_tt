
using Microsoft.EntityFrameworkCore;

namespace entity_framework_core.Data
{
    public class DbInitializers
    {
        public static void CreateDb() 
        {
            using var _dbContext = new MyDbContext();

            var dbName = _dbContext.Database.GetDbConnection().Database;

            var status = _dbContext.Database.EnsureCreated();

            if (status) 
            {         
                Console.WriteLine($"Khoi tao Database {dbName}");
            }

            else { Console.WriteLine($"Khoi tao Database {dbName} that bai"); }
        }


        public static void RecreateDb() 
        {
            using var _dbContext = new MyDbContext();

            var dbName = _dbContext.Database.GetDbConnection().Database;

            if (_dbContext.Database.EnsureDeleted()) 
            { 
                Console.WriteLine($"Xoa thanh cong {dbName}");
            }

            _dbContext.Database.EnsureCreated();
            Console.WriteLine($"Khoi tao/Tao lai Database {dbName} thanh cong");
        }   

        public static void DeleteDb() 
        {
            using var _dbContext = new MyDbContext();

            var dbName = _dbContext.Database.GetDbConnection().Database;

            var status = _dbContext.Database.EnsureDeleted();

            if (status)
            {
                Console.WriteLine($"Xoa Database {dbName} thanh cong");
            }

            else { Console.WriteLine($"Xoa Database {dbName} that bai"); }

        }
    }
}
