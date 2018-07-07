using System.Collections.Generic;

namespace ASTBuilder
{
    public abstract class AbstractTypeDescp
    {

    }
    public class ClassTypeDescriptor : AbstractTypeDescp
    {
        List<string> modifiers = new List<string>();
        public List<string> Modifiers
        {
            get
            {
                return modifiers;
            }
            set
            {
                if (value != null)
                    modifiers = value;
            }

        }


        public ClassTypeDescriptor()
        {
        }

        public SymbolTable ClassSymbolTable { get; internal set; }
    }
    public class MethodDclTypeDescp : AbstractTypeDescp
    {
        List<string> modifiers = new List<string>();

        public class paramList_type
        {
            List<string> names = new List<string>();
            List<string> types = new List<string>();


            public List<string> Names
            {
                get
                {
                    return names;
                }
                set
                {
                    if (value != null)
                        names = value;
                }

            }
            public List<string> Types
            {
                get
                {
                    return types;
                }
                set
                {
                    if (value != null)
                        types = value;
                }

            }

            internal List<PrimitiveType> Typess { get; set; }
        }
        public paramList_type paramList = new paramList_type();

        public List<string> Modifiers
        {
            get
            {
                return modifiers;
            }
            set
            {
                if (value != null)
                    modifiers = value;
            }
        }


        public MethodDclTypeDescp()
        {
            this.nameSpaceVar = "";
        }

        public SymbolTable MethodSymbolTable { get; internal set; }
        public string isDefinedIn { get; internal set; }

        public string returnType { get; internal set; }
        public string Name { get; internal set; }
        public string nameSpaceVar { get; internal set; }
    }
    public class JaveTypeTypeDescriptor : AbstractTypeDescp
    {

    }
    
    public class IntegerTypeDescriptor : AbstractTypeDescp
    {

    }
    public class BooleanTypeDescriptor : AbstractTypeDescp
    {

    }
    public class StringTypeDescriptor : AbstractTypeDescp
    {

    }
    public class ContainerTypeDescriptor : AbstractTypeDescp
    {

    }
    public class ErrorTypeDescriptor : AbstractTypeDescp
    {
        // Can be used to specify error for assignment
        public string LHS_Type, RHS_Type;

    }
    // For assignment
    public class AssignmentTypeDescriptor : AbstractTypeDescp
    {
        public string LHS_Type, RHS_Type;

    }

    // Arg list to methods
    public class ArgListTypeDescriptor : AbstractTypeDescp
    {
        private List<AbstractNode> @params;

        public string LHS_Type, RHS_Type;
        public List<AbstractNode> Params
        {
            get
            {
                return @params;
            }
            set
            {
                if (value != null)
                    @params = value;
            }
        }

        public string OwningMethod { get; internal set; }
     
    }

    public class MethodCallTypeDescriptor : AbstractTypeDescp
    {

        public MethodCallTypeDescriptor()
        {
            nameSpaceVar = "";

        }
        public string name { get; internal set; }
        public string nameSpaceVar { get; internal set; }
        public string returnType { get; internal set; }
        internal ArgList argsRoot { get; set; }

    }
    public class SelectionStmtTypeDescriptor : AbstractTypeDescp
    {
        public AbstractNode ElsePathNode { get; internal set; }
        public AbstractNode StatementNode { get; internal set; }

        public string name { get; internal set; }
    }
 
    public class IterationStmtTypeDescriptor : AbstractTypeDescp
    {

        public string name { get; internal set; }
        public AbstractNode ConditionNode { get; internal set; }
        public AbstractNode BlockNode { get; internal set; }
    }
    

}