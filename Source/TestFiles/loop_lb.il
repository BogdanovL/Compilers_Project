//-----------------------------------------------------------------
//IL Header - Lenny Bogdanov
//File: loop_lb.il 
//Source File: loop_lb.txt 
//Date: Friday, June 2, 2017 6:31:58 PM
//-----------------------------------------------------------------


.assembly extern mscorlib {}
.assembly a{} //Not sure why this was necessary for me to do


.class public auto ansi beforefieldinit loop 
extends [mscorlib]System.Object 

{
.method static public void 
 main () cil managed
{
.entrypoint
.maxstack 1000 // Should be using sethi ullman algorithm here!
.locals init (
[0]	int32	gobble,
[1]	int32	i
)
ldc.i4		 0
stloc 1 //i
ldc.i4		 1
stloc 0 //gobble
ldloc 1 //i
ldc.i4		 1
add
stloc 1 //i
ldloc 0 //gobble
ldloc 1 //i
mul
stloc 0 //gobble
ret
}

}
