using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ASTBuilder;

namespace ASTBuilder
{

    internal class Modifier : AbstractNode
    {
        // Constructors
        internal Modifier(String s)
            : base(s)
        {
            // Mandatory Init
            this.NodeType = typeof(Modifier);
        }
        // If we have a chain of Modifiers
        internal Modifier(String s, AbstractNode n)
            : base(s)
        {
            // Mandatory Init
            this.NodeType = typeof(Modifier);

            this.adoptChildren(n);
        }

        public IEnumerable<object> ModifierTokens { get; internal set; } // TODO
    }

    internal class Identifier : AbstractNode
    {
        internal Identifier(String s)
            : base(s)
        {
            // Mandatory Init
            this.NodeType = typeof(Identifier);

        }
    }

    internal class ClassDcl : AbstractNode
    {
        internal ClassDcl(AbstractNode Modifier, AbstractNode Identifier, AbstractNode ClassBody, String s = null)
            : base()
        {

            this.adoptChildren(Modifier);
            //  this.adoptChildren(Identifier);
            this.adoptChildren(ClassBody);
            this.NodeType = this.GetType();

            this.name = Identifier.Name;
        }
        public override String AttrPrint()
        {
            String retval = "\nType ";
            Attributes c = this.attrRef;
            ClassTypeDescriptor t = c.typeInfo;
            retval += t.GetType().ToString();
            if (t.Modifiers.Count > 0)
                retval += "\nModifiers:";
            foreach (String m in t.Modifiers)
                retval += m + " ";
            return retval;
        }
    }

    internal class ClassBody : AbstractNode
    {

        internal ClassBody(AbstractNode FieldDesc = null)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            if (FieldDesc != null)
                this.adoptChildren(FieldDesc);
        }
    }

    internal class MethodDcl : AbstractNode
    {
        internal MethodDcl(AbstractNode Modifiers, AbstractNode TypeSpecifier, AbstractNode MethodDeclarator, AbstractNode MethodBody)
            : base()
        {

            this.adoptChildren(Modifiers);
            this.adoptChildren(TypeSpecifier);
            this.adoptChildren(MethodDeclarator);
            this.adoptChildren(MethodBody);

            this.NodeType = this.GetType();

        }

        public override String AttrPrint()
        {
            String retVal;
            retVal = String.Format("\nType: {0} Named {1}", Utilities.TypeDescToString(this.attrRef), this.attrRef.typeInfo.Name);
            // Modifiers
            retVal += "\nModifiers: ";
            MethodDclTypeDescp mtd = this.attrRef.typeInfo;
            for (int i = mtd.Modifiers.Count -1; i >= 0; i --)
            {
                retVal += mtd.Modifiers[i] + " ";
            }
            // Return
            retVal += String.Format("\nReturn: {0}\n", mtd.returnType);

            // Class
            retVal += String.Format("Defined in class: {0}\n", mtd.isDefinedIn);

            // Params done in AST body        
            if (mtd.paramList.Names.Count == 0)
                retVal += "No parameters";
            else
            {
                retVal += "See Tree below for parameter type info. Summarized here: \n";
                for (int i = 0; i < mtd.paramList.Names.Count; i++)
                {
                    retVal += mtd.paramList.Types[i] + " named " + mtd.paramList.Names[i] + " ";
                }

            }

            return retVal;
        }

    }
    internal class MethodDcla : AbstractNode
    {
        internal MethodDcla(AbstractNode MethodDeclaratorName, AbstractNode ParameterList = null)
            : base()
        {

            this.adoptChildren(MethodDeclaratorName);
            if (ParameterList != null)
                this.adoptChildren(ParameterList);

            this.NodeType = this.GetType();

        }

    }
    internal class StructDcl : AbstractNode
    {
        internal StructDcl(AbstractNode Modifier, AbstractNode Identifier, AbstractNode ClassBody, String s = null)
            : base()
        {

            this.adoptChildren(Modifier);
            //  this.adoptChildren(Identifier); // don't need this if storing name
            this.adoptChildren(ClassBody);
            this.NodeType = this.GetType();

            this.name = Identifier.Name;
        }
    }
    internal class PrimitiveType : AbstractNode
    {
        internal PrimitiveType(String s)
            : base(s)
        {
            this.NodeType = this.GetType();
        }
    }
    internal class ArraySpecifier : AbstractNode
    {
        internal ArraySpecifier(AbstractNode TypeName)
            : base()
        {
            this.NodeType = this.GetType();

            this.adoptChildren(TypeName);

        }
    }
    internal class TypeName : AbstractNode
    {
        internal TypeName(AbstractNode type)
            : base()
        {
            this.NodeType = this.GetType();
            this.adoptChildren(type);

        }
    }
    internal class TypeSpecifier : AbstractNode
    {
        internal TypeSpecifier(AbstractNode type)
            : base()
        {
            this.NodeType = this.GetType();
            this.adoptChildren(type);

        }
    }
    internal class FieldDcls : AbstractNode
    {

        internal FieldDcls(AbstractNode FieldDeclaration)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            this.adoptChildren(FieldDeclaration);

        }
        internal FieldDcls(AbstractNode FieldDeclarations, AbstractNode FieldDeclaration)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            this.adoptChildren(FieldDeclaration);
            this.adoptChildren(FieldDeclarations);


        }
    }

    internal class FieldDcl : AbstractNode
    {
        internal FieldDcl(String s)
            : base(s)
        {
            // Mandatory Init
            this.NodeType = this.GetType();
        }
        internal FieldDcl(AbstractNode Decl)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            this.adoptChildren(Decl);
        }
    }

    internal class FieldVarDcl : AbstractNode
    {
        internal FieldVarDcl(AbstractNode Modifier, AbstractNode TypeSpecifier, AbstractNode FieldVarDecls)
            : base()
        {

            this.adoptChildren(Modifier);
            this.adoptChildren(TypeSpecifier);
            this.adoptChildren(FieldVarDecls);
            this.NodeType = this.GetType();

        }
    }
    internal class FieldVarDcls : AbstractNode
    {
        internal FieldVarDcls(AbstractNode FieldVariableDeclarators, AbstractNode FieldVariableDeclaratorName)
            : base()
        {

            this.adoptChildren(FieldVariableDeclarators);
            this.adoptChildren(FieldVariableDeclaratorName);
            this.NodeType = this.GetType();

        }
        internal FieldVarDcls(AbstractNode FieldVariableDeclarators)
            : base()
        {

            this.adoptChildren(FieldVariableDeclarators);
            this.NodeType = this.GetType();
        }
    }
    internal class ParamList : AbstractNode
    {

        internal ParamList(AbstractNode Parameter)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            this.adoptChildren(Parameter);

        }
        internal ParamList(AbstractNode ParameterList, AbstractNode Parameter)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            this.adoptChildren(Parameter);
            this.adoptChildren(ParameterList);

        }
    }

    internal class Param : AbstractNode
    {
        internal Param(AbstractNode TypeSpecifier, AbstractNode DeclaratorName)
            : base()
        {

            this.adoptChildren(TypeSpecifier);
            this.adoptChildren(DeclaratorName);

            this.NodeType = this.GetType();

        }

    }
    internal class QualifiedName : AbstractNode
    {

        internal QualifiedName(AbstractNode Identifier)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();
            this.name = Identifier.Name;

            // this.adoptChildren(Identifier); // probably don't need this any more
        }
        internal QualifiedName(AbstractNode qualName, AbstractNode Identifier)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();
            this.name = qualName.Name + "." + Identifier.Name;
            // Delete children's name
            QualifiedName q = qualName as QualifiedName;
            q.removeName();
            this.adoptChildren(qualName);
            // this.adoptChildren(Identifier); // probably don't need this any more

        }
        public void removeName()
        {
            this.name = null;

        }
    }
    internal class DclName : AbstractNode
    {

        internal DclName(String s, AbstractNode Identifier)
            : base(s)
        {
            // Mandatory Init
            this.NodeType = this.GetType();
            this.name += " -> " + Identifier.Name;
            // this.adoptChildren(Identifier); // probably don't need this any more
        }
    }
    internal class Block : AbstractNode
    {
        public readonly bool isEmpty = true;
        internal Block(AbstractNode LocalVarDeclaAndStatment)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            if (LocalVarDeclaAndStatment != null)
            {
                this.adoptChildren(LocalVarDeclaAndStatment);
                isEmpty = false;
            }
        }
    }
    internal class UnaryOp : AbstractNode
    {
        internal UnaryOp(string s)
            : base(s)
        {
            // Mandatory Init
            this.NodeType = this.GetType();
        }
    }
    internal class LocalVarDeclOrStmt : AbstractNode
    {
        internal LocalVarDeclOrStmt(AbstractNode LocalVarDeclaOrStatment)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            this.adoptChildren(LocalVarDeclaOrStatment);
        }
    }
    internal class PrimaryExp : AbstractNode
    {
        internal PrimaryExp(AbstractNode QualifiedOrNotJustNAME)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            this.adoptChildren(QualifiedOrNotJustNAME);
        }
    }
    internal class NotJustName : AbstractNode
    {
        internal NotJustName(AbstractNode SpecialNameOrComplexPri)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            this.adoptChildren(SpecialNameOrComplexPri);
        }
    }
    internal class ComplexPri : AbstractNode
    {
        internal ComplexPri(AbstractNode ExprOrComplexPrimaryNP)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            this.adoptChildren(ExprOrComplexPrimaryNP);
        }
    }
    internal class StringArg : AbstractNode
    {
        internal StringArg(String lit)
            : base(lit)
        {
            // Mandatory Init
            this.NodeType = this.GetType();
        }

        public override String AttrPrint()
        {
            String retVal;

            retVal = String.Format("{0}", Utilities.attrToType(this.attrRef));
            return retVal;
        }
    }
    internal class FieldAccess : AbstractNode
    {
        internal FieldAccess(AbstractNode NotJustName, AbstractNode Identifier)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            this.adoptChildren(NotJustName);
            //  this.adoptChildren(Identifier); // don't need this if storing name
            this.name = Identifier.Name;
        }
    }
    internal class MethodCall : AbstractNode
    {
        internal MethodCall(AbstractNode MethodReference)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            this.adoptChildren(MethodReference);
        }
        internal MethodCall(AbstractNode MethodReference, AbstractNode ArgumentList)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            this.adoptChildren(MethodReference);
            this.adoptChildren(ArgumentList);

        }
        public override String AttrPrint()
        {
            String retVal;
            if (Utilities.attrToType(this.attrRef) == "ErrorTypeDescriptor")
                retVal = String.Format("\nType: {0} (check that this method exists in symbol table)",
                    Utilities.attrToType(this.attrRef));
            else
            {
                retVal = String.Format("\nType: {0} Named {1}", Utilities.TypeDescToString(this.attrRef),
                    this.attrRef.typeInfo.name);
                if (this.attrRef.typeInfo.argsRoot == null)
                    retVal += ", with no arguments";
                else
                    retVal += ", with arguments (see tree below). \n(ArgList pointer stored in TypeDescp)";
            }
            return retVal;
        }
    }
    internal class MethodRef : AbstractNode
    {
        internal MethodRef(AbstractNode CompPriNPQualorSpecialName)
            : base()
        {
            // Mandatory Init
            this.NodeType = this.GetType();

            this.adoptChildren(CompPriNPQualorSpecialName);

        }
    }
    internal class SpecialName : AbstractNode
    {
        internal SpecialName(String s)
            : base(s)
        {
            // Mandatory Init
            this.NodeType = this.GetType();

        }
    }
    internal class Number : AbstractNode
    {
        readonly double value;
        internal Number(String s)
            : base(s)
        {
            // Mandatory Init
            this.NodeType = typeof(Identifier);

            value = Convert.ToDouble(s);

        }
        public int eval()
        {
            return Int32.Parse(this.Name);
        }
    }
    internal class Expr : AbstractNode
    {
        // PrimaryExpression
        internal Expr(AbstractNode PrimaryExpression)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();
            this.adoptChildren(PrimaryExpression);
        }
        // Relational
        internal Expr(AbstractNode Expr1, AbstractNode Expr2, string s)
            : base(s)
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(Expr1);
            this.adoptChildren(Expr2);
            // Expr2.adoptChildren(Expr1);

        }
        public override String AttrPrint()
        {
            String retVal;

            retVal = String.Format("{0}", Utilities.attrToType(this.attrRef));
            return retVal;
        }

        public int eval()
        {
            // If only one child, eval will return it as the value
            List<dynamic> anl = new List<dynamic>();
            dynamic potentialNode = this.Child.First;
            while (potentialNode != null)
            {
                anl.Add(potentialNode);
                potentialNode = potentialNode.Sib;
            }
            if (anl.Count == 0)
                return Int32.Parse(this.Name);
            else
            {
                switch (this.Name)
                {
                    case "-":
                        return anl[0].eval() - anl[1].eval;
                    case "+":
                        return anl[0].eval() + anl[1].eval();
                    case "/":
                        return anl[0].eval()/ anl[1].eval();

                    case "*":
                        return anl[0].eval() * anl[1].eval();

                    case "%":
                        return anl[0].eval() % anl[1].eval();

                    case "|":
                        return anl[0].eval() | anl[1].eval();

                    case "&":
                        return anl[0].eval() & anl[1].eval();

                    case "^":
                        return anl[0].eval() ^ anl[1].eval();

                    case "<":
                    case ">":
                    case "<=":
                    case ">=":
                    case "||":
                    case "&&":
                    case "==":
                    case "!=":

                    case null:


                    default:
                        return 999;

                }
            }
        }
    }
    internal class AssignmentExpr : AbstractNode
    {
        internal AssignmentExpr(AbstractNode Expr1, AbstractNode Expr2)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            QualifiedName q;
            // Brand it as LHS
            
            // Adopt expression
            this.adoptChildren(Expr2);
            // Brand it as RHS
            q = Expr2 as QualifiedName;
            if (Expr2.NodeType.Name == "QualifiedName")
                q.Name += " (The RHS)";
            
            // Adopt expression
            this.adoptChildren(Expr1);
            // Brand it as LHS
            q = Expr1 as QualifiedName;
            if (Expr1.NodeType.Name == "QualifiedName")
                q.Name += " (The LHS)";
            // Expr2.adoptChildren(Expr1);

        }
        public override String AttrPrint()
        {
            String retVal;
            if (Utilities.attrToType(this.attrRef) == "ErrorTypeDescriptor")
                retVal = String.Format("{0} (assignment impossible)", Utilities.attrToType(this.attrRef));
            else
            {
                AssignmentTypeDescriptor ATD = this.attrRef.typeInfo;

                retVal = String.Format("{0} \n {1} assigned to {2}", Utilities.attrToType(this.attrRef), ATD.LHS_Type, ATD.RHS_Type);
            }
            return retVal;
        }
    }
    internal class ArgList : AbstractNode
    {
        // PrimaryExpression
        internal ArgList(AbstractNode Expression)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(Expression);
        }
        // Relational
        internal ArgList(AbstractNode ArgumentList, AbstractNode Expression)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(Expression);
            this.adoptChildren(ArgumentList);

        }
        public override String AttrPrint()
        {
            String retVal;
            if (Utilities.attrToType(this.attrRef) == "ErrorTypeDescriptor")
                retVal = String.Format("\nType: {0} (check argument in this method call)", Utilities.attrToType(this.attrRef));
            else
            {
                ArgListTypeDescriptor ATD = this.attrRef.typeInfo;
                retVal = String.Format("\nType: {0} for Method-Call called {1} \nWith {2} Params: ", Utilities.attrToType(this.attrRef), ATD.OwningMethod, ATD.Params.Count); 
                foreach (AbstractNode an in ATD.Params) 
                {
                    retVal += Utilities.attrToType(an.attrRef);
                }
            }
            return retVal;
        }

    }
    internal class Stmt : AbstractNode
    {
        // Empty, Expression, Selection, Iteration, Return, Block
        internal Stmt(AbstractNode Stmt)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            if (Stmt == null)
                this.name = "Empty";
            else
                this.adoptChildren(Stmt);

        }
        // Expression or Return
        internal Stmt(AbstractNode Expression, String s)
            : base(s)
        {
            // Mandatory
            this.NodeType = this.GetType();

            if (Expression != null)
                this.adoptChildren(Expression);
        }

        // Iteration  
        internal Stmt(AbstractNode Expression, AbstractNode Statement, String s)
            : base(s)
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(Expression);
            this.adoptChildren(Statement);

        }
        public override String AttrPrint()
        {
            String retVal;
            if (Utilities.attrToType(this.attrRef) == "ErrorTypeDescriptor")
                retVal = String.Format("\nType: {0} (check argument in this loop-statement)", Utilities.attrToType(this.attrRef));
            else
            {
                IterationStmtTypeDescriptor ITD = this.attrRef.typeInfo;
                retVal = String.Format("\nType: {0} (Loop-Statement) with valid boolean condition", Utilities.attrToType(this.attrRef));
                // If we have an else path
                if (ITD.ConditionNode != null)
                    retVal += String.Format("\nCondition Type: {0}", Utilities.attrToType(ITD.ConditionNode.attrRef));
                if (ITD.BlockNode != null)
                    retVal += String.Format("\nBlock Type: {0}", ITD.BlockNode.NodeType.Name);
               

            }

            return retVal;
        }


    }
    internal class SelectionStmt : AbstractNode
    {
        // Selection  
        AbstractNode elseClause = null;
        internal SelectionStmt(AbstractNode Expression, AbstractNode Statement, AbstractNode ElseStatement)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(Expression);
            this.adoptChildren(Statement);
            this.adoptChildren(ElseStatement);
            // Brand this guy as an else clause  
                ElseStatement.Name += " (Else Clause)";

            elseClause = ElseStatement;

        }
        internal SelectionStmt(AbstractNode Expression, AbstractNode Statement)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(Expression);
            this.adoptChildren(Statement);

        }
        public override String AttrPrint()
        {
            String retVal;
            if (Utilities.attrToType(this.attrRef) == "ErrorTypeDescriptor")
                retVal = String.Format("\nType: {0} (check argument in this if-statement)", Utilities.attrToType(this.attrRef));
            else
            {
                SelectionStmtTypeDescriptor SSTD = this.attrRef.typeInfo;
                retVal = String.Format("\nType: {0} (If-Statement) with valid boolean argument", Utilities.attrToType(this.attrRef));
                // If we have an else path
                if (SSTD.ElsePathNode != null)
                    retVal += String.Format("\nStatement Type: {0}\nElse-Path Type: {1}", Utilities.attrToType(SSTD.StatementNode.attrRef), Utilities.attrToType(SSTD.ElsePathNode.attrRef));
                else
                    retVal += String.Format("\nStatement Type: {0} and No Else-Path", Utilities.attrToType(SSTD.StatementNode.attrRef));
                                
            }

            return retVal;
        }
    }
    internal class LocalVarDecls : AbstractNode
    {
        internal LocalVarDecls(AbstractNode LocalVariableDeclaratorName)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(LocalVariableDeclaratorName);

        }
        internal LocalVarDecls(AbstractNode LocalVariableDeclarators, AbstractNode LocalVariableDeclaratorName)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(LocalVariableDeclaratorName);
            this.adoptChildren(LocalVariableDeclarators);


        }
    }
    internal class LocalVarDeclStmt : AbstractNode
    {
        internal LocalVarDeclStmt(AbstractNode StructDeclaration)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(StructDeclaration);

        }
        internal LocalVarDeclStmt(AbstractNode TypeSpecifier, AbstractNode LocalVariableDeclarators)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(LocalVariableDeclarators);
            this.adoptChildren(TypeSpecifier);

        }

    }
    internal class LocalVarDeclsAndStmts : AbstractNode
    {
        internal LocalVarDeclsAndStmts(AbstractNode LocalVariableDeclarationOrStatement, AbstractNode LocalVariableDeclarationsAndStatements)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(LocalVariableDeclarationsAndStatements);
            this.adoptChildren(LocalVariableDeclarationOrStatement);

        }
        internal LocalVarDeclsAndStmts(AbstractNode LocalVariableDeclarationOrStatement)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(LocalVariableDeclarationOrStatement);

        }

    }
    internal class StaticInit : AbstractNode
    {
        internal StaticInit(AbstractNode Block)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(Block);

        }
    }
    internal class MethodBody : AbstractNode
    {
        internal MethodBody(AbstractNode Block)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(Block);

        }

    }
    internal class Constructor : AbstractNode
    {
        internal Constructor(AbstractNode Modifiers, AbstractNode MethodDeclarator, AbstractNode Block)
            : base()
        {
            // Mandatory
            this.NodeType = this.GetType();

            this.adoptChildren(Modifiers);
            this.adoptChildren(MethodDeclarator);
            this.adoptChildren(Block);

        }

    }

}

