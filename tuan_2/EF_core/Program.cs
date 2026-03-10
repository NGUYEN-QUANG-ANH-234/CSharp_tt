using EF_core.Data;
using EF_core.Repositories;

namespace EF_core
{
    internal class Program
    {            
        static void Main(string[] args)
        {
            // Entity -> Model, Database
            // Database -> SQL Server : Data 01 -> DbContext
            // --product

            Console.WriteLine("Hello, World!");

            //DbInitializers.CreateDb();
            //DbInitializers.DropDb();
            //DbInitializers.RecreateDb();

            //CRUD
            //ProductsRepo.InsertProduct(1, "Iphone", "Description_1");
            //ProductsRepo.ReadProduct();
            //ProductsRepo.RenameProduct(1, "Iphone");
            //ProductsRepo.DeleteProduct(1);

            Console.ReadKey();
        }
    }
}
