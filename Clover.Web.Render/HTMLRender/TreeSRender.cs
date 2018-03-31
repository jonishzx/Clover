using System;
using System.Collections.Generic;
using System.Text;

using Clover.Config.Menu;
using Clover.Core.Collection;

namespace Clover.Web.HTMLRender
{
    /// <summary>
    /// 数结构输出
    /// </summary>
    public class TreeSRender
    {
        /*
         *  <li>
        <span>Folder</span>
        <ul>
            <li>
                <span>Sub Folder 1</span>
                <ul>
                    <li>
                        <span><a href="#">File 11</a></span>
                    </li>
                    <li>
                        <span>File 12</span>
                    </li>
                    <li>
                        <span>File 13</span>
                    </li>
                </ul>
            </li>
            <li>
                <span>File 2</span>
            </li>
            <li>
                <span>File 3</span>
            </li>
        </ul>
    </li>
    <li>
        <span>File21</span>
    </li>
         */
        /// <summary>
        /// 输出指定节点的子节点
        /// </summary>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public static string RenderNodeS(Tree<MenuItem> tree, string parentid)
        {
            StringBuilder sb = new StringBuilder(500);
         
            rcRenderChildren(tree.FindChildren(parentid), sb, parentid, true);
        
            return sb.ToString();
        }
       
        private static void rcRenderChildren(List<TreeNode<MenuItem>> nodes, StringBuilder sb, string parentid, bool first)
        {
            if (first)
                sb.AppendLine("<ul class=\"tree\">");
            else
                sb.AppendLine("<ul>");
          
            foreach (TreeNode<MenuItem> item in nodes)
            {
                List<TreeNode<MenuItem>> children = item.getChildren();
               
                MenuItem it = item.getNode();

                sb.AppendLine("<li>");
                sb.AppendLine("<span><a href=\"" + it.Url + "\" target=\"" + it.Target + "\" >"
                        + it.Name + "</a></span>");

                if (children.Count > 0)
                {
                    rcRenderChildren(children, sb, parentid, false);
                }

                sb.AppendLine("</li>");
            }

            sb.AppendLine("</ul>");
        }
    }
}
