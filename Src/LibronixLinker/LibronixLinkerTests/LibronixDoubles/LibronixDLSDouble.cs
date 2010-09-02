#if !__MonoCS__
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using LibronixDLS;
using stdole;
using Utility;

namespace SIL.FieldWorks.TE.LibronixLinker
{
	// not working on Linux because Libronix interop DLLs have a reference to stdole.dll
	// which doesn't exist on Linux. Substituting IPictureDisp with our own definition
	// doesn't work. It wouldn't accept our IPictureDisp as return parameter for
	// LbxWindowLinkSetDouble.Image.get

	#region LbxApplicationDouble
	internal class LbxApplicationDouble: LbxApplication
	{
		private readonly LbxWindowsDouble m_Windows = new LbxWindowsDouble();

		#region DILbxApplication6 Members

		public DILbxCommandBars ActionCommandBars
		{
			get { throw new NotImplementedException(); }
		}

		void DILbxApplication.StopActiveJob(UtilJobStatus Status)
		{
			StopActiveJob(Status);
		}

		void DILbxApplication2.StopActiveJob(UtilJobStatus Status)
		{
			StopActiveJob(Status);
		}

		void DILbxApplication3.StopActiveJob(UtilJobStatus Status)
		{
			StopActiveJob(Status);
		}

		void DILbxApplication4.StopActiveJob(UtilJobStatus Status)
		{
			StopActiveJob(Status);
		}

		void DILbxApplication5.StopActiveJob(UtilJobStatus Status)
		{
			StopActiveJob(Status);
		}

		void DILbxApplication6.StopActiveJob(UtilJobStatus Status)
		{
			StopActiveJob(Status);
		}

		public void Activate()
		{
			throw new NotImplementedException();
		}

		public bool Active
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxPopupWindow ActiveDialogWindow
		{
			get { throw new NotImplementedException(); }
		}

		public LbxApplicationEvent ActiveEvent
		{
			get { throw new NotImplementedException(); }
		}

		public Utility.LbxJobCallback ActiveJobCallback
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxPopupWindow ActivePopupWindow
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxWindow ActiveWindow
		{
			get { throw new NotImplementedException(); }
		}

		public LbxApplicationAddins Addins
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxQueryParser AdvancedQueryParser
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxAnnotationsCollection AnnotationsCollection
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxApplication Application
		{
			get { return this; }
		}

		public int CallSoon(object Callback, int Priority)
		{
			throw new NotImplementedException();
		}

		public void CancelCallSoon(int Cookie)
		{
			throw new NotImplementedException();
		}

		public string Caption
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public DILbxCitationFormats CitationFormats
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxCommandBars CommandBars
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxCompanies Companies
		{
			get { throw new NotImplementedException(); }
		}

		public object Constants
		{
			get { throw new NotImplementedException(); }
		}

		LbxJobCallback DILbxApplication6.ActiveJobCallback
		{
			get { return ActiveJobCallback; }
		}

		LbxJobCallback DILbxApplication5.ActiveJobCallback
		{
			get { return ActiveJobCallback; }
		}

		LbxJobCallback DILbxApplication4.ActiveJobCallback
		{
			get { return ActiveJobCallback; }
		}

		LbxJobCallback DILbxApplication3.ActiveJobCallback
		{
			get { return ActiveJobCallback; }
		}

		LbxJobCallback DILbxApplication2.ActiveJobCallback
		{
			get { return ActiveJobCallback; }
		}

		LbxJobCallback DILbxApplication.ActiveJobCallback
		{
			get { return ActiveJobCallback; }
		}

		public Utility.LbxJobCallback CreateJobCallback(string Name)
		{
			throw new NotImplementedException();
		}

		public DILbxSearchAgent CreateSearchAgent()
		{
			throw new NotImplementedException();
		}

		public DILbxDataFileManager DataFileManager
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxDataTypeManager DataTypeManager
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxDataTypeOptions DataTypeOptions
		{
			get { throw new NotImplementedException(); }
		}

		public string DescribeCommand(string Command)
		{
			throw new NotImplementedException();
		}

		public LbxDialogBars DialogBars
		{
			get { throw new NotImplementedException(); }
		}

		public LbxDialogs Dialogs
		{
			get { throw new NotImplementedException(); }
		}

		public LbxDocumentTypes DocumentTypes
		{
			get { throw new NotImplementedException(); }
		}

		public void ExecuteCommand(string Command, object JobCallback)
		{
			throw new NotImplementedException();
		}

		public string FireEvent(string Event, string Data)
		{
			throw new NotImplementedException();
		}

		LbxJobCallback DILbxApplication6.CreateJobCallback(string Name)
		{
			return CreateJobCallback(Name);
		}

		LbxJobCallback DILbxApplication5.CreateJobCallback(string Name)
		{
			return CreateJobCallback(Name);
		}

		LbxJobCallback DILbxApplication4.CreateJobCallback(string Name)
		{
			return CreateJobCallback(Name);
		}

		LbxJobCallback DILbxApplication3.CreateJobCallback(string Name)
		{
			return CreateJobCallback(Name);
		}

		LbxJobCallback DILbxApplication2.CreateJobCallback(string Name)
		{
			return CreateJobCallback(Name);
		}

		LbxJobCallback DILbxApplication.CreateJobCallback(string Name)
		{
			return CreateJobCallback(Name);
		}

		public DILbxResourceVisualFilters GlobalVisualFilters
		{
			get { throw new NotImplementedException(); }
		}

		public int Height
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public DILbxIndexedXml IndexedXml
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxPopupWindow InfoWindow
		{
			get { throw new NotImplementedException(); }
		}

		public void InitializeView(object View)
		{
			throw new NotImplementedException();
		}

		public bool IsDialogBarVisible(string Name)
		{
			throw new NotImplementedException();
		}

		public bool IsUnicodeBuild
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxKeyLinkHandler KeyLinkHandler
		{
			get { throw new NotImplementedException(); }
		}

		public int Left
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public DILbxLibrarian Librarian
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxUpdateApplication LibronixUpdate
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxLicenseManager LicenseManager
		{
			get { throw new NotImplementedException(); }
		}

		public void LoadCommandBar(object CommandBarElement)
		{
			throw new NotImplementedException();
		}

		public bool LoadWorkspace(object Document, string Options, DlsSaveChanges SaveChanges)
		{
			throw new NotImplementedException();
		}

		LbxScriptLog DILbxApplication.ScriptLog
		{
			get { return ScriptLog; }
		}

		LbxScriptLog DILbxApplication2.ScriptLog
		{
			get { return ScriptLog; }
		}

		LbxScriptLog DILbxApplication3.ScriptLog
		{
			get { return ScriptLog; }
		}

		LbxScriptLog DILbxApplication4.ScriptLog
		{
			get { return ScriptLog; }
		}

		LbxScriptLog DILbxApplication5.ScriptLog
		{
			get { return ScriptLog; }
		}

		LbxScriptLog DILbxApplication6.ScriptLog
		{
			get { return ScriptLog; }
		}

		public LbxApplicationMSXML MSXML
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxMetadataConverter MetadataConverter
		{
			get { throw new NotImplementedException(); }
		}

		public void Move(int Left, int Top)
		{
			throw new NotImplementedException();
		}

		public string Name
		{
			get { throw new NotImplementedException(); }
		}

		public LbxNamedFileTable NamedFileTable
		{
			get { throw new NotImplementedException(); }
		}

		public LbxApplicationOptions Options
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxApplication Parent
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxPericopeSets PericopeSets
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxPopupWindow PopupWindow
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxQueryParser QueryParser
		{
			get { throw new NotImplementedException(); }
		}

		public bool Quit(DlsSaveChanges SaveChanges)
		{
			throw new NotImplementedException();
		}

		public LbxWindows RecentWindows
		{
			get { throw new NotImplementedException(); }
		}

		public void RegisterSearchAgent(DILbxSearchAgent SearchAgent)
		{
			throw new NotImplementedException();
		}

		public LbxReports Reports
		{
			get { throw new NotImplementedException(); }
		}

		public void Resize(int Width, int Height)
		{
			throw new NotImplementedException();
		}

		public LbxResourceAssociations ResourceAssociations
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxResourceTypes ResourceTypes
		{
			get { throw new NotImplementedException(); }
		}

		public void SaveWorkspace(object Document, string Options)
		{
			throw new NotImplementedException();
		}

		public LbxScriptLog ScriptLog
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxScriptUtil ScriptUtil
		{
			get { throw new NotImplementedException(); }
		}

		public LbxApplicationSessionData SessionData
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxPreferences SessionPreferences
		{
			get { throw new NotImplementedException(); }
		}

		public LbxApplicationShells Shells
		{
			get { throw new NotImplementedException(); }
		}

		public void ShowVisualCue(string Name, int X, int Y)
		{
			throw new NotImplementedException();
		}

		public void StopActiveJob(Utility.UtilJobStatus Status)
		{
			throw new NotImplementedException();
		}

		public LbxStringTable StringTable
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxSyntaxDatabases SyntaxDatabases
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxLibrarySystem System
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxFileManager SystemFileManager
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxPopupWindow TipWindow
		{
			get { throw new NotImplementedException(); }
		}

		public LbxToolTypes ToolTypes
		{
			get { throw new NotImplementedException(); }
		}

		public int Top
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public DILbxTrayIcons TrayIcons
		{
			get { throw new NotImplementedException(); }
		}

		public void UnregisterSearchAgent(DILbxSearchAgent SearchAgent)
		{
			throw new NotImplementedException();
		}

		public int UsableHeight
		{
			get { throw new NotImplementedException(); }
		}

		public int UsableLeft
		{
			get { throw new NotImplementedException(); }
		}

		public int UsableTop
		{
			get { throw new NotImplementedException(); }
		}

		public int UsableWidth
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxLibraryUser User
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxFileManager UserFileManager
		{
			get { throw new NotImplementedException(); }
		}

		public object UserInterfaceData
		{
			get { throw new NotImplementedException(); }
		}

		public bool Visible
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public LbxVisualFilterTypes VisualFilterTypes
		{
			get { throw new NotImplementedException(); }
		}

		public int Width
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public LbxWindowLinkSets WindowLinkSets
		{
			get { return new LbxWindowLinkSetsDouble(); }
		}

		public DlsWindowState WindowState
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public LbxWindowTypes WindowTypes
		{
			get { throw new NotImplementedException(); }
		}

		public LbxWindows Windows
		{
			get { return m_Windows; }
		}

		public DILbxResourceVisualFilters _GlobalVisualFilterTypes
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxResourceVisualFilters _VisualFilterTypes
		{
			get { throw new NotImplementedException(); }
		}

		[IndexerName("ActiveCommandParameters")]
		public Utility.LbxParameters this[int nIndex]
		{
			get	{ throw new NotImplementedException(); }
		}

		public DILbxPreferences get_SystemPreferences(string Name)
		{
			throw new NotImplementedException();
		}

		public DILbxPreferences get_UserPreferences(string Name)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
	#endregion

	#region LbxWindow

	internal class LbxWindowDouble : LbxWindow
	{
		#region DILbxWindow3 Members

		public void Activate()
		{
			throw new NotImplementedException();
		}

		public bool Active
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxApplication Application
		{
			get { throw new NotImplementedException(); }
		}

		public string Caption
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool Close(DlsSaveChanges SaveChanges)
		{
			throw new NotImplementedException();
		}

		public DILbxCommandBars CommandBars
		{
			get { throw new NotImplementedException(); }
		}

		public bool Enabled
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void ExecuteAction(string Action, object JobCallback)
		{
			throw new NotImplementedException();
		}

		public int Height
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public LbxWindowHistory History
		{
			get { throw new NotImplementedException(); }
		}

		public object Info
		{
			get { return new LbxResourceWindowInfoDouble {ActiveDataType = "bible", ActiveReference = "bible.1.2.3"}; }
		}

		public bool IsActionEnabled(string Action)
		{
			throw new NotImplementedException();
		}

		public int Left
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string LinkCommand
		{
			get { throw new NotImplementedException(); }
		}

		public string LinkTitle
		{
			get { throw new NotImplementedException(); }
		}

		public void Move(int Left, int Top)
		{
			throw new NotImplementedException();
		}

		public string Name
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxApplication Parent
		{
			get { throw new NotImplementedException(); }
		}

		public object QueryActionObject(string Action, object JobCallback)
		{
			throw new NotImplementedException();
		}

		public int QueryActionState(string Action)
		{
			throw new NotImplementedException();
		}

		public string QueryActionText(string Action)
		{
			throw new NotImplementedException();
		}

		public void Refresh()
		{
			throw new NotImplementedException();
		}

		public void Resize(int Width, int Height)
		{
			throw new NotImplementedException();
		}

		public bool SaveChanges(DlsSaveChanges SaveChanges)
		{
			throw new NotImplementedException();
		}

		public void ShowSubstituteView(string Name, string ViewParams, string LinkCommand, string LinkTitle, bool AddToHistory)
		{
			throw new NotImplementedException();
		}

		public DILbxReportView SubstituteView
		{
			get { throw new NotImplementedException(); }
		}

		public int Top
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Type { get; set; }

		public int UsableHeight
		{
			get { throw new NotImplementedException(); }
		}

		public int UsableLeft
		{
			get { throw new NotImplementedException(); }
		}

		public int UsableTop
		{
			get { throw new NotImplementedException(); }
		}

		public int UsableWidth
		{
			get { throw new NotImplementedException(); }
		}

		public object View
		{
			get { throw new NotImplementedException(); }
		}

		public bool Visible
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public int Width
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public DlsWindowState WindowState
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
	#endregion

	#region LbxWindowsDouble
	internal class LbxWindowsDouble: LbxWindows, IEnumerator<LbxWindow>
	{
		private List<LbxWindow> m_Windows = new List<LbxWindow>();
		private int m_iWindows = -1;

		internal LbxWindowsDouble()
		{
			m_Windows.Add(new LbxWindowDouble {Type = "resource"});
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public DILbxWindow Add(string Type, string Name, bool Visible)
		{
			throw new NotImplementedException();
		}

		public void Arrange(DlsWindowArrangeStyle Style)
		{
			throw new NotImplementedException();
		}

		public bool SaveChanges(DlsSaveChanges SaveChanges)
		{
			throw new NotImplementedException();
		}

		public bool Close(DlsSaveChanges SaveChanges)
		{
			throw new NotImplementedException();
		}

		public DILbxApplication Application
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxApplication Parent
		{
			get { throw new NotImplementedException(); }
		}

		public int Count
		{
			get { return m_Windows.Count; }
		}

		public DILbxWindow this[object index]
		{
			get { return m_Windows[(int)index]; }
		}

		IEnumerator DILbxWindows.GetEnumerator()
		{
			return GetEnumerator();
		}

		private IEnumerator GetEnumerator()
		{
			return m_Windows.GetEnumerator();
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			m_iWindows++;
			return m_iWindows < m_Windows.Count;
		}

		public void Reset()
		{
			m_iWindows = -1;
		}

		public LbxWindow Current
		{
			get { return m_Windows[m_iWindows]; }
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}
	}

	#endregion

	#region LbxResourceWindowInfo
	internal class LbxResourceWindowInfoDouble: LbxResourceWindowInfo
	{

		#region DILbxResourceWindowInfo4 Members

		public string ActiveDataType { get; set; }

		public string ActiveReference { get; set; }

		public DILbxApplication Application
		{
			get { throw new NotImplementedException(); }
		}

		public bool GoToNextResource(object pJobCallback)
		{
			throw new NotImplementedException();
		}

		public bool GoToPreviousResource(object pJobCallback)
		{
			throw new NotImplementedException();
		}

		public static string Reference { get; set; }
		public bool GoToReference(string reference, int minScore, int maxScore, string navigationID, object pJobCallback)
		{
Console.WriteLine("Setting reference to {0}", reference);
			Reference = reference;
			return true;
		}

		public bool GoToReferenceEx(string Reference, int MinScore, int MaxScore, string NavigationID, string Options, object pJobCallback)
		{
			throw new NotImplementedException();
		}

		public DILbxWindow Parent
		{
			get { throw new NotImplementedException(); }
		}

		public bool ReferenceTarget
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string ResourceID
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxResourceVisualFilters VisualFilters
		{
			get { throw new NotImplementedException(); }
		}

		public string get_ResourceAssociationID(string Type)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
	#endregion

	#region LbxWindowLinkSetDouble
	internal class LbxWindowLinkSetDouble: LbxWindowLinkSet
	{

		#region DILbxWindowLinkSet Members

		public DILbxApplication Application
		{
			get { throw new NotImplementedException(); }
		}

		public IPictureDisp Image
		{
			get { throw new NotImplementedException(); }
		}

		public string Name
		{
			get { throw new NotImplementedException(); }
		}

		public DILbxApplication Parent
		{
			get { throw new NotImplementedException(); }
		}

		public string Title
		{
			get { throw new NotImplementedException(); }
		}

		public uint Transparent
		{
			get { throw new NotImplementedException(); }
		}

		public LbxWindowLinkSetWindows Windows
		{
			get { return new LbxWindowLinkSetWindowsDouble(); }
		}

		#endregion
	}
	#endregion

	#region LbxWindowLinkSetsDouble

	internal class LbxWindowLinkSetsDouble: LbxWindowLinkSets
	{

	#region DILbxWindowLinkSets Members

		public LbxWindowLinkSet  Add(string Name, string Title, IPictureDisp Picture, uint Transparent)
		{
 			throw new NotImplementedException();
		}

		public DILbxApplication  Application
		{
			get { throw new NotImplementedException(); }
		}

		public void  Clear()
		{
 			throw new NotImplementedException();
		}

		public int  Count
		{
			get { throw new NotImplementedException(); }
		}

		public IEnumerator  GetEnumerator()
		{
 			throw new NotImplementedException();
		}

		public DILbxApplication  Parent
		{
			get { throw new NotImplementedException(); }
		}

		public void  Remove(object Index)
		{
 			throw new NotImplementedException();
		}

		public LbxWindowLinkSet  this[object index]
		{
			get { return new LbxWindowLinkSetDouble(); }
		}

		#endregion
	}
	#endregion

	#region LbxWindowLinkSetWindowsDouble
	internal class LbxWindowLinkSetWindowsDouble : LbxWindowLinkSetWindows
	{
		#region DILbxWindowLinkSetWindows Members

		public void Add(DILbxWindow Window)
		{
			throw new NotImplementedException();
		}

		public DILbxApplication Application
		{
			get { throw new NotImplementedException(); }
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { throw new NotImplementedException(); }
		}

		public int Find(DILbxWindow Window)
		{
			return 1;
		}

		public IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public LbxWindowLinkSet Parent
		{
			get { throw new NotImplementedException(); }
		}

		public void Remove(int nIndex)
		{
			throw new NotImplementedException();
		}

		public DILbxWindow this[int Index]
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}

	#endregion
}
#endif
