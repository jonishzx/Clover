using System;
using System.Collections.Generic;

namespace Clover.Core.Domain
{
    public class AnonymousAccount : IAccount
    {
        public static readonly string AnonymousID = "AnonymousID";

        public static readonly string AnonymousUserName = "AnonymousUser";

        public string UniqueId
        {
            get
            {
                return AnonymousAccount.AnonymousID;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public string AccountCode
        {
            get
            {
                return AnonymousAccount.AnonymousID;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public string CurrGroupId
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public string CurrPositionId
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public string UserName
        {
            get
            {
                return AnonymousAccount.AnonymousUserName;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public string Permission
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public string CurrGroupName
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public string CurrPositionName
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public string Email
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public string MappingAccount
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public IRole[] Roles
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public System.Collections.Generic.IList<string> GroupIds
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public System.Collections.Generic.IList<IGroupPosition> GroupPositions
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public string CurrRoleId
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public string CurrRoleName
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
        public string EmpType
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
        public string CarBrandCode
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
        public string AreaCode
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
        public int? VendorId
        {
            get { return null; }
            set
            {
                throw new System.NotImplementedException();
            }
        }
        public string CarBrandName
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
        public string AreaName
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
        public string VendorCode
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
        public string VendorName
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
        public string EmpTypeName
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
        public string HeadOfficeFlg { get; set; }
    }
}
