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
using ResourceDriver;

namespace SIL.FieldWorks.TE.LibronixLinker
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
	/// Class that receives position changed events from Libronix
	/// </summary>
	/// <remarks>There is only one instance of this class per link set.</remarks>
	/// 
	/// <developernote>
	/// <para>The following comment and some of the code is obsolete. We had some race conditions
	/// and found an alternative way to return quickly (see Application_Idle and caller) without using
	/// threads.</para>
	/// <para>We create a separate thread for connecting to Libronix. The reason is
	/// that we want to return ASAP when we receive a PositionChanged notification from 
	/// Libronix otherwise things get messed up in Libronix. When we handle PositionChanged 
	/// event, we go to a different verse which can take quite a while. When we receive the 
	/// notifications on a separate thread (and then deal with them asynchronously in the main
	/// thread) we can return much faster.</para>
	/// <para>Most methods of this class is executed on a separate thread (LibronixSync). The 
	/// public methods are called from the main thread and thus are executed on the main thread. 
	/// The Libronix thread basically just creates the Libronix COM objects and then starts a 
	/// message loop and waits for any events that Libronix might send.
	/// Any public method and any method that can be called from Libronix (i.e. any event 
	/// handling method) needs to synchronize the access to member variables, i.e. needs to
	/// use lock statements!</para>
	/// </developernote>
	/// ----------------------------------------------------------------------------------------
	public class LibronixPositionHandler: IDisposable, ILogosPositionHandler, ILogosPositionHandlerInternal
	{
		#region LinkInfo struct
		/// <summary></summary>
		private struct LinkInfo
		{
			/// <summary>The resource window info in Libronix</summary>
			public readonly LbxResourceWindowInfo WindowInfo;
			/// <summary>A cookie that identifies the connection</summary>
			public readonly int Cookie;
			/// <summary>Positions list in Libronix</summary>
			public readonly LbxResourcePositions Positions;

			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Initializes a new instance of the LinkInfo struct.
			/// </summary>
			/// <param name="windowInfo">The window info.</param>
			/// <param name="cookie">The cookie.</param>
			/// <param name="positions">The positions.</param>
			/// --------------------------------------------------------------------------------
			public LinkInfo(LbxResourceWindowInfo windowInfo, int cookie,
				LbxResourcePositions positions)
			{
				WindowInfo = windowInfo;
				Cookie = cookie;
				Positions = positions;
			}
		}
		#endregion

		///// <summary>Represents the method that will raise the PositionChanged event</summary>
		///// <param name="e">Event arguments.</param>
		//private delegate void PositionChangedDelegate(PositionChangedEventArgs e);

		#region Member variables
		/// <summary>Reference counter for this instance. We have only one instance per link set.
		/// If the reference counter goes to 0 we delete the object.</summary>
		private volatile int m_ReferenceCount;
		/// <summary><c>true</c> if the object is (almost) deleted</summary>
		private volatile bool m_fDisposed;
		/// <summary>Control used to marshal between threads. This control gets created on the
		/// main thread.</summary>
		private Control m_MarshalingControl;
		/// <summary>Used for locking</summary>
		/// <remarks>Note: we can't use lock(this) (or equivalent) since we use lock(this)
		/// in the Dispose(bool) method which might result in dead locks!
		/// </remarks>
		private readonly object m_Synchronizer = new object();

		private LbxApplication m_libronixApp;
		private LbxResourcePositionChangedBridge m_PositionChangedBridge;
		private List<LinkInfo> m_cookies = new List<LinkInfo>();
		private LbxApplicationEventsBridge m_ApplicationEventsBridge;
		private int m_ApplicationEventsCookie;

		/// <summary>The link set which we want to use</summary>
		private volatile int m_LinkSet;
		/// <summary><c>true</c> if we are in the middle of sending or receiving a sync message</summary>
		private bool m_fProcessingMessage;

		private bool m_fLogosAppQuit;

		/// <summary>Timer used to poll Libronix for current references.</summary>
		protected Timer m_pollTimer;
		// Reference most recently retrieved from Libronix (-1 if none)
		private int m_bcvRef = -1;

		/// <summary>Fired when the scroll position of a resource is changed in Libronix</summary>
		/// <remarks>The BcvRef reference is in BBCCCVVV format. The book number
		/// is 1-39 for OT books and 40-66 for NT books. Apocrypha are not supported. The 
		/// conversion between Libronix and BBCCCVVV format doesn't consider any versification
		/// issues.</remarks>
		/// <developernote><see href="http://andyclymer.blogspot.com/2008/01/safe-event-snippet.html"/>
		/// and <see href="http://code.logos.com/blog/2008/03/assigning_to_c_events.html"/>
		/// why we assign a do-nothing event handler.</developernote>
		public event EventHandler<PositionChangedEventArgs> PositionChanged = delegate { };

		private event EventHandler<DisposedEventArgs> DisposedEvent;

		private PositionChangedEventArgs m_lastPositionArgs; // set by Libronix, used in onIdle
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
			m_MarshalingControl = new Control();
			m_MarshalingControl.CreateControl();

			Initialize(libronixApp);
		}

		#endregion

		#region Dispose related methods
		#if DEBUG
		internal int m_instance;
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
			Debug.Fail("Not disposed: " + GetType().Name + "; instance: " + m_instance);
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
				// we do lock(this) because we want to prevent concurrent Dispose calls,
				// but we don't want to interfere with any other methods because that might
				// cause dead-locks - Dispose might be called on a separate thread.
				lock (this)
				{
					if (!m_fDisposed)
					{
						Dispose(true);

						GC.SuppressFinalize(this);
					}
				}
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
				Debug.WriteLine("Disposing: " + m_instance);
				Close();

				RaiseDisposedEvent();

				if (m_pollTimer != null)
				{
					m_pollTimer.Stop();
					m_pollTimer.Dispose();
				}

				if (m_MarshalingControl != null)
					m_MarshalingControl.Dispose();
			}

			m_cookies = null;
			m_pollTimer = null;
			m_MarshalingControl = null;

			m_fDisposed = true;
		}

		private void RaiseDisposedEvent()
		{
			if (m_MarshalingControl.InvokeRequired)
				m_MarshalingControl.BeginInvoke((MethodInvoker)RaiseDisposedEvent);
			else if (DisposedEvent != null)
				DisposedEvent(this, new DisposedEventArgs(m_LinkSet, m_fLogosAppQuit));
		}
		#endregion

		#region Private methods and properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the Libronix COM objects.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal void Initialize(LbxApplication libApp)
		{
			m_libronixApp = libApp;
			if (m_libronixApp == null)
				return;

			m_PositionChangedBridge = null;
			StartPollTimer();

			m_ApplicationEventsBridge = CreateLbxApplicationEventsBridge();
			m_ApplicationEventsBridge.EventFired += OnApplicationEventsBridgeEventFired;
			m_ApplicationEventsCookie = m_ApplicationEventsBridge.Connect(m_libronixApp);

			SetupPositionEvents();
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
			if (m_pollTimer == null)
			{
				m_pollTimer = new Timer {Interval = 1000};
				m_pollTimer.Tick += OnTick;
				m_pollTimer.Start();
			}
		}

		/// <summary>
		/// See if we can get a current reference from some linked resource in Libronix.
		/// If so, and if it is different from our current one, raise the appropriate event.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="sender"></param>
		protected void OnTick(object sender, EventArgs args)
		{
			try
			{
				lock (m_Synchronizer)
				{
					if (m_libronixApp == null)
						return;
					LbxWindows windows = m_libronixApp.Application.Windows;
					foreach (LbxWindow window in windows)
					{
						if (window.Type == "resource")
						{
							var windowInfo = (LbxResourceWindowInfo)window.Info;
							if (windowInfo.ActiveDataType == "bible")
							{
								int bcvRef = ConvertToBcv(windowInfo.ActiveReference);
								if (bcvRef >= 0)
								{
									if (bcvRef != m_bcvRef)
									{
										m_bcvRef = bcvRef;
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
			}
			catch (COMException)
			{
				// We're getting a COMException if Libronix got closed. We try to reinitialize.
				Close();
				// TODO: Initialize();
			}
			catch (Exception e)
			{
				Debug.Fail("Got exception in OnTick: " + e.Message);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Closes the connection to Libronix
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void Close()
		{
			lock (this)
			{
				if (m_PositionChangedBridge != null)
				{
					if (m_cookies != null)
						RemovePositionEvents();
					m_PositionChangedBridge.PositionChanged -= OnPositionChangedInLibronix;
				}
				m_PositionChangedBridge = null;
				if (m_cookies != null) 
					m_cookies.Clear();
				if (m_pollTimer != null)
				{
					m_pollTimer.Stop();
					m_pollTimer.Dispose();
					m_pollTimer = null;
				}

				if (m_ApplicationEventsBridge != null)
				{
					m_ApplicationEventsBridge.Disconnect(m_libronixApp, m_ApplicationEventsCookie);
					m_ApplicationEventsBridge.EventFired -= OnApplicationEventsBridgeEventFired;
				}
				m_ApplicationEventsBridge = null;

				m_libronixApp = null;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Subscribes to the Position changed event for all windows in current link set.
		/// </summary>
		/// <remarks>NOTE: subscribing to all windows in the link set fires the position changed 
		/// event multiple times. However, if we don't do it and the first window happens not to
		/// have the same range as the other resources, then we don't get event.</remarks>
		/// ------------------------------------------------------------------------------------
		private void SetupPositionEvents()
		{
			m_cookies.Clear();
			try
			{
				if (m_libronixApp == null || m_PositionChangedBridge == null)
					return;

				LbxWindows windows = m_libronixApp.Application.Windows;
				foreach (LbxWindow window in windows)
				{
					if (window.Type == "resource")
					{
						var windowInfo = (LbxResourceWindowInfo)window.Info;
						if (windowInfo.ActiveDataType == "bible")
						{
							int cookie = m_PositionChangedBridge.Connect(windowInfo);
							var view = (LbxResourceView)window.View;
							var resource = (LbxResource)view.Resource;
							var positions = (LbxResourcePositions)resource.get_Positions(null);
							m_cookies.Add(new LinkInfo(windowInfo, cookie, positions));
						}
					}
				}
			}
			catch (COMException e)
			{
				m_libronixApp = null;
				Debug.Fail("Got exception in LibronixPositionHandler.SetupPositionEvents: " + e.Message);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Helper method to disconnect from Libronix.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void RemovePositionEvents()
		{
			foreach (LinkInfo info in m_cookies)
				m_PositionChangedBridge.Disconnect(info.WindowInfo, info.Cookie);
			m_cookies.Clear();
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
		/// <param name="e">The <see cref="SIL.FieldWorks.TE.LibronixLinker.PositionChangedEventArgs"/>
		/// instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		protected virtual void RaisePositionChangedEvent(PositionChangedEventArgs e)
		{
			// Needs to return fast, so hold off actually moving until idle.
			m_lastPositionArgs = e;
			Application.Idle += Application_Idle;
			//if (m_MarshalingControl.InvokeRequired)
			//{
			//    m_MarshalingControl.BeginInvoke(new PositionChangedDelegate(RaisePositionChangedEvent), 
			//        e);
			//}
			//else
			//    PositionChanged(this, e);
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			if (m_lastPositionArgs != null && PositionChanged != null)
				PositionChanged(this, m_lastPositionArgs);
			Application.Idle -= Application_Idle;
			m_lastPositionArgs = null;
		}
		#endregion

		#region Event handler
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Called when the position in Libronix changed.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="reference">The reference.</param>
		/// <param name="significance">The significance.</param>
		/// <param name="navigationId">The navigation ID.</param>
		/// ------------------------------------------------------------------------------------
		private void OnPositionChangedInLibronix(string position, string reference, int significance,
			string navigationId)
		{
			if (m_fProcessingMessage)
				return;
			m_fProcessingMessage = true;
			try
			{
				int bcvRef = -1;
				lock (m_Synchronizer)
				{
					if (m_libronixApp == null)
						return;

					int i = 0;
					for (; i < m_cookies.Count; i++)
					{
						// Find the resource window that can display the current position.
						if (m_cookies[i].Positions.IsValid(position))
							break;
					}
					Debug.Assert(i < m_cookies.Count,
						"Didn't find any Libronix resource that is able to show current position");

					LbxWindowLinkSet linkSet = m_libronixApp.WindowLinkSets[m_LinkSet];
					if (i < m_cookies.Count && linkSet.Windows.Find(m_cookies[i].WindowInfo.Parent) >= 0)
					{
						bcvRef = ConvertToBcv(m_cookies[i].WindowInfo.ActiveReference);
						Debug.WriteLineIf(bcvRef < 0, "Got reference that couldn't be converted: " +
							m_cookies[i].WindowInfo.ActiveReference);
					}
				}
				if (bcvRef > -1)
					RaisePositionChangedEvent(new PositionChangedEventArgs(bcvRef));
			}
			catch (Exception e)
			{
				Debug.Fail("Got exception in OnPositionChanged: " + e.Message);
			}
			finally
			{
				m_fProcessingMessage = false;
			}
		}

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
				// NOTE: locking is done inside of Refresh method
				if (eventString == "WindowOpened" || eventString == "WindowClosed")
				{
					Refresh();
				}
				else if (eventString == "ApplicationClosing")
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

		///// ------------------------------------------------------------------------------------
		///// <summary>
		///// Called when a thread is about to exit.
		///// </summary>
		///// <param name="sender">The sender.</param>
		///// <param name="e">The <see cref="System.EventArgs"/> instance containing the event 
		///// data.</param>
		///// ------------------------------------------------------------------------------------
		//private void OnThreadExit(object sender, EventArgs e)
		//{
		//    if (Thread.CurrentThread.Name == "LibronixSync")
		//    {
		//        Close();
		//    }
		//}

		#endregion

		#region Public methods and properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the available link sets.
		/// </summary>
		/// <developernote>This method is called on the main thread</developernote>
		/// ------------------------------------------------------------------------------------
		public List<string> AvailableLinkSets
		{
			get
			{
				var linkSets = new List<string>();

				lock (m_Synchronizer)
				{
					if (m_libronixApp != null)
					{
						// Add all Libronix link sets to the list
						linkSets.AddRange(from LbxWindowLinkSet linkSet in m_libronixApp.WindowLinkSets
						                  select linkSet.Title.Replace("&", ""));
					}
				}

				return linkSets;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Refreshes the list of current window
		/// </summary>
		/// <developernote>This method is called on the main thread</developernote>
		/// ------------------------------------------------------------------------------------
		public void Refresh()
		{
			lock (m_Synchronizer)
			{
				// Remove the old events
				RemovePositionEvents();
				SetupPositionEvents();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Refreshes the specified link set.
		/// </summary>
		/// <param name="linkSet">The link set.</param>
		/// <developernote>This method is called on the main thread</developernote>
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
		/// <developernote>This method is called on the main thread</developernote>
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
				lock (m_Synchronizer)
				{
					if (m_libronixApp == null)
						return;

					LbxWindowLinkSet linkSet = m_libronixApp.WindowLinkSets[m_LinkSet];
					LbxWindows windows = m_libronixApp.Application.Windows;
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

/*
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Starts this thread.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <developernote>This method is called on the Libronix thread</developernote>
		/// ------------------------------------------------------------------------------------
		private void Start(object data)
		{
			// The main thread that created us is in a Wait state at this point, so we don't 
			// have to lock here. Not doing it helps in the case where we run into a timeout
			// because creating the timer didn't complete.
			Initalize();

			// Signal the main thread that we're ready
			EventWaitHandle waitHandle = data as EventWaitHandle;
			if (waitHandle != null)
				waitHandle.Set();

			// Start a message loop and wait for any event that Libronix sends us
			Application.Run();
		}
*/

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
