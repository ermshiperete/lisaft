using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logos4Lib;

namespace SIL.FieldWorks.TE.LibronixLinker.Logos4Doubles
{
	class LogosDataTypeReferenceDouble : LogosDataTypeReference
	{
		#region ILogosDataTypeReference Members

		public int CompareTo(LogosDataTypeReference reference)
		{
			throw new NotImplementedException();
		}

		public LogosDataType DataType
		{
			get { return new LogosDataTypeDouble(); }
		}

		public object Details
		{
			get { return new LogosBibleReferenceDetailsDouble(); }
		}

		public string DetailsKind
		{
			get
			{
				if (LogosApplicationDouble.VersionNo < 2)
					throw new NotImplementedException();
				return "Bible";
			}
		}

		public bool Intersects(LogosDataTypeReference reference)
		{
			throw new NotImplementedException();
		}

		public bool IsEqualTo(LogosDataTypeReference reference)
		{
			throw new NotImplementedException();
		}

		public bool IsRange
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference RangeEnd
		{
			get { throw new NotImplementedException(); }
		}

		public LogosDataTypeReference RangeStart
		{
			get { throw new NotImplementedException(); }
		}

		public string Render(string style)
		{
			throw new NotImplementedException();
		}

		public string Save()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
