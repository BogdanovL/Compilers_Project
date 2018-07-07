//-----------------------------------------------------------------
//IL Header - Lenny Bogdanov
//File: twomethods0.il 
//Source File: twomethods0.txt 
//Date: Sunday, June 4, 2017 2:06:31 PM
//-----------------------------------------------------------------


.assembly extern mscorlib {}
.assembly a{} //Not sure why this was necessary for me to do


.class public auto ansi beforefieldinit twomethods0 
extends [mscorlib]System.Object 

{
.method static public void 
 main () cil managed
{
.entrypoint
.maxstack 1000 // Should be using sethi ullman algorithm here!
call void twomethods0 :: check()
ldstr		 "Back in main"
call void [mscorlib]System.Console :: WriteLine(string)
ret
}
.method static public void 
 check () cil managed
{
.maxstack 1000 // Should be using sethi ullman algorithm here!
ldstr		 "In the check method"
call void [mscorlib]System.Console :: WriteLine(string)
ret
}

}
