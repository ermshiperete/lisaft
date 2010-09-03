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
			get { return "My Resource"; }
		}

		#endregion
	}
}
