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
using LibronixDLS;

namespace SIL.Utils
{
	internal class LibronixPositionHandlerFactory : ILogosPositionHandlerFactory
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates an instance of the <see cref="LibronixPositionHandler"/> class.
		/// </summary>
		/// <param name="fStart"><c>true</c> to start Libronix if it isn't running,
		/// otherwise <c>false</c>.</param>
		/// <param name="linkSet">The 0-based index of the link set in Libronix.</param>
		/// <param name="waitForReady">set to <c>true</c> to wait for Libronix/Logos app to
		/// complete starting up.</param>
		/// <returns>
		/// An instance of the <see cref="ILogosPositionHandler"/> class.
		/// </returns>
		/// <exception cref="LibronixNotRunningException">Thrown if Libronix is installed but
		/// not currently running and <paramref name="fStart"/> is <c>false</c>.</exception>
		/// <exception cref="LibronixNotInstalledException">Thrown if Libronix is not
		/// installed on this machine.</exception>
		/// ------------------------------------------------------------------------------------
		public ILogosPositionHandler CreateInstance(bool fStart, int linkSet, bool waitForReady)
		{
			LbxApplication libronixApp = null;
			try
			{
				// If Libronix isn't running, we'll get an exception here
				object libApp = Marshal.GetActiveObject("LibronixDLS.LbxApplication");

				libronixApp = libApp as LbxApplication;
			}
			catch (COMException e)
			{
				if ((uint)e.ErrorCode == 0x800401E3) // MK_E_UNAVAILABLE
				{	// Installed, but not running
					if (fStart)
					{
						try
						{
							// try to start
							libronixApp = new LbxApplicationClass { Visible = true };
						}
						catch (Exception e1)
						{
							libronixApp = null;
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
			if (libronixApp == null)
				return null;

			return new LibronixPositionHandler(linkSet, libronixApp);
		}

		/// <summary>
		/// Gets the status of the Logos app
		/// </summary>
		/// <value>Returns <c>true</c> if the Logos application is running</value>
		public bool IsLogosRunning
		{
			get { return Process.GetProcessesByName("LDLS").Length > 0; }
		}
	}
}
