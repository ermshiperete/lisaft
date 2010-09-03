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
using System.Collections;
using System.Collections.Generic;
using Logos4Lib;

namespace SIL.Utils.Logos4Doubles
{
	#region LogosCollectionDouble class
	class LogosCollectionDouble<T, TD> where TD: T, new()
	{
		private readonly List<T> m_list;

		public LogosCollectionDouble()
		{
			m_list = new List<T> {new TD()};
		}

		#region T Members

		public int Count
		{
			get { return m_list.Count; }
		}

		public IEnumerator GetEnumerator()
		{
			return m_list.GetEnumerator();
		}

		public T this[int index]
		{
			get { return m_list[index]; }
		}

		#endregion
	}
	#endregion

	class LogosReferenceOrHeadwordCollectionDouble :
		LogosCollectionDouble<LogosReferenceOrHeadword, LogosReferenceOrHeadwordDouble>,
		LogosReferenceOrHeadwordCollection
	{
	}

	class LogosPanelCollectionDouble :
		LogosCollectionDouble<LogosPanel, LogosPanelDouble>,
		LogosPanelCollection
	{
	}

}
