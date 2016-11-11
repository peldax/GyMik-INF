# Gymik-INF

[![Version](https://img.shields.io/badge/version-1.0-brightgreen.svg)](https://github.com/peldax/Gymik-INF/releases/tag/v1.0)

[![Maintained](https://img.shields.io/badge/maintained-no-red.svg)](https://github.com/peldax/Gymik-INF/releases)
[![Issues](https://img.shields.io/badge/issues-0-brightgreen.svg)](https://github.com/peldax/Gymik-INF/issues)

##### School year 2013/2014
##### Programming language - C-Sharp
##### Language of Comments - English

This is my graduation work for subject INF at Gymnázium Mikulášské náměstí, Plzeň.

This little application evaulates mathematical expressions (the console appliacation) or draws graph of mathematical function (the gui application). I implemented this solution at highschool without any theoretical knowledge of parsers of an type.

## Example

```
[peldax@myPC]$ mono ConsoleEvaulator.exe 
Enter your expression.
For more information use 'help' command or 'exit' to close.
help
------------------------------------
This application supports:

+ - * / . , ( )

pi (as 3.1415....) and e (as 2.7182....)

Exponential Notation: E (e.g. 1E-1 = 0.1)

and following functions:

Sine: sin(x)
Cosine: cos(x)
Tangent: tg(x)
Cotangent: cot(x)
Square root: sqrt(x)
Absolute value: abs(x)
Natural logarithm: ln(x)
------------------------------------
ln(0) 
-Infinity
1+1
2
exit
[peldax@myPC]$
```

### How to build and run on linux:

You are required to install `mono` package.

```
mcs ConsoleEvaulator.cs
mono ConsoleEvaulator.exe
```

GUI application for graphs is of type WPF, which is not supported by mono.
