using entity_framework_core.Data;
using entity_framework_core.Models.Entities;
using entity_framework_core.Repositories;
using entity_framework_core.Utilities;
using Sprache;

// THAO TAC VOI MIGRATION (SNAPSHOT)
/* 
 * dotnet ef migrations add {MigrationName} => Tao Migration
 * dotnet ef migrations list => Kiem tra cac Migrations hien co
 * dotnet ef migrations remove {MigrationName} => Xoa Migration
 * 
 * dotnet ef database update {MigrationName} => Update phien ban Migration mong muon
 * dotnet ef database drop -f
 */

namespace entity_framework_core
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // --- CAU HINH HE THONG ---

            using var dbContext = new MyDbContext();

            // -------------------------

            // --- THAO TAC CSDL MYSQL ---

            //DbInitializers.CreateDb();
            //DbInitializers.DeleteDb();
            //DbInitializers.RecreateDb();

            // ---------------------------

            // --- KHOI TAO RECORD GIA CHO CAC BANG DU LIEU ---

            //MockDataGenerator.SeedUser(dbContext, 100000);
            //Console.WriteLine("Hoan thanh tao seed bang User");
            //MockDataGenerator.SeedPost(dbContext, 100000);
            //Console.WriteLine("Hoan thanh tao seed bang Post");
            //MockDataGenerator.SeedComment(dbContext, 100000);
            //Console.WriteLine("Hoan thanh tao seed bang Comment");

            // ------------------------------------------------

            Console.ReadKey();
        }
    }
}
