using System;
using System.Windows.Forms;
using SIL.FieldWorks.TE.LibronixLinker;

namespace LibronixSantaFeTranslator
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			try
			{
				Application.Run(new LiSaFT());
			}
			catch (LibronixNotInstalledException e)
			{
				MessageBox.Show(e.Message);
			}
		}
	}
}