using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{

    class Program
    {
        public static void PrintTree(AbstractNode root)
        {

            Console.WriteLine(root.print("", 0));
        }
        static void Main(string[] args)
        {
            while (true)
            {
                var parser = new TCCLParser();
                Console.WriteLine("Enter a file name.");
                string userInput = Console.ReadLine();

                while (!File.Exists(userInput))
                {
                    Console.WriteLine("No such file. Try again.");
                    userInput = Console.ReadLine();
                }


                Console.WriteLine("Parsing file " + userInput);
                Console.WriteLine("------------------------------------------------------------------------");

                parser.Parse(userInput);
                Console.WriteLine("------------------------------------------------------------------------");

                Console.WriteLine("Parsing complete");
            }

        }
    }
}
