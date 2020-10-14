sam-ba(not samba) GUI for windows



<Introduce>
SAM-BA software provides an open set of tools for in-system programming of internal and external memories connected to MICROCHIPS MCUs and MPUs.

This program supports the programming in a GUI environment.

It is made in C#.


<Test environment>

CPU : SAMA5D2

MEMORY : NAND with 2bit ecc support.

HOST : Windows 10

SAM-BA : 3.2.1

.NET Framework : 4.0

<Necessary>
	
download sam-ba3.2.1 below link

https://github.com/atmelcorp/sam-ba/releases/tag/v3.2.1


<How to compile>
	
Double click "compile.bat"

*If didn't work, install .net framework.

(Installed by default)

*If also didn't work, modify csc directory path in "compile.bat" file.


<How to use>
	
Prepare xml file.

Xml file format is as follows.

==================================================
<NAND>
	<MTD>
		<name>bootstrap</name>
		<address>0x00000000</address>
		<size>0x00040000</size>
		<file>MYIR-nandflashboot-uboot-3.10.0-rc3.bin</file>
	</MTD>
</NAND>
==================================================
(The sample file exists as "myir.xml")

Double click "AtmelNandFlash.exe" file.

And click [LOAD] button, select the xml file you have already created.

Finally. select the desired checkbox, and click [PROGRAM] button to program.

[REASE] button delete whole nandflash.


<To do>
	
cannot change Header.

cannot save and modifing.
