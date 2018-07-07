using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{

    class Utilities
    {
        public static string attrToType(Attributes a)
        {
            return a.typeInfo.ToString().Split('.')[1];
        }

        internal static string TypeDescToString(Attributes tempAttr)
        {
            string retVal;
            string PrimType = Utilities.attrToType(tempAttr);
            switch (PrimType)
            {
                case "IntegerTypeDescriptor":
                    retVal = "int32"; // odn't just change to int32. we don't see the other function call then
                    break;
                case "MethodCallTypeDescriptor":
                    retVal = "Method-Call";
                    break;
                case "MethodDclTypeDescp":
                    retVal = "Method-Declaration";
                    break;
                case "ErrorTypeDescriptor":
                    retVal = "Error! Something went wrong (not in symbol table?)";
                    break;
                default:
                    retVal = tempAttr.typeInfo.GetType().ToString().Split(' ')[1];
                    break;
            }
            return retVal;
        }

        internal static string FilterOutKeywords(string name)
        {
            // These keywords cannot be used as variables! 
            // Don't look up strings demared with LHS or RHS
            if (name.Contains("LHS"))
                // Just take first element
                name = name.Split(' ')[0]; // until not good enough
            if (name.Contains("RHS"))
                // Just take first element
                name = name.Split(' ')[0]; // until not good enough
            if (name.Contains("Else"))
                // Just take first element
                name = name.Split(' ')[0]; // until not good enough
            return name;
        }

        public static void TypeVisitAllChildren(AbstractNode node)
        {
            // Go ahead and type-visit the children
            TypeVisitor t = new TypeVisitor();
            AbstractNode an;
            an = node.Child;
            while (an != null)
            {
                an.Accept(t);
                if (an.Sib != an)
                    an = an.Sib;
            }
        }

        public static void CodeGenVisitAllChildren(AbstractNode node, CodeGenVisitor c)
        {
            AbstractNode an;
            an = node.Child;
            while (an != null)
            {
                an.Accept(c);
                if (an.Sib != an)
                    an = an.Sib;
            }
        }

        internal static void DclVisitAllChildren(AbstractNode node, TopDclVisitor d)
        {

            AbstractNode an;
            an = node.Child;
            while (an != null)
            {
                an.Accept(d);
                if (an.Sib != an)
                    an = an.Sib;
            }
        }

        internal static List<AbstractNode> GetChildren(AbstractNode root, Type type)
        {
            List<AbstractNode> ChildrenList = new List<AbstractNode>();
            AbstractNode child = root.Child;
            while (child != null)
            {
                // If the child is what we want, add it to the list
                if (child.GetType() == type)
                {
                    ChildrenList.Add(child);
                    // If a list is represnted by chaining identical nodes together go down this path
                    while (child.Child != null && child.Child.GetType() == type)
                    {
                        child = child.Child;
                        ChildrenList.Add(child);
                    }
                }
                // If I have myself as a child, consider that an equal state to this, and explore that path
                else if (child.GetType() == root.GetType())
                {
                    ChildrenList.AddRange(Utilities.GetChildren(child, type));
                }

                // Look at sibling
                child = child.Sib;
            }
            return ChildrenList;
        }

        // Visits first occurence of child
        internal static void VisitSpecificChild(AbstractNode node, Type type, CodeGenVisitor codeGenVisitor)
        {

            AbstractNode an;
            an = node.Child;
            while (an != null)
            {
                if (an.NodeType == type)
                {
                    an.Accept(codeGenVisitor);
                    break;
                }
                else
                    VisitSpecificChild(an, type, codeGenVisitor);

                if (an.Sib != an)
                    an = an.Sib;
            }
        }

        internal static List<AbstractNode> GetMethodBodyChildren(AbstractNode node, CodeGenVisitor c)
        {
            // return children that are Assignment Expressions and also ones that are Method Calls 
            List<AbstractNode> retVal = new List<AbstractNode>();
            AbstractNode an;
            an = node.Child;
            while (an != null)
            {
                if (an.GetType() == typeof(AssignmentExpr) || an.GetType() == typeof(MethodCall) ||
                    an.GetType() == typeof(SelectionStmt) || an.GetType() == typeof(Stmt))
                {
                    retVal.Add(an);
                }
                if (an.GetType() == node.GetType())
                    retVal.AddRange(GetMethodBodyChildren(an, c));
                if (an.Sib != an)
                    an = an.Sib;
            }
            return retVal;

        }

        internal static void VisitThenChildren(SelectionStmt node, CodeGenVisitor codeGenVisitor)
        {
            AbstractNode an;
            an = node.Child;
            while (an != null)
            {
                if (an.Name == null || !an.Name.Contains("Else"))
                {
                    if (an.GetType() != typeof(Expr))
                        an.Accept(codeGenVisitor);
                }

                if (an.Sib != an)
                    an = an.Sib;
            }
        }

        internal static void VisitElseChildren(SelectionStmt node, CodeGenVisitor codeGenVisitor)
        {
            {
                AbstractNode an;
                an = node.Child;
                while (an != null)
                {
                    if (an.Name != null && an.Name.Contains("Else"))
                    {
                        if (an.GetType() != typeof(Expr))
                            an.Accept(codeGenVisitor);

                    }

                    if (an.Sib != an)
                        an = an.Sib;
                }
            }
        }

        internal static void VisitLoopBodyChildren(Stmt node, CodeGenVisitor codeGenVisitor)
        {
            // Get local var decls
            // Find the localvardeclsandstmts child
            Utilities.VisitSpecificChild(node, typeof(LocalVarDeclsAndStmts), codeGenVisitor);
            // And visit all of its children
            // Get list of children that are not variable declarations (only assignment expr and method calls currently supported)
            List<AbstractNode> loopBodyChidlren = Utilities.GetMethodBodyChildren(node, codeGenVisitor);
            // Process them in reverse order
            for (int i = loopBodyChidlren.Count - 1; i >= 0; i--)
            {
                // filter out statement for us
                if (loopBodyChidlren[i].Name != null && !loopBodyChidlren[i].Name.Contains(("Iteration")))
                    loopBodyChidlren[i].Accept(codeGenVisitor);
            }

            // 
        }
    }
}


