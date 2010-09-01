// Stephen Toub
// Slightly modified; based on http://msdn.microsoft.com/en-us/magazine/cc163417.aspx

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace NetMatters
{
    public class MessageReceivedEventArgs : EventArgs
    {
        private readonly Message m_message;
        public MessageReceivedEventArgs(Message message)
        {
        	m_message = message;
        }

    	public Message Message
    	{
    		get { return m_message; }
    	}
    }

    public static class MessageEvents
    {
        private static readonly object m_lock = new object();
        private static MessageWindow m_window;
        private static IntPtr m_windowHandle;
    	private static SynchronizationContext Context { get; set; }

    	public static event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public static void WatchMessage(int message)
        {
            EnsureInitialized();
            m_window.RegisterEventForMessage(message);
        }

        public static IntPtr WindowHandle
        {
            get
            {
                EnsureInitialized();
                return m_windowHandle;
            }
        }

        private static void EnsureInitialized()
        {
            lock (m_lock)
            {
            	if (m_window != null) 
					return;

            	Context = AsyncOperationManager.SynchronizationContext;
            	using (ManualResetEvent mre = new ManualResetEvent(false))
            	{
            		Thread t = new Thread((ThreadStart)delegate
                       	{
                       		m_window = new MessageWindow();
                       		m_windowHandle = m_window.Handle;
                       		mre.Set();
                       		Application.Run();
                       	})
            		           	{
            		           		Name = "MessageEvents message loop", 
									IsBackground = true
            		           	};
            		t.Start();

            		mre.WaitOne();
            	}
            }
        }

        private class MessageWindow : Form
        {
            private readonly ReaderWriterLock m_lock = new ReaderWriterLock();
            private readonly Dictionary<int, bool> m_messageSet = new Dictionary<int, bool>();

            public void RegisterEventForMessage(int messageID)
            {
                m_lock.AcquireWriterLock(Timeout.Infinite);
                m_messageSet[messageID] = true;
                m_lock.ReleaseWriterLock();
            }

            protected override void WndProc(ref Message m)
            {
                m_lock.AcquireReaderLock(Timeout.Infinite);
                bool handleMessage = m_messageSet.ContainsKey(m.Msg);
                m_lock.ReleaseReaderLock();

                if (handleMessage)
                {
                    MessageEvents.Context.Post(delegate(object state)
                    {
                        EventHandler<MessageReceivedEventArgs> handler = 
							MessageEvents.MessageReceived;
                        if (handler != null) 
							handler(null, new MessageReceivedEventArgs((Message)state));
                    }, m);
                }

                base.WndProc(ref m);
            }
        }
    }
}