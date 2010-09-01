namespace SIL.FieldWorks.TE.LibronixLinker
{
	internal interface ILogosPositionHandlerFactory
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
		/// <exception cref="LibronixNotInstalledException">Thrown if Libronix is not
		/// installed on this machine.</exception>
		/// ------------------------------------------------------------------------------------
		ILogosPositionHandler CreateInstance(bool fStart, int linkSet, bool waitForReady);

		/// <summary>
		/// Gets the status of the Logos app
		/// </summary>
		/// <value>Returns <c>true</c> if the Logos application is running</value>
		bool IsLogosRunning { get; }
	}
}