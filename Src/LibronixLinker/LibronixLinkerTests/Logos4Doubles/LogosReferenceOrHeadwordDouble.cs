using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logos4Lib;

namespace SIL.FieldWorks.TE.LibronixLinker.Logos4Doubles
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
