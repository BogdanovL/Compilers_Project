using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{

    using System;
    using SymTable_Type = Dictionary<string, Attributes>;

    /// <summary>
    /// Solution authored by:
    /// 
    ///  Lenny Bogdanov
    /// 
    /// </summary>

    public class SymbolTable : Symtab, SymtabInterface
    {
        List<SymTable_Type> symTables = new List<SymTable_Type>();
        int symTableIdx;

        public SymbolTable()
        {
            symTableIdx = 0;
            symTables.Add(new SymTable_Type());

            // Add some basic things to the symbol table
            Attributes a;

            // Integer
            IntegerTypeDescriptor ITD = new IntegerTypeDescriptor();
            a = new Attributes("Type");
            a.typeInfo = ITD;
            symTables[0].Add("int32", a);

            // Java types
            JaveTypeTypeDescriptor JTD = new JaveTypeTypeDescriptor();
            a = new Attributes("Some Java Thing");
            a.typeInfo = JTD;
            symTables[0].Add("java.io.PrintStream", a);

            // Some methods

            string[] knownMethods = { "Write",
                "WriteLine",
            "ps.print",
            };
            foreach (string s in knownMethods)
            {
                a = new Attributes("MethodCall");
                MethodDclTypeDescp MTD = new MethodDclTypeDescp();
                a.typeInfo = MTD;
                MTD.nameSpaceVar = "[mscorlib]System.Console";
                MTD.Name = s;
                MTD.returnType = "void"; 
                symTables[0].Add(s, a);
            }
        }

        /// <summary>
        /// Should never return a negative integer 
        /// </summary>

        public override int CurrentNestLevel
        {
            get
            {
                return symTableIdx; // you must fix this
            }
        }

        /// <summary>
        /// Opens a new scope, retaining outer ones </summary>

        public virtual void incrNestLevel()
        {
            symTableIdx++;
            if (symTableIdx == symTables.Count)
                symTables.Add(new SymTable_Type());
        }

        /// <summary>
        /// Closes the innermost scope </summary>

        public virtual void decrNestLevel()
        {
            symTableIdx--;
        }

        /// <summary>
        /// Enter the given symbol information into the symbol table.  If the given
        ///    symbol is already present at the current nest level, do whatever is most
        ///    efficient, but do NOT throw any exceptions from this method.
        /// </summary>

        public virtual void enter(string s, Attributes info)
        {
            if (!symTables[symTableIdx].ContainsKey(s))
                symTables[symTableIdx].Add(s, info);
            else
            {
                Console.WriteLine("Symbol already declared.");
                // Overwite
                symTables[symTableIdx][s] = info;
            }
        }

        /// <summary>
        /// Returns the information associated with the innermost currently valid
        ///     declaration of the given symbol.  If there is no such valid declaration,
        ///     return null.  Do NOT throw any excpetions from this method.
        /// </summary>

        public virtual Attributes lookup(string s)
        {
            Attributes retval = null;
            int i = symTableIdx;
            do
            {

                symTables[i].TryGetValue(s, out retval);
                i--;
                if (retval != null)
                    break;
            }
            while (i >= 0);
            return retval;
        }


    }

}
