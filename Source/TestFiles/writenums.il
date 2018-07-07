//-----------------------------------------------------------------
//IL Header - Lenny Bogdanov
//File: writenums.il 
//Source File: writenums.txt 
//Date: Sunday, June 4, 2017 2:03:42 PM
//-----------------------------------------------------------------


.assembly extern mscorlib {}
.assembly a{} //Not sure why this was necessary for me to do


.class public auto ansi beforefieldinit writenums 
extends [mscorlib]System.Object 

{
.method public void 
 main () cil managed
{
.entrypoint
.maxstack 1000 // Should be using sethi ullman algorithm here!
ldc.i4		 5
call void [mscorlib]System.Console :: WriteLine(int32)
ldc.i4		 5
ldc.i4		 2
mul
ldc.i4		 6
ldc.i4		 4
div
add
call void [mscorlib]System.Console :: WriteLine(int32)
ret
}

}
