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
using Logos4Lib;

namespace SIL.Utils.Logos4Doubles
{
	internal class Logos4PositionHandlerDouble: Logos4PositionHandler
	{
		public PositionChangedEventArgs EventArgs;

		public Logos4PositionHandlerDouble(int linkSet, LogosApplication logosApp) 
			: base(linkSet, logosApp)
		{
			PositionChanged += OnPositionChanged;

			s_Books = new Logos4BibleBooksDouble(null);
		}

		private void OnPositionChanged(object sender, PositionChangedEventArgs e)
		{
			EventArgs = e;
		}

		public void CallOnPanelChanged(object objPanel)
		{
			OnPanelChanged(objPanel, null);
		}

	}
}
