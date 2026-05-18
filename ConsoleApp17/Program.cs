internal class Program
{
    private static void Main(string[] args)
    {
        InputOutput io = new InputOutput("test.pas");

        string error = string.Empty;

        while (!io.EOF)
        {
            Console.WriteLine(io.CurrentChar);

            io.NextCh();
        }

        Console.WriteLine("Конец файла");

        foreach (string item in io.Errors)
        {
            error = item;

            Console.WriteLine(error);
        }
    }
}