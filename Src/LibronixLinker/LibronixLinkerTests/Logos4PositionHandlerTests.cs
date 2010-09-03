// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2010, SIL International. All Rights Reserved.
// <copyright from='2010' to='2010' company='SIL International'>
//		Copyright (c) 2010, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: Logos4PositionHandlerTests.cs
// Responsibility: EberhardB
// 
// <remarks>
// </remarks>
// ---------------------------------------------------------------------------------------------
using NUnit.Framework;
using SIL.Utils.Logos4Doubles;

namespace SIL.Utils
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class Logos4PositionHandlerTests
	{
		private Logos4PositionHandlerDouble m_LibronixPositionHandler;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Fixture setup
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			LogosPositionHandlerFactory.ResetFactories();
			LogosPositionHandlerFactory.AddFactory(
				new Logos4PositionHandlerFactoryDouble { Logos4IsInstalled = true });
			m_LibronixPositionHandler =
				LogosPositionHandlerFactory.CreateInstance(true, 0, false) as Logos4PositionHandlerDouble;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[TestFixtureTearDown]
		public void FixtureTeardown()
		{
			m_LibronixPositionHandler.Dispose();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests receiving references from Libronix with a normal reference and different API
		/// versions
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ReceiveReference_Normal(
			[Values(1,2)] int versionNo)
		{
			LogosApplicationDouble.VersionNo = versionNo;
			m_LibronixPositionHandler.Dispose();
			m_LibronixPositionHandler =
				LogosPositionHandlerFactory.CreateInstance(true, 0, false) as Logos4PositionHandlerDouble;

			LogosBibleReferenceDetailsDouble.Reference = string.Format("{0}.2.3", 
				LogosBibleReferenceDetailsDouble.Books[1, 1]);
			m_LibronixPositionHandler.CallOnPanelChanged(new LogosPanelDouble());
			Assert.AreEqual(001002003, m_LibronixPositionHandler.EventArgs.BcvRef);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests receiving references from Libronix with a reference from the apocrypha
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ReceiveReference_Apocrypha()
		{
			LogosBibleReferenceDetailsDouble.Reference = "Bar.1.2";
			m_LibronixPositionHandler.CallOnPanelChanged(new LogosPanelDouble());
			Assert.IsNull(m_LibronixPositionHandler.EventArgs);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests receiving references from Libronix with a reference that contains a chapter
		/// string that we don't handle. We treat it as chapter 0.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ReceiveReference_UnhandledChapterA()
		{
			LogosBibleReferenceDetailsDouble.Reference = "Ge.A.2";
			m_LibronixPositionHandler.CallOnPanelChanged(new LogosPanelDouble());
			Assert.AreEqual(001000002, m_LibronixPositionHandler.EventArgs.BcvRef);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests receiving references from Libronix with a reference that contains a chapter
		/// string that we don't handle. We treat it as chapter 0.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ReceiveReference_UnhandledChapter1A()
		{
			LogosBibleReferenceDetailsDouble.Reference = "Ge.1A.2";
			m_LibronixPositionHandler.CallOnPanelChanged(new LogosPanelDouble());
			Assert.AreEqual(001000002, m_LibronixPositionHandler.EventArgs.BcvRef);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests receiving references from Libronix with a reference that contains a chapter
		/// string that we don't handle. We treat it as chapter 0.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ReceiveReference_UnhandledChapterTitle()
		{
			LogosBibleReferenceDetailsDouble.Reference = "Ge.Title.2";
			m_LibronixPositionHandler.CallOnPanelChanged(new LogosPanelDouble());
			Assert.AreEqual(001000002, m_LibronixPositionHandler.EventArgs.BcvRef);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests receiving references from Libronix with a reference that contains a verse
		/// string that we don't handle.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ReceiveReference_UnhandledVerse()
		{
			LogosBibleReferenceDetailsDouble.Reference = "Ge.1.1a";
			m_LibronixPositionHandler.CallOnPanelChanged(new LogosPanelDouble());
			Assert.AreEqual(001001001, m_LibronixPositionHandler.EventArgs.BcvRef);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests receiving references from Libronix with a reference that contains a verse
		/// string that we don't handle.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ReceiveReference_UnhandledVerseTitle()
		{
			LogosBibleReferenceDetailsDouble.Reference = "Ge.1.Title";
			m_LibronixPositionHandler.CallOnPanelChanged(new LogosPanelDouble());
			Assert.AreEqual(001001000, m_LibronixPositionHandler.EventArgs.BcvRef);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests sending a reference to Logos 4.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SendReference_OT(
			[Values(1, 2)] int versionNo)
		{
			LogosApplicationDouble.VersionNo = versionNo;
			m_LibronixPositionHandler.Dispose();
			m_LibronixPositionHandler =
				LogosPositionHandlerFactory.CreateInstance(true, 0, false) as Logos4PositionHandlerDouble;

			m_LibronixPositionHandler.SetReference(01002003);
			Assert.AreEqual("Ge.2.3", LogosBibleReferenceDetailsDouble.Reference);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests sending a reference to Logos 4.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SendReference_NT()
		{
			m_LibronixPositionHandler.SetReference(66002003);
			Assert.AreEqual("Re.2.3", LogosBibleReferenceDetailsDouble.Reference);
		}
	}
}
