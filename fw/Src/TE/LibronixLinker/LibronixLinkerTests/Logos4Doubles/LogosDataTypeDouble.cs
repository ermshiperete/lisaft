using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logos4Lib;

namespace SIL.FieldWorks.TE.LibronixLinker.Logos4Doubles
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
