using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ASTBuilder
{

    /// <summary>
    /// All AST nodes are subclasses of this node.  This node knows how to
    /// link itself with other siblings and adopt children.
    /// Each node gets a node number to help identify it distinctly in an AST.
    /// </summary>
    public abstract class AbstractNode : IVisitableNode
    {
        protected string name;

        private static int nodeNums = 0;
        private int nodeNum;
        private AbstractNode mysib;
        protected AbstractNode parent;
        private AbstractNode child;
        private AbstractNode firstSib;
        private Type type;


        protected AbstractNode(string name = null)
        {
            parent = null;
            mysib = null;
            firstSib = this;
            child = null;
            nodeNum = ++nodeNums;
            attrRef = null;

            this.name = name;
        }


        /// <summary>
        /// Join the end of this sibling's list with the supplied sibling's list </summary>
        public virtual AbstractNode makeSibling(AbstractNode sib)
        {
            if (sib == null)
            {
                throw new Exception("Call to makeSibling supplied null-valued parameter");
            }
            AbstractNode appendAt = this;
            while (appendAt.mysib != null)
            {
                appendAt = appendAt.mysib;
            }
            appendAt.mysib = sib.firstSib;


            AbstractNode ans = sib.firstSib;
            ans.firstSib = appendAt.firstSib;
            while (ans.mysib != null)
            {
                ans = ans.mysib;
                ans.firstSib = appendAt.firstSib;
            }
            return (ans);
        }

        /// <summary>
        /// Adopt the supplied node and all of its siblings under this node </summary>
        public virtual AbstractNode adoptChildren(AbstractNode n)
        {
            if (n != null)
            {
                if (this.child == null)
                {
                    this.child = n.firstSib;
                }
                else
                {
                    this.child.makeSibling(n);
                }
            }
            for (AbstractNode c = this.child; c != null; c = c.mysib)
            {
                c.parent = this;
            }
            return this;
        }

        public virtual AbstractNode orphan()
        {
            mysib = parent = null;
            firstSib = this;
            return this;
        }

        public virtual AbstractNode abandonChildren()
        {
            child = null;
            return this;
        }

        private AbstractNode Parent
        {
            set
            {
                this.parent = value;
            }
            get
            {
                return (parent);
            }
        }


        public virtual AbstractNode Sib
        {
            get
            {
                return (mysib);
            }
        }

        public virtual AbstractNode Child
        {
            get
            {
                return (child);
            }
        }

        public virtual AbstractNode First
        {
            get
            {
                return (firstSib);
            }
        }

        public virtual Type NodeType
        {
            get
            {
                return type;
            }
            set
            {
                this.type = value;
            }
        }

        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public override string ToString()
        {
            return ("" + Name);
        }

        public virtual string dump()
        {
            Type t = NodeType;
            string tString = (t != null) ? ("<" + t.ToString() + "> ") : "";

            return "" + NodeNum + ": " + tString + whatAmI() + "  \"" + ToString() + "\"";
        }


        public virtual int NodeNum
        {
            get
            {
                return nodeNum;
            }
        }

        private static string trimClass(string cl)
        {
            int dollar = cl.LastIndexOf('$');
            int dot = cl.LastIndexOf('.');
            int trimAt = (dollar > dot) ? dollar : dot;
            if (trimAt >= 0)
            {
                cl = cl.Substring(trimAt + 1);
            }
            return cl;
        }

        private static Type objectClass = (new object()).GetType();
        private static void debugMsg(string s)
        {
        }
        //  private static System.Collections.IEnumerator interfaces(Type c)
        //  {
        //  Type iClass = c;
        //  ArrayList v = new ArrayList();
        //  while (iClass != objectClass)
        //  {
        //debugMsg("Looking for interface  match in " + iClass.Name);
        //Type[] interfaces = iClass.Interfaces;
        //	 for (int i = 0; i < interfaces.Length; i++)
        //	 {
        //	  debugMsg("   trying interface " + interfaces[i]);
        //		  v.Add(interfaces[i]);
        //		Type[] superInterfaces = interfaces[i].Interfaces;
        //		for (int j = 0; j < superInterfaces.Length; ++j)
        //		{
        //	  debugMsg("   trying super interface " + superInterfaces[j]);
        //			  v.Add(superInterfaces[j]);
        //		}

        //	 }
        // iClass = iClass.BaseType;
        //  }
        //  return v.elements();
        //  }

        /// <summary>
        /// Reflectively indicate the class of "this" node </summary>
        public virtual string whatAmI()
        {
            string ans = trimClass(this.GetType().ToString());
            return ans;  /* temporary until remainder is fixed */
            //ISet s = new HashSet();
            //System.Collections.IEnumerator e = interfaces(this.GetType());
            //while (e.MoveNext())
            //{
            //    Type c = (Type)e.Current;
            //    string str = trimClass(c.ToString());
            //    if (!(str.Equals("DontPrintMe") || str.Equals("ReflectiveVisitable")))
            //    {
            //        s.Add(trimClass(c.ToString()));
            //    }
            //}
            //return ans + s.ToString();
        }

        //private void internWalk(int level, Visitable v)
        //{
        //v.pre(level, this);
        //for (AbstractNode c = child; c != null; c = c.mysib)
        //{
        //c.internWalk(level + 1, v);
        //}
        //v.post(level, this);
        //}

        /// <summary>
        /// Reflective visitor pattern </summary>
        public void Accept(IReflectiveVisitor v)
        {
            v.Visit(this);
        }

        internal virtual string print(String s, int childLevel)
        {
            const string spacestring = " ";

            // Maybe create some spaces
            String spaces = "";
            if (childLevel > 0)
            {
                for (int i = 0; i < childLevel; i++)
                    spaces += spacestring;
            }

            /* *** PRINT Statements *** */
            // If I am a local root
            // Special case for expressions. My print function is getting messy. 
            if (this.parent == null || this.GetType() == typeof(Expr) || this.parent.GetType() != this.GetType())
            {
                childLevel++; // My children will be shown with with a leading space
                for (int i = 0; i < childLevel; i++)
                    spaces += spacestring;

                Console.Write("\n");

                // For each child level print a pipe
                PrintPipe(childLevel, spacestring);

                // Declare my type and my name
                Console.Write(s + String.Format("<{0}>", this.GetType().ToString()));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(s + String.Format(" {0}", this.Name));
                Console.ResetColor();
                if (this.attrRef != null)
                {
                    String printThis = AttrPrint();


                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" " + printThis);
                    Console.ResetColor();




                }

            }
            // If I am a same-type child.
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(s + String.Format(" {0}", this.Name));
                Console.ResetColor();
            }

            // If I have a child, 
            if (this.Child != null)
            {
                s += this.Child.print(s, childLevel);
            }
            // If I have a sibling (that is not me)
            if (this.mysib != null)// && this.mysib.GetType() != this.GetType())
            {
                childLevel--;
                // Reduce indentation
                s += this.Sib.print(s, childLevel);
            }


            return s;

        }

        private static void PrintPipe(int childLevel, string spacestring)
        {
            for (int i = 0; i < childLevel; i++)
            {
                var colors = Enum.GetValues(typeof(ConsoleColor));

                // Skip the dark colors in the enum
                Console.ForegroundColor = (ConsoleColor)(colors.GetValue((i) % (colors.Length - 7) + 7));
                Console.Write(spacestring + "|");
                Console.ResetColor();

            }
        }

        public virtual string AttrPrint()
        {
            //throw new NotImplementedException();
            String retval = String.Format("Type {0}", this.attrRef.typeInfo.ToString());
            return retval.Split('.')[1];
        }

        public dynamic attrRef { get; internal set; }


        /// <summary>
        /// Obsolete, do not use! </summary>
        //public virtual void walkTree(Visitable v)
        //{
        //internWalk(0, v);
        //}
    }

}