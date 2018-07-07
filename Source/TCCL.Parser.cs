using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ASTBuilder
{
    internal partial class TCCLParser
    {
        internal static FileStream currentFileStream;

        public string filename { get; private set; }

        public TCCLParser() : base(null) { }

        public void Parse(string filename)
        {
            using (StreamReader SR = new StreamReader(File.OpenRead(filename)))
            {
                Console.Write(SR.ReadToEnd());
            }
            Console.WriteLine("------------------------------------------------------------------------");
            this.Scanner = new TCCLScanner(File.OpenRead(filename));
            this.Parse();
            this.filename = filename;
            //   PrintTree();
            DoSemantics();
            PrintTree();
            GenCode();
        }
        public void Parse(Stream strm)
        {
            this.Scanner = new TCCLScanner(strm);
            this.Parse();
            //   PrintTree();
            DoSemantics();
            PrintTree();
            GenCode();
        }
        public void PrintTree()
        {
            PrintVisitor visitor = new PrintVisitor();
            Console.WriteLine("Starting to print AST ");
            Console.WriteLine(CurrentSemanticValue.print("", 0));
        }

        public void DoSemantics()
        {
            SemanticsVisitor visitor = new SemanticsVisitor();
            Console.WriteLine("Starting semantic checking");
            visitor.CheckSemantics(CurrentSemanticValue);

        }

        public void GenCode()
        {
            // Create file
            string ilFileName = filename.Split('.')[0] + ".il";
            if (File.Exists(ilFileName))
                File.Delete(ilFileName);
            using (FileStream fs = File.Create(ilFileName))
            {
                CodeGenVisitor visitor = new CodeGenVisitor();
                Console.WriteLine("Starting code generation");

                // Write file header
                StringBuilder sb = new StringBuilder();
                Byte[] info;
                sb.AppendLine(String.Format("//-----------------------------------------------------------------", ilFileName, filename));
                sb.AppendLine(String.Format("//IL Header - Lenny Bogdanov\n//File: {0} \n//Source File: {1} \n//Date: {2} {3}",
                    ilFileName, filename, System.DateTime.Now.ToLongDateString(), System.DateTime.Now.ToLongTimeString()));
                sb.AppendLine(String.Format("//-----------------------------------------------------------------", ilFileName, filename));
                sb.AppendLine(String.Format("\n", ilFileName, filename));
                sb.AppendLine(String.Format(".assembly extern mscorlib {{}}"));
                sb.AppendLine(String.Format(".assembly a{{}} //Not sure why this was necessary for me to do", ilFileName, filename));
                sb.AppendLine(String.Format("\n", ilFileName, filename));


                info = new UTF8Encoding(true).GetBytes(sb.ToString());
                fs.Write(info, 0, info.Length);
                // Visit AST
                // This is how we'll give the code gen reader access to the file we've opened
                currentFileStream = fs;
                CurrentSemanticValue.Accept(visitor);
                fs.Close();

            }
            Console.WriteLine(filename + " -> " + filename.Split('.')[0] + ".il");

        }

    }
}
