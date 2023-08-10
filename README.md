sam-ba(not samba) GUI for windows

![title](https://blogfiles.pstatic.net/MjAyMzA4MTBfMzIg/MDAxNjkxNjQwMjg1NDkw.kvdToc_GzDKyzBs2KcLU8M9_Z9T-f2dmhXY07HvxC0Ig.qR26dSsUWFxvuz-OVuoq8VSbE4PJW6BFRGYjNwBXRIEg.PNG.5boon/sam_ba_gui.png "image")

<br>
-20201104<br>
NAND header updated.

<br>
-Introduce<br>
SAM-BA software provides an open set of tools for in-system programming of internal and external memories connected to MICROCHIPS MCUs and MPUs.

This program supports the programming in a GUI environment.

It is made in C#.

<br>
-Test environment<br>

CPU : SAMA5D2

MEMORY : NAND with 2bit ecc support.

HOST : Windows 10

SAM-BA : 3.2.1

.NET Framework : 4.0

<br>
-Necessary<br>
	
download sam-ba3.2.1 below link

https://github.com/atmelcorp/sam-ba/releases/tag/v3.2.1

<br>
-How to compile<br>
	
Double click "compile.bat"

*If didn't work, install .net framework.

(Installed by default)

*If also didn't work, modify csc directory path in "compile.bat" file.

<br>
-How to use<br>
	
Prepare xml file.

Xml file format is as follows.

==============================<br>
&#60;NAND&#62;<br>
&emsp;	&#60;MTD&#62;<br>
&emsp;&emsp;		&#60;name&#62;bootstrap&#60;/name&#62;<br>
&emsp;&emsp;		&#60;address&#62;0x00000000&#60;/address&#62;<br>
&emsp;&emsp;		&#60;size&#62;0x00040000&#60;/size&#62;<br>
&emsp;&emsp;		&#60;file&#62;MYIR-nandflashboot-uboot-3.10.0-rc3.bin&#60;/file&#62;<br>
&emsp;	&#60;/MTD&#62;<br>
&emsp;	&#60;HEADER&#62;<br>
&emsp;&emsp;0xc0c00405<br>
&emsp;	&#60;/HEADER&#62;<br>
&#60;/NAND&#62;<br>
==============================<br>

(The sample file exists as "myir.xml")

Double click "AtmelNandFlash.exe" file.

And click [LOAD] button, select the xml file you have already created.

Finally. select the desired checkbox, and click [PROGRAM] button to program.

[REASE] button delete whole nandflash.

<br>
-To do<br>
	
cannot save and modifing.
