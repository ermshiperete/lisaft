﻿using System;
namespace SIL.FieldWorks.TE.LibronixLinker
{
	/// <summary/>
	public class DisposedEventArgs: EventArgs
	{

		/// <summary/>
		public DisposedEventArgs(int linkSet, bool logosAppQuit)
		{
			LinkSet = linkSet;
			LogosAppQuit = logosAppQuit;
		}

		/// <summary>The link set (0-based)</summary>
		public int LinkSet { get; set; }
		
		/// <summary><c>true</c> if the Dispose happened because the Libronix/Logos
		/// app closed, <c>false</c> if the calling app is closing.</summary>
		public bool LogosAppQuit { get; set; }
	}

	/// <summary>
	/// Reference counting and other internal methods for ILogosPositionHandler
	/// </summary>
	internal interface ILogosPositionHandlerInternal
	{
		/// <summary>
		/// Increments the reference count
		/// </summary>
		void AddRef();
		/// <summary>
		/// Decrements the reference count
		/// </summary>
		/// <returns>The new reference count after decrementing</returns>
		int Release();

		event EventHandler<DisposedEventArgs> Disposed;
	}
}
