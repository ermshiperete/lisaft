using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logos4Lib;

namespace SIL.FieldWorks.TE.LibronixLinker.Logos4Doubles
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
