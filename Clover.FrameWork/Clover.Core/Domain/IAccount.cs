using System;
using System.Collections.Generic;

namespace Clover.Core.Domain
{
    public interface IAccount
    {
        /// <summary>
        /// 用户主键
        /// </summary>
        string UniqueId
        {
            get;
            set;
        }
        /// <summary>
        /// 登录名称
        /// </summary>
        string AccountCode
        {
            get;
            set;
        }
        /// <summary>
        /// 当前部门id
        /// </summary>
        string CurrGroupId
        {
            get;
            set;
        }
        /// <summary>
        /// 当前岗位id
        /// </summary>
        string CurrPositionId
        {
            get;
            set;
        }
        /// <summary>
        /// 当前角色id
        /// </summary>
        string CurrRoleId
        {
            get;
            set;
        }
        /// <summary>
        /// 当前角色名称
        /// </summary>
        string CurrRoleName
        {
            get;
            set;
        }
        /// <summary>
        /// 部门列表
        /// </summary>
        IList<string> GroupIds
        {
            get;
            set;
        }
        /// <summary>
        /// 部门岗位列表
        /// </summary>
        IList<IGroupPosition> GroupPositions
        {
            get;
            set;
        }
        /// <summary>
        /// 当前所在部门名称
        /// </summary>
        string CurrGroupName
        {
            get;
            set;
        }
        /// <summary>
        /// 当前岗位名称
        /// </summary>
        string CurrPositionName
        {
            get;
            set;
        }
        /// <summary>
        /// 用户名称
        /// </summary>
        string UserName
        {
            get;
            set;
        }
        /// <summary>
        /// 权限
        /// </summary>
        string Permission
        {
            get;
            set;
        }
        /// <summary>
        /// 角色列表
        /// </summary>
        IRole[] Roles
        {
            get;
            set;
        }
        /// <summary>
        /// 邮箱
        /// </summary>
        string Email
        {
            get;
            set;
        }
        /// <summary>
        /// 映射账户
        /// </summary>
        string MappingAccount
        {
            get;
            set;
        }

        /// <summary>
        /// 人员类别
        /// </summary>
        string EmpType
        {
            get;
            set;
        }

        /// <summary>
        /// 渠道（厂牌）编码
        /// </summary>
        string CarBrandCode
        {
            get;
            set;
        }

        /// <summary>
        /// 区域编码
        /// </summary>
        string AreaCode
        {
            get;
            set;
        }

        /// <summary>
        /// 门店Id
        /// </summary>
        int? VendorId
        {
            get;
            set;
        }

        /// <summary>
        /// 登录类别名称
        /// </summary>
        string EmpTypeName { get; set; }

        /// <summary>
        /// 渠道（厂牌）名称
        /// </summary>
        string CarBrandName { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        string AreaName { get; set; }

        /// <summary>
        /// 门店编码
        /// </summary>
        string VendorCode { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        string VendorName { get; set; }

        /// <summary>
        /// 是否总店
        /// </summary>
        string HeadOfficeFlg { get; set; }
    }
}
