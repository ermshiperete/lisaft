// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2010, SIL International. All Rights Reserved.
// <copyright from='2010' to='2010' company='SIL International'>
//		Copyright (c) 2010, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SIL.Utils
{
	/// <summary/>
	public class CreatedEventArgs: EventArgs
	{
		/// <summary>
		/// Gets or sets the logos position handler.
		/// </summary>
		public ILogosPositionHandler PositionHandler { get; set; }
	}

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Factory class that creates a Libronix or Logos 4 position handler object.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public static class LogosPositionHandlerFactory
	{
		/// <summary>Set to <c>true</c> if we know for sure that Libronix/Logos 4 isn't 
		/// installed. If set to <c>false</c> it might be installed (but Libronix/Logos 4 not 
		/// running), or we haven't tried yet.</summary>
		private static volatile bool s_fNotInstalled;
		/// <summary>A list of ILogosPositionHandler instances based on the link set.</summary>
		private static volatile Dictionary<int, ILogosPositionHandler> s_Instances =
			new Dictionary<int, ILogosPositionHandler>();
		/// <summary>Timer that we use to check if Libronix got started</summary>
		private static Timer s_timer;

		#region Available factories handling
		private static volatile List<ILogosPositionHandlerFactory> s_Factories = 
			new List<ILogosPositionHandlerFactory>();

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Empties the list of factories. This also resets the IsNotInstalled state.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal static void ResetFactories()
		{
			s_Factories = new List<ILogosPositionHandlerFactory>();
			IsNotInstalled = false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Adds a factory to the list of factories
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal static void AddFactory(ILogosPositionHandlerFactory factory)
		{
			s_Factories.Add(factory);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the default list of factories
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal static void LoadDefaultFactories()
		{
			AddFactory(new Logos4PositionHandlerFactory());
			AddFactory(new LibronixPositionHandlerFactory());
		}

		private static IEnumerable<ILogosPositionHandlerFactory> GetFactory()
		{
			if (s_Factories.Count == 0)
				LoadDefaultFactories();
			
			foreach (var factory in s_Factories)
				yield return factory;
		}

		#endregion

		/// <summary>Event that is raised after the LogosPositionHandler got created.</summary>
		/// <remarks>The event might be raised later than calling CreateInstance if Libronix/
		/// Logos4 isn't running and we're told not to start it. In this case the event will
		/// be fired after the user starts Libronix/Logos4.</remarks>
		public static event EventHandler<CreatedEventArgs> Created;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Resets the created events. This should be used only in Unit tests.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal static void ResetCreatedEvent()
		{
			Created = delegate { };
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates an instance of a <see cref="ILogosPositionHandler"/> class.
		/// </summary>
		/// <param name="fStart"><c>true</c> to start Libronix/Logos 4 if it isn't running, 
		/// otherwise <c>false</c>.</param>
		/// <param name="linkSet">The 0-based index of the link set in Libronix/Logos 4.</param>
		/// <param name="fThrowNotRunningException"><c>true</c> to throw not running exception</param>
		/// <returns>
		/// An instance of the <see cref="ILogosPositionHandler"/> class.
		/// </returns>
		/// <exception cref="LibronixNotRunningException">Thrown if Libronix/Logos 4 is installed but
		/// not currently running and <paramref name="fStart"/> is <c>false</c>.</exception>
		/// <exception cref="LibronixNotInstalledException">Thrown if neither Libronix nor 
		/// Logos 4 are installed on this machine.</exception>
		/// <developernote>This method is called on the main thread</developernote>
		/// ------------------------------------------------------------------------------------
		public static ILogosPositionHandler CreateInstance(bool fStart, int linkSet,
			bool fThrowNotRunningException)
		{
			if (IsNotInstalled)
				throw new LibronixNotInstalledException("Libronix isn't installed", null);

			if (!s_Instances.ContainsKey(linkSet))
			{
				lock (typeof (ILogosPositionHandler))
				{
					// We have to check again. It's possible that another thread was in the 
					// middle of creating an instance while we were waiting for the lock.
					if (!s_Instances.ContainsKey(linkSet))
					{
						bool notInstalled = false;
						foreach (var factory in GetFactory())
						{
							try
							{
								var newHandler = factory.CreateInstance(fStart, linkSet, false);
								if (newHandler != null)
								{
									OnCreated(linkSet, newHandler);
								}
								notInstalled = false;
								break;
							}
							catch (LibronixNotInstalledException)
							{
								notInstalled = true;
							}
						}

						if (notInstalled)
						{
							IsNotInstalled = true;
							throw new LibronixNotInstalledException("Libronix/Logos isn't installed", null);
						}
					}
				}
			}
			ILogosPositionHandler handler;
			if (s_Instances.TryGetValue(linkSet, out handler))
				((ILogosPositionHandlerInternal)handler).AddRef();
			else
			{
				StartTimer(linkSet);
				if (fThrowNotRunningException)
					throw new LibronixNotRunningException("Libronix/Logos isn't running", null);
			}

			return handler;
		}

		private static void OnPosHandlerDisposed(object sender, DisposedEventArgs e)
		{
			if (s_Instances.ContainsKey(e.LinkSet))
				s_Instances.Remove(e.LinkSet);

			// Logos application exited; start looking for new instance again
			if (e.LogosAppQuit)
				StartTimer(e.LinkSet);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Starts the timer that checks if Libronix/Logos gets started
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static void StartTimer(int linkSet)
		{
			if (s_timer != null)
			{
				if (!s_timer.Enabled)
					s_timer.Start();
				return;
			}

			s_timer = new Timer { Interval = 100, Tag = linkSet };
			s_timer.Tick += OnTimer;
			s_timer.Start();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Called from the timer. We have to check if Libronix/Logos got started in the mean time.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private static void OnTimer(object sender, EventArgs e)
		{
			foreach (var factory in GetFactory())
			{
				if (factory.IsLogosRunning)
				{
					// Libronix got started!
					var linkSet = (int)s_timer.Tag;
					var positionHandler = factory.CreateInstance(false, linkSet, true);
					if (positionHandler != null)
					{
						s_timer.Stop();
						OnCreated(linkSet, positionHandler);
						break;
					}
				}
			}
		}

		private static void OnCreated(int linkSet, ILogosPositionHandler positionHandler)
		{
			((ILogosPositionHandlerInternal)positionHandler).Disposed += OnPosHandlerDisposed;
			s_Instances.Add(linkSet, positionHandler);
			if (Created != null)
				Created(null, new CreatedEventArgs { PositionHandler = positionHandler });
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether Libronix or Logos 4 is not installed.
		/// </summary>
		/// <value>Returns <c>true</c> if we know for sure that Libronix/Logos 4 isn't 
		/// installed. If <c>false</c> it might be installed (but Libronix/Logos 4 not running), 
		/// or we haven't tried starting it yet.</value>
		/// ------------------------------------------------------------------------------------
		public static bool IsNotInstalled
		{
			get { return s_fNotInstalled; }
			private set { s_fNotInstalled = value; }
		}
	}
}
