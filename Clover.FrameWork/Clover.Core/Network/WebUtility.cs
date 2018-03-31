using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Clover.Core.Common;

namespace Clover.Core.NetWork
{
    public class WebUtility
    {
        /// <summary>
        /// 获取当前访问者的IP
        /// </summary>
        /// <returns></returns>
        public static string GetViewerIP()
        {
            /*
             * 一、没有使用代理服务器的情况：

                  REMOTE_ADDR = 您的 IP
                  HTTP_VIA = 没数值或不显示
                  HTTP_X_FORWARDED_FOR = 没数值或不显示

            二、使用透明代理服务器的情况：Transparent Proxies

                  REMOTE_ADDR = 最后一个代理服务器 IP
                  HTTP_VIA = 代理服务器 IP
                  HTTP_X_FORWARDED_FOR = 您的真实 IP ，经过多个代理服务器时，这个值类似如下：203.98.182.163, 203.98.182.163, 203.129.72.215。

               这类代理服务器还是将您的信息转发给您的访问对象，无法达到隐藏真实身份的目的。

            三、使用普通匿名代理服务器的情况：Anonymous Proxies

                  REMOTE_ADDR = 最后一个代理服务器 IP
                  HTTP_VIA = 代理服务器 IP
                  HTTP_X_FORWARDED_FOR = 代理服务器 IP ，经过多个代理服务器时，这个值类似如下：203.98.182.163, 203.98.182.163, 203.129.72.215。

               隐藏了您的真实IP，但是向访问对象透露了您是使用代理服务器访问他们的。

            四、使用欺骗性代理服务器的情况：Distorting Proxies

                  REMOTE_ADDR = 代理服务器 IP
                  HTTP_VIA = 代理服务器 IP
                  HTTP_X_FORWARDED_FOR = 随机的 IP ，经过多个代理服务器时，这个值类似如下：203.98.182.163, 203.98.182.163, 203.129.72.215。

               告诉了访问对象您使用了代理服务器，但编造了一个虚假的随机IP代替您的真实IP欺骗它。

            五、使用高匿名代理服务器的情况：High Anonymity Proxies (Elite proxies)

                  REMOTE_ADDR = 代理服务器 IP
                  HTTP_VIA = 没数值或不显示
                  HTTP_X_FORWARDED_FOR = 没数值或不显示 ，经过多个代理服务器时，这个值类似如下：203.98.182.163, 203.98.182.163, 203.129.72.215。

               完全用代理服务器的信息替代了您的所有信息，就象您就是完全使用那台代理服务器直接访问对象。
             * 
             * 
             * 
             */
            string result = String.Empty;

            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(result))//表示没有使用代理服务器的情况
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (string.IsNullOrEmpty(result)) //使用透明代理服务器的情况
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }

            if (string.IsNullOrEmpty(result) || !StringHelper.IsValidIP(result))
            {
                return "127.0.0.1";
            }

            return result;
        }
        /// <summary>
        /// 将指定的相对路径转化为绝对URL路径
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static string ConvertAbsoulteUrl(string relativeUrl)
        {
            string rtnurl = relativeUrl;

            if (relativeUrl.IndexOf("?") >= 0)
            {
                string url = System.Text.RegularExpressions.Regex.Match(relativeUrl, @"(\S)*\?").Value.Replace("?", "");
                string query = System.Text.RegularExpressions.Regex.Replace(relativeUrl, @"(\S)*\?", "");

                rtnurl = VirtualPathUtility.ToAbsolute(url) + "?" + query;
            }
            else
            {
                try
                {
                    rtnurl = VirtualPathUtility.ToAbsolute(relativeUrl);
                }
                catch
                {
                }

            }
            return rtnurl;
        }

    }
}

