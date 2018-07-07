//-----------------------------------------------------------------
//IL Header - Lenny Bogdanov
//File: hello.il 
//Source File: hello.txt 
//Date: Sunday, June 4, 2017 2:04:05 PM
//-----------------------------------------------------------------


.assembly extern mscorlib {}
.assembly a{} //Not sure why this was necessary for me to do


.class public auto ansi beforefieldinit hello 
extends [mscorlib]System.Object 

{
.method static public void 
 main () cil managed
{
.entrypoint
.maxstack 1000 // Should be using sethi ullman algorithm here!
ldstr		 "Hello, World"
call void [mscorlib]System.Console :: WriteLine(string)
ret
}

}
