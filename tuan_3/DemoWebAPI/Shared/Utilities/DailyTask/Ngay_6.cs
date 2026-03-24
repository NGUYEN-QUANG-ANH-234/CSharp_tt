using DemoWebAPI.Infrastructure.Data;
using DemoWebAPI.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoWebAPI.Shared.Utilities.DailyTask
{
    internal class Ngay_6
    {
        public static async Task SeedData(MyDbContext dbContext)
        {
            Console.WriteLine("DAY 6: TẠO DATASET");
            await MockDataGenerator.SeedAll(dbContext);
            DataVisualizer.ShowSeedReport(100000, 100000, 100000);
        }
    }
}
