using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logos4Lib;

namespace SIL.FieldWorks.TE.LibronixLinker.Logos4Doubles
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
			throw new NotImplementedException();
		}

		public LogosPanelCollection GetOpenPanels()
		{
			return new LogosPanelCollectionDouble();
		}

		public LogosPanel GetPanel(string PanelId)
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
