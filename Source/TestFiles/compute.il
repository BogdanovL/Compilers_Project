//-----------------------------------------------------------------
//IL Header - Lenny Bogdanov
//File: compute.il 
//Source File: compute.txt 
//Date: Sunday, June 4, 2017 2:04:37 PM
//-----------------------------------------------------------------


.assembly extern mscorlib {}
.assembly a{} //Not sure why this was necessary for me to do


.class public auto ansi beforefieldinit compute 
extends [mscorlib]System.Object 

{
.method public void 
 main () cil managed
{
.entrypoint
.maxstack 1000 // Should be using sethi ullman algorithm here!
.locals init (
[0]	int32	x,
[1]	int32	w
)
ldc.i4		 3
ldc.i4		 4
add
stloc 0 //x
ldstr		 "Value of x = "
call void [mscorlib]System.Console :: Write(string)
ldloc 0		 //x
call void [mscorlib]System.Console :: WriteLine(int32)
ldloc 0 //x
ldc.i4		 5
sub
stloc 1 //w
ldstr		 "Value of w = "
call void [mscorlib]System.Console :: Write(string)
ldloc 1		 //w
call void [mscorlib]System.Console :: WriteLine(int32)
ldstr		 "value of big expression ="
call void [mscorlib]System.Console :: Write(string)
ldloc 1 //w
ldloc 0 //x
mul
ldc.i4		 12
ldloc 1 //w
div
add
call void [mscorlib]System.Console :: WriteLine(int32)
ret
}

}
