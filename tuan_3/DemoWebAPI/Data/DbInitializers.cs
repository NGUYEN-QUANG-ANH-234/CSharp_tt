using DemoWebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace DemoWebAPI.Data
{
    public class DbInitializers
    {
        public static void CreateDb(MyDbContext dbContext) 
        {
            var dbName = dbContext.Database.GetDbConnection().Database;

            var status = dbContext.Database.EnsureCreated();

            if (status) 
            {         
                Console.WriteLine($"Khoi tao Database {dbName}");
            }

            else { Console.WriteLine($"Khoi tao Database {dbName} that bai"); }
        }


        public static void RecreateDb(MyDbContext dbContext) 
        {
            var dbName = dbContext.Database.GetDbConnection().Database;

            if (dbContext.Database.EnsureDeleted()) 
            { 
                Console.WriteLine($"Xoa thanh cong {dbName}");
            }

            dbContext.Database.EnsureCreated();
            Console.WriteLine($"Khoi tao/Tao lai Database {dbName} thanh cong");
        }   

        public static void DeleteDb(MyDbContext dbContext) 
        {
            var dbName = dbContext.Database.GetDbConnection().Database;

            var status = dbContext.Database.EnsureDeleted();

            if (status)
            {
                Console.WriteLine($"Xoa Database {dbName} thanh cong");
            }

            else { Console.WriteLine($"Xoa Database {dbName} that bai"); }

        }
    }
}
