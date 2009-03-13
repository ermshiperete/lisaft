using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SIL.FieldWorks.Common.ScriptureUtils;
using SIL.FieldWorks.TE.LibronixLinker;

namespace LibronixSantaFeTranslator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Translates sync messages from Libronix to Santa Fe format (e.g. TE and Paratext)
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class LiSaFT : Form
	{
		private LibronixPositionHandler m_positionHandler;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="LiSaFT"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public LiSaFT()
		{
			InitializeComponent();

			try
			{
				InitLibronix();
			}
			catch (LibronixNotRunningException e)
			{
				// If Libronix isn't running there is a chance that the user will start it
				// later, so we keep running and give the user the possibility to call Refresh
				// later. However, if Libronix isn't installed we don't catch the exception
				// here but rather in the caller - there is no point in keep running.
				m_positionHandler = null;
				MessageBox.Show(e.Message);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Inits the libronix.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void InitLibronix()
		{
			if (m_positionHandler != null)
				return;

			m_positionHandler = LibronixPositionHandler.CreateInstance(
				Properties.Settings.Default.StartLibronix, Properties.Settings.Default.LinkSet, true);
			m_positionHandler.PositionChanged += OnPositionChanged;

			// Add all Libronix link sets to the combo box.
			if (m_positionHandler.AvailableLinkSets.Count > 0)
			{
				m_LinkSetCombo.Items.Clear();
				foreach (string linkSet in m_positionHandler.AvailableLinkSets)
					m_LinkSetCombo.Items.Add(linkSet);
			}

			if (Properties.Settings.Default.LinkSet < m_LinkSetCombo.Items.Count)
				m_LinkSetCombo.SelectedIndex = Properties.Settings.Default.LinkSet;
			else
				m_LinkSetCombo.SelectedIndex = 0;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Called when the position in Libronix changed
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="SIL.FieldWorks.TE.LibronixLinker.PositionChangedEventArgs"/> 
		/// instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void OnPositionChanged(object sender, PositionChangedEventArgs e)
		{
			ScrReference scrRef = new ScrReference(e.BcvRef);
			if (!scrRef.Valid)
				return;

			System.Diagnostics.Debug.WriteLine(string.Format(
				"New position: {0}; Book {1}, Chapter {2}, Verse {3}; {4}",
				e.BcvRef, scrRef.Book, scrRef.Chapter, scrRef.Verse, scrRef.AsString));
			SantaFeFocusMessageHandler.SendFocusMessage(scrRef.AsString);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Renew the subscriptions to all open windows in Libronix
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event 
		/// data.</param>
		/// ------------------------------------------------------------------------------------
		private void OnRefresh(object sender, EventArgs e)
		{
			if (m_positionHandler != null)
				m_positionHandler.Refresh(Properties.Settings.Default.LinkSet);
			else
			{
				try
				{
					InitLibronix();
				}
				catch (LibronixNotRunningException)
				{
					// still not running :-)
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Display the configuration dialog
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event 
		/// data.</param>
		/// ------------------------------------------------------------------------------------
		private void OnConfig(object sender, EventArgs e)
		{
			ShowInTaskbar = true;
			WindowState = FormWindowState.Normal;
			m_LinkSetCombo.SelectedIndex = Properties.Settings.Default.LinkSet;
			chkbStartLibronix.Checked = Properties.Settings.Default.StartLibronix;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Called when the user presses the OK button on the dialog.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event 
		/// data.</param>
		/// ------------------------------------------------------------------------------------
		private void OnOk(object sender, EventArgs e)
		{
			Properties.Settings.Default.LinkSet = m_LinkSetCombo.SelectedIndex;
			Properties.Settings.Default.StartLibronix = chkbStartLibronix.Checked;
			Properties.Settings.Default.Save();
			m_positionHandler.Refresh(Properties.Settings.Default.LinkSet);
			OnCancel(sender, e);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Called when the user presses the Cancel button on the dialog.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event 
		/// data.</param>
		/// ------------------------------------------------------------------------------------
		private void OnCancel(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Minimized;
			ShowInTaskbar = false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Exit the application
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event 
		/// data.</param>
		/// ------------------------------------------------------------------------------------
		private void OnExit(object sender, EventArgs e)
		{
			Close();
		}


	}
}