//-----------------------------------------------------------------
//IL Header - Lenny Bogdanov
//File: good1p.il 
//Source File: good1p.txt 
//Date: Tuesday, May 30, 2017 7:19:10 PM
//-----------------------------------------------------------------


.assembly extern mscorlib {}
.assembly a{} //Not sure why this was necessary for me to do


.class static public auto ansi beforefieldinit good1p 
extends [mscorlib]System.Object 

{
.method static public void 
 main431 () cil managed
{
.entrypoint
.maxstack 1000 // could certainly improve this line...at some point...
ret
}
.method static public void 
 outInt (int32 x
int32 n,
) cil managed
{
.maxstack 1000 // could certainly improve this line...at some point...
ldarg.0		 //n
call void [mscorlib]System.Console :: ps.print(int32)
ret
}

}
