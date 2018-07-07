using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using QUT.GplexBuffers;

namespace ASTBuilder
{
    /*-------------------------------------------------------CODE GEN-----------------------------------------------------------*/
    class CodeGenVisitor : IReflectiveVisitor
    {
        private Dictionary<string, Dictionary<string, int>> localVarNumber =
            new Dictionary<string, Dictionary<string, int>>();
        private Dictionary<string, Dictionary<string, int>> localArgNumber =
            new Dictionary<string, Dictionary<string, int>>();
        // temporarily hold on to method name as we process arguments
        private String methodName = "";
        private int labelNumber = 0;

        public void Visit(dynamic node)
        {
            this.VisitNode(node);
        }

        public void VisitNode(AbstractNode node)
        {
            /*  if (node.Child != null)
                  node.Child.Accept(this);

              if (node.Sib != null)
                  node.Sib.Accept(this);*/


        }

        protected void VisitNode(ClassDcl node)
        {

            // Write class header
            StringBuilder sb = new StringBuilder();
            Byte[] info;
            ClassTypeDescriptor CTD = node.attrRef.typeInfo;
            string modList = "";
            foreach (string modifier in CTD.Modifiers)
                modList += modifier + " ";
            modList = modList.ToLower();
            string className = node.Name;
            sb.AppendLine(String.Format(".class {0}auto ansi beforefieldinit {1} \nextends [mscorlib]System.Object \n", modList, className));
            info = new UTF8Encoding(true).GetBytes(sb.ToString());
            TCCLParser.currentFileStream.Write(info, 0, info.Length);

            // Write method header
            /* ------------------------ Visit All Fields -------------------------*/

            // Create a list of ClassBody children (should only be one)
            Utilities.VisitSpecificChild(node, typeof(ClassBody), this);

        }
        protected void VisitNode(ClassBody node)
        {

            // Write class header
            StringBuilder sb = new StringBuilder();

            // Open bracket 
            InsertIntoFile("{\n");

            // Create a list of FieldDcls children  (should only be one)
            Utilities.VisitSpecificChild(node, typeof(FieldDcls), this);
            
            // Close bracket 
            InsertIntoFile("\n}\n");
        }
        protected void VisitNode(MethodDcl node)
        {
            // Get a type descriptor for this method dcl
            MethodDclTypeDescp m = node.attrRef.typeInfo as MethodDclTypeDescp;
            StringBuilder sb = new StringBuilder();
            Byte[] info;

            methodName = m.Name;

            // Build modifier list
            string modList = "";
            foreach (string modifier in m.Modifiers)
                modList += modifier + " ";
            modList = modList.ToLower();

            // Build parameter list
            string theFunctArgs = "";
            int numberOfParams = 0;
            // Correlates variable names with stack position
            Dictionary<string, int> localArgs = new Dictionary<string, int>();

            if (m.paramList != null)
            {
                int localArgIdx = 0;
                foreach (string name in m.paramList.Names)
                {
                    localArgs.Add(name, localArgIdx++);
                    string entry = m.paramList.Types[numberOfParams] + " " + name;
                    if (numberOfParams++ > 0)
                        entry += ",";
                    theFunctArgs += entry + "\n";

                }

            }
            localArgNumber.Add(methodName, localArgs);

            sb.AppendLine(String.Format(".method {0}{1} \n {2} ({3}) cil managed", modList, m.returnType.ToLower(), m.Name, theFunctArgs));
            info = new UTF8Encoding(true).GetBytes(sb.ToString());
            TCCLParser.currentFileStream.Write(info, 0, info.Length);

            InsertIntoFile("{\n");

            if (m.Name.Contains("main") || m.Name.Contains("Main"))
            {
                InsertIntoFile(".entrypoint\n");
            }
            InsertIntoFile(".maxstack 1000 // Should be using sethi ullman algorithm here!\n"); // for now

            // If field decls, do them here too
            Utilities.VisitSpecificChild(node, typeof(LocalVarDecls), this);

            // Visi LocalVarsDeclsAndStatments
            // See if we have any fields
            Utilities.VisitSpecificChild(node, typeof(LocalVarDeclsAndStmts), this);

            // Visit Method Calls


            // Etc?
            InsertIntoFile("ret\n");

            InsertIntoFile("}\n");

        }
        protected void VisitNode(LocalVarDecls node)
        {
            // Correlates variable names with stack position
            Dictionary<string, int> localVars = new Dictionary<string, int>();
            int localVarIdx = 0;
            // We want to add some variables to the .il file
            // Visit all of DclName children
            List<DclName> declNameChildren = Utilities.GetChildren(node, typeof(DclName)).Cast<DclName>().ToList();
            String declString = "";
            if (declNameChildren.Count >0)
                declString = String.Format(".locals init (\n");
            foreach (DclName d in declNameChildren)
            {
                String varType = Utilities.TypeDescToString(d.attrRef);
                String varName = d.Name.Split(' ')[2];

                declString += String.Format("[{0}]\t{1}\t{2}", localVarIdx,
                    varType, varName);
                localVars.Add(varName, localVarIdx);
                localVarIdx++;
                if (localVarIdx > 0 && localVarIdx != declNameChildren.Count)
                    declString += ",";
                declString += "\n";

                // Store the stack order of this variable

            }
            localVarNumber.Add(methodName, localVars);
            InsertIntoFile(declString + ")\n");
            // commas
           

        }
        protected void VisitNode(LocalVarDeclsAndStmts node)
        {
            // Get local var decls
            // Get list of children that are not variable declarations (only assignment expr and method calls currently supported)
            List<AbstractNode> methodBodyChildren = Utilities.GetMethodBodyChildren(node, this);
            // Process them in reverse order
            for (int i = methodBodyChildren.Count - 1; i >= 0; i--)
            {
                methodBodyChildren[i].Accept(this);
            }

        }
        protected void VisitNode(SelectionStmt node)
        {
            // Visit boolean EXPR
            Utilities.VisitSpecificChild(node, typeof(Expr), this);
            InsertIntoFile("brfalse.s\t\t" + Label(labelNumber) + "\n");

            // Visit Then Clause
            Utilities.VisitThenChildren(node, this);
            InsertIntoFile("br.s\t\t" + Label(labelNumber+1) + "\n");
            InsertIntoFile(Label((labelNumber)) + ":\t\t nop\n");

            // Visit Else Clause
            Utilities.VisitElseChildren(node, this);
            InsertIntoFile(Label(labelNumber+1) + ":\t\t nop\n");
            labelNumber += 2;
        }
        protected void VisitNode(Stmt node)
        {
            // Iteration case (While loop)
            if (node.Name.Contains("Iteration"))
            {
                // Basically the same thing as if but we'll go back to the first conditional when we're done
                InsertIntoFile(Label((labelNumber)) + ":\t\t nop\n");

                // Visit boolean EXPR 
                Utilities.VisitSpecificChild(node, typeof(Expr), this);
                InsertIntoFile("brfalse.s\t\t" + Label(labelNumber+1) + "\n");

                // Visit Loop Body 
                Utilities.VisitLoopBodyChildren(node, this);
                InsertIntoFile("br.s\t\t" + Label(labelNumber) + "\n");
                InsertIntoFile(Label((labelNumber+1)) + ":\t\t nop\n");


                labelNumber+=2;
            }
      
        }



        protected void VisitNode(AssignmentExpr node)
        {
            Utilities.CodeGenVisitAllChildren(node, this);
        }
        protected void VisitNode(FieldDcls node)
        {
            // Get list of MethodDecls and list of MethodDcl and visit each
            List<MethodDcl> methodDclChildren = Utilities.GetChildren(node, typeof(MethodDcl)).Cast<MethodDcl>().ToList();
            foreach (MethodDcl md in methodDclChildren)
            {
                this.VisitNode(md);
            }

        }
        protected void VisitNode(FieldDcl node)
        {
            Utilities.VisitSpecificChild(node, typeof(MethodDcl), this);

        }
        protected void VisitNode(Number node)
        {
            InsertIntoFile(String.Format("ldc.i4\t\t {0}\n", node.Name));
        }
        protected void VisitNode(Expr node)
        {
            // Visit children *first*
            Utilities.CodeGenVisitAllChildren(node, this);
            string CIL_instruction = "";
            switch (node.Name)
            {
                case "-":
                    CIL_instruction = "sub";
                    break;
                case "+":
                    CIL_instruction="add";
                    break;
                case "/":
                    CIL_instruction = "div";
                    break;

                case "*":
                    CIL_instruction = "mul";
                    break;

                case "%":
                case "|":
                case "&":
                case "^":
                case "<":
                case ">":
                    CIL_instruction = "clt";
                    break;

                case "<=":
                case ">=":
                case "||":
                case "&&":
                case "==":
                    CIL_instruction = "ceq";
                    break;

                case "!=":

                case null:
                    break;


                default:
                    CIL_instruction = "Lenny, you suck";
                    break;


            }
            InsertIntoFile(String.Format(CIL_instruction + "\n"));

        }
        protected void VisitNode(QualifiedName node)
        {
           string CIL_instruction = "";
            int stackPosition = GetStackPosit(Utilities.FilterOutKeywords(node.Name));
            if (node.Name.Contains("LHS")) // part of an assignment expression
                CIL_instruction = String.Format("stloc {0} //{1}", stackPosition,
                    Utilities.FilterOutKeywords(node.Name));
            else CIL_instruction = String.Format("ldloc {0} //{1}", stackPosition, Utilities.FilterOutKeywords(node.Name));
            InsertIntoFile(CIL_instruction + "\n");

        }
        protected void VisitNode(MethodCall node)
        {


            // deal with arguments first
            // Arguments are in the arglist node's type descriptor.
            // If arguments were provided
            // We'll need one of these to understand what we're looking at
            MethodCallTypeDescriptor mtd = null;

            ArgListTypeDescriptor atd = null;
            ArgList proposedArgs = null;
            // "If the arg list for this has errors" is what we're trying to get at with this block
            // If we aren't error type descriptor try to get the args root
            if (Utilities.attrToType(node.attrRef) != "ErrorTypeDescriptor")
            {
                proposedArgs = node.attrRef.typeInfo.argsRoot;

                // If arguments are invalid
                if (proposedArgs != null && proposedArgs.attrRef.typeInfo.GetType() == typeof(ErrorTypeDescriptor))
                    Console.WriteLine("Error in code generator. ErrorTypeDescriptor encountered.");
                // If arguments are valid


                mtd = node.attrRef.typeInfo;
                string paramResult = "";
                if (proposedArgs != null)
                    atd = mtd.argsRoot.attrRef.typeInfo;
                // Keep track of how many arguments we're loading onto the stack prior to a function call
                if (atd != null)
                    foreach (var element in atd.Params)
                    {
                        if (Utilities.attrToType(element.attrRef).Contains("String"))
                        {
                            paramResult += "string";
                            // Overhead of pushing string to the stack
                            InsertIntoFile(String.Format("ldstr\t\t \"{0}\"\n", element.Name));
                        }
                        else if (Utilities.attrToType(element.attrRef).Contains("Integer"))
                        {
                            paramResult += "int32";

                            // If Identifier, use the number directly
                            if (element.NodeType.Name == "Identifier")
                                InsertIntoFile(String.Format("ldc.i4\t\t {0}\n", element.Name));

                            // If Qualified Name use variable name
                            else if (element.NodeType.Name == "QualifiedName")
                            {
                                int varNumber = 0;
                                // Either this is coming from function args (see here).
                                Dictionary<string, int> localArgs = new Dictionary<string, int>();
                                localArgNumber.TryGetValue(methodName, out localArgs);
                                if (localArgs.ContainsKey(element.Name))
                                {
                                    varNumber = GetArgPosit(element.Name);
                                    InsertIntoFile(String.Format("ldarg {0}\t\t //{1}\n", varNumber, element.Name));
                                }
                                else
                                {
                                    varNumber = GetStackPosit(element.Name);
                                    InsertIntoFile(String.Format("ldloc {0}\t\t //{1}\n", varNumber, element.Name));
                                }

                                // Or this is coming from function local vars (see here)

                            }

                            // If Expr, generate instructions to evaluate expr
                            else if (element.NodeType.Name == "Expr")
                            {
                                // Visit all expr nodes and have them generate the code. The other way to do this 
                                // would be as defined in this comment block which I was told not to do in class
                                // (But works!)
                                /*
                                Expr e = element as Expr;
                                int expressionEval = e.eval();
                                InsertIntoFile(String.Format("ldc.i4\t\t {0}\n", expressionEval));
                                */
                                element.Accept(this);

                            }
                        }

                        else
                        {
                            paramResult += "DEFAULT uh oh";
                        }
                    }

                // call method
                String nameSpaceString = mtd.nameSpaceVar;
                InsertIntoFile(String.Format("call {0} {1} :: {2}({3})\n", mtd.returnType.ToLower(),
                    mtd.nameSpaceVar, mtd.name, paramResult));


            }


#if false // Get a type descriptor for this method dcl
            MethodDclTypeDescp m = node.attrRef.typeInfo as MethodDclTypeDescp;
            StringBuilder sb = new StringBuilder();
            Byte[] info;


            // Build modifier list
            string modList = "";
            foreach (string modifier in m.Modifiers)
                modList += modifier + " ";
            modList = modList.ToLower();

            // Build parameter list
            string theFunctArgs = "";
            int numberOfParams = 0;
            if (m.paramList != null)
            {
                foreach (string name in m.paramList.Names)
                {
                    string entry = name + " " + m.paramList.Types;
                    if (numberOfParams++ > 0)
                        entry += ",";
                    theFunctArgs += entry + "\n";

                }
            }
            sb.AppendLine(String.Format(".method {0}{1} \n {2} ({3}) cil managed", modList, m.returnType, m.Name, theFunctArgs));
            info = new UTF8Encoding(true).GetBytes(sb.ToString());
            TCCLParser.currentFileStream.Write(info, 0, info.Length);

            InsertIntoFile("{\n");

            if (m.Name.Contains("main") || m.Name.Contains("Main"))
            {
                InsertIntoFile(".entrypoint\n");
            }
            InsertIntoFile(".maxstack 1000\n"); // for now

            // If field decls, do them here too
            // Visit children
            Utilities.CodeGenVisitAllChildren(node, this);
#endif

        }

        private int GetStackPosit(string nodeName)
        {
            Dictionary<string, int> localVarLookup;
            localVarNumber.TryGetValue(methodName, out localVarLookup);
            int stackPosition;
            localVarLookup.TryGetValue(nodeName, out stackPosition);
            return stackPosition;
        }
        private int GetArgPosit(string nodeName)
        {
            Dictionary<string, int> localArgLookup;
            localArgNumber.TryGetValue(methodName, out localArgLookup);
            int stackPosition;
            localArgLookup.TryGetValue(nodeName, out stackPosition);
            return stackPosition;
        }
        private string Label(int arg)
        {
            return ("lbl_" + arg);
        }

        private static void InsertIntoFile(string s)
        {
            byte[] info;
            info = new UTF8Encoding(true).GetBytes(s);
            TCCLParser.currentFileStream.Write(info, 0, info.Length);
        }


    }

}
