﻿// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2010, SIL International. All Rights Reserved.
// <copyright from='2010' to='2010' company='SIL International'>
//		Copyright (c) 2010, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System.Collections.ObjectModel;
using Logos4Lib;

namespace SIL.Utils
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Contains information about a Bible book: name, abbreviation and id
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	internal class BibleBook
	{
		internal BibleBook(string name, string abbrev, int bookId)
		{
			Name = name;
			Abbrev = abbrev;
			BookId = bookId;
		}
		internal string Name;
		internal string Abbrev;
		internal int BookId;
	}

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Book names and abbreviations as used in Logos 4
	/// </summary>
	/// <remarks><see href="http://wiki.logos.com/COM_API_Bible_Book_Abbreviations"/></remarks>
	/// ----------------------------------------------------------------------------------------
	internal class Logos4BibleBooks : KeyedCollection<string, BibleBook>
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="Logos4BibleBooks"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Logos4BibleBooks(LogosApplication logosApplication)
		{
			// Add dummy book at index 0
			Initialize(logosApplication);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Retrieve the book names and abbreviations from Logos 4
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected virtual void Initialize(LogosApplication logosApplication)
		{
			if (logosApplication == null)
				return;

			Add(new BibleBook("Dummy", "Dummy", -1));

			// Now add all books from Logos
			int id = 1;
			var book = ((LogosBibleDataTypeDetails)logosApplication.DataTypes.GetDataType("Bible").Details).FirstBook;
			while (book != null)
			{
				Add(new BibleBook(book.Render(string.Empty), ((LogosBibleReferenceDetails)book.Details).Book, id++));
				book = ((LogosBibleReferenceDetails)book.Details).NextBook;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Extract the key from the specified element.
		/// </summary>
		/// <param name="item">The element from which to extract the key.</param>
		/// <returns>The key for the specified element.</returns>
		/// ------------------------------------------------------------------------------------
		protected override string GetKeyForItem(BibleBook item)
		{
			return item.Abbrev;
		}
	}
}
