using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ASTBuilder
{
    /*-------------------------------------------------------SEMANTICS----------------------------------------------------------*/
    class SemanticsVisitor : IReflectiveVisitor
    {
        // This method is the key to implenting the Reflective Visitor pattern
        // in C#, using the 'dynamic' type specification to accept any node type.
        // The subsequent call to VisitNode does dynamic lookup to find the
        // appropriate Version.

        public void Visit(dynamic node)
        {
            this.VisitNode(node);
        }

        // Call this method to begin the semantic checking process
        public void CheckSemantics(AbstractNode node)
        {
            if (node == null)
            {
                return;
            }
            TopDclVisitor tdv = new TopDclVisitor();
            // Do Top-Declaration pass
            node.Accept(tdv);
            ///More here
        }
        public void VisitNode(AbstractNode node)
        {
            if (node.Child != null)
                node.Child.Accept(this);

            if (node.Sib != null)
                node.Sib.Accept(this);

        }

        public void VisitNode(Modifier node)
        {

        }
        protected virtual void VisitNode(ClassDcl node)
        {
            // Create a type descriptor for the class

            // Put a new symbol table into the descriptor
            // Create a new attributes structure
            // Put the descriptor in the attributes structure
            // Put the class name and attributes structure in the symbol table.

            // ---------- 

            // Configure the descriptor


            // Create a symbol table to hodl the names declared within the class

            // Create an attributes structure for the class 

            // Enter class name into current symbol table  

            // Open scope in the symbol table

            // Accept the dcl visitor for fields and methods




            Console.WriteLine("Visiting class");
        }

    }
    /*---------------------------------------------------------TYPE--------------------------------------------------------------*/
    class TypeVisitor : SemanticsVisitor, IReflectiveVisitor
    {
        public new void Visit(dynamic node)
        {
            this.VisitNode(node);
        }
        public void VisitNode(QualifiedName node)
        {
            // Get this symbol from the symbol table


            AbstractAttr proposedType = TopDclVisitor.currentSymbolTable.lookup(Utilities.FilterOutKeywords(node.Name));

            if (proposedType == null)
            {
                TopDclVisitor.Error(String.Format("Warning! Type {0} does not exist in symbol table",
                    Utilities.FilterOutKeywords(node.Name)));

                proposedType = new Attributes("Error");
                proposedType.typeInfo = new ErrorTypeDescriptor();
            }
            node.attrRef = proposedType;
        }
        public void VisitNode(PrimitiveType node)
        {
            // Get this symbol from the symbol table
            AbstractAttr proposedType = TopDclVisitor.currentSymbolTable.lookup(Utilities.FilterOutKeywords(node.Name));
            if (proposedType == null)
            {
                TopDclVisitor.Error(String.Format("Warning! Type {0} does not exist in symbol table!", node.Name));
                proposedType = new Attributes("Error");
                proposedType.typeInfo = new ErrorTypeDescriptor();
            }
            node.attrRef = proposedType;
        }


        public void VisitNode(Expr node)
        {
            Utilities.TypeVisitAllChildren(node);

            Attributes exprAttr = new Attributes("Expr");
            node.attrRef = exprAttr;

            ErrorTypeDescriptor ETD = new ErrorTypeDescriptor();

            switch (node.Name)
            {
                case "-":
                case "+":
                case "/":
                case "*":
                case "%":
                case "|":
                case "&":
                case "^":
                    IntegerTypeDescriptor ITD = new IntegerTypeDescriptor();
                    if (allChildrenAreInts(node) == true)
                        exprAttr.typeInfo = ITD;
                    else
                        exprAttr.typeInfo = ETD;
                    break;
                case "<":
                case ">":
                case "<=":
                case ">=":
                case "||":
                case "&&":
                case "==":
                case "!=":
                    BooleanTypeDescriptor BTD = new BooleanTypeDescriptor();
                    if (allChildrenAreSame(node) == true)
                        exprAttr.typeInfo = BTD;
                    else
                        exprAttr.typeInfo = ETD;
                    break;
                case null:
                    ContainerTypeDescriptor CTD = new ContainerTypeDescriptor();
                    exprAttr.typeInfo = CTD;

                    break;

            }

        }

        private bool allChildrenAreSame(Expr node)
        {
            {
                Utilities.TypeVisitAllChildren(node);

                AbstractNode child = node.Child;
                AbstractNode lastChild = node.Child;
                Boolean isOkay = true;
                // Child
                while (child != null)
                {
                    if (child.attrRef != null)
                    {
                        // If no typeref, visit this node first
                        if (child.attrRef.typeInfo == null)
                        {
                            TypeVisitor t = new TypeVisitor();
                            child.Accept(t);
                        }
                        // If we have a typeref, evaluate the node
                        else
                        {
                            if (Utilities.attrToType(lastChild.attrRef) !=
                                 Utilities.attrToType(child.attrRef))
                            {
                                isOkay = false;
                                lastChild = child;
                            }

                            //  Do the same thing to child's child
                            isOkay &= allChildrenAreInts(child);
                            // Now that we're done, move on to sibling
                            child = child.Sib;
                        }
                    }
                }
                return isOkay;
            }
        }

        private bool allChildrenAreInts(AbstractNode node)
        {
            Utilities.TypeVisitAllChildren(node);

            AbstractNode child = node.Child;
            Boolean isOkay = true;
            // Child
            while (child != null)
            {
                if (child.attrRef != null)
                {
                    // If no typeref, visit this node first
                    if (child.attrRef.typeInfo == null)
                    {
                        TypeVisitor t = new TypeVisitor();
                        child.Accept(t);
                    }
                    // If we have a typeref, evaluate the node
                    else if (Utilities.attrToType(child.attrRef) != "IntegerTypeDescriptor")
                        isOkay = false;


                    //  Do the same thing to child's child
                    isOkay &= allChildrenAreInts(child);
                    // Now that we're done, move on to sibling
                    child = child.Sib;
                }
            }
            return isOkay;
        }

        public void VisitNode(StringArg node)
        {
            Attributes a = new Attributes("Number");
            StringTypeDescriptor STD = new StringTypeDescriptor();
            a.typeInfo = STD;
            node.attrRef = a;

        }
        public void VisitNode(Number node)
        {
            Attributes a = new Attributes("Number");
            IntegerTypeDescriptor ITD = new IntegerTypeDescriptor();
            a.typeInfo = ITD;
            node.attrRef = a;

        }
    }
    /*---------------------------------------------------------TOPDCL--------------------------------------------------------*/

    class TopDclVisitor : SemanticsVisitor, IReflectiveVisitor
    {
        // Keep track of current symbol table  
        public static SymbolTable currentSymbolTable;
        // Keep track of class name we're within (instead of navigating back up the tree)
        private static String currentClassName;

        public TopDclVisitor()
        {
            currentSymbolTable = new SymbolTable();
        }

        public new void Visit(dynamic node)
        {
            this.VisitNode(node);
        }
        public void VisitNode(LocalVarDeclsAndStmts node)
        {
            // Here we want to visit all LocalVarDclStmts and then Statments ORDER MATTERS HERE!  

            // Create a list of all Local Var Dcl children and visit them
            List<LocalVarDeclStmt> LVDChildren = Utilities.GetChildren(node, typeof(LocalVarDeclStmt)).Cast<LocalVarDeclStmt>().ToList();
            foreach (LocalVarDeclStmt l in LVDChildren)
            {
                this.VisitNode(l);
            }

            // Create a list of all AssignmentExpr children and visit them
            List<AssignmentExpr> assChildren = Utilities.GetChildren(node, typeof(AssignmentExpr)).Cast<AssignmentExpr>().ToList();
            foreach (AssignmentExpr a in assChildren)
            {
                this.VisitNode(a);
            }

            // Create a list of all methodcall children and visit them 
            List<MethodCall> methChildren = Utilities.GetChildren(node, typeof(MethodCall)).Cast<MethodCall>().ToList();
            foreach (MethodCall m in methChildren)
            {
                this.VisitNode(m);
            }
            // Create a list of all SelectionStmt children and visit them
            List<SelectionStmt> SelectionChildren = Utilities.GetChildren(node, typeof(SelectionStmt)).Cast<SelectionStmt>().ToList();
            foreach (SelectionStmt s in SelectionChildren)
            {
                this.VisitNode(s);
            }
            // Create a list of all Stmt children and visit them
            List<Stmt> StmtChildren = Utilities.GetChildren(node, typeof(Stmt)).Cast<Stmt>().ToList();
            foreach (Stmt s in StmtChildren)
            {
                this.VisitNode(s);
            }
        }
        /*||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||*/
        protected override void VisitNode(ClassDcl node)
        {
            // Create a type descriptor for the class
            ClassTypeDescriptor typeRef = new ClassTypeDescriptor();
            // Put a new symbol table into the descriptor
            typeRef.ClassSymbolTable = new SymbolTable();

            // Create a new attributes structure
            Attributes attr = new Attributes("classDcl");
            // Put the descriptor in the attributes structure
            attr.typeInfo = typeRef;

            // Put the class name and attributes structure in the symbol table.
            String name = Utilities.FilterOutKeywords(node.Name);
            // Record our name globally
            currentClassName = Utilities.FilterOutKeywords(node.Name);
            currentSymbolTable.enter(name, attr);
            // Decorate AST with attrRef
            node.attrRef = attr;

            /* -----------------------------*/
            // Configure the descriptor

            // Create a list of all modifier children
            List<Modifier> modifierChildren = Utilities.GetChildren(node, typeof(Modifier)).Cast<Modifier>().ToList();

            // Process modifier children
            List<String> listOfModiifers = new List<String>();
            foreach (Modifier m in modifierChildren)
            {
                listOfModiifers.Add(m.Name);
            }

            // Add the list of modifiers to the Type Descriptor
            typeRef.Modifiers = listOfModiifers;

            // Switch current symbol table to class's
            //currentSymbolTable = typeRef.ClassSymbolTable;
            currentSymbolTable.incrNestLevel(); 

            /* ------------------------ Visit All Fields -------------------------*/

            // Create a list of ClassBody children (should only be one)
            List<ClassBody> classBodyChildren = Utilities.GetChildren(node, typeof(ClassBody)).Cast<ClassBody>().ToList();
            foreach (ClassBody c in classBodyChildren)
            {
                this.VisitNode(c);
            }
            currentSymbolTable.decrNestLevel();

        }
        public void VisitNode(MethodDcl node)
        {
            /* --------- Gather some info --------------*/
            // Get Name
            String name = GetMethodDclName(node);
            // Get Return TYpe
            String returnType = GetMethodDclRetType(node);
            // Get class name that owns this method
            String definedInClass = currentClassName;

            // Get modifiers
            // Create a list of all modifier children
            List<Modifier> modifierChildren = Utilities.GetChildren(node, typeof(Modifier)).Cast<Modifier>().ToList();
            // Process modifier children
            List<String> listOfModifiers = new List<String>();
            foreach (Modifier m in modifierChildren)
            {
                listOfModifiers.Add(m.Name);
            }

            // Get parameters  
            List<string> types = new List<string>();
            List<string> names = new List<string>();
            GetMethodDclParams(node, ref types, ref names);

            // Create a type descriptor for the method
            MethodDclTypeDescp typeRef = new MethodDclTypeDescp();
            // Create a new attributes structure
            Attributes attr = new Attributes("method");

            /* ------------ Configure Attributes ----------------*/
            // Put the descriptor in the attributes structure
            attr.typeInfo = typeRef;

            /* --------- Configure Type Descriptor --------------*/
            // Put a new symbol table into Type Descriptor
            typeRef.MethodSymbolTable = new SymbolTable();
            // Put the name into Type Descriptor
            typeRef.Name = name;
            // Put the retury type into Type Descriptor
            typeRef.returnType = returnType;
            // Put the list of modifiers into Type Descriptor
            typeRef.Modifiers = listOfModifiers;
            // Put the owning class into Type Descriptor
            typeRef.isDefinedIn = definedInClass;
            // Put the list of param types into Type Descriptor
            typeRef.paramList.Types = types;
            // Put the list of param names into Type Descriptor
            typeRef.paramList.Names = names;
            // Namespace
            typeRef.nameSpaceVar = typeRef.isDefinedIn; //was confused between the two until this was the easier solution

            // Throw all of the arguments into the symbol table;
            currentSymbolTable.incrNestLevel();
            for (int i = 0; i < names.Count; i++)
            {
                // If type exists in symbol table
                Attributes aa = currentSymbolTable.lookup(types[i]);
                if (aa != null)
                    currentSymbolTable.enter(names[i], aa);

            }

            /* ------ Ready to record into symbol table ---------*/
            // Record ourselves into the symbol table
            currentSymbolTable.enter(name, attr);
            // Decorate AST with attrRef
            node.attrRef = attr;

            /* --------------- Visit All Fields -----------------*/
           // currentSymbolTable = typeRef.MethodSymbolTable;

            // Create a list of Block children (should only be one)
            List<Block> methodBodyChildren = Utilities.GetChildren(node, typeof(Block)).Cast<Block>().ToList();
            foreach (Block b in methodBodyChildren)
            {
                this.VisitNode(b);
            }
            // Put symbol table back when we're done
            currentSymbolTable.decrNestLevel();

        }
        public void VisitNode(Block node)
        {
            // Just forward to LocalVarDclsAndStmts. This is probably not the most efficient tree ever. 

            // Create a list of LocalVarDclsAndStmts children (should only be one)
            List<LocalVarDeclsAndStmts> LVDAS = Utilities.GetChildren(node, typeof(LocalVarDeclsAndStmts)).Cast<LocalVarDeclsAndStmts>().ToList();
            foreach (LocalVarDeclsAndStmts l in LVDAS)
            {
                this.VisitNode(l);
            }
        }
        public void VisitNode(AssignmentExpr node)
        {
            String RHS_Type;
            Utilities.TypeVisitAllChildren(node);

            Attributes a = new Attributes("Assignment");
            AssignmentTypeDescriptor ATD = new AssignmentTypeDescriptor();

            // Get Qualified Name
            List<QualifiedName> matchingChildren = Utilities.GetChildren(node, typeof(QualifiedName)).Cast<QualifiedName>().ToList();
            ATD.LHS_Type = Utilities.attrToType(matchingChildren[0].attrRef);

            RHS_Type = ATD.LHS_Type;
            a.typeInfo = ATD;
            node.attrRef = a;

            // Error type if problem with assignment
            if (IsAssignable(ATD.LHS_Type, node, ref RHS_Type) == false)
            {
                Error(String.Format("Warning! Cannot assign {0} to {1}", RHS_Type, ATD.LHS_Type));
                node.attrRef.typeInfo = new ErrorTypeDescriptor();
            }
            ATD.RHS_Type = RHS_Type;
        }
        public void VisitNode(LocalVarDeclStmt node)
        {

            // Type of this declaration
            Type type = null;

            // See if we have a primitive type  
            var primitiveTypeChildren = Utilities.GetChildren(node, typeof(PrimitiveType)).Cast<PrimitiveType>().ToList();
            // If we have a primitive type go visit it
            if (primitiveTypeChildren.Count > 0)
            {
                TypeVisitor t = new TypeVisitor();
                primitiveTypeChildren[0].Accept(t);
                Attributes a = primitiveTypeChildren[0].attrRef;
                type = a.typeInfo.GetType();
            }
            // Otherwise we better have a qualified name
            else
            {
                var qualNameChildren = Utilities.GetChildren(node, typeof(QualifiedName)).Cast<QualifiedName>().ToList();
                // If we have a qualified name go visit it
                if (qualNameChildren.Count > 0)
                {
                    TypeVisitor t = new TypeVisitor();
                    qualNameChildren[0].Accept(t);
                    Attributes a = qualNameChildren[0].attrRef;
                    type = a.typeInfo.GetType();
                }
            }
            // Now we have a type. Could be error, or a valid type.

            Attributes myAttr = new Attributes("VarDcl");
            myAttr.typeInfo = type;
            node.attrRef = myAttr;
            // Go to LocalVarDecls and Get all DclName children
            LocalVarDecls dclsNode = Utilities.GetChildren(node, typeof(LocalVarDecls)).Cast<LocalVarDecls>().ToList()[0];
            List<DclName> matchingChildren = Utilities.GetChildren(dclsNode, typeof(DclName)).Cast<DclName>().ToList();
            foreach (DclName d in matchingChildren)
            {
                // Enter these children into the symbol table
                // Extract the name (ugly but the name is currently "LocalVar -> xxxx" and we want xxxx)
                String name = d.Name.Split(' ')[2];
                name = Utilities.FilterOutKeywords(name);
                currentSymbolTable.enter(name, myAttr);

                // Also, go ahead and record the type for all of these nodes for printing
                d.attrRef = myAttr;
            }
        }
        public void VisitNode(FieldDcls node)
        {
            // Create a list of method children
            List<MethodDcl> methodDclChildren = Utilities.GetChildren(node, typeof(MethodDcl)).Cast<MethodDcl>().ToList();

            // Visit all methods (reverse order to go top down
            for (int i = methodDclChildren.Count -1; i >= 0; i--)
            {
                this.VisitNode(methodDclChildren[i]);
            }
        }
        public void VisitNode(ClassBody node)
        {
            // Create a list of FieldDcls children  (should only be one)
            List<FieldDcls> matchingChildren = Utilities.GetChildren(node, typeof(FieldDcls)).Cast<FieldDcls>().ToList();
            foreach (FieldDcls f in matchingChildren)
            {
                this.VisitNode(f);
            }
        }
        public void VisitNode(ArgList node)
        {

            // Arg list for which method call?
            String owningMethod = null;
            // Find sibling with the method call info
            AbstractNode tempNode;
            tempNode = node.First;

            while (tempNode != null)
            {
                // Find qualified name sibling and leave
                if (tempNode.GetType() == typeof(QualifiedName))
                {
                    owningMethod = Utilities.FilterOutKeywords(tempNode.Name);
                    break;
                }
                tempNode = tempNode.Sib;
            }


            Utilities.TypeVisitAllChildren(node);

            Attributes a = new Attributes("ArgList");
            ArgListTypeDescriptor ATD = new ArgListTypeDescriptor();
            a.typeInfo = ATD;
            node.attrRef = a;

            // Error type if problem with assignment
            if (IsValidArgList(node) == false)
            {
                Error(String.Format("Warning! Invalid Argument List when calling method {0}", owningMethod));
                node.attrRef.typeInfo = new ErrorTypeDescriptor();
            }

            // Also add the parameters to the typeInfo for printing later
            List<AbstractNode> listOfParams = new List<AbstractNode>();
            ATD.Params = listOfParams;
            ATD.OwningMethod = owningMethod;
            //Add every child to the parameters
            tempNode = node.Child;

            while (tempNode != null)
            {
                listOfParams.Add(tempNode);
                // Find qualified name sibling and leave
                if (tempNode.GetType() == typeof(QualifiedName))
                {
                    owningMethod = Utilities.FilterOutKeywords(tempNode.Name);
                    break;
                }
                tempNode = tempNode.Sib;
            }

        }
        public void VisitNode(MethodCall node)
        {
            Attributes a = new Attributes("MethodCall");
            MethodCallTypeDescriptor MTD = new MethodCallTypeDescriptor();
            ErrorTypeDescriptor ETD = new ErrorTypeDescriptor();

            node.attrRef = a;

            /* --------------- Type Check Qualified Name and extract data -----------------*/
            // Get Name
            String name = GetMethCallName(node);

            /* ----------------------- Semantics Visit ArgList (if exists)--------------------------*/
            // Go to ArgList
            List<ArgList> argListChildren = Utilities.GetChildren(node, typeof(ArgList)).Cast<ArgList>().ToList();
            // If Args exist...
            if (argListChildren.Count > 0)
            {
                // Type check the argument Node
                argListChildren[0].Accept(this);
                // Record it in our type ref
                MTD.argsRoot = argListChildren[0];
            }


            /* ------------ Configure Attributes ----------------*/
            // Put the descriptor in the attributes structure
            a.typeInfo = MTD;

            /* --------- Configure Type Descriptor --------------*/
            MTD.name = name;
            Attributes attr = currentSymbolTable.lookup(name);
            // If method exists in symbol table
            if (attr != null)
            {
                MTD.returnType = attr.typeInfo.returnType;
                MTD.nameSpaceVar = attr.typeInfo.nameSpaceVar;
                a.typeInfo = MTD;
            }
            // Method not found in symbol table
            else
            {
                a.typeInfo = ETD;

            }

            // Decorate AST with attrRef
            node.attrRef = a;


        }
        public void VisitNode(SelectionStmt node)
        {
            //currentSymbolTable.incrNestLevel();
            // Dcl-Visit all children
            Utilities.DclVisitAllChildren(node, this);
            // We always have an expression
            // We always have a statement
            // We sometimes have an else statement

            Attributes a = new Attributes("SelectionStmt");
            SelectionStmtTypeDescriptor STD = new SelectionStmtTypeDescriptor();
            STD.StatementNode = null;
            STD.ElsePathNode = null;
            STD.StatementNode = null;
            ErrorTypeDescriptor ETD = new ErrorTypeDescriptor();

            // If our expression is a boolean, we are valid
            a.typeInfo = ETD;
            List<Expr> matchingChildren = Utilities.GetChildren(node, typeof(Expr)).Cast<Expr>().ToList();
            if (Utilities.attrToType(matchingChildren[0].attrRef) == "BooleanTypeDescriptor")
            {
                STD.StatementNode = matchingChildren[0];
                a.typeInfo = STD;
            }
            // Get all children
            AbstractNode an;
            an = node.Child;
            while (an != null)
            {
                // Filter out all expr from this 
                if (Utilities.attrToType(an.attrRef) != "BooleanTypeDescriptor")
                {
                    // Add the else to the else in the type descp
                    if (an.Name != null && an.Name.Contains("Else"))
                        STD.ElsePathNode = an;
                    else
                        STD.StatementNode = an;

                }
                an = an.Sib;
            }


            node.attrRef = a;

            //currentSymbolTable.decrNestLevel();

        }
        public void VisitNode(Stmt node)
        {
            // Iteration statements handled here
            if (node.Name != null && node.Name.Contains("Iteration"))
            {
                // Dcl-Visit all children
                Utilities.DclVisitAllChildren(node, this);
                // We always have an expression
                // We always have a block

                Attributes a = new Attributes("IterationStmt");
                IterationStmtTypeDescriptor ITD = new IterationStmtTypeDescriptor();
                ITD.BlockNode = null;
                ITD.ConditionNode = null;
                ErrorTypeDescriptor ETD = new ErrorTypeDescriptor();

                // If our expression is a boolean, we are valid
                a.typeInfo = ETD;
                List<Expr> matchingChildren = Utilities.GetChildren(node, typeof(Expr)).Cast<Expr>().ToList();
                if (Utilities.attrToType(matchingChildren[0].attrRef) == "BooleanTypeDescriptor")
                {
                    a.typeInfo = ITD;
                }
                // Get all children
                AbstractNode an;
                an = node.Child;
                while (an != null)
                {
                    // Filter out all expr from this 
                    if (an.NodeType.Name == "Expr")
                    {
                        ITD.ConditionNode = an;
                    }
                    if (an.NodeType.Name == "Block")
                    {
                        ITD.BlockNode = an;
                    }
                    an = an.Sib;
                }


                node.attrRef = a;

                //currentSymbolTable.decrNestLevel();
            }
        }
        public void VisitNode(Expr node)
        {
            TypeVisitor t = new TypeVisitor();
            node.Accept(t);
        }

        private bool IsValidArgList(AbstractNode node)
        {
            // If we weant to check method signatures, this is where we do it
            // For now the check is implicitly arithmetic consistancy 

            AbstractNode child = node.Child;
            Boolean isOkay = true;
            // Child
            while (child != null)
            {
                // If any of the children are error type, propogate that info up
                if (child.attrRef != null && child.attrRef.typeInfo.GetType() == typeof(ErrorTypeDescriptor))
                {
                    isOkay = false;
                }
                //  Do the same thing to child's child
                isOkay &= IsValidArgList(child);
                // Now that we're done, move on to sibling
                child = child.Sib;
            }
            return isOkay;
        }


        private bool IsAssignable(string LHS_Type, AbstractNode node, ref String RHS_Type)
        {
            AbstractNode child = node.Child;
            Boolean isOkay = true;
            // Child
            while (child != null)
            {
                // If child does not have the same type as RHS
                if (child.attrRef != null && Utilities.attrToType(child.attrRef) != LHS_Type)
                {
                    // Record type of child
                    RHS_Type = Utilities.attrToType(child.attrRef);
                    // Record that we are not okay
                    isOkay = false;
                }
                // Do the same thing to child's child
                isOkay &= IsAssignable(LHS_Type, child, ref RHS_Type);
                // Now that we're done, move on to sibling
                child = child.Sib;
            }
            return isOkay;
        }

        public static void Error(string s)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(s);
            Console.ResetColor();
        }

        /* MethodDcl */
        private void GetMethodDclParams(MethodDcl node, ref List<string> types, ref List<string> names)
        {
            List<ParamList> paramListChildren;
            List<Param> paramChildren;
            // Go to MethodDcla
            List<MethodDcla> methodDclaChildren = Utilities.GetChildren(node, typeof(MethodDcla)).Cast<MethodDcla>().ToList();
            // Go to ParamList if it exists
            if (methodDclaChildren.Count > 0)
            {
                paramListChildren = Utilities.GetChildren(methodDclaChildren[0], typeof(ParamList)).Cast<ParamList>().ToList();
                // Create a list of all of the params
                if (paramListChildren.Count > 0)
                {
                    paramChildren = Utilities.GetChildren(paramListChildren[0], typeof(Param)).Cast<Param>().ToList();

                    foreach (Param p in paramChildren)
                    {
                        // Go to Primitive type and extract the type
                        // Create a list of primitivetype children (should only be one)  
                        List<PrimitiveType> typeChildren = Utilities.GetChildren(p, typeof(PrimitiveType)).Cast<PrimitiveType>().ToList();
                        // Go ahead and type visit the node we found
                        TypeVisitor t = new TypeVisitor();
                        typeChildren[0].Accept(t);
                        Attributes tempAttr = typeChildren[0].attrRef;
                        types.Add(Utilities.TypeDescToString(tempAttr));

                        // Go to DclName and extract the name
                        // Create a list of DclName children (should only be one)  
                        List<DclName> nameChildren = Utilities.GetChildren(p, typeof(DclName)).Cast<DclName>().ToList();
                        // Extract the name (ugly but the name is currently "Declarator -> xxxx" and we want xxxx)
                        names.Add(nameChildren[0].Name.Split(' ')[2]);
                        // We can also add some attr Refs to it for tree printing
                        nameChildren[0].attrRef = tempAttr;
                    }
                }
            }
        }
        private string GetMethodDclRetType(MethodDcl node)
        {
            string methodRT;
            // ReturnType is in a PrimitiveType 
            // Go to MethodDcla
            List<PrimitiveType> matchingChildren = Utilities.GetChildren(node, typeof(PrimitiveType)).Cast<PrimitiveType>().ToList();
            methodRT = matchingChildren[0].Name;
            return methodRT;
        }
        private string GetMethodDclName(MethodDcl node)
        {
            string methodName;
            // Name is in a DclName under a MethodDcla child. 
            // Go to MethodDcla
            List<MethodDcla> methodDclaChildren = Utilities.GetChildren(node, typeof(MethodDcla)).Cast<MethodDcla>().ToList();
            // Go to DclName
            List<DclName> dclNameChildren = Utilities.GetChildren(methodDclaChildren[0], typeof(DclName)).Cast<DclName>().ToList();
            // Extract name (ugly but the name is currently "Method -> xxxx" and we want xxxx)
            methodName = dclNameChildren[0].Name.Split(' ')[2];
            return methodName;
        }
        /* MethodCall */
        private string GetMethCallName(MethodCall node)
        {
            string methodName;
            // Type visit the QUalifiedName node
            TypeVisitor t = new TypeVisitor();

            // Go to MethodDcla
            List<QualifiedName> qualNameChildren = Utilities.GetChildren(node, typeof(QualifiedName)).Cast<QualifiedName>().ToList();
            qualNameChildren[0].Accept(t);

            // Extract name
            methodName = qualNameChildren[0].Name;

            return methodName;
        }
        /* Field Dcls */

      
    }

}
