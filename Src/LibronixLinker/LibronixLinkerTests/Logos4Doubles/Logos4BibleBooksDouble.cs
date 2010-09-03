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
	class Logos4BibleBooksDouble: Logos4BibleBooks
	{
		public Logos4BibleBooksDouble(LogosApplication logosApplication) : base(logosApplication)
		{
		}

		protected override void Initialize(LogosApplication logosApplication)
		{
			var books = LogosBibleReferenceDetailsDouble.Books;
			for (int i = 0; i < books.GetLength(0); i++)
			{
				Add(new BibleBook(books[i, 0], books[i, 1], i));
			}
		}
	}
}
