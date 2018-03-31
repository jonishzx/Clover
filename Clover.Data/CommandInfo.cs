using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Clover.Data
{
    public enum EffentNextType
    {
        
        
        
        None,

        
        
        
        WhenHaveContine,

        
        
        
        WhenNoHaveContine,

        
        
        
        ExcuteEffectRows,

        
        
        
        SolicitationEvent
    }

    public class CommandInfo
    {
        public string CommandText;
        public EffentNextType EffentNextType = EffentNextType.None;
        public object OriginalData = null;
        public DbParameter[] Parameters;
        public object ShareObject = null;

        public CommandInfo()
        {
        }

        public CommandInfo(string sqlText, SqlParameter[] para)
        {
            CommandText = sqlText;
            Parameters = para;
        }

        public CommandInfo(string sqlText, SqlParameter[] para, EffentNextType type)
        {
            CommandText = sqlText;
            Parameters = para;
            EffentNextType = type;
        }

        private event EventHandler _solicitationEvent;

        public event EventHandler SolicitationEvent
        {
            add { _solicitationEvent += value; }
            remove { _solicitationEvent -= value; }
        }

        public void OnSolicitationEvent()
        {
            if (_solicitationEvent != null)
            {
                _solicitationEvent(this, new EventArgs());
            }
        }
    }
}