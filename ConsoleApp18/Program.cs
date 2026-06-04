using System;
using System.IO;
namespace Компилятор
{
    class Program
    {
        static void Main(string[] args)
        {
            string path =
            "program.pas";
            if (File.Exists(path) == false)
            {
                Console.WriteLine(
                "Файл программы не найден!"
                );
                return;
            }
            Console.WriteLine("Код:\n");
            string[] lines =
            File.ReadAllLines(path);
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine(
            "\nСинтаксический анализ:\n"
            );
            InputOutput inputOutput =
             new InputOutput(path);
            LexicalAnalyzer lexicalAnalyzer =
            new LexicalAnalyzer(
            inputOutput
            );
            SyntaxAnalyzer syntaxAnalyzer =
            new SyntaxAnalyzer(
            lexicalAnalyzer
            );
            syntaxAnalyzer.Analyze();
            Console.WriteLine(
            "\nАнализ завершен."
            );
        }
    }
}