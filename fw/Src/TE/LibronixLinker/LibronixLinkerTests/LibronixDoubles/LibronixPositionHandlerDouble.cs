using System;
using System.Windows.Forms;
using LibronixDLS;
using LibronixDLSUtility;

namespace SIL.FieldWorks.TE.LibronixLinker
{
	internal class LibronixPositionHandlerDouble : LibronixPositionHandler
	{
		private PositionChangedEventArgs m_eventArgs;

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="LibronixPositionHandlerDouble"/> class.
		/// </summary>
		/// <param name="linkSet">The link set.</param>
		/// <param name="libronixApp">The libronix app.</param>
		/// --------------------------------------------------------------------------------
		public LibronixPositionHandlerDouble(int linkSet, LbxApplication libronixApp):
			base(linkSet, libronixApp)
		{
		}

		protected override LbxApplicationEventsBridge CreateLbxApplicationEventsBridge()
		{
			return new LbxApplicationEventsBridgeDouble();
		}

		protected override void RaisePositionChangedEvent(PositionChangedEventArgs e)
		{
			m_eventArgs = e;
		}

		protected override void StartPollTimer()
		{
			// do nothing
		}

		/// <summary>
		/// Calls the OnTick method
		/// </summary>
		public int CallOnTick()
		{
			OnTick(null, EventArgs.Empty);
			return m_eventArgs.BcvRef;
		}
	}
}