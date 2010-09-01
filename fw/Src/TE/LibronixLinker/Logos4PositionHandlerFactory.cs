using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Logos4Lib;

namespace SIL.FieldWorks.TE.LibronixLinker
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
					throw new LibronixNotInstalledException("Libronix isn't installed", null);
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
