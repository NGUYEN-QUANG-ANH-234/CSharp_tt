using Serilog;
using Microsoft.Extensions.Logging;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ngay_5.Engines
{
    public static class ChartVisualizer
    {
        public static void Export(ILogger logger, long syncMs, long asyncMs, long seqMs, long parMs, int lines)
        {
            try
            {
                string path = Path.Combine(AppContext.BaseDirectory, "Reports");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                // --- BIỂU ĐỒ I/O ---
                var plt1 = new ScottPlot.Plot();
                var ioBars = plt1.Add.Bars(new double[] { (double)syncMs, (double)asyncMs });

                // Hiển thị xử lý đọc file đồng bộ
                ioBars.Bars[0].FillColor = new ScottPlot.Color(52, 152, 219);
                // Hiển thị xử lý đọc file bất đồng bộ
                ioBars.Bars[1].FillColor = new ScottPlot.Color(230, 126, 34);

                plt1.Title($"I/O Performance ({lines:N0} lines)");
                plt1.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual();
                plt1.Axes.Bottom.SetTicks(new double[] { 0, 1 }, new string[] { "Sync I/O", "Async I/O" });
                plt1.SavePng(Path.Combine(path, "io.png"), 600, 400);

                // --- BIỂU ĐỒ CPU ---
                var plt2 = new ScottPlot.Plot();
                var cpuBars = plt2.Add.Bars(new double[] { (double)seqMs, (double)parMs });

                // Hiển thị xử lý tuần tự
                cpuBars.Bars[0].FillColor = new ScottPlot.Color(142, 68, 173);
                cpuBars.Bars[0].Label = "Sequential";

                // Hiển thị xử lý song song 
                cpuBars.Bars[1].FillColor = new ScottPlot.Color(22, 160, 133);
                cpuBars.Bars[1].Label = "Parallel";

                plt2.Title($"CPU Performance Comparison ({lines:N0} lines)");

                // Thiết lập nhãn cho trục X phân biệt
                plt2.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual();
                plt2.Axes.Bottom.SetTicks(new double[] { 0, 1 }, new string[] { "Tuần tự (Seq)", "Song song (Para)" });

                plt2.YLabel("Thời gian (ms)");
                plt2.SavePng(Path.Combine(path, "cpu.png"), 600, 400);

                logger.LogInformation("Đã xuất biểu đồ màu tại: {Path}", path);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi xuất biểu đồ: {Message}", ex.Message);
            }
        }
    }
}