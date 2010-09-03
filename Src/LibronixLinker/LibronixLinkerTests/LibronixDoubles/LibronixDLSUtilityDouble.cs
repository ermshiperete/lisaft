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
using LibronixDLSUtility;

namespace SIL.Utils
{
	#region LbxApplicationEventsBridgeDouble
#pragma warning disable 67 // The event EventFired is never used
	class LbxApplicationEventsBridgeDouble : LbxApplicationEventsBridge
	{
		public int Connect(object Object)
		{
			return 0;
		}

		public void Disconnect(object Object, int Cookie)
		{
		}

		event DLbxApplicationEvents_EventFiredEventHandler DLbxApplicationEvents_Event.EventFired
		{
			add { }
			remove { }
		}
	}
#pragma warning restore 67
	#endregion

}
