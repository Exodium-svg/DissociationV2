using System.Diagnostics;

internal class Program
{
    public static async Task Main()
    {



        await Process.GetCurrentProcess().WaitForExitAsync();
    }
}