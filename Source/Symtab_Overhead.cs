using System;

namespace ASTBuilder

{

    /// <summary>
    /// Abstract class so you can print out messages that are properly indented to
    /// reflect the current nest level.
    /// </summary>

    public abstract class Symtab
    {

        public abstract int CurrentNestLevel { get; }

        public virtual void @out(string s)
        {
            string tab = "";
            for (int i = 1; i <= CurrentNestLevel; ++i)
            {
                tab += "  ";
            }
            Console.WriteLine(tab + s);
        }

        public virtual void err(string s)
        {
            @out("Error: " + s);
            Console.Error.WriteLine("Error: " + s);
            Environment.Exit(-1);
        }

        // public virtual void @out(AbstractNode n, string s)
        // {
        //@out("" + n.NodeNum + ": " + s + " " + n);
        // }
        // public virtual void err(AbstractNode n, string s)
        // {
        //err("" + n.NodeNum + ": " + s + " " + n);
        // }

    }



    public interface SymtabInterface
    {
        /// Open a new nested symbol table
        void incrNestLevel();

        /// Close an existing nested symbol table

        void decrNestLevel();

        int CurrentNestLevel { get; }

        Attributes lookup(string id);

        void enter(string id, Attributes s);

        /// This lets you put out a message about a node, indented by the current nest level 
        //    void @out(AbstractNode n, string message);
        //    void err(AbstractNode n, string message);
        void @out(string message);
        void err(string message);
    }




}