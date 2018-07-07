//-----------------------------------------------------------------
//IL Header - Lenny Bogdanov
//File: iftest.il 
//Source File: iftest.txt 
//Date: Sunday, June 4, 2017 2:07:34 PM
//-----------------------------------------------------------------


.assembly extern mscorlib {}
.assembly a{} //Not sure why this was necessary for me to do


.class public auto ansi beforefieldinit iftest 
extends [mscorlib]System.Object 

{
.method static public void 
 main () cil managed
{
.entrypoint
.maxstack 1000 // Should be using sethi ullman algorithm here!
.locals init (
[0]	int32	j,
[1]	int32	i
)
ldc.i4		 0
stloc 1 //i
ldc.i4		 1
stloc 0 //j
ldstr		 "TCCL if test"
call void [mscorlib]System.Console :: WriteLine(string)
ldloc 1 //i
ldc.i4		 10
clt
brfalse.s		lbl_0
ldstr		 "In then part"
call void [mscorlib]System.Console :: WriteLine(string)
br.s		lbl_1
lbl_0:		 nop
ldloc 0 //j
stloc 1 //i
lbl_1:		 nop
ldloc 1 //i
ldc.i4		 10
ceq
brfalse.s		lbl_2
ldloc 1 //i
stloc 0 //j
br.s		lbl_3
lbl_2:		 nop
ldstr		 "In else part"
call void [mscorlib]System.Console :: WriteLine(string)
lbl_3:		 nop
ret
}

}
