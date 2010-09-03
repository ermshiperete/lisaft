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
	class LogosNavigationRequestDouble : LogosNavigationRequest
	{
		#region ILogosNavigationRequest Members

		public string Article { get; set; }

		public string Headword { get; set; }

		public string HeadwordLanguage { get; set; }

		public LogosDataTypeReference Reference { get; set; }

		public string ResourceId { get; set; }

		#endregion
	}
}
