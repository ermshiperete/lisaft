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

namespace SIL.Utils.Logos4Doubles
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Test double of a factory that creates a Logos4 position handler
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	internal class Logos4PositionHandlerFactoryDouble : ILogosPositionHandlerFactory
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether Logos4 is installed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool Logos4IsInstalled { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether Logos4 is running.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool Logos4IsRunning { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether to toggle the Logos4IsRunning with each call.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool ToggleLogos4Running { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets an already running instance of Logos4.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Logos4PositionHandler Logos4PositionHandler { get; set; }

		#region ILogosPositionHandlerFactory Members

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates an instance of the <see cref="Logos4PositionHandler"/> class.
		/// </summary>
		/// <param name="fStart"><c>true</c> to start Logos4 if it isn't running,
		/// otherwise <c>false</c>.</param>
		/// <param name="linkSet">The 0-based index of the link set in Logos4.</param>
		/// <param name="waitForReady">set to <c>true</c> to wait for Libronix/Logos app to
		/// complete starting up.</param>
		/// <returns>
		/// An instance of the <see cref="ILogosPositionHandler"/> class.
		/// </returns>
		/// <exception cref="LibronixNotRunningException">Thrown if Logos4 is installed but
		/// not currently running and <paramref name="fStart"/> is <c>false</c>.</exception>
		/// <exception cref="LibronixNotInstalledException">Thrown if Logos4 is not
		/// installed on this machine.</exception>
		/// ------------------------------------------------------------------------------------
		public ILogosPositionHandler CreateInstance(bool fStart, int linkSet, bool waitForReady)
		{
			ILogosPositionHandler handler = null;
			if (!Logos4IsInstalled)
			{
				throw new LibronixNotInstalledException("Logos4 isn't installed", null);
			}

			if (Logos4IsRunning || fStart)
			{
				Logos4PositionHandler = 
					new Logos4PositionHandlerDouble(linkSet, new LogosApplicationDouble());
			}

			return Logos4PositionHandler;
		}

		private int m_running = -1;
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the status of the Logos app
		/// </summary>
		/// <value>Returns <c>true</c> if the Logos application is running</value>
		/// ------------------------------------------------------------------------------------
		public bool IsLogosRunning
		{
			get
			{
				var running = m_running;
				switch (m_running)
				{
					case -1:
						m_running = Logos4IsRunning ? 1 : 0;
						break;
					case 0:
						m_running = 1;
						break;
					case 1:
						m_running = 0;
						break;
				}
				System.Diagnostics.Debug.WriteLine(string.Format("IsLogosRunning is {0}; setting it to {1}", running, m_running));
				return running == 1;
			}
		}

		#endregion
	}
}
