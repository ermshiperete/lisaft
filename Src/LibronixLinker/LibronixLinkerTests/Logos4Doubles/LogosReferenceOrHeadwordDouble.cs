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
	class LogosReferenceOrHeadwordDouble : LogosReferenceOrHeadword
	{
		#region ILogosReferenceOrHeadword Members

		public string Headword
		{
			get { throw new NotImplementedException(); }
		}

		public string HeadwordLanguage
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference Reference
		{
			get { return new LogosDataTypeReferenceDouble(); }
		}

		#endregion
	}
}
