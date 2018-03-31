using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;

namespace Clover.Core.Base
{
    
    
    
    public class SingleRunMethod<T1, T2>
    {
        static readonly object sync = new object();

        int _timeoutmillis;

        Timer m_timer = null;
        Dictionary<T1, T2> _methods = new Dictionary<T1, T2>();

        TAction<object> _methodHandler = null;

        
        
        
        
        public SingleRunMethod(TAction<object> watchHandler) : this(watchHandler, 100)
        {
        }

        
        
        
        
        
        public SingleRunMethod(TAction<object> watchHandler, int timerInterval)
        {
            m_timer = new System.Threading.Timer(new TimerCallback(OnTimer), null, Timeout.Infinite, Timeout.Infinite);
            _timeoutmillis = timerInterval;
            _methodHandler = watchHandler;
        }

        
        
        
        
        
        public void OnExecute(T1 objectID, T2 value)
        {
            lock (sync)
            {
                if (!_methods.ContainsKey(objectID))
                {
                    _methods.Add(objectID, value);
                }
            }
            m_timer.Change(_timeoutmillis, Timeout.Infinite);
        }

        private void OnTimer(object state)
        {
            Dictionary<T1, T2> backup;

            lock (sync)
            {
                backup = new Dictionary<T1, T2>(_methods);
                _methods.Clear();
            }

            foreach (KeyValuePair<T1, T2> kvp in backup)
            {
                _methodHandler(kvp.Value);
            }
        }
    }
}
