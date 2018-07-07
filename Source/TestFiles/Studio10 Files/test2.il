//
//  Team members:
//
//
//
//
//  Change the code as indicated by the instructions at *** below
//  


.assembly extern mscorlib {}  
.assembly addnums {}  

.method public static int32 Add(int32, int32) cil managed
{
    .maxstack 2
    ldarg.0 // load the first argument;
    ldarg.1 // load the second argument;
    add     // add them;
    ret     // return the result;
}

.method static void main()
{
    .entrypoint

    .maxstack 4
    
    .locals init (int32 ,
                  int32 ,
                  int32 )
    ldstr "First number: "
    call void [mscorlib]System.Console::Write(string)
    call string [mscorlib]System.Console::ReadLine()
    call int32 [mscorlib]System.Int32::Parse(string)
    stloc 0
    
    ldstr "Second number: "
    call void [mscorlib]System.Console::Write(string)
    call string [mscorlib]System.Console::ReadLine()
    call int32 [mscorlib]System.Int32::Parse(string)
    stloc 1

// ***
// Replace the next three instructions by a more complicated sequence that does the same thing:
// Using the Add method above instead of the ‘add’ instruction, add 5 to the first number, 
// add -5 to the second number, then add the results of those two computations.
// Do this without introducing any new local variables.

    ldloc 0
	ldc.i4.5
    call int32 Add(int32, int32)
	stloc 1
	ldloc 1
	ldc.i4.6
	    call int32 Add(int32, int32)
	stloc 2
	//ldc.i4.m5
	// ldloc 1
	//Add
    // add

/// End of instructions to replace

    stloc 2

    ldstr "{0} + {1} = {2}"
    
    ldloc 0
    box int32
    
    ldloc 1
    box int32
    
    ldloc 2
    box int32
    
    call void [mscorlib]System.Console::WriteLine(string, object, object, object) 
   
    ret
}