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
using Logos4Lib;

namespace SIL.Utils.Logos4Doubles
{
	class LogosApplicationDouble: LogosApplication
	{
		static LogosApplicationDouble()
		{
			VersionNo = 2;
		}

		public static int VersionNo { get; set; }

		#region ILogosApplication Members

		public void Activate()
		{
			throw new NotImplementedException();
		}

		public int ApiVersion
		{
			get { return VersionNo; }
		}

		public LogosCopyBibleVerses CopyBibleVerses
		{
			get { throw new NotImplementedException(); }
		}

		public LogosNavigationRequest CreateNavigationRequest()
		{
			return new LogosNavigationRequestDouble();
		}

		public LogosDataTypes DataTypes
		{
			get { throw new NotImplementedException(); }
		}

		public void ExecuteUri(string Uri)
		{
			throw new NotImplementedException();
		}

		public void Exit()
		{
			throw new NotImplementedException();
		}

		public LogosPanel GetActivePanel()
		{
			return new LogosPanelDouble();
		}

		public LogosPanelCollection GetOpenPanels()
		{
			return new LogosPanelCollectionDouble();
		}

		public LogosPanel GetPanel(string panelId)
		{
			throw new NotImplementedException();
		}

		public LogosLibrary Library
		{
			get { throw new NotImplementedException(); }
		}

		public void Navigate(LogosNavigationRequest Request)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ILogosApplicationEvents_Event Members

		event ILogosApplicationEvents_ExitingEventHandler ILogosApplicationEvents_Event.Exiting
		{
			add { }
			remove { }
		}

		event ILogosApplicationEvents_PanelActivatedEventHandler ILogosApplicationEvents_Event.PanelActivated
		{
			add { }
			remove { }
		}

		event ILogosApplicationEvents_PanelChangedEventHandler ILogosApplicationEvents_Event.PanelChanged
		{
			add { }
			remove { }
		}


		event ILogosApplicationEvents_PanelClosedEventHandler ILogosApplicationEvents_Event.PanelClosed
		{
			add { }
			remove { }
		}

		event ILogosApplicationEvents_PanelOpenedEventHandler ILogosApplicationEvents_Event.PanelOpened
		{
			add { }
			remove { }
		}

		#endregion
	}
}
