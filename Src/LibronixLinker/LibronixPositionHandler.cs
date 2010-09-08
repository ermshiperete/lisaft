// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2008, SIL International. All Rights Reserved.
// <copyright from='2007' to='2008' company='SIL International'>
//		Copyright (c) 2007-2008, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: LibronixPositionHandler.cs
// Responsibility: TE Team
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LibronixDLS;
using LibronixDLSUtility;

namespace SIL.Utils
{
	#region Exceptions
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// The exception that is thrown if Libronix is not installed
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class LibronixNotInstalledException : ApplicationException
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="LibronixNotInstalledException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		/// ------------------------------------------------------------------------------------
		public LibronixNotInstalledException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// The exception that is thrown if Libronix is not currently running so that it is 
	/// impossible to connect to the Libronix COM object.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class LibronixNotRunningException : ApplicationException
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="LibronixNotRunningException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		/// ------------------------------------------------------------------------------------
		public LibronixNotRunningException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
	#endregion

	#region PositionChangedEventArgs class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Provides data for the PositionChanged event.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class PositionChangedEventArgs : EventArgs
	{
		private readonly int m_BcvRef;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="PositionChangedEventArgs"/> class.
		/// </summary>
		/// <param name="bcvRef">The BBCCCVVV reference.</param>
		/// ------------------------------------------------------------------------------------
		public PositionChangedEventArgs(int bcvRef)
		{
			m_BcvRef = bcvRef;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the BBCCCVVV reference.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int BcvRef
		{
			get { return m_BcvRef; }
		}
	}
	#endregion

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Class that polls for changed positions in Libronix
	/// </summary>
	/// <remarks>There is only one instance of this class per link set.
	/// This implementation doesn't take link sets into consideration when receiving references
	/// from Libronix.
	/// While this polling approach is not ideal it works around the crash described in 
	/// TE-6457.</remarks>
	/// ----------------------------------------------------------------------------------------
	public class LibronixPositionHandler : IDisposable, ILogosPositionHandler, ILogosPositionHandlerInternal
	{
		#region Member variables
		/// <summary>Reference counter for this instance. We have only one instance per link set.
		/// If the reference counter goes to 0 we delete the object.</summary>
		private volatile int m_ReferenceCount;
		/// <summary><c>true</c> if the object is (almost) deleted</summary>
		private volatile bool m_fDisposed;

		private LbxApplication m_LibronixApp;
		private LbxApplicationEventsBridge m_ApplicationEventsBridge;
		private int m_ApplicationEventsCookie;

		/// <summary>The link set which we want to use</summary>
		private volatile int m_LinkSet;
		/// <summary><c>true</c> if we are in the middle of sending or receiving a sync message</summary>
		private bool m_fProcessingMessage;

		private bool m_fLogosAppQuit;

		/// <summary>Timer used to poll Libronix for current references.</summary>
		protected Timer m_PollTimer;
		// Reference most recently retrieved from Libronix (-1 if none)
		private int m_BcvRef = -1;

		/// <summary>Fired when the scroll position of a resource is changed in Libronix</summary>
		/// <remarks>The bcvRef reference is in BBCCCVVV format. The book number
		/// is 1-39 for OT books and 40-66 for NT books. Apocrypha are not supported. The 
		/// conversion between Libronix and BBCCCVVV format doesn't consider any versification
		/// issues.</remarks>
		/// <developernote><see href="http://andyclymer.blogspot.com/2008/01/safe-event-snippet.html"/>
		/// and <see href="http://code.logos.com/blog/2008/03/assigning_to_c_events.html"/>
		/// why we assign a do-nothing event handler.</developernote>
		public event EventHandler<PositionChangedEventArgs> PositionChanged = delegate { };

		private event EventHandler<DisposedEventArgs> DisposedEvent;

		private PositionChangedEventArgs m_LastPositionArgs; // set by Libronix, used in onIdle
		#endregion

		#region Construction
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="LibronixPositionHandler"/> class.
		/// </summary>
		/// <param name="linkSet">The 0-based index of the link set in Libronix.</param>
		/// <param name="libronixApp">The Libronix application</param>
		/// ------------------------------------------------------------------------------------
		internal LibronixPositionHandler(int linkSet, LbxApplication libronixApp)
		{
			m_LinkSet = linkSet;
			Initialize(libronixApp);
		}

		#endregion

		#region Dispose related methods
#if DEBUG
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="LibronixPositionHandler"/> is reclaimed by garbage collection.
		/// </summary>
		/// <developernote>
		/// We don't implement a finalizer in production code. Finalizers should only be 
		/// implemented if we have to free unmanaged resources. We have COM objects, but those 
		/// are wrapped in a managed wrapper, so they aren't considered unmanaged here.
		/// <see href="http://code.logos.com/blog/2008/02/the_dispose_pattern.html"/>
		/// </developernote>
		/// ------------------------------------------------------------------------------------
		~LibronixPositionHandler()
		{
			Debug.Fail("Not disposed: " + GetType().Name);
		}
#endif

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting 
		/// unmanaged resources.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void Dispose()
		{
			if (((ILogosPositionHandlerInternal)this).Release() > 0)
			{
				Debug.WriteLine("Dispose called, but having still " + m_ReferenceCount + " references");
				return;
			}

			if (!m_fDisposed)
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="fDisposeManagedObjs"><c>true</c> to release both managed and unmanaged 
		/// resources; <c>false</c> to release only unmanaged resources.</param>
		/// 
		/// <developernote>
		/// We don't implement a finalizer. Finalizers should only be implemented if we have to 
		/// free unmanaged resources. We have COM objects, but those are wrapped in a managed 
		/// wrapper, so they aren't considered unmanaged here.
		/// <see href="http://code.logos.com/blog/2008/02/the_dispose_pattern.html"/>
		/// </developernote>
		/// ------------------------------------------------------------------------------------
		protected virtual void Dispose(bool fDisposeManagedObjs)
		{
			if (fDisposeManagedObjs)
			{
				Close();

				RaiseDisposedEvent();

				if (m_PollTimer != null)
				{
					m_PollTimer.Stop();
					m_PollTimer.Dispose();
				}
			}

			m_PollTimer = null;

			m_fDisposed = true;
		}

		private void RaiseDisposedEvent()
		{
			if (DisposedEvent != null)
				DisposedEvent(this, new DisposedEventArgs(m_LinkSet, m_fLogosAppQuit));
		}
		#endregion

		#region Private methods and properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initalizes the Libronix COM objects.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal void Initialize(LbxApplication libApp)
		{
			m_LibronixApp = libApp;
			if (m_LibronixApp == null)
				return;

			StartPollTimer();

			m_ApplicationEventsBridge = CreateLbxApplicationEventsBridge();
			m_ApplicationEventsBridge.EventFired += OnApplicationEventsBridgeEventFired;
			m_ApplicationEventsCookie = m_ApplicationEventsBridge.Connect(m_LibronixApp);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates the LbxApplicationEventsBridge object.
		/// </summary>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		protected virtual LbxApplicationEventsBridge CreateLbxApplicationEventsBridge()
		{
			return new LbxApplicationEventsBridgeClass();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Starts the timer that is used to poll Libronix for changing references.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected virtual void StartPollTimer()
		{
			if (m_PollTimer == null)
			{
				m_PollTimer = new Timer { Interval = 1000 };
				m_PollTimer.Tick += OnPollTimerTick;
				m_PollTimer.Start();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// See if we can get a current reference from some linked resource in Libronix.
		/// If so, and if it is different from our current one, raise the appropriate event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		protected void OnPollTimerTick(object sender, EventArgs args)
		{
			try
			{
				if (m_LibronixApp == null)
					return;
				var windows = m_LibronixApp.Application.Windows;
				foreach (LbxWindow window in windows)
				{
					if (window.Type == "resource")
					{
						var windowInfo = (LbxResourceWindowInfo)window.Info;
						if (windowInfo.ActiveDataType == "bible")
						{
							var bcvRef = ConvertToBcv(windowInfo.ActiveReference);
							if (bcvRef >= 0)
							{
								if (bcvRef != m_BcvRef)
								{
									m_BcvRef = bcvRef;
									RaisePositionChangedEvent(new PositionChangedEventArgs(bcvRef));
								}
								// It's very important that we return, whether or not the reference is different,
								// the first time we find a valid reference source. Otherwise, if Libronix is
								// displaying two relevant windows at different locations, TE starts oscillating,
								// since each tick one of them has the same reference as last time, so the OTHER
								// is selected as the one to switch to.
								return;
							}
						}
					}
				}
			}
			catch (COMException)
			{
				// We're getting a COMException if Libronix got closed. We try to reinitialize.
				Close();
			}
			catch (Exception e)
			{
				Debug.Fail("Got exception in OnPollTimerTick: " + e.Message);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Closes the connection to Libronix
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void Close()
		{
			if (m_PollTimer != null)
			{
				m_PollTimer.Stop();
				m_PollTimer.Dispose();
				m_PollTimer = null;
			}

			if (m_ApplicationEventsBridge != null)
			{
				try
				{
					m_ApplicationEventsBridge.Disconnect(m_LibronixApp, m_ApplicationEventsCookie);
				}
				catch
				{
					// ignore any errors
				}
				m_ApplicationEventsBridge.EventFired -= OnApplicationEventsBridgeEventFired;
			}
			m_ApplicationEventsBridge = null;

			m_LibronixApp = null;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Converts a reference in Libronix format to BBCCCVVV format.
		/// </summary>
		/// <param name="reference">The reference in Libronix format: bible.1.1.3</param>
		/// <returns>BBCCCVVV reference, or -1 if invalid reference</returns>
		/// <developernote>We could use ScrReference to do parts of this; however, the idea is 
		/// to have as few as possible dependencies on LibronixLinker project so that it can 
		/// easily be used in other projects. ScrReference has several dependencies as well as 
		/// requirements for versification files etc. </developernote>
		/// ------------------------------------------------------------------------------------
		internal static int ConvertToBcv(string reference)
		{
			if (reference == null)
				return -1;

			string[] parts = reference.Split('.');
			if (parts[0] != "bible")
				return -1;

			int book = Convert.ToInt32(parts[1]);
			int chapter = Convert.ToInt32(parts[2]);
			int verse = Convert.ToInt32(parts[3]);

			if ((book > 39 && book <= 60) || book > 87)
				return -1; // can't handle apocrypha

			// Translate Libronix book number to BBCCCVVV format:
			// Libronix: 1-39: OT; 61-87 NT
			// BBCCCVVV: 1-39: OT; 40-66 NT
			// Currently we don't support apocrypha. If we would, they would get 67-87/92/99.
			// However, we would have to figure out the mapping between Libronix and
			// Paratext because Paratext has more books in the apocrypha than Libronix.
			// See TE-3996 for the mapping in Paratext.
			if (book > 39)
				book -= 21;
			if (chapter > 1 && verse == 0)
				verse = 1;
			if (chapter == 0 && verse == 0)
				chapter = 1;

			return book % 100 * 1000000 + chapter % 1000 * 1000 + verse % 1000;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Build the Libronix reference string
		/// </summary>
		/// <param name="bcvRef">The BCV reference.</param>
		/// <returns>The reference string as expected by Libronix</returns>
		/// <developernote>We could use ScrReference to do parts of this; however, the idea is 
		/// to have as few as possible dependencies on LibronixLinker project so that it can 
		/// easily be used in other projects. ScrReference has several dependencies as well as 
		/// requirements for versification files etc. </developernote>
		/// ------------------------------------------------------------------------------------
		internal static string ConvertFromBcv(int bcvRef)
		{
			// REVIEW: we don't check that BCV reference is valid. Should we?
			int bookNum = bcvRef / 1000000 % 100;
			if (bookNum > 39)
				bookNum += 21; // NT shifted up 21

			int chapter = bcvRef / 1000 % 1000;
			int verse = bcvRef % 1000;

			return string.Format("bible.{0}.{1}.{2}", bookNum, chapter, verse);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Called when the position changed.
		/// </summary>
		/// <param name="e">The <see cref="SIL.Utils.PositionChangedEventArgs"/>
		/// instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		protected virtual void RaisePositionChangedEvent(PositionChangedEventArgs e)
		{
			// Needs to return fast, so hold off actually moving until idle.
			m_LastPositionArgs = e;
			Application.Idle += OnApplicationIdle;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Called when the application is idle.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void OnApplicationIdle(object sender, EventArgs e)
		{
			if (m_LastPositionArgs != null && PositionChanged != null)
				PositionChanged(this, m_LastPositionArgs);
			Application.Idle -= OnApplicationIdle;
			m_LastPositionArgs = null;
		}
		#endregion

		#region Event handler
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Called when the application events bridge event is fired. We use this event to know
		/// when a window gets opened or closed. In that case we want to refresh our 
		/// subscription to the position changed event so that we get notified when the scroll
		/// position of that window changes.
		/// </summary>
		/// <param name="eventString">The event string.</param>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		private string OnApplicationEventsBridgeEventFired(string eventString, string data)
		{
			if (m_fProcessingMessage)
				return string.Empty;
			m_fProcessingMessage = true;
			try
			{
				if (eventString == "ApplicationClosing")
				{
					// Libronix is closing - stop using it, but start checking for new instance
					Close();
					m_fLogosAppQuit = true;
					Dispose();
				}
			}
			catch (Exception e)
			{
				Debug.Fail("Got exception in OnApplicationEventsBridgeEventFired: " + e.Message);
			}
			finally
			{
				m_fProcessingMessage = false;
			}
			return string.Empty;
		}

		#endregion

		#region Public methods and properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the available link sets.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public List<string> AvailableLinkSets
		{
			get
			{
				var linkSets = new List<string>();
				if (m_LibronixApp != null)
				{
					// Add all Libronix link sets to the list
					linkSets.AddRange(from LbxWindowLinkSet linkSet in m_LibronixApp.WindowLinkSets
									  select linkSet.Title.Replace("&", ""));
				}
				return linkSets;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Refreshes the list of current window
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void Refresh()
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Refreshes the specified link set.
		/// </summary>
		/// <param name="linkSet">The link set.</param>
		/// ------------------------------------------------------------------------------------
		public void Refresh(int linkSet)
		{
			m_LinkSet = linkSet;
			Refresh();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Sends the scroll synchronization message to Libronix for all resource windows in
		/// the current link set.
		/// </summary>
		/// <param name="bcvRef">The reference in BBCCCVVV format.</param>
		/// ------------------------------------------------------------------------------------
		public void SetReference(int bcvRef)
		{
			if (m_fProcessingMessage)
				return;

			m_fProcessingMessage = true;
			try
			{
				LbxResourceWindowInfo info = null;
				string reference = null;
				if (m_LibronixApp == null)
					return;

				var linkSet = m_LibronixApp.WindowLinkSets[m_LinkSet];
				var windows = m_LibronixApp.Application.Windows;
				foreach (LbxWindow window in windows)
				{
					if (window.Type == "resource" && linkSet.Windows.Find(window) >= 0)
					{
						info = (LbxResourceWindowInfo)window.Info;
						if (info.ActiveDataType == "bible")
						{
							reference = ConvertFromBcv(bcvRef);
							break; // don't go on, the last info found may NOT have the right type.
						}
					}
				}
				// don't lock here since calling GoToReference causes a callback from Libronix
				if (info != null && reference != null)
					info.GoToReference(reference, 0, int.MaxValue, string.Empty, null);
			}
			catch (Exception e)
			{
				// just ignore any errors
				Debug.Fail("Got exception in SetLibronixFocus: " + e.Message);
			}
			finally
			{
				m_fProcessingMessage = false;
			}
		}
		#endregion

		#region IRefCount Members

		void ILogosPositionHandlerInternal.AddRef()
		{
			m_ReferenceCount++;
		}

		int ILogosPositionHandlerInternal.Release()
		{
			m_ReferenceCount--;
			return m_ReferenceCount;
		}

		/// <summary>
		/// Fired when the object gets disposed.
		/// </summary>
		event EventHandler<DisposedEventArgs> ILogosPositionHandlerInternal.Disposed
		{
			add { DisposedEvent += value; }
			remove { DisposedEvent -= value; }
		}

		#endregion
	}
}
