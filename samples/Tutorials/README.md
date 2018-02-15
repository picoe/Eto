Eto.Forms Tutorials
===================

How to Build
------------

These tutorials use a script to copy the required platform dependencies:

- buildapp.sh is used for OS X or Linux
- buildapp.cmd is used for Windows


How to Run
----------

You can run from the IDE, though on OS X an .app bundle is created in the output directory that you must run separately, 
otherwise it will run the GTK platform on OS X.

To run the MonoMac platform directly in MonoDevelop for OS X, you must create a MonoMac project separate from your GUI 
executable project.  See the Eto.Test.Mac application as an example of how this looks.