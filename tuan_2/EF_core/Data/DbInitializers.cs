using Microsoft.EntityFrameworkCore;

namespace EF_core.Data {
    public class DbInitializers {
        public static void CreateDb()
        {
            using var _dbContext = new MyDbContext();

            string _dbName = _dbContext.Database.GetDbConnection().Database;

            Console.WriteLine(_dbName);

            var _createDbResult = _dbContext.Database.EnsureCreated();

            if (_createDbResult)
            {
                Console.WriteLine("Ket noi thanh cong");
            }
            else
            {
                Console.WriteLine("Ket noi that bai");
            }

            Console.WriteLine(_dbName);
        }

        public static void DropDb()
        {
            using var _dbContext = new MyDbContext();

            string _dbName = _dbContext.Database.GetDbConnection().Database;

            Console.WriteLine(_dbName);

            var _dropDbResult = _dbContext.Database.EnsureDeleted();

            if (_dropDbResult)
            {
                Console.WriteLine("Xoa CSDL thanh cong");
            }
            else
            {
                Console.WriteLine("Xoa CSDL that bai");
            }

            Console.WriteLine(_dbName);
        }


        public static void RecreateDb()
        {
            using var _dbContext = new MyDbContext();
            var _dbName = _dbContext.Database.GetDbConnection().Database;

            Console.WriteLine($"Thuc hien tao lai CSDL {_dbName}");

            if (_dbContext.Database.EnsureDeleted())
            {
                Console.WriteLine($"Da xoa CSDL {_dbName} thanh cong");
                //_dbContext.Database.EnsureCreated();
            }

            //_dbContext.Database.EnsureCreated();

            if (_dbContext.Database.EnsureCreated()) {
                Console.WriteLine($"Tao thanh cong CSDL {_dbName}");            
            }
        }
    }

}