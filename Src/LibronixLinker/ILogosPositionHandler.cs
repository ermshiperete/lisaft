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
using System.Collections.Generic;

namespace SIL.Utils
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Abstract interface for sending and receiving references from/to Logos
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public interface ILogosPositionHandler
	{
		/// <summary>Fired when the scroll position of a resource is changed in Logos</summary>
		/// <remarks>The BcvRef reference is in BBCCCVVV format. The book number
		/// is 1-39 for OT books and 40-66 for NT books.</remarks>
		event EventHandler<PositionChangedEventArgs> PositionChanged;

		/// <summary>
		/// Gets the available link sets.
		/// </summary>
		List<string> AvailableLinkSets { get; }

		/// <summary>
		/// Refreshes the list of current window
		/// </summary>
		void Refresh();

		/// <summary>
		/// Refreshes the specified link set.
		/// </summary>
		/// <param name="linkSet">The link set.</param>
		void Refresh(int linkSet);

		/// <summary>
		/// Sends the scroll synchronization message to Logos for all resource windows in
		/// the current link set.
		/// </summary>
		/// <param name="bcvRef">The reference in BBCCCVVV format.</param>
		void SetReference(int bcvRef);
	}

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// ILogosPositionHandler extension methods
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public static class LogosPositionHandlerExtensions
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting 
		/// unmanaged resources.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void Dispose(this ILogosPositionHandler positionHandler)
		{
			var disposable = positionHandler as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}
	}
}
