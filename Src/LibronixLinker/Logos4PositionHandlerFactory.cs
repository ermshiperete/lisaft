// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2010, SIL International. All Rights Reserved.
// <copyright from='2010' to='2010' company='SIL International'>
//		Copyright (c) 2010, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Logos4Lib;

namespace SIL.Utils
{
	class Logos4PositionHandlerFactory: ILogosPositionHandlerFactory
	{
		#region ILogosPositionHandlerFactory Members

		public ILogosPositionHandler CreateInstance(bool fStart, int linkSet, bool waitForReady)
		{
			LogosApplication logosApplication = null;
			try
			{
				// If Libronix isn't running, we'll get an exception here
				var libApp = Marshal.GetActiveObject("LogosBibleSoftware.Application");

				logosApplication = libApp as LogosApplication;
			}
			catch (COMException e)
			{
				if ((uint)e.ErrorCode == 0x800401E3) // MK_E_UNAVAILABLE
				{	// Installed, but not running
					if (fStart || waitForReady)
					{
						try
						{
							// try to start
							var launcher = new LogosLauncherClass();
							launcher.LaunchApplication(string.Empty);
							for (int i = 0; i < 1000 && launcher.Application == null; i++)
								Thread.Sleep(100);
							logosApplication = launcher.Application;
						}
						catch (Exception e1)
						{
							logosApplication = null;
							Debug.Fail("Got exception in Initialize trying to start Libronix: " + e1.Message);
						}
					}
				}
				else
				{
					// Not installed
					throw new LibronixNotInstalledException(string.Format("Logos4 isn't installed (error code 0x{0:x})", (uint)e.ErrorCode), e);
				}
			}
			catch (Exception e)
			{
				Debug.Fail("Got exception in Initialize trying to get running Libronix object: " + e.Message);
			}
			if (logosApplication == null)
				return null;

			return new Logos4PositionHandler(linkSet, logosApplication);
		}

		public bool IsLogosRunning
		{
			get { return Process.GetProcessesByName("Logos4").Length > 0; }
		}

		#endregion
	}
}
