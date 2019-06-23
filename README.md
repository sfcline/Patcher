# Patcher

A self-updating patcher for Mabinogi that can be easily set up for any server. The patcher functions by launching
as an executable and checking for update to the dynamic link library (DLL). After the patcher is up to date it 
will load the DLL. The DLL holds both the GUI and patching functions for updating the client. A DLL is used
instead of an additional executable to prevent confusion. Multiple executables can cause users to run the patcher instead
of the launcher. By using a DLL instead of an additional executable, there is only one executable to run while still providing the
patcher a way to update itself.

![Program](https://github.com/sfcline/Patcher/blob/master/patcher.gif?raw=true)

## Features
* Open Source
* Easy to setup for any server
* Written in C#
* Automatically downloads and applies patches
* Capable of self-patching for GUI and functionality updates
* Optional display of patch logs

## Getting Started

Requirements For Execution:
* Microsoft .NET 4.6.1

Requirements For Modifying:
* Microsoft Visual Studio with .NET 4.6.1

Instructions:
* Change const string variables as needed in Program.cs and Form.cs
* Change website URL of browser or remove browser in C# forms Design
* Set versions of client and patcher both locally and remotely
* Build both projects
* Move Launcher, DLL, and version files to Mabinogi folder
* Execute Launcher.exe

## Built With

* Microsoft Visual Studio 2017

## Author

* Stephen Cline

## License

sfcline/Patcher is licensed under the GNU Lesser General Public License v2.1

## Acknowledgments

* https://stackoverflow.com
* https://docs.microsoft.com
* http://cdn.mabiclassic.com

## Key Programming Concepts Utilized

* GUI development and functionality using C#
* The basics of programming in C#
* Creating and using DLLs in C#
