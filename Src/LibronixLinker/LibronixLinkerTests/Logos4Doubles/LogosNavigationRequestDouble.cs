using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logos4Lib;

namespace SIL.FieldWorks.TE.LibronixLinker.Logos4Doubles
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
