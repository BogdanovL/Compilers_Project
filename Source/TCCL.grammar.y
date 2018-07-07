%namespace ASTBuilder
%partial
%parsertype TCCLParser
%visibility internal
%tokentype Token
%YYSTYPE AbstractNode


%start CompilationUnit

%token STATIC, STRUCT, QUESTION, RSLASH, MINUSOP, NULL, INT, OP_EQ, OP_LT, COLON, OP_LOR
%token ELSE, PERCENT, THIS, CLASS, PIPE, PUBLIC, PERIOD, HAT, COMMA, VOID, TILDE
%token LPAREN, RPAREN, OP_GE, SEMICOLON, IF, NEW, WHILE, PRIVATE, BANG, OP_LE, AND 
%token LBRACE, RBRACE, LBRACKET, RBRACKET, BOOLEAN, INSTANCEOF, ASTERISK, EQUALS, PLUSOP
%token RETURN, OP_GT, OP_NE, OP_LAND, INT_NUMBER, IDENTIFIER, LITERAL, SUPER

%right EQUALS
%left  OP_LOR
%left  OP_LAND
%left  PIPE
%left  HAT
%left  AND
%left  OP_EQ, OP_NE
%left  OP_GT, OP_LT, OP_LE, OP_GE
%left  PLUSOP, MINUSOP
%left  ASTERISK, RSLASH, PERCENT
%left  UNARY 

%%

CompilationUnit		:	ClassDeclaration														{$$ = $1; Program.PrintTree($$);}
					;

ClassDeclaration	:	Modifiers CLASS Identifier ClassBody									{ $$ = new ClassDcl($1, $3, $4); } // modifiers, identifiers, classbody
					;

Modifiers			:	PUBLIC																	{ $$ = new Modifier("PUBLIC");       }
					|	PRIVATE																	{ $$ = new Modifier("PRIVATE");      }
					|	STATIC																	{ $$ = new Modifier("STATIC");       }
					|	Modifiers PUBLIC														{ $$ = new Modifier("PUBLIC", $1);   }  
					|	Modifiers PRIVATE					   									{ $$ = new Modifier("PRIVATE", $1);  }	
					|	Modifiers STATIC														{ $$ = new Modifier("STATIC", $1);   }	
					;
ClassBody			:	LBRACE FieldDeclarations RBRACE											{ $$ = new ClassBody($2);            }	
					|	LBRACE RBRACE															{ $$ = new ClassBody();			     }
					;

FieldDeclarations	:	FieldDeclaration														{ $$ = new FieldDcls($1);			}
					|	FieldDeclarations FieldDeclaration										{ $$ = new FieldDcls($1, $2);		}	
					;

FieldDeclaration	:	FieldVariableDeclaration SEMICOLON										{ $$ = ($1); }  // { $$ = new FieldDcl($1);			}
					|	MethodDeclaration														{ $$ = ($1); }  // { $$ = new FieldDcl($1);			}
					|	ConstructorDeclaration													{ $$ = ($1); }  // { $$ = new FieldDcl($1);			}
					|	StaticInitializer														{ $$ = ($1); }  // { $$ = new FieldDcl($1);			}
					|	StructDeclaration														{ $$ = ($1); }  // { $$ = new FieldDcl($1);			}
					;

StructDeclaration	:	Modifiers STRUCT Identifier ClassBody									{ $$ = new StructDcl($1, $3, $4); } // modifiers, identifiers, classbody
					;



/*
 * This isn't structured so nicely for a bottom up parse.  Recall
 * the example I did in class for Digits, where the "type" of the digits
 * (i.e., the base) is sitting off to the side.  You'll have to do something
 * here to get the information where you want it, so that the declarations can
 * be suitably annotated with their type and modifier information.
 */
FieldVariableDeclaration	:	Modifiers TypeSpecifier FieldVariableDeclarators				{ $$ = new FieldVarDcl($1, $2, $3); } // modifier, typeSpecifier, fieldVarDeclarators
							;

TypeSpecifier				:	TypeName														{ $$ = ($1); }  // { $$ = new TypeSpecifier($1);   }
							| 	ArraySpecifier													{ $$ = ($1); }  // { $$ = new TypeSpecifier($1);   }
							;

TypeName					:	PrimitiveType													{ $$ = ($1); }  // { $$ = new TypeName($1);   }
							|   QualifiedName													{ $$ = ($1); }  // { $$ = new TypeName($1);   }
							;

ArraySpecifier				: 	TypeName LBRACKET RBRACKET										{ $$ = new ArraySpecifier($1);   }
							;
								
PrimitiveType				:	BOOLEAN															{ $$ = new PrimitiveType("BOOLEAN");   }
							|	INT																{ $$ = new PrimitiveType("int32");   }	 
							|	VOID															{ $$ = new PrimitiveType("VOID");   }	
							;

FieldVariableDeclarators	:	FieldVariableDeclaratorName										{ $$ = new FieldVarDcls($1); } 
							|   FieldVariableDeclarators COMMA FieldVariableDeclaratorName		{ $$ = new FieldVarDcls($1, $3); }
							;


MethodDeclaration			:	Modifiers TypeSpecifier MethodDeclarator MethodBody				{ $$ = new MethodDcl($1, $2, $3, $4); }
							;

MethodDeclarator			:	MethodDeclaratorName LPAREN ParameterList RPAREN				{ $$ = new MethodDcla($1, $3); }
							|   MethodDeclaratorName LPAREN RPAREN								{ $$ = new MethodDcla($1); }
							;

ParameterList				:	Parameter														{ $$ = new ParamList($1); }	
							|   ParameterList COMMA Parameter									{ $$ = new ParamList($1, $3); }				
							;

Parameter					:	TypeSpecifier DeclaratorName									{ $$ = new Param($1, $2); }	
							;

QualifiedName				:	Identifier														{ $$ = new QualifiedName($1); }	
							|	QualifiedName PERIOD Identifier									{ $$ = new QualifiedName($1, $3); }	
							;

DeclaratorName				:	Identifier														{ $$ = new DclName("Declarator", $1); }	
							;

MethodDeclaratorName		:	Identifier														{ $$ = new DclName("Method", $1); }		
							;

FieldVariableDeclaratorName	:	Identifier														{ $$ = new DclName("FieldVar", $1); }		
							;

LocalVariableDeclaratorName	:	Identifier														{ $$ = new DclName("LocalVar", $1); }		
							;

MethodBody					:	Block															{ $$ = ($1); }  // { $$ = new MethodBody($1); }		
							;

ConstructorDeclaration		:	Modifiers MethodDeclarator Block								{ $$ = new Constructor($1, $2, $3); }		
							;

StaticInitializer			:	STATIC Block													{ $$ = new StaticInit($2); }		
							;
		
/*
 * These can't be reorganized, because the order matters.
 * For example:  int i;  i = 5;  int j = i;
 */
Block						:	LBRACE LocalVariableDeclarationsAndStatements RBRACE			{ $$ = new Block($2); }
							|   LBRACE RBRACE													{ $$ = new Block(null); }
							;

LocalVariableDeclarationsAndStatements	:	LocalVariableDeclarationOrStatement					{ $$ = new LocalVarDeclsAndStmts($1); }
										|   LocalVariableDeclarationsAndStatements LocalVariableDeclarationOrStatement		{ $$ = new LocalVarDeclsAndStmts($1,$2); }
										;

LocalVariableDeclarationOrStatement	:	LocalVariableDeclarationStatement					    { $$ = ($1); }  // { $$ = new LocalVarDeclOrStmt($1); }
									|   Statement												{ $$ = ($1); }  // { $$ = new LocalVarDeclOrStmt($1); }
									;

LocalVariableDeclarationStatement	:	TypeSpecifier LocalVariableDeclarators SEMICOLON		{ $$ = new LocalVarDeclStmt($1, $2); }
									|   StructDeclaration                      					{ $$ = new LocalVarDeclStmt($1); }				
									;

LocalVariableDeclarators	:	LocalVariableDeclaratorName										{ $$ = new LocalVarDecls($1); }
							|   LocalVariableDeclarators COMMA LocalVariableDeclaratorName		{ $$ = new LocalVarDecls($1, $3); }
							;

							
																						    
Statement					:	EmptyStatement												 	{ $$ = ($1); }  // { $$ = new Stmt($1); }
							|	ExpressionStatement SEMICOLON								 	{ $$ = ($1); }  // { $$ = new Stmt($1); }
							|	SelectionStatement											 	{ $$ = ($1); }  // { $$ = new Stmt($1); }
							|	IterationStatement											 	{ $$ = ($1); }  // { $$ = new Stmt($1); }
							|	ReturnStatement												 	{ $$ = ($1); }  // { $$ = new Stmt($1); }
							|   Block														 	{ $$ = ($1); }  // { $$ = new Stmt($1); }
							;															    
																						    
EmptyStatement				:	SEMICOLON													 	{ $$ = new Stmt(null); }
							;															    
																						    
ExpressionStatement			:	Expression													 	{ $$ = ($1); }  // { $$ = new Stmt($1, "Expression"); }
							;															    
																						    
/*																						    
 *  You will eventually have to address the shift/reduce error that						    
 *     occurs when the second IF-rule is uncommented.									    
 *																						    
 */																						    
																						    
SelectionStatement			:	IF LPAREN Expression RPAREN Statement ELSE Statement		 	{ $$ = new SelectionStmt($3, $5, $7); }
							|   IF LPAREN Expression RPAREN Statement						 	{ $$ = new SelectionStmt($3, $5); }
							;															    
																						    
																						    
IterationStatement			:	WHILE LPAREN Expression RPAREN Statement					 	{ $$ = new Stmt($3, $5, "Iteration"); }
							;															    
																						    
ReturnStatement				:	RETURN Expression SEMICOLON									 	{ $$ = new Stmt($2, "Return"); }
							|   RETURN            SEMICOLON									 	{ $$ = new Stmt(null, "Return"); }
							;															    

ArgumentList				:	Expression														{ $$ = new ArgList($1); }
							|   ArgumentList COMMA Expression									{ $$ = new ArgList($1, $3); }
							;


Expression					:	QualifiedName EQUALS Expression									{ $$ = new AssignmentExpr($1, $3); }
							|   Expression OP_LOR Expression   /* short-circuit OR */			{ $$ = new Expr($1, $3, "||"); }	
							|   Expression OP_LAND Expression   /* short-circuit AND */			{ $$ = new Expr($1, $3, "&&"); }
							|   Expression PIPE Expression										{ $$ = new Expr($1, $3, "|"); }
							|   Expression HAT Expression										{ $$ = new Expr($1, $3, "^"); }
							|   Expression AND Expression										{ $$ = new Expr($1, $3, "&"); }
							|	Expression OP_EQ Expression										{ $$ = new Expr($1, $3, "=="); }
							|   Expression OP_NE Expression 									{ $$ = new Expr($1, $3, "!="); }
							|	Expression OP_GT Expression										{ $$ = new Expr($1, $3, ">"); }
							|	Expression OP_LT Expression										{ $$ = new Expr($1, $3, "<"); }
							|	Expression OP_LE Expression										{ $$ = new Expr($1, $3, "<="); }
							|	Expression OP_GE Expression										{ $$ = new Expr($1, $3, ">="); }
							|   Expression PLUSOP Expression									{ $$ = new Expr($1, $3, "+"); }
							|   Expression MINUSOP Expression									{ $$ = new Expr($1, $3, "-"); }
							|	Expression ASTERISK Expression									{ $$ = new Expr($1, $3, "*"); }
							|	Expression RSLASH Expression									{ $$ = new Expr($1, $3, "/"); }
							|   Expression PERCENT Expression									{ $$ = new Expr($1, $3, "%"); }
							|	ArithmeticUnaryOperator Expression  %prec UNARY					{ $$ = new Expr($1, $2, "Unary"); } // what is this
							|	PrimaryExpression												{ $$ = ($1); }  //{ $$ = new Expr($1);  }
							;

ArithmeticUnaryOperator		:	PLUSOP															{ $$ = ($1); }  //{ $$ = new UnaryOp("+"); }
							|   MINUSOP															{ $$ = ($1); }  //{ $$ = new UnaryOp("-"); }
							;
							
PrimaryExpression			:	QualifiedName													{ $$ = ($1); }  //{ $$ = new PrimaryExp($1); } 
							|   NotJustName														{ $$ = ($1); }  //{ $$ = new PrimaryExp($1); } 
							;

NotJustName					:	SpecialName														{ $$ = ($1); }  // { $$ = new NotJustName($1); } 
							|   ComplexPrimary													{ $$ = ($1); }  // { $$ = new NotJustName($1); } 
							;

ComplexPrimary				:	LPAREN Expression RPAREN										{ $$ = ($2); }  // { $$ = new ComplexPri($2); } 
							|   ComplexPrimaryNoParenthesis										{ $$ = ($1); }  // { $$ = new ComplexPri($1); } 
							;

ComplexPrimaryNoParenthesis	:	LITERAL															{$$ = new StringArg((((TCCLScanner)Scanner).yystringval)); } 
							|   Number															{ $$ = ($1); }  // { $$ = new ComplexPriNoParens($1); } 
							|	FieldAccess														{ $$ = ($1); }  // { $$ = new ComplexPriNoParens($1); } 
							|	MethodCall														{ $$ = ($1); }  // { $$ = new ComplexPriNoParens($1); } 
							;

FieldAccess					:	NotJustName PERIOD Identifier									{ $$ = new FieldAccess($1, $3); } 
							;		

MethodCall					:	MethodReference LPAREN ArgumentList RPAREN						{ $$ = new MethodCall($1, $3); } 
							|   MethodReference LPAREN RPAREN									{ $$ = new MethodCall($1); } 
							;

MethodReference				:	ComplexPrimaryNoParenthesis										{ $$ = ($1); }  // { $$ = new MethodRef($1); } 
							|	QualifiedName													{ $$ = ($1); }  // { $$ = new MethodRef($1); } 
							|   SpecialName														{ $$ = ($1); }  // { $$ = new MethodRef($1); } 
							;

SpecialName					:	THIS															{ $$ = new SpecialName("THIS"); } 
							|	NULL															{ $$ = new SpecialName("NULL"); } 
							;

Identifier					:	IDENTIFIER														 {$$ = new Identifier(((TCCLScanner)Scanner).yytext); }  
							;

Number						:	INT_NUMBER														{ $$ = new Number(((TCCLScanner)Scanner).yytext); } 					
							;

%%

