If you want to build on Linux, you have to copy stdole.dll to the Externals directory.

On Linux LibronixLinker and LibronixLinkerTests build, all tests pass, but Lisaft won't to work
(and there's no Libronix/Logos4 on Linux anyways).

To use LibronixLinker in your code:
===================================

(For full code, see file LiSaFT.cs)

- Subscribe to the LogosPositionHandlerFactory.Created event so you get notified once Logos/Libronix
	becomes available
	
	{
	...
		LogosPositionHandlerFactory.Created += OnLogosPositionHandlerCreated;
	...
	}

	private void OnLogosPositionHandlerCreated(object sender, CreatedEventArgs e)
	{
		m_positionHandler = e.PositionHandler;
		m_positionHandler.PositionChanged += OnPositionInLibronixChanged;
	}
	
- Create an ILogosPositionHandler object by calling LogosPositionHandlerFactory.CreateInstance

		private void InitLibronix()
		{
			if (m_positionHandler != null)
				return;

			m_positionHandler = LogosPositionHandlerFactory.CreateInstance(
				Properties.Settings.Default.StartLibronix, Properties.Settings.Default.LinkSet, 
				true);
			if (m_positionHandler != null)
				OnLogosPositionHandlerCreated(null, new CreatedEventArgs { PositionHandler = m_positionHandler});
		}

- Subscribe to the ILogosPositionHandler.PositionChanged event so you get notified when the
	reference changes in Logos/Libronix
	
		private void OnPositionInLibronixChanged(object sender, PositionChangedEventArgs e)
		{
			Debug.WriteLine("New reference is " + e.BcvRef.ToString());
		}

- To send a reference to Logos, call SetReference

		private void SendReference(int bcvRef)
		{
			if (m_positionHandler != null)
				m_positionHandler.SetReference(bcvRef);
		}

