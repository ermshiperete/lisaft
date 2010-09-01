using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logos4Lib;

namespace SIL.FieldWorks.TE.LibronixLinker.Logos4Doubles
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
