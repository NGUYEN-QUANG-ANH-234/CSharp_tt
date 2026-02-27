using entity_framework_core.Data;

namespace entity_framework_core
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            DbInitializers.CreateDb();
        }
    }
}
