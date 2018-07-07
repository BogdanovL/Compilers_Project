//-----------------------------------------------------------------
//IL Header - Lenny Bogdanov
//File: twomethods1.il 
//Source File: twomethods1.txt 
//Date: Sunday, June 4, 2017 2:26:56 PM
//-----------------------------------------------------------------


.assembly extern mscorlib {}
.assembly a{} //Not sure why this was necessary for me to do


.class public auto ansi beforefieldinit twomethods1 
extends [mscorlib]System.Object 

{
.method static public void 
 main () cil managed
{
.entrypoint
.maxstack 1000 // Should be using sethi ullman algorithm here!
ldc.i4		 12345
call void twomethods1 :: check(int32)
ldstr		 "Back in main"
call void [mscorlib]System.Console :: WriteLine(string)
ret
}
.method static public void 
 check (int32 x
) cil managed
{
.maxstack 1000 // Should be using sethi ullman algorithm here!
ldstr		 "The argument to check was "
call void [mscorlib]System.Console :: Write(string)
ldarg 0		 //x
call void [mscorlib]System.Console :: WriteLine(int32)
ret
}

}
