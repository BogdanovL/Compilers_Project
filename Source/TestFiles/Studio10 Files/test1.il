//
//  Team members:
//
//
//
//
//  Answer questions 1) - 3) below
//     and turn in this file, appropriately modified,  with the console output
//

.assembly extern mscorlib {}  
.assembly addnums {}  
.method static void main()
{
    .entrypoint

//
//  1) What happens if you change the stack limit to 1?  
//   ANSWER: 
//
//  2) What determines the stack limit?
//   ANSWER:
//

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
    ldloc 0
    ldloc 1
    add
    stloc 2
    ldstr "{0} + {1} = {2}"
    
    ldloc 0
    box int32
    
    ldloc 1
    box int32
    
    ldloc 2
    box int32
    
    call void [mscorlib]System.Console::WriteLine(string, object, object, object) 
   
//  3) What is computed by the following instructions??
//   ANSWER:
//
    ldc.i4.s 100 // push 100 onto the stack in short form
    ldloc 0	// load local variable 0 onto the stack
    mul
    ldloc 1
    ldc.i4.s 31
    mul
    add
    stloc 2

// Modify this format string to describe the computation performed above

    ldstr "{0} * 100 + {1} * 31 = {2}"
    
    ldloc 0
    box int32
    
    ldloc 1
    box int32
    
    ldloc 2
    box int32
    
    call void [mscorlib]System.Console::WriteLine(string, object, object, object) 
   
    ret
}