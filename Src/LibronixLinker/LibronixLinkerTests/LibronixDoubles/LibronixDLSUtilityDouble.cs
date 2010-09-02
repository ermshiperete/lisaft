﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibronixDLSUtility;

namespace SIL.FieldWorks.TE.LibronixLinker
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