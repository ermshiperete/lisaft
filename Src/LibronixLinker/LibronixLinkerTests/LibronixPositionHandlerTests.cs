// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2008, SIL International. All Rights Reserved.
// <copyright from='2008' to='2008' company='SIL International'>
//		Copyright (c) 2008, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: LibronixPositionHandlerTests.cs
// Responsibility: TE Team
// 
// <remarks>
// </remarks>
// ---------------------------------------------------------------------------------------------
using NUnit.Framework;

namespace SIL.Utils
{
#if !__MonoCS__
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Tests the ConvertFrom/ToBcv methods of the LibronixPositionHandler class
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	[Platform(Exclude="Linux", Reason="LibronixLinker isn't ported to Linux")]
	public class LibronixPositionHandlerTests
	{
		private LibronixPositionHandler m_LibronixPositionHandler;

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
				new LibronixPositionHandlerFactoryDouble {LibronixIsInstalled = true});
			m_LibronixPositionHandler = 
				LogosPositionHandlerFactory.CreateInstance(true, 0, false) as LibronixPositionHandler;
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

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Invalid references passed to ConvertToBcv return -1
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ConvertToBcv_Null()
		{
			Assert.AreEqual(-1, LibronixPositionHandler.ConvertToBcv(null));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Invalid references passed to ConvertToBcv return -1
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ConvertToBcv_InvalidFormat()
		{
			Assert.AreEqual(-1, LibronixPositionHandler.ConvertToBcv("1.1.1"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Invalid references passed to ConvertToBcv return -1
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ConvertToBcv_Apcrypha()
		{
			Assert.AreEqual(-1, LibronixPositionHandler.ConvertToBcv("bible.40.1.1"));
			Assert.AreEqual(-1, LibronixPositionHandler.ConvertToBcv("bible.88.1.1"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Valid reference should return a BCV reference
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ConvertToBcv_OT()
		{
			Assert.AreEqual(1001001, LibronixPositionHandler.ConvertToBcv("bible.1.1.1"));
			Assert.AreEqual(39001001, LibronixPositionHandler.ConvertToBcv("bible.39.1.1"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Valid reference should return a BCV reference
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ConvertToBcv_NT()
		{
			Assert.AreEqual(40001001, LibronixPositionHandler.ConvertToBcv("bible.61.1.1"));
			Assert.AreEqual(66001001, LibronixPositionHandler.ConvertToBcv("bible.87.1.1"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Valid BCV reference returns the corresponding Libronix reference
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ConvertFromBcv_OT()
		{
			Assert.AreEqual("bible.1.1.1", LibronixPositionHandler.ConvertFromBcv(1001001));
			Assert.AreEqual("bible.39.1.1", LibronixPositionHandler.ConvertFromBcv(39001001));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Valid BCV reference returns the corresponding Libronix reference
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ConvertFromBcv_NT()
		{
			Assert.AreEqual("bible.61.1.1", LibronixPositionHandler.ConvertFromBcv(40001001));
			Assert.AreEqual("bible.87.1.1", LibronixPositionHandler.ConvertFromBcv(66001001));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests receiving references from Libronix
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ReceiveReference()
		{
			Assert.AreEqual(001002003, ((LibronixPositionHandlerDouble)m_LibronixPositionHandler).CallOnTick());
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests sending references to Libronix
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SendReference()
		{
			m_LibronixPositionHandler.SetReference(005004003);
			Assert.AreEqual("bible.5.4.3", LbxResourceWindowInfoDouble.Reference);
		}
	}
#endif
}
