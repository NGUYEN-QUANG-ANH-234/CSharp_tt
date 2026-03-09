using DemoWebAPI.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoWebAPI.Utilities.DailyTask
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
