// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2005, SIL International. All Rights Reserved.   
// <copyright from='2005' to='2005' company='SIL International'>
//		Copyright (c) 2005, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: ScrReference.cs
// Responsibility: TE Team
// 
// <remarks>
// </remarks>
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace SIL.FieldWorks.Common.ScriptureUtils
{
	#region SCVersification enum
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Versification types
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public enum SCVersification
	{
		/// <summary></summary>
		scUnknown = 0,
		/// <summary>Like Greek and Hebrew text</summary>
		scOriginal = 1,
		/// <summary>Like LXX Greek OT text</summary>
		scSeptuagint = 2,
		/// <summary>Like Latin Vulgate</summary>
		scVulgate  = 3,
		/// <summary>Like RSV, etc.</summary>
		scEnglish = 4,
		/// <summary></summary>
		scRussianCanonical = 5,
		/// <summary></summary>
		scRussianOrthodox = 6,
		/// <summary>User specified custom versification //! NOT IMPLEMENTED YET.</summary>
		scCustom = 7
	}
	#endregion

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Represents a scripture reference
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public struct ScrReference
	{
		#region Member variables
		private static readonly List<string> s_SIL_BookCodes = new List<string>(new[]
		{
			"",	// 0th entry is invalid, book indices are 1-based
			"GEN", "EXO", "LEV", "NUM", "DEU", "JOS", "JDG", "RUT", "1SA", "2SA",
			"1KI", "2KI", "1CH", "2CH", "EZR", "NEH", "EST", "JOB", "PSA", "PRO",
			"ECC", "SNG", "ISA", "JER", "LAM", "EZK", "DAN", "HOS", "JOL", "AMO",
			"OBA", "JON", "MIC", "NAM", "HAB", "ZEP", "HAG", "ZEC", "MAL", "MAT",
			"MRK", "LUK", "JHN", "ACT", "ROM", "1CO", "2CO", "GAL", "EPH", "PHP",
			"COL", "1TH", "2TH", "1TI", "2TI", "TIT", "PHM", "HEB", "JAS", "1PE",
			"2PE", "1JN", "2JN", "3JN", "JUD", "REV"
		});

		/// <summary>This hash table maps book names to a book number</summary>
		private static Dictionary<string, int> s_bookNameToCode;

		/// <summary>map of the number of chapters in each book</summary>
		private static int[] s_bookChapterCount;

		/// <summary>map of the number of verses in each chapter of a book</summary>
		private static int[][] s_chapterVerseCount;

		/// <summary></summary>
		public static readonly ScrReference Empty = new ScrReference(0);

		private int m_book;
		private int m_chapter;
		private int m_verse;
		private int m_segment;
		#endregion

		#region Constructors
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Construct a reference with an initial BBCCCVVV value
		/// </summary>
		/// <param name="initialRef"></param>
		/// ------------------------------------------------------------------------------------
		public ScrReference(int initialRef): this(GetBookFromBcv(initialRef), 
			GetChapterFromBcv(initialRef), GetVerseFromBcv(initialRef), 0)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Construct a reference with a book, chapter, and verse
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public ScrReference(int book, int chapter, int verse): this(book, chapter, verse, 0)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Copy a reference
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public ScrReference(ScrReference from): this(from.Book, from.Chapter, from.Verse, from.Segment)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initialize a reference - utility for the constructors
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public ScrReference(int book, int chapter, int verse, int segment)
		{
			m_book = book;
			m_chapter = chapter;
			m_verse = verse;
			m_segment = segment;

			if (s_bookChapterCount == null)
				InitializeVersification();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="ScrReference"/> struct.
		/// </summary>
		/// <param name="strReference">The reference as a string.</param>
		/// ------------------------------------------------------------------------------------
		public ScrReference(string strReference)
		{
			m_book = 0;
			m_chapter = 0;
			m_verse = 0;
			m_segment = 0;

			ParseRefString(strReference);
		}

		#endregion

		#region Operators
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Implicit conversion of a <see cref="ScrReference"/> to an integer
		/// </summary>
		/// <param name="scrRef">The <see cref="ScrReference"/> to be converted</param>
		/// <returns>An integer representing a Scripture Reference as a BBCCCVVV value</returns>
		/// ------------------------------------------------------------------------------------
		public static implicit operator int(ScrReference scrRef)
		{
			return scrRef.BBCCCVVV;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Implicit conversion of an integer to a <see cref="ScrReference"/>.
		/// </summary>
		/// <param name="nBCV">The integer representing a Scripture Reference as a BBCCVVV value
		/// </param>
		/// <returns>A <see cref="ScrReference"/></returns>
		/// ------------------------------------------------------------------------------------
		public static implicit operator ScrReference(int nBCV)
		{
			return new ScrReference(nBCV);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// less than operator
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool operator < (ScrReference left, ScrReference right)
		{
			if (left.BBCCCVVV == right.BBCCCVVV)
				return left.Segment < right.Segment;
			return left.BBCCCVVV < right.BBCCCVVV;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// less than or equal operator
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool operator <= (ScrReference left, ScrReference right)
		{
			if (left.BBCCCVVV == right.BBCCCVVV)
				return left.Segment <= right.Segment;
			return left.BBCCCVVV <= right.BBCCCVVV;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// greater than operator
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool operator > (ScrReference left, ScrReference right)
		{
			return right < left;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// greater than or equal operator
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool operator >= (ScrReference left, ScrReference right)
		{
			if (left.BBCCCVVV == right.BBCCCVVV)
				return left.Segment >= right.Segment;
			return left.BBCCCVVV >= right.BBCCCVVV;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// equals operator
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool operator == (ScrReference left, ScrReference right)
		{
			return (left.BBCCCVVV == right.BBCCCVVV && left.Segment == right.Segment);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// equals operator
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool operator == (int left, ScrReference right)
		{
			return (left == right.BBCCCVVV && right.Segment == 0);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// equals operator
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool operator == (ScrReference left, int right)
		{
			return (right == left);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// not equals operator
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool operator != (ScrReference left, ScrReference right)
		{
			return !(left == right);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// not equals operator
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool operator != (int left, ScrReference right)
		{
			return !(left == right);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// not equals operator
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool operator != (ScrReference left, int right)
		{
			return !(left == right);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>Equals is same as ==</summary>
		/// ------------------------------------------------------------------------------------
		public override bool Equals(object o)
		{
			return this == (ScrReference)o;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determine which BBCCCVVV reference is closest to this reference
		/// </summary>
		/// <param name="left">first reference to check</param>
		/// <param name="right">secton reference to check</param>
		/// <returns>0 if the left item is closest, 1 if the right item is closest</returns>
		/// ------------------------------------------------------------------------------------
		public int ClosestTo(int left, int right)
		{
			int bbcccvvv = BBCCCVVV;
			if (Math.Abs(left - bbcccvvv) <= Math.Abs(right - bbcccvvv))
				return 0;
			return 1;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// GetHashCode uses the BBCCCVVV as the hash code
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public override int GetHashCode()
		{
			return BBCCCVVV;
		}
		#endregion
		
		#region GetNumberOfChaptersInRange
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the total number of chapters from the specified start ref to the end reference.
		/// </summary>
		/// <remarks>Note: This is based on the total number of chapters for the current
		/// versification.  If there are some chapters missing in a book present those will 
		/// not be accounted for.</remarks>
		/// <param name="booksPresent">a bool array indicating the presence of each book.</param>
		/// <param name="refStart">Scripture reference where importing begins.</param>
		/// <param name="refEnd">Scripture reference where importing ends.</param>
		/// <returns>The total number of chapters between the start and end references
		/// (inclusively) in books that are present.
		/// </returns>
		/// ------------------------------------------------------------------------------------
		public static int GetNumberOfChaptersInRange(bool[] booksPresent, ScrReference refStart,
			ScrReference refEnd)
		{
			// Determine which chapter number to use in the start reference
			int startChapter = (refStart.Chapter == 0) ? 1 : refStart.Chapter;

			// Consider the case where the start and end references are in the same book.
			if (refStart.Book == refEnd.Book)
				return booksPresent[refStart.Book] ? refEnd.Chapter - startChapter + 1 : 0;
			
			// Add up the number of chapters for the books from the start to the end.
			int expectedChapters = 0;
			for (int book = refStart.Book; book <= refEnd.Book; book++)
			{
				if (booksPresent[book])
				{
					ScrReference scRef = new ScrReference(book, 1, 1);
					if (book == refStart.Book)
						expectedChapters += refStart.LastChapter - startChapter + 1;
					else if (book == refEnd.Book)
						expectedChapters += refEnd.Chapter;
					else
						expectedChapters += scRef.LastChapter;
				}
			}

			return expectedChapters;
		}
		#endregion

		#region Versification Initialization
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Read the versification files into static tables
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static void InitializeVersification()
		{
			// build the hash table that maps book names to numbers
			s_bookNameToCode = new Dictionary<string, int>(66);
			for (int i = 1; i <= 66; i++)
				s_bookNameToCode.Add(s_SIL_BookCodes[i], i);

			// create the arrays for the book chapter count and the chapter verse count
			s_bookChapterCount = new int[67];
			s_bookChapterCount[0] = 0;
			s_chapterVerseCount = new int[67][];

			//// get the path to the versification file
			//string folder = DirectoryFinder.GetFWCodeSubDirectory("Translation Editor");
			//string versificationFileName = Path.Combine(folder, "eng.vrs");
			//try 
			//{
			//    // Read lines of the versification file to build tables
			//    using (StreamReader sr = new StreamReader(versificationFileName)) 
			//    {
			//        String line;
			//        while ((line = sr.ReadLine()) != null) 
			//        {
			//            // ignore lines that start with a hash mark - they are comments
			//            if (line[0] == '#')
			//                continue;

			//            // lines with an '=' character are mappings - ignore these
			//            if (line.IndexOf('=') != -1)
			//                continue;

			//            // get the book number for the line
			//            string bookCode = line.Substring(0, 3);
			//            if (s_bookNameToCode.ContainsKey(bookCode))
			//            {
			//                int book = s_bookNameToCode[bookCode];
			//                List<int> chapterInfo = new List<int>();
			//                foreach (string segment in line.Substring(4).Split(' '))
			//                {
			//                    int split = segment.IndexOf(':');
			//                    int chapter = Int32.Parse(segment.Substring(0, split));
			//                    int verseCount = Int32.Parse(segment.Substring(split + 1));
			//                    chapterInfo.Add(verseCount);
			//                }
			//                s_bookChapterCount[book] = chapterInfo.Count;
			//                s_chapterVerseCount[book] = new int[chapterInfo.Count + 1];

			//                // allow chapter 0 to have 1 verse for intro material references
			//                s_chapterVerseCount[book][0] = 1;
			//                for (int i = 1; i <= chapterInfo.Count; i++)
			//                    s_chapterVerseCount[book][i] = chapterInfo[i - 1];
			//            }
			//        }
			//    }
			//}
			//catch
			//{
			//    // If there was an error reading the versification files, then fail
			//    s_bookChapterCount = null;
			//    throw new Exception("Versification files are missing - rerun installation");
			//}
		}
		#endregion

		#region Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets/sets the book portion of the reference
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int Book
		{
			get { return m_book; }
			set { m_book = value; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets/sets the chapter portion of the reference
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int Chapter
		{
			get { return m_chapter; }
			set { m_chapter = value; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets/sets the verse portion of the reference
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int Verse
		{
			get { return m_verse; }
			set { m_verse = value; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets/sets the segment portion of the reference
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int Segment
		{
			get { return m_segment; }
			set { m_segment = value; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Return the maximum book number
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int LastBook
		{
			get { return 66; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the last valid chapter number for the book
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int LastChapter
		{
			get
			{
				if (s_bookChapterCount == null)
					InitializeVersification();
				try
				{
					return s_bookChapterCount[m_book]; 
				}
				catch
				{
					return 0;
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the last valid verse number for the chapter
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int LastVerse
		{
			get
			{
				if (s_bookChapterCount == null)
					InitializeVersification();

				try { return s_chapterVerseCount[m_book][m_chapter]; }
				catch { return 0; }
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the verse reference as a string
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string AsString
		{
			get { return ToString(BBCCCVVV); }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the current versification scheme
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SCVersification Versification
		{
			get { return SCVersification.scEnglish; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determine if the reference is valid.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool Valid
		{
			get
			{
				if (s_bookChapterCount == null)
					InitializeVersification();

				if (!BookIsValid)
					return false;
				if (m_chapter < 1)// || m_chapter > s_bookChapterCount[m_book])
					return false;
				if (m_verse == 0 && m_chapter > 1)
					return false;
				if (m_verse < 0)// || m_verse > s_chapterVerseCount[m_book][m_chapter])
					return false;
				return true;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determine if the book is valid.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool BookIsValid
		{
			get
			{
				return (m_book >= 1 && m_book <= 66);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets/sets the reference as a book/chapter/verse integer
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int BBCCCVVV
		{
			get
			{
				return (m_book * 1000000 + m_chapter * 1000 + m_verse);
			}
			set
			{
				Book = GetBookFromBcv(value);
				Chapter = GetChapterFromBcv(value);
				Verse = GetVerseFromBcv(value);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines if a reference is empty
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool IsEmpty
		{
			get { return m_book == 0 && m_chapter == 0 && m_verse == 0 && m_segment == 0; }
		}
		#endregion

		#region Public methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the last valid chapter number for a specified book
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static int LastChapterForBook(int book)
		{
			if (s_bookChapterCount == null)
				InitializeVersification();
			try 
			{ 
				return s_bookChapterCount[book]; 
			}
			catch 
			{ 
				return 0; 
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the last valid verse number for a given book and chapter
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static int LastVerseForChapter(int book, int chapter)
		{
			if (s_bookChapterCount == null)
				InitializeVersification();

			try { return s_chapterVerseCount[book][chapter]; }
			catch { return 0; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the last valid reference for a book
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static ScrReference LastReferenceForBook(int book)
		{
			if (s_bookChapterCount == null)
				InitializeVersification();

			try
			{
				int chapter = s_bookChapterCount[book];
				int verse = s_chapterVerseCount[book][chapter];
				return new ScrReference(book, chapter, verse);
			}
			catch
			{
				return ScrReference.Empty;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the verse reference as a string
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public override string ToString()
		{
			return ToString(BBCCCVVV);
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the verse reference as a string
		/// </summary>
		/// <param name="bcv">The bcv</param>
		/// <returns>The verse reference</returns>
		/// ------------------------------------------------------------------------------------
		public static string ToString(int bcv)
		{
			return string.Format("{0} {1}:{2}", s_SIL_BookCodes[GetBookFromBcv(bcv)],
				GetChapterFromBcv(bcv), GetVerseFromBcv(bcv));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handy little utility function for making a Reference string.
		/// </summary>
		/// <param name="bookName">The Scripture book name</param>
		/// <param name="nBbCccVvvStart">The beginning Scripture reference</param>
		/// <param name="nBbCccVvvEnd">The ending Scripture reference</param>
		/// <param name="chapterVerseSeparator">Character(s) used to delimit the chapter and verse
		/// number</param>
		/// <param name="verseBridge">Character(s) used to connect two references, indicating a
		/// range</param>
		/// <returns>The reference range as a formatted string.</returns>
		/// ------------------------------------------------------------------------------------
		public static string MakeReferenceString(string bookName, int nBbCccVvvStart,
			int nBbCccVvvEnd, string chapterVerseSeparator, string verseBridge)
		{
			ScrReference startRef = new ScrReference(nBbCccVvvStart);
			ScrReference endRef = new ScrReference(nBbCccVvvEnd);
			bookName = bookName.Trim();

			// Build strings to use for the chapter/verse separator and the verse bridge.
			// This method is always used for displaying references in the UI and the separator
			// strings may be surrounded by direction characters. So, we want to get rid of the
			// direction characters before using the strings.
			if (chapterVerseSeparator.Length == 3)
				chapterVerseSeparator = chapterVerseSeparator.Substring(1, 1);
			if (verseBridge.Length == 3)
				verseBridge = verseBridge.Substring(1, 1);

			if (startRef.Chapter != endRef.Chapter)
			{
				return bookName + " " + startRef.Chapter + chapterVerseSeparator + 
					startRef.Verse + verseBridge + endRef.Chapter +
					chapterVerseSeparator + endRef.Verse;
			}
			if (startRef.Verse != endRef.Verse)
			{
				return bookName + " " + startRef.Chapter + chapterVerseSeparator + 
					startRef.Verse + verseBridge + endRef.Verse;
			}
			if (startRef.Verse != 0)
			{
				return bookName + " " + startRef.Chapter + chapterVerseSeparator + 
					startRef.Verse;
			}
			return bookName;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Validate all of the fields to make sure they are in a valid range.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void Validate()
		{
			if (s_bookChapterCount == null)
				InitializeVersification();

			if (m_book < 1)
				m_book = 1;
			if (m_book > 66)
				m_book = 66;

			if (m_chapter < 1)
				m_chapter = 1;
			if (m_chapter > s_bookChapterCount[m_book])
				m_chapter = s_bookChapterCount[m_book];

			//REVIEW: Chapter 2 Verse 0 should be invalid.
			if (m_verse < 0)
				m_verse = 0;
			if (m_verse > s_chapterVerseCount[m_book][m_chapter])
				m_verse = s_chapterVerseCount[m_book][m_chapter];
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Set the versification scheme - currently ignored
		/// </summary>
		/// <param name="NewVersification"></param>
		/// ------------------------------------------------------------------------------------
		public void ChangeVersification(SCVersification NewVersification)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Empty the contents of the reference
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void SetEmpty()
		{
			m_book = 0;
			m_chapter = 0;
			m_verse = 0;
			m_segment = 0;
		}
		#endregion

		#region Book/Chapter/Verse conversions
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Map an SIL book code to a book number
		/// </summary>
		/// <param name="book"></param>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		public static int BookToNumber(string book)
		{
			if (s_bookChapterCount == null)
				InitializeVersification();

			string key = (book.Length > 3) ? book.Substring(0, 3) : book;

			if (!s_bookNameToCode.ContainsKey(key))
				return -1;
			return s_bookNameToCode[key];
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Map a book number to an SIL book code
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string NumberToBookCode(int bookNumber)
		{
			if (bookNumber < 1 || bookNumber > 66)
				return string.Empty;
			return s_SIL_BookCodes[bookNumber];
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the verse part of the specified bcv
		/// </summary>
		/// <param name="bcv">The bcv to parse</param>
		/// <returns>The verse part of the specified bcv</returns>
		/// ------------------------------------------------------------------------------------
		public static int GetVerseFromBcv(int bcv)
		{
			return (bcv % 1000);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the chapter part of the specified bcv
		/// </summary>
		/// <param name="bcv">The bcv to parse</param>
		/// <returns>The chapter part of the specified bcv</returns>
		/// ------------------------------------------------------------------------------------
		public static int GetChapterFromBcv(int bcv)
		{
			return (bcv / 1000) % 1000;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the book part of the specified bcv
		/// </summary>
		/// <param name="bcv">The bcv to parse</param>
		/// <returns>The book part of the specified bcv</returns>
		/// ------------------------------------------------------------------------------------
		public static int GetBookFromBcv(int bcv)
		{
			return (bcv / 1000000);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Convert a chapter number string into an integer and ignore the remaining text
		/// </summary>
		/// <param name="chapterString"></param>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		public static int ChapterToInt(string chapterString)
		{
			string dummy;

			return ChapterToInt(chapterString, out dummy);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Convert a chapter number string into an integer
		/// </summary>
		/// <param name="chapterString">string representing the chapter number</param>
		/// <param name="remainingText">returns the remaining non-number portion of the string</param>
		/// <returns>The chapter number</returns>
		/// ------------------------------------------------------------------------------------
		public static int ChapterToInt(string chapterString, out string remainingText)
		{
			remainingText = string.Empty;
			if (chapterString == null)
				throw new ArgumentNullException("chapterString");
			chapterString = chapterString.TrimStart();
			if (chapterString == string.Empty)
				throw new ArgumentException("The chapter string was empty");
			if (!Char.IsDigit(chapterString[0]))
				throw new ArgumentException("The chapter string does not start with a digit");

			int chapter = 0;
			for (int i = 0; i < chapterString.Length; i++)
			{
				char ch = chapterString[i];
				if (Char.IsDigit(ch))
				{
					chapter = chapter * 10 + (int)Char.GetNumericValue(ch);
					if (chapter > Int16.MaxValue)
						chapter = Int16.MaxValue;
				}
				else
				{
					remainingText = chapterString.Substring(i);
					break;
				}
			}
			if (chapter == 0)
				throw new ArgumentException("The chapter number evaluated to 0");
			return chapter;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// A version of VerseToInt that returns the starting verse value. 
		/// </summary>
		/// <param name="sourceString"></param>
		/// <returns>the starting verse value</returns>
		/// ------------------------------------------------------------------------------------
		public static int VerseToIntStart(string sourceString)
		{
			int startVerse, endVerse;
			VerseToInt(sourceString, out startVerse, out endVerse);
			return startVerse;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// A version of VerseToInt that returns the ending verse value. 
		/// </summary>
		/// <param name="sourceString"></param>
		/// <returns>the ending verse value</returns>
		/// ------------------------------------------------------------------------------------
		public static int VerseToIntEnd(string sourceString)
		{
			int startVerse, endVerse;
			VerseToInt(sourceString, out startVerse, out endVerse);
			return endVerse;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// This is a helper function to get a starting and ending verse number from a string
		/// which may or may not represent a verse bridge. Ignore any unusual syntax.
		/// </summary>
		/// <param name="sVerseNum">the string representing the verse number(s).</param>
		/// <param name="nVerseStart">the starting verse number in sVerseNum.</param>
		/// <param name="nVerseEnd">the ending verse number in sVerseNum (will be different from
		/// startRef if sVerseNum represents a verse bridge).</param>
		/// ------------------------------------------------------------------------------------
		public static void VerseToInt(string sVerseNum, out int nVerseStart, out int nVerseEnd)
		{
			int nFactor = 1;
			int nVerseT = 0;
			nVerseStart = nVerseEnd = 0;
			// nVerseFirst is the left-most (or right-most if R2L) non-zero number found.
			int nVerseFirst = nVerseT;
			bool fVerseBridge = false;
			if (sVerseNum == null)
				return;
			// REVIEW JohnW (TomB): For robustness, our initial implementation will assume
			// that the first set of contiguous numbers is the starting verse number and
			// the last set of contiguous numbers is the ending verse number. This way, we
			// don't have to know what all the legal possibilities of bridge markers and
			// sub-verse segment indicators are.
			for (int i = sVerseNum.Length - 1; i >= 0; i--)
			{
				int numVal = -1;
				if (Char.IsDigit(sVerseNum[i]))
					numVal = (int)Char.GetNumericValue(sVerseNum[i]);

				if (numVal >= 0 && numVal <= 9)
				{
					if (nFactor > 100) // verse number greater than 999
					{
						// REVIEW JohnW (TomB): Need to decide how we want to display this.
						nVerseT = 999;
					}
					else
					{
						nVerseT += nFactor * numVal;
						nFactor *= 10;
					}
					nVerseFirst = nVerseT;
				}
				else if (nVerseT > 0)
				{
					if (!fVerseBridge)
					{
						fVerseBridge = true;
						nVerseFirst = nVerseEnd = nVerseT;
					}
					nVerseT = 0;
					nFactor = 1;
				}
			}
			nVerseStart = nVerseFirst;
			if (!fVerseBridge)
				nVerseEnd = nVerseFirst;

			// Don't want to use an assertion for this because it could happen due to bad input data.
			// If this causes problems, just pick one ref and use it for both or something.
			// TODO TomB: Later, we need to catch this and flag it as an error.
			//Assert(nVerseStart <= nVerseEnd);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Overloaded version of VerseToScrRef that ignores some parameters
		/// </summary>
		/// <param name="sourceString">string containing the verse number</param>
		/// <param name="firstRef">first reference that will have the verse portion adjusted</param>
		/// <param name="lastRef">last reference that will have the verse portion adjusted</param>
		/// <returns>true if converted successfully</returns>
		/// ------------------------------------------------------------------------------------
		public static bool VerseToScrRef(string sourceString, ref ScrReference firstRef, 
			ref ScrReference lastRef)
		{
			string dummy1, dummy2;

			return VerseToScrRef(sourceString, out dummy1, out dummy2, ref firstRef, ref lastRef);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Extract the verse numbers from a verse string. Determine verse number begin and end 
		/// values as well as verse segment values. Ignore any unusual syntax.
		/// Limitations: This class does not have access to the FDO Scripture bridge character 
		/// or to a character property engine with PUA character information.
		/// </summary>
		/// <param name="sourceString">string from which to attempt extracting verse numbers
		/// </param>
		/// <param name="literalVerse">returns the text that was converted to a verse number
		/// </param>
		/// <param name="remainingText">returns the remaining text after the verse number
		/// </param>
		/// <param name="firstRef">returns the first reference</param>
		/// <param name="lastRef">returns the last reference in the case of a verse bridge
		/// </param>
		/// <returns>true if converted successfully</returns>
		/// ------------------------------------------------------------------------------------
		public static bool VerseToScrRef(string sourceString, out string literalVerse,
			out string remainingText, ref ScrReference firstRef, ref ScrReference lastRef)
		{
			int firstVerse = 0;
			int lastVerse = 0;
			bool inFirst = true;
			int stringSplitPos = 0;
			int iDashCount = 0;

			// break the string out into the verse number portion and the following text portion.
			char prevChar = '\0';
			int i = 0;
			while (i < sourceString.Length)
			{
				char ch = sourceString[i];
				if (Char.IsDigit(ch))
				{
					stringSplitPos = i + 1;
				}
				else if (Char.IsLetter(ch))
				{
					if (prevChar == '.')
					{
						stringSplitPos = i;
						break;
					}
					if (Char.IsLetter(prevChar) || Char.IsPunctuation(prevChar))
					{
						stringSplitPos = i - 1;
						break;
					}
				}
				else if (ch != '.' && Char.IsPunctuation(ch))
				{
					if (iDashCount > 0)
					{
						if (prevChar == '-')
							stringSplitPos = i - 1;
						else
							stringSplitPos = i;
						break;
					}
					iDashCount++;
				}
				else if (ch == '\u200f' || ch == '\u200e')
				{
					// RTL and LTR marks
					// don't let these characters be saved as prevChar
					i++;
					continue;
				}
				else if (ch == '.')
				{
				}
				else
				{
					// all other characters (including space) terminate the verse number
					stringSplitPos = i;
					break;
				}
				prevChar = ch;
				i++;
			}

			literalVerse = sourceString.Substring(0, stringSplitPos);
			remainingText = sourceString.Substring(stringSplitPos) ?? string.Empty;

			// parse the verse string to get the verse numbers out.
			prevChar = '\0';
			int firstSegment = 0;
			int lastSegment = 0;
			foreach (char ch in literalVerse)
			{
				if (Char.IsDigit(ch))
				{
					// Add the digit to either the first or last verse in the bridge
					if (inFirst)
					{
						firstVerse = firstVerse * 10 + (int)Char.GetNumericValue(ch);
						if (firstVerse > Int16.MaxValue)
							return false; // whoops, we got too big!
					}
					else
					{
						lastVerse = lastVerse * 10 + (int)Char.GetNumericValue(ch);
						if (lastVerse > Int16.MaxValue)
							return false; // whoops, we got too big!
					}
				}
				else if (Char.IsLetter(ch))
				{
					// letters are used for segments
					if (Char.IsDigit(prevChar))
					{
						if (inFirst)
						{
							// for the first verse in the segment, look at the old
							// reference and increment the segment if the verse
							// number has not changed. Otherwise, start the segment
							// at 1.
							if (firstVerse == lastRef.Verse)
								firstSegment = lastRef.Segment + 1;
							else
								firstSegment = 1;
						}
						else
						{
							// If the verses are the same in the bridge, then the second segment
							// will be one greater than the first segment, otherwise it will be 1.
							if (firstVerse == lastVerse)
								lastSegment = firstSegment + 1;
							else
								lastSegment = 1;
						}
					}
					else
					{
						// if there is not a digit preceding a segment letter, this is an
						// error so quit.
						return false;
					}
				}
				else
				{
					// any other character will switch to the second verse in the bridge
					inFirst = false;
				}
				prevChar = ch;
			}

			if (lastVerse == 0)
			{
				lastVerse = firstVerse;
				lastSegment = firstSegment;
			}
			firstRef.Verse = firstVerse;
			firstRef.Segment = firstSegment;
			lastRef.Verse = lastVerse;
			lastRef.Segment = lastSegment;
			return true;
		}
		#endregion

		#region Private helper methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Parses Scripture reference string.
		/// </summary>
		/// <param name="sTextToBeParsed">Reference string the user types in.</param>
		/// <remarks>This method is pretty similar to MultilingScrBooks.ParseRefString, but
		/// it deals only with SIL codes.</remarks>
		/// ------------------------------------------------------------------------------------
		private void ParseRefString(string sTextToBeParsed)
		{
			// Get first token
			string sTrimedText = sTextToBeParsed.TrimStart(null); // Trim all std white space
			string sToken = sTrimedText.Substring(0, 3); // 3 letter SIL code
			string sAfterToken = sTrimedText.Substring(3);

			// Determine book number
			m_book = s_SIL_BookCodes.IndexOf(sToken.ToUpper());
			if (!BookIsValid)
				return;

			// Break out the chapter and verse numbers
			bool inChapter = true;

			m_verse = -1;

			// If there is no chapter:verse portion then just set 1:1
			if (sAfterToken == string.Empty)
				m_chapter = m_verse = 1;

			foreach (char ch in sAfterToken)
			{
				if (Char.IsDigit(ch))
				{
					if (inChapter)
					{
						m_chapter *= 10;
						m_chapter += (int)Char.GetNumericValue(ch);
					}
					else
					{
						if (m_verse < 0)
							m_verse = (int)Char.GetNumericValue(ch);
						else
						{
							m_verse *= 10;
							m_verse += (int)Char.GetNumericValue(ch);
						}
					}
				}
				else if (!char.IsWhiteSpace(ch))
				{
					if (inChapter)
						inChapter = false;
					else 
					{
						// got an invalid character
						m_book = -1;
						return;
					}
				}
			}

			// If there was no verse specified, then make it 1
			if (m_verse == -1)
				m_verse = 1;
		}
		#endregion

		#region ClearTables
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Clear out the versification tables - only used for testing
		/// </summary>
		/// ------------------------------------------------------------------------------------
		static public void ClearTables()
		{
			s_bookChapterCount = null;
		}
		#endregion
	}
}
