using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Logos4Lib;

namespace SIL.FieldWorks.TE.LibronixLinker
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Class that receives position changed events from Logos 4
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class Logos4PositionHandler: IDisposable, ILogosPositionHandler, ILogosPositionHandlerInternal
	{
		#region Member variables
		/// <summary>Fired when the scroll position of a resource is changed in Logos 4</summary>
		/// <remarks>The BcvRef reference is in BBCCCVVV format. The book number
		/// is 1-39 for OT books and 40-66 for NT books. Apocrypha are not supported. The 
		/// conversion between Libronix and BBCCCVVV format doesn't consider any versification
		/// issues.</remarks>
		/// <developernote><see href="http://andyclymer.blogspot.com/2008/01/safe-event-snippet.html"/>
		/// and <see href="http://code.logos.com/blog/2008/03/assigning_to_c_events.html"/>
		/// why we assign a do-nothing event handler.</developernote>
		public event EventHandler<PositionChangedEventArgs> PositionChanged = delegate { };

		private bool m_fDisposed;
		/// <summary>Reference counter for this instance. We have only one instance per link set.
		/// If the reference counter goes to 0 we delete the object.</summary>
		private volatile int m_ReferenceCount;
		private event EventHandler<DisposedEventArgs> DisposedEvent;
		private LogosApplication m_logosApp;
		/// <summary>The link set (1-based)</summary>
		private int m_linkSet;
		private bool m_fLogosAppQuit;
		/// <summary>Control used to marshal between threads. This control gets created on the
		/// main thread.</summary>
		private Control m_MarshalingControl;

		private bool m_fInProgress;
		private bool m_fSkipNextLogosChange;
		/// <summary></summary>
		protected static Logos4BibleBooks s_Books;
		// Reference most recently retrieved from Libronix (-1 if none)
		private int m_bcvRef = -1;
		#endregion

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="Logos4PositionHandler"/> class.
		/// </summary>
		/// <param name="linkSet">The link set (0-based).</param>
		/// <param name="logosApp">The Logos app.</param>
		/// ------------------------------------------------------------------------------------
		public Logos4PositionHandler(int linkSet, LogosApplication logosApp)
		{
			m_linkSet = linkSet + 1;
			m_MarshalingControl = new Control();
			m_MarshalingControl.CreateControl();
			Initialize(logosApp);
		}

		private void Initialize(LogosApplication logosApplication)
		{
			m_logosApp = logosApplication;
			if (m_logosApp == null)
				return;

			m_logosApp.Exiting += OnLogosExiting;
			m_logosApp.PanelChanged += OnPanelChanged;
		}

		#region Dispose related methods
#if DEBUG
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Logos4PositionHandler"/> is reclaimed by garbage collection.
		/// </summary>
		/// <developernote>
		/// We don't implement a finalizer in production code. Finalizers should only be 
		/// implemented if we have to free unmanaged resources. We have COM objects, but those 
		/// are wrapped in a managed wrapper, so they aren't considered unmanaged here.
		/// <see href="http://code.logos.com/blog/2008/02/the_dispose_pattern.html"/>
		/// </developernote>
		/// ------------------------------------------------------------------------------------
		~Logos4PositionHandler()
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
				return;

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
				RaiseDisposedEvent();

				// needs to come last
				if (m_MarshalingControl != null)
					m_MarshalingControl.Dispose();
			}

			m_MarshalingControl = null;
			m_fDisposed = true;
		}

		#endregion

		#region Event handlers
		private void OnLogosExiting()
		{
			m_fLogosAppQuit = true;
			Dispose();
		}

		private delegate void PanelChangedDelegate(object objPanel, object reserved);

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Called when the panel in Logos 4 changed
		/// </summary>
		/// <remarks>Needs to run on the main thread</remarks>
		/// ------------------------------------------------------------------------------------
		protected void OnPanelChanged(object objPanel, object reserved)
		{
			if (m_MarshalingControl.InvokeRequired)
				m_MarshalingControl.BeginInvoke((PanelChangedDelegate)OnPanelChanged, objPanel, reserved);
			else
			{
				if (m_fInProgress)
					return;


				m_fInProgress = true;
				try
				{
					// Check wether the panel is in the correct link set
					var panel = (LogosPanel)objPanel;
					if (!panel.LinkSet.StartsWith("Index:") || panel.LinkSet.Substring(6) != m_linkSet.ToString())
						return;

					if (panel.Title != m_logosApp.GetActivePanel().Title)
						return;

					var references = panel.GetCurrentReferencesAndHeadwords();
					var dataTypeReference = references[0].Reference;
					if (m_logosApp.ApiVersion < 2 || dataTypeReference.DetailsKind == "Bible")
					{
						var refDetails = dataTypeReference.Details as LogosBibleReferenceDetails;
						var bcvRef = ConvertToBcv(refDetails);
						if (bcvRef > -1 && bcvRef != m_bcvRef && !m_fSkipNextLogosChange)
							RaisePositionChangedEvent(this, new PositionChangedEventArgs(bcvRef));
						m_bcvRef = bcvRef;
						m_fSkipNextLogosChange = false;
					}
				}
				finally
				{
					m_fInProgress = false;
				}
			}
		}

		#endregion

		#region Private methods and properties
		private void RaiseDisposedEvent()
		{
			if (m_MarshalingControl.InvokeRequired)
				m_MarshalingControl.BeginInvoke((MethodInvoker)RaiseDisposedEvent);
			else if (DisposedEvent != null)
				DisposedEvent(this, new DisposedEventArgs(m_linkSet - 1, m_fLogosAppQuit));
		}

		private void RaisePositionChangedEvent(object sender, PositionChangedEventArgs e)
		{
			if (m_MarshalingControl.InvokeRequired)
				m_MarshalingControl.BeginInvoke(
					(EventHandler<PositionChangedEventArgs>)RaisePositionChangedEvent, sender, e);
			else if (PositionChanged != null)
				PositionChanged(sender, e);
		}

		private Logos4BibleBooks Books
		{
			get { return s_Books ?? (s_Books = new Logos4BibleBooks(m_logosApp)); }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Converts a Logos4 reference object to BBCCCVVV format.
		/// </summary>
		/// <param name="reference">The Logos4 Bible reference object</param>
		/// <returns>BBCCCVVV reference, or -1 if invalid reference</returns>
		/// <developernote>We could use ScrReference to do parts of this; however, the idea is 
		/// to have as few as possible dependencies on LibronixLinker project so that it can 
		/// easily be used in other projects. ScrReference has several dependencies as well as 
		/// requirements for versification files etc. </developernote>
		/// ------------------------------------------------------------------------------------
		internal int ConvertToBcv(LogosBibleReferenceDetails reference)
		{
			if (reference == null)
				return -1;

			var bookId = Books[reference.Book].BookId;
			int chapter, verse;
			int.TryParse(reference.Chapter, out chapter);
			if (!int.TryParse(reference.Verse, out verse))
			{
				var lastIndex = reference.Verse.LastIndexOfAny(new[]
				                                               	{
				                                               		'0',
				                                               		'1',
				                                               		'2',
				                                               		'3',
				                                               		'4',
				                                               		'5',
				                                               		'6',
				                                               		'7',
				                                               		'8',
				                                               		'9'
				                                               	}
					);
				if (lastIndex >= 0 && lastIndex < reference.Verse.Length)
					int.TryParse(reference.Verse.Substring(0, lastIndex + 1), out verse);
				else
					verse = 0;
			}

			if ((bookId > 39 && bookId <= 60) || bookId > 87)
				return -1; // can't handle apocrypha

			// Translate Logos4 book number to BBCCCVVV format:
			// Logos4: 1-39: OT; 61-87 NT
			// BBCCCVVV: 1-39: OT; 40-66 NT
			// Currently we don't support apocrypha. If we would, they would get 67-87/92/99.
			// However, we would have to figure out the mapping between Logos4 and
			// Paratext because Paratext has more books in the apocrypha than Logos4.
			// See TE-3996 for the mapping in Paratext.
			if (bookId > 39)
				bookId -= 21;
			if (chapter > 1 && verse == 0)
				verse = 1;
			if (chapter == 0 && verse == 0)
				chapter = 1;

			return bookId % 100 * 1000000 + chapter % 1000 * 1000 + verse % 1000;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Build the Logos 4 reference string
		/// </summary>
		/// <param name="bcvRef">The BCV reference.</param>
		/// <returns>The reference string as expected by Logos 4</returns>
		/// ------------------------------------------------------------------------------------
		internal string ConvertFromBcv(int bcvRef)
		{
			// REVIEW: we don't check that BCV reference is valid. Should we?
			var bookNum = bcvRef / 1000000 % 100;
			if (bookNum > 39)
				bookNum += 21; // NT shifted up 21

			var chapter = bcvRef / 1000 % 1000;
			var verse = bcvRef % 1000;

			return string.Format("{0}.{1}.{2}", Books[bookNum].Abbrev, chapter, verse);
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
				return new List<string>
				       	{
				       		"Link set A",
				       		"Link set B",
				       		"Link set C",
				       		"Link set D",
				       		"Link set E",
				       		"Link set F"
				       	};
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
		/// <param name="linkSet">The link set (0-based).</param>
		/// ------------------------------------------------------------------------------------
		public void Refresh(int linkSet)
		{
			m_linkSet = linkSet + 1;
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
			if (m_fInProgress)
				return;

			m_fInProgress = true;
			try
			{
				// try to find first reference in desired link group
				var openPanels = m_logosApp.GetOpenPanels();
				LogosPanel panel = null;
				for (var i = 0; i < openPanels.Count; i++)
				{
					if (openPanels[i].LinkSet == string.Format("Index:{0}", m_linkSet))
					{
						panel = openPanels[i];
						break;
					}
				}
				if (panel == null)
					return;

				var currentReferences = panel.GetCurrentReferencesAndHeadwords();
				if (currentReferences.Count < 1 || currentReferences[0].Reference == null ||
					currentReferences[0].Reference.DataType == null)
					return;

				var navRequest = m_logosApp.CreateNavigationRequest();
				navRequest.Reference = currentReferences[0].Reference.DataType.ParseReference(ConvertFromBcv(bcvRef));
				panel.Navigate(navRequest);
				m_fSkipNextLogosChange = true;
			}
			finally
			{
				m_fInProgress = false;
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