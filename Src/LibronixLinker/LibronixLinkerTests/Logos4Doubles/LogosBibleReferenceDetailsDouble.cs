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
using System;
using Logos4Lib;

namespace SIL.Utils.Logos4Doubles
{
	class LogosBibleReferenceDetailsDouble : LogosBibleReferenceDetails
	{
		public static string[,] Books = {
											   { "", ""},
		                                   	{"Genesis", "Ge"},
		                                   	{"Exodus", "Ex"},
		                                   	{"Leviticus", "Le"},
		                                   	{"Numbers", "Nu"},
		                                   	{"Deuteronomy", "Dt"},
		                                   	{"Joshua", "Jos"},
		                                   	{"Judges", "Jdg"},
		                                   	{"Ruth", "Ru"},
		                                   	{"1 Samuel", "1Sa"},
		                                   	{"2 Samuel", "2Sa"},
		                                   	{"1 Kings", "1Ki"},
		                                   	{"2 Kings", "2Ki"},
		                                   	{"1 Chronicles", "1Ch"},
		                                   	{"2 Chronicles", "2Ch"},
		                                   	{"Ezra", "Ezr"},
		                                   	{"Nehemiah", "Ne"},
		                                   	{"Esther", "Es"},
		                                   	{"Job", "Job"},
		                                   	{"Psalms", "Ps"},
		                                   	{"Proverbs", "Pr"},
		                                   	{"Ecclesiastes", "Ec"},
		                                   	{"Song of Solomon", "So"},
		                                   	{"Isaiah", "Is"},
		                                   	{"Jeremiah", "Je"},
		                                   	{"Lamentations", "La"},
		                                   	{"Ezekiel", "Eze"},
		                                   	{"Daniel", "Da"},
		                                   	{"Hosea", "Ho"},
		                                   	{"Joel", "Joe"},
		                                   	{"Amos", "Am"},
		                                   	{"Obadiah", "Ob"},
		                                   	{"Jonah", "Jon"},
		                                   	{"Micah", "Mic"},
		                                   	{"Nahum", "Na"},
		                                   	{"Habakkuk", "Hab"},
		                                   	{"Zephaniah", "Zep"},
		                                   	{"Haggai", "Hag"},
		                                   	{"Zechariah", "Zec"},
		                                   	{"Malachi", "Mal"},
		                                   	{"Tobit", "Tob"},
		                                   	{"Judith", "Jdt"},
		                                   	{"Greek Esther", "GkEs"},
		                                   	{"Wisdom of Solomon", "Wis"},
		                                   	{"Sirach", "Sir"},
		                                   	{"Baruch", "Bar"},
		                                   	{"Letter of Jeremiah", "LetJer"},
		                                   	{"Song of Three Youths", "SongThr"},
		                                   	{"Susanna", "Sus"},
		                                   	{"Bel and the Dragon", "Bel"},
		                                   	{"1 Maccabees", "1Mac"},
		                                   	{"2 Maccabees", "2Mac"},
		                                   	{"1 Esdras", "1Esd"},
		                                   	{"Prayer of Manasseh", "PrMan"},
		                                   	{"Psalm 151", "Ps151"},
		                                   	{"3 Maccabees", "3Mac"},
		                                   	{"2 Esdras", "2Esd"},
		                                   	{"4 Maccabees", "4Mac"},
		                                   	{"Odes", "Ode"},
		                                   	{"Psalms of Solomon", "PsSol"},
		                                   	{"Epistle to the Laodiceans", "Laod"},
		                                   	{"Matthew", "Mt"},
		                                   	{"Mark", "Mk"},
		                                   	{"Luke", "Lk"},
		                                   	{"John", "Jn"},
		                                   	{"Acts", "Ac"},
		                                   	{"Romans", "Ro"},
		                                   	{"1 Corinthians", "1Co"},
		                                   	{"2 Corinthians", "2Co"},
		                                   	{"Galatians", "Ga"},
		                                   	{"Ephesians", "Eph"},
		                                   	{"Philippians", "Php"},
		                                   	{"Colossians", "Col"},
		                                   	{"1 Thessalonians", "1Th"},
		                                   	{"2 Thessalonians", "2Th"},
		                                   	{"1 Timothy", "1Ti"},
		                                   	{"2 Timothy", "2Ti"},
		                                   	{"Titus", "Tt"},
		                                   	{"Philemon", "Phm"},
		                                   	{"Hebrews", "Heb"},
		                                   	{"James", "Jas"},
		                                   	{"1 Peter", "1Pe"},
		                                   	{"2 Peter", "2Pe"},
		                                   	{"1 John", "1Jn"},
		                                   	{"2 John", "2Jn"},
		                                   	{"3 John", "3Jn"},
		                                   	{"Jude", "Jud"},
		                                   	{"Revelation", "Re"}
		                                   };

		public static string Reference { get; set; }

		private static void SplitReference(out string book, out string chapter, out string verse)
		{
			var parts = Reference.Split('.');
			book = parts[0];
			chapter = parts[1];
			verse = parts[2];
		}
		#region ILogosBibleReferenceDetails Members

		public string Book
		{
			get
			{
				string book, chapter, verse;
				SplitReference(out book, out chapter, out verse);
				return book;
			}
		}

		public string Chapter
		{
			get
			{
				string book, chapter, verse;
				SplitReference(out book, out chapter, out verse);
				return chapter;
			}
		}

		public string Verse
		{
			get
			{
				string book, chapter, verse;
				SplitReference(out book, out chapter, out verse);
				return verse;
			}
		}

		public LogosDataTypeReference FirstBook
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference FirstChapter
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference FirstVerse
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference LastBook
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference LastChapter
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference LastVerse
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference NextBook
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference NextChapter
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference NextVerse
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference PreviousBook
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference PreviousChapter
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference PreviousVerse
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
