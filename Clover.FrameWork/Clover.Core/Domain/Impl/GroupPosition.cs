using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clover.Core.Domain.Impl
{
    public class GroupPosition : IGroupPosition
    {
        public string GroupId
        {
            get;
            set;
        }

        public string PositionId
        {
            get;
            set;
        }
    }
}
