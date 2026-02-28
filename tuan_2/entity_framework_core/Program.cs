using entity_framework_core.Data;
using entity_framework_core.Models.Entities;
using entity_framework_core.Repositories;
using entity_framework_core.Utilities;
using Sprache;

namespace entity_framework_core
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var dbContext = new MyDbContext();

            Console.WriteLine("Hello, World!");

            //DbInitializers.CreateDb();
            //DbInitializers.DeleteDb();
            DbInitializers.RecreateDb();

            Console.WriteLine("Nhap so record cho tung seed cua bang User, Post, Comment: ");
            int userRecords = Convert.ToInt32(Console.ReadLine());
            int postRecords = Convert.ToInt32(Console.ReadLine());
            int commentRecords = Convert.ToInt32(Console.ReadLine());


            MockDataGenerator.SeedUser(dbContext, userRecords);
            Console.WriteLine("Hoan thanh tao seed bang User");
            MockDataGenerator.SeedPost(dbContext, postRecords);
            Console.WriteLine("Hoan thanh tao seed bang Post");
            MockDataGenerator.SeedComment(dbContext, commentRecords);
            Console.WriteLine("Hoan thanh tao seed bang Comment");



            Console.ReadKey();
        }
    }
}
