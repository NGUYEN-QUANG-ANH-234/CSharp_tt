using Serilog;
using Microsoft.Extensions.Logging;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ngay_5.DataAccess
{
    internal class SyncLoadRecords
    {
        public List<string> LoadRecords(ILogger logger, string filePath, int limit) {

            var records = new List<string>();

            try
            { 
                records = File.ReadLines(filePath).Take(limit).ToList(); 
            }

            catch (FileNotFoundException ex)
            {
                logger.LogCritical(ex, "File không tìm thấy tại đường dẫn: {FilePath}", filePath);
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogCritical(ex, "Không có quyền truy cập file. Hãy thử chạy với quyền Admin.");
                throw;
            }
            catch (IOException ex)
            {
                logger.LogCritical(ex, "Lỗi I/O: File đang bị khóa hoặc lỗi phần cứng đĩa.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Lỗi hệ thống không xác định xảy ra trong quá trình Load dữ liệu.");
                throw;
            }

            return records;
        }

    }
}
