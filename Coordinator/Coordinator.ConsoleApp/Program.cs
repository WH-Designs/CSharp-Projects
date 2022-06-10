using Coordinator.Lib;

namespace Coordinator.ConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Enter the filename:\n");
        string filename = Console.ReadLine();

        var coordinator = new Coordinator();

        coordinator.ReadJobs(filename);

        coordinator.Run();
    }
}