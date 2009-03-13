LiSaFT
======

(Libronix Santa Fe Translator)

This tool translates sync messages from Libronix into the Santa Fe standard which is used by 
programs like TE, Paratext, etc. This means if you scroll to a Bible passage in Libronix,
TE and/or Paratext will scroll to that passage as well.


Requirements
------------

You need Microsoft .NET Framework 2.0 installed on your machine. If you have version 1.1
or later of TE installed then you already have it installed, otherwise you can get it from Microsoft's
web page (<http://www.microsoft.com/downloads/details.aspx?FamilyID=0856EACB-4362-4B0D-8EDD-AAB15C5E04F5&displaylang=en>)


Installation
------------

Simply extract all the files in the zip file into a new directory on your local hard drive. 
(It is also possible to put them on a network drive, but that requires some modifications in 
.NET config files since .NET by default disallows the execution of programs from network drives).


Usage
-----

Start Libronix first, open the desired resources and set the link sets.

When you start the tool (LiSaFT.exe) you get an icon in the notification area of the task bar.

If you set a link set other than "Set A" you have to change it in the tool: right click on the
icon in the notification area, choose Config from the menu and choose the link set that
corresponds to the link set you chose in Libronix.

If you now go to a different passage (in a linked ressource window) in Libronix, TE should
go to the same passage.


Known Issues
------------

1)	List of linked windows doesn't get updated automatically when you start Libronix, open
	new resource files or change the link set in Libronix. You have to right click on the LiSaFT 
	icon in the noticiation area and select Refresh from the menu.

2)	If you have Libronix running on Vista and you get an error message saying that Libronix
	isn't running when you try to start LiSaFT, you have to run LiSaFT as Administrator (right
	click on lisaft.exe, select Properties. Switch to the Compatibility tab and check the "Always
	run as administrator" checkbox).
	Also make sure that you have the latest Libronix version installed. This issue will most likely
	be fixed with Libronix 3.0e or later.
	
	
Why isn't this part of TE?
--------------------------

This functionality will eventually be a part of FieldWorks. Right now it isn't due to
insufficient resources that could implement all the details.

Besides, there are some technical issues that need to be resolved (see issue 1 above).