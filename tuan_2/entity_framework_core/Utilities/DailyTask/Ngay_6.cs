using entity_framework_core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace entity_framework_core.Utilities.DailyTask
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
