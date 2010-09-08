// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2010, SIL International. All Rights Reserved.
// <copyright from='2007' to='2010' company='SIL International'>
//		Copyright (c) 2007-2010, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LibronixDLS;

namespace SIL.Utils
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Class to manage saving and restoring libronix workspace.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class LibronixWorkspaceManager
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// If Libronix is running, save its workspace in the specified file.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void SaveWorkspace(string path)
		{
			try
			{
				// If Libronix isn't running, we'll get an exception here
				var libronixApp = Marshal.GetActiveObject("LibronixDLS.LbxApplication") as LbxApplication;
				if (libronixApp == null)
					return;

				var document = libronixApp.MSXML.CreateDocument(0) as MSXML2.DOMDocument;
				libronixApp.SaveWorkspace(document, "");
				document.save(path);
			}
			catch (COMException)
			{
				return;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// If Libronix is NOT running, start it up, and restore the specified workspace.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void RestoreIfNotRunning(string path)
		{
			var worker = new BackgroundWorker();
			worker.DoWork += OnDoWork;
			worker.RunWorkerCompleted += OnRunWorkerCompleted;
			worker.RunWorkerAsync(path);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the RunWorkerCompleted event of the worker control. We use it to display
		/// a message box if an error occured.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
				MessageBox.Show(e.Error.Message);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Restores the workspace.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static void RestoreWorkspace(string path)
		{
#if !__MonoCS__
			// TODO-Linux: Implement
			LbxApplication libronixApp = null;
			try
			{
				// If Libronix isn't running, we'll get an exception here
				Marshal.GetActiveObject("LibronixDLS.LbxApplication");
				return; // It IS running; don't disturb it.
			}
			catch (COMException e)
			{
				if ((uint)e.ErrorCode == 0x800401E3) // MK_E_UNAVAILABLE
				{
					// try to start
					libronixApp = new LbxApplicationClass();
				}
			}
			if (libronixApp == null) // can't start, or not installed.
				return;
			try
			{
				// Try to load workspace.
				if (!File.Exists(path))
				{
					libronixApp.Visible = true; //let them see it, anyway.
					return;
				}
				var document = libronixApp.MSXML.CreateDocument(0) as MSXML2.DOMDocument;
				document.load(path);

				libronixApp.LoadWorkspace(document, "", DlsSaveChanges.dlsPromptToSaveChanges);
				libronixApp.Visible = true; //only after we reload the workspace, to save flashing.
			}
			catch (Exception)
			{
				libronixApp.Visible = true; //let them see it, anyway.
				ReportLoadProblem();
			}
#endif
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the DoWork event of the worker control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static void OnDoWork(object sender, DoWorkEventArgs e)
		{
			RestoreWorkspace(e.Argument as string);
		}

#if !__MonoCS__
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Reports the load problem.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static void ReportLoadProblem()
		{
			throw new Exception("Unable to reload Libronix workspace");
		}
#endif
	}
}
