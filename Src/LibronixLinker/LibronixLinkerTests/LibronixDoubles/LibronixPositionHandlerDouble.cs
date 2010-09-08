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
using LibronixDLS;
using LibronixDLSUtility;

namespace SIL.Utils
{
	internal class LibronixPositionHandlerDouble : LibronixPositionHandler
	{
		private PositionChangedEventArgs m_eventArgs;

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="LibronixPositionHandlerDouble"/> class.
		/// </summary>
		/// <param name="linkSet">The link set.</param>
		/// <param name="libronixApp">The libronix app.</param>
		/// --------------------------------------------------------------------------------
		public LibronixPositionHandlerDouble(int linkSet, LbxApplication libronixApp):
			base(linkSet, libronixApp)
		{
		}

		protected override LbxApplicationEventsBridge CreateLbxApplicationEventsBridge()
		{
			return new LbxApplicationEventsBridgeDouble();
		}

		protected override void RaisePositionChangedEvent(PositionChangedEventArgs e)
		{
			m_eventArgs = e;
		}

		protected override void StartPollTimer()
		{
			// do nothing
		}

		/// <summary>
		/// Calls the OnTick method
		/// </summary>
		public int CallOnTick()
		{
			OnPollTimerTick(null, EventArgs.Empty);
			return m_eventArgs.BcvRef;
		}
	}
}