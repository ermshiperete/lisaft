using System;
using NUnit.Framework;
using SIL.FieldWorks.TE.LibronixLinker.Logos4Doubles;

namespace SIL.FieldWorks.TE.LibronixLinker
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class LogosPositionHandlerFactoryTests
	{
		private LibronixPositionHandlerFactoryDouble m_LibronixFactory;

		/// <summary/>
		[SetUp]
		public void Setup()
		{
			m_LibronixFactory = new LibronixPositionHandlerFactoryDouble();
			LogosPositionHandlerFactory.ResetCreatedEvent();
			LogosPositionHandlerFactory.ResetFactories();
			LogosPositionHandlerFactory.AddFactory(m_LibronixFactory);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the IsNotInstalled property
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void IsNotInstalled()
		{
			m_LibronixFactory.LibronixIsInstalled = false;
			Assert.IsFalse(LogosPositionHandlerFactory.IsNotInstalled);
			try
			{
				LogosPositionHandlerFactory.CreateInstance(false, 0, false);
			}
			catch (LibronixNotInstalledException)
			{
			}
			Assert.IsTrue(LogosPositionHandlerFactory.IsNotInstalled);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Simulates two different versions of Logos. The first version is not installed, but
		/// the second is.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SecondFactoryIsInstalled_Logos4()
		{
			m_LibronixFactory.LibronixIsInstalled = false;

			// Simulate a second factory
			LogosPositionHandlerFactory.AddFactory(
				new Logos4PositionHandlerFactoryDouble {Logos4IsInstalled = true, Logos4IsRunning = true});

			using (var posHandler = (IDisposable)LogosPositionHandlerFactory.CreateInstance(false, 0, false))
			{
				Assert.IsFalse(LogosPositionHandlerFactory.IsNotInstalled);
				Assert.IsInstanceOf(typeof (Logos4PositionHandler), posHandler);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Simulates two different versions of Logos. The first version is not installed, but
		/// the second is.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SecondFactoryIsInstalled_Libronix()
		{
			m_LibronixFactory.LibronixIsInstalled = true;
			m_LibronixFactory.LibronixIsRunning = true;

			LogosPositionHandlerFactory.ResetFactories();
			LogosPositionHandlerFactory.AddFactory(
				new Logos4PositionHandlerFactoryDouble { Logos4IsInstalled = false });
			LogosPositionHandlerFactory.AddFactory(m_LibronixFactory);

			using (var posHandler = (IDisposable)LogosPositionHandlerFactory.CreateInstance(false, 0, false))
			{
				Assert.IsFalse(LogosPositionHandlerFactory.IsNotInstalled);
				Assert.IsInstanceOf(typeof (LibronixPositionHandler), posHandler);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Simulates two different versions of Logos. The first version is not installed, but
		/// the second is but is not running.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		[ExpectedException(typeof(LibronixNotRunningException))]
		public void SecondFactoryIsInstalled_NotRunning()
		{
			m_LibronixFactory.LibronixIsInstalled = false;

			// Simulate a second factory
			LogosPositionHandlerFactory.AddFactory(
				new Logos4PositionHandlerFactoryDouble { Logos4IsInstalled = true, Logos4IsRunning = false });

			LogosPositionHandlerFactory.CreateInstance(false, 0, true);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Simulates two different versions of Logos. The first version is installed but not
		/// running, the second is not installed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		[ExpectedException(typeof(LibronixNotRunningException))]
		public void FirstFactoryIsInstalled_NotRunning()
		{
			m_LibronixFactory.LibronixIsInstalled = true;
			m_LibronixFactory.LibronixIsRunning = false;

			// Simulate a second factory
			LogosPositionHandlerFactory.AddFactory(
				new Logos4PositionHandlerFactoryDouble { Logos4IsInstalled = false, Logos4IsRunning = false });

			LogosPositionHandlerFactory.CreateInstance(false, 0, true);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that we get an exception if Libronix isn't installed
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		[ExpectedException(typeof(LibronixNotInstalledException))]
		public void NotInstalled()
		{
			m_LibronixFactory.LibronixIsInstalled = false;
			LogosPositionHandlerFactory.CreateInstance(false, 0, true);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that we get a <c>null</c> object if Libronix is installed but not running and
		/// we don't want an exception to be thrown
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void DontStartNoException()
		{
			m_LibronixFactory.LibronixIsInstalled = true;
			m_LibronixFactory.LibronixIsRunning = false;
			Assert.IsNull(LogosPositionHandlerFactory.CreateInstance(false, 0, false));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that we get a <c>null</c> object if Libronix is installed but not running and
		/// we don't want an exception to be thrown
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		[ExpectedException(typeof(LibronixNotRunningException))]
		public void DontStart()
		{
			m_LibronixFactory.LibronixIsInstalled = true;
			m_LibronixFactory.LibronixIsRunning = false;
			LogosPositionHandlerFactory.CreateInstance(false, 0, true);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that Libronix gets started if it isn't already running.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void Start()
		{
			m_LibronixFactory.LibronixIsInstalled = true;
			m_LibronixFactory.LibronixIsRunning = false;
			using (var handler = LogosPositionHandlerFactory.CreateInstance(true, 0, false) as LibronixPositionHandler)
			{
				Assert.IsNotNull(handler);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the already running instance gets returned
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AlreadyRunning()
		{
			m_LibronixFactory.LibronixIsInstalled = true;
			m_LibronixFactory.LibronixIsRunning = true;
			using (m_LibronixFactory.LibronixPositionHandler =
				new LibronixPositionHandlerDouble(0, new LbxApplicationDouble()))
			{
				using (var handler = LogosPositionHandlerFactory.CreateInstance(true, 0, false) as LibronixPositionHandler)
				{
					Assert.IsNotNull(handler);
				}
			}
		}
	}
}
