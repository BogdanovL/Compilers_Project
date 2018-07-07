//
//  Team members:
//
//
//
//
//
//  Change the code as indicated by the instructions at *** below
//  

.assembly extern mscorlib {}  
.assembly addnums {}  
.method static void main()
{
    .entrypoint

//
//  ***
//  Rewrite the code to subtract the smaller of the two numbers from the other.
//  Change the output message to match this new computation.
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
   
    ret
}