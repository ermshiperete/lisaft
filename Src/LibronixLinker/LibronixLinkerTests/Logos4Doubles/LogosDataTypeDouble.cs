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
	class LogosDataTypeDouble : LogosDataType
	{
		#region ILogosDataType Members

		public string AbbreviatedTitle
		{
			get { throw new NotImplementedException(); }
		}

		public string Alias
		{
			get { throw new NotImplementedException(); }
		}

		public object Details
		{
			get { throw new NotImplementedException(); }
		}

		public string DetailsKind
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference ParseReference(string text)
		{
			LogosBibleReferenceDetailsDouble.Reference = text;
			return new LogosDataTypeReferenceDouble();
		}

		public LogosDataTypeParsedReferenceCollection ScanForReferences(string text)
		{
			throw new NotImplementedException();
		}

		public string SortTitle
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
