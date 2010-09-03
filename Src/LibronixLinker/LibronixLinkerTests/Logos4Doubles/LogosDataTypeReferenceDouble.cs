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
