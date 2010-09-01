using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logos4Lib;

namespace SIL.FieldWorks.TE.LibronixLinker.Logos4Doubles
{
	internal class LogosPanelDouble: LogosPanel
	{
		#region ILogosPanel Members

		public object Details
		{
			get { throw new NotImplementedException(); }
		}

		public string DetailsKind
		{
			get { throw new NotImplementedException(); }
		}

		public LogosReferenceOrHeadwordCollection GetCurrentReferencesAndHeadwords()
		{
			return new LogosReferenceOrHeadwordCollectionDouble();
		}

		public bool IsOpen
		{
			get { throw new NotImplementedException(); }
		}

		public string Kind
		{
			get { throw new NotImplementedException(); }
		}

		public string LinkSet
		{
			get { return "Index:1"; }
		}

		public void Navigate(LogosNavigationRequest request)
		{
		}

		public string PanelId
		{
			get { throw new NotImplementedException(); }
		}

		public string Title
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
