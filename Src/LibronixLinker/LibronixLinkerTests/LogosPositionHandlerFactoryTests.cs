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
		private Logos4PositionHandlerFactoryDouble m_LogosFactory;

		/// <summary/>
		[SetUp]
		public void Setup()
		{
			m_LogosFactory = new Logos4PositionHandlerFactoryDouble();
			LogosPositionHandlerFactory.ResetCreatedEvent();
			LogosPositionHandlerFactory.ResetFactories();
			LogosPositionHandlerFactory.AddFactory(m_LogosFactory);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the IsNotInstalled property
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void IsNotInstalled()
		{
			m_LogosFactory.Logos4IsInstalled = false;
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
		[Platform(Exclude = "Linux", Reason = "Libronix tests not working on Linux because of stdole dependency")]
		public void SecondFactoryIsInstalled_Libronix()
		{
			m_LogosFactory.Logos4IsInstalled = false;

			// Simulate a second factory
			LogosPositionHandlerFactory.AddFactory(
				new LibronixPositionHandlerFactoryDouble {LibronixIsInstalled = true, LibronixIsRunning = true});

			using (var posHandler = (IDisposable)LogosPositionHandlerFactory.CreateInstance(false, 0, false))
			{
				Assert.IsFalse(LogosPositionHandlerFactory.IsNotInstalled);
				Assert.IsInstanceOf(typeof (LibronixPositionHandler), posHandler);
			}
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
			m_LogosFactory.Logos4IsInstalled = true;
			m_LogosFactory.Logos4IsRunning = true;

			LogosPositionHandlerFactory.ResetFactories();
			LogosPositionHandlerFactory.AddFactory(
				new LibronixPositionHandlerFactoryDouble { LibronixIsInstalled = false });
			LogosPositionHandlerFactory.AddFactory(m_LogosFactory);

			using (var posHandler = (IDisposable)LogosPositionHandlerFactory.CreateInstance(false, 0, false))
			{
				Assert.IsFalse(LogosPositionHandlerFactory.IsNotInstalled);
				Assert.IsInstanceOf(typeof (Logos4PositionHandler), posHandler);
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
			m_LogosFactory.Logos4IsInstalled = false;

			// Simulate a second factory
			LogosPositionHandlerFactory.AddFactory(
				new LibronixPositionHandlerFactoryDouble { LibronixIsInstalled = true, LibronixIsRunning = false });

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
			m_LogosFactory.Logos4IsInstalled = true;
			m_LogosFactory.Logos4IsRunning = false;

			// Simulate a second factory
			LogosPositionHandlerFactory.AddFactory(
				new LibronixPositionHandlerFactoryDouble { LibronixIsInstalled = false, LibronixIsRunning = false });

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
			m_LogosFactory.Logos4IsInstalled = false;
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
			m_LogosFactory.Logos4IsInstalled = true;
			m_LogosFactory.Logos4IsRunning = false;
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
			m_LogosFactory.Logos4IsInstalled = true;
			m_LogosFactory.Logos4IsRunning = false;
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
			m_LogosFactory.Logos4IsInstalled = true;
			m_LogosFactory.Logos4IsRunning = false;
			using (var handler = (IDisposable)LogosPositionHandlerFactory.CreateInstance(true, 0, false))
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
			m_LogosFactory.Logos4IsInstalled = true;
			m_LogosFactory.Logos4IsRunning = true;
			using (m_LogosFactory.Logos4PositionHandler = new Logos4PositionHandlerDouble(0, new LogosApplicationDouble()))
			{
				using (var handler = (IDisposable)LogosPositionHandlerFactory.CreateInstance(true, 0, false))
				{
					Assert.IsNotNull(handler);
				}
			}
		}
	}
}
