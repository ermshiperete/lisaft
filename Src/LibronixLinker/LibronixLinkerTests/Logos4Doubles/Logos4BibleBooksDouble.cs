using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logos4Lib;

namespace SIL.FieldWorks.TE.LibronixLinker.Logos4Doubles
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
