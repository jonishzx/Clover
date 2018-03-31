using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clover.Component.Excel.Common.Validate
{
    public class OperationState
    {
         public bool Result { set; get; }

        public List<String> MessageList
        {
            set
            {
                messageList = value;
            }

            get
            {
                return messageList;
            }
        }

        public List<String> FullMessageList
        {
            set
            {
                fullMessageList = value;
            }

            get
            {
                return fullMessageList;
            }
        }

        private List<String> fullMessageList;
        private List<String> messageList;

        public OperationState()
        {
            Result = true;
            messageList = new List<string>();
            fullMessageList = new List<string>();
        }

        public void AddOperationState(OperationState result)
        {
            this.Result = this.Result && result.Result;

            for (int i = 0; i < result.MessageList.Count; i++)
            {
                this.MessageList.Add(result.MessageList[i]);
            }
        }
    }
}
