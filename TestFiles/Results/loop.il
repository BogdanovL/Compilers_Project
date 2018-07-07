//-----------------------------------------------------------------
//IL Header - Lenny Bogdanov
//File: loop.il 
//Source File: loop.txt 
//Date: Sunday, June 4, 2017 2:08:17 PM
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
ldstr		 "Beginning loop test"
call void [mscorlib]System.Console :: WriteLine(string)
lbl_0:		 nop
ldloc 1 //i
ldc.i4		 10
clt
brfalse.s		lbl_1
ldloc 0		 //gobble
call void [mscorlib]System.Console :: WriteLine(int32)
ldloc 1 //i
ldc.i4		 1
add
stloc 1 //i
ldloc 0 //gobble
ldloc 1 //i
mul
stloc 0 //gobble
br.s		lbl_0
lbl_1:		 nop
ret
}

}
