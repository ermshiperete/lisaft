
namespace SIL.FieldWorks.TE.LibronixLinker
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Test double of a factory that creates a Libronix position handler
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	internal class LibronixPositionHandlerFactoryDouble: ILogosPositionHandlerFactory
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether Libronix is installed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool LibronixIsInstalled { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether Libronix is running.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is running; otherwise, <c>false</c>.
		/// </value>
		/// ------------------------------------------------------------------------------------
		public bool LibronixIsRunning { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether to toggle the LibronixIsRunning with each call.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool ToggleLibronixRunning { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets an already running instance of Libronix.
		/// </summary>
		/// <value>The libronix position handler.</value>
		/// ------------------------------------------------------------------------------------
		public LibronixPositionHandler LibronixPositionHandler { get; set; }

		#region ILogosPositionHandlerFactory Members
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
			ILogosPositionHandler handler = null;
			if (!LibronixIsInstalled)
			{
				throw new LibronixNotInstalledException("Libronix isn't installed", null);
			}

#if !__MonoCS__
			if (LibronixIsRunning || fStart)
			{
				LibronixPositionHandler = new LibronixPositionHandlerDouble(linkSet, new LbxApplicationDouble());
			}
#endif

			return LibronixPositionHandler;
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
						m_running = LibronixIsRunning ? 1 : 0;
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
