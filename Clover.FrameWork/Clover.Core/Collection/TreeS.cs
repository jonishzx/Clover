using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Core.Collection
{
    
    
    
    public interface ISNode
    {
        string Id { get; set; }
        String Name { get; set; }
        string ParentId { get; set; }

        int ViewOrder { get; set; }
    }

    
    
    
    public interface ISNodeBinder
    {
        String Bind(ISNode TreeNode);
    }

    #region TreeNode 泛型

    [Serializable]
    
        
        
        
    public class TreeNode<T> : IComparable, IComparable<T> where T : ISNode
    {
        private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();
        private readonly T _rawNode;
        private int _depth = -1;
        private TreeNode<T> _nextNode;
        private TreeNode<T> _parent;
        private TreeNode<T> _prevNode;
        private Tree<T> _tree;

        public TreeNode()
        {
        }

        public TreeNode(T TreeNode)
        {
            _rawNode = TreeNode;
        }

        
        
        
        
        public T getNode()
        {
            return _rawNode;
        }

        

        
        
        
        
        public void setTree(Tree<T> tree)
        {
            _tree = tree;
        }

        
        
        
        
        public TreeNode<T> getParent()
        {
            if (string.IsNullOrEmpty(getNode().ParentId)) return null;
            if (_parent == null)
            {
                _parent = _tree.FindById(getNode().ParentId);
            }
            return _parent;
        }

        
        
        
        
        public int getDepth()
        {
            if (_depth < 0)
            {
                if (string.IsNullOrEmpty(getNode().ParentId))
                {
                    _depth = 0;
                }
                else
                {
                    _depth = getParent().getDepth() + 1;
                }
            }

            return _depth;
        }

        
        
        
        
        public List<TreeNode<T>> getChildren()
        {
            return _children;
        }

        internal void addChildren(TreeNode<T> TreeNode)
        {
            _children.Add(TreeNode);
        }

        

        
        
        
        
        public TreeNode<T> getPrev()
        {
            return _prevNode;
        }

        
        
        
        
        public TreeNode<T> getNext()
        {
            return _nextNode;
        }

        
        
        
        
        internal void setPrev(TreeNode<T> TreeNode)
        {
            _prevNode = TreeNode;
        }

        
        
        
        
        internal void setNext(TreeNode<T> TreeNode)
        {
            _nextNode = TreeNode;
        }

        
        
        
        
        internal Boolean indent()
        {
            if (getPrev() == null) return true;
            return getDepth() > getPrev().getDepth();
        }

        
        
        
        
        internal Boolean outdent()
        {
            if (getPrev() == null) return false;
            return getDepth() < getPrev().getDepth();
        }

        
        
        
        
        internal int getOutdentCount()
        {
            return getPrev().getDepth() - getDepth();
        }

        #region IComparable<T> 成员

        public int CompareTo(T other)
        {
            return _rawNode.ViewOrder - other.ViewOrder;
        }

        #endregion

        #region IComparable 成员

        public int CompareTo(object obj)
        {
            return _rawNode.ViewOrder - ((TreeNode<T>) obj)._rawNode.ViewOrder;
        }

        #endregion
    }

    #endregion

    #region Tree

    [Serializable]
    
        
        
        
    public class Tree<T> where T : ISNode
    {
        private readonly List<T> _rawList;
        private List<TreeNode<T>> _allOrdered;
        private List<T> _allOrderedNode;

        
        
        
        
        public Tree(List<T> nodeList)
        {
            _rawList = nodeList;
            initProxyList();
        }

        
        
        
        
        
        
        public TreeNode<T> FindById(string id)
        {
            return getById(id);
        }

        
        
        
        
        
        public TreeNode<T> FindParent(string id)
        {
            TreeNode<T> proxy = getById(id);
            if (proxy == null) return default(TreeNode<T>);
            return proxy.getParent();
        }

        
        
        
        
        
        public List<TreeNode<T>> FindPath(string id)
        {
            var nodePath = new List<TreeNode<T>>();

            TreeNode<T> proxy = getById(id);
            if (proxy == null) return nodePath;

            nodePath.Add(proxy);

            TreeNode<T> currentNode = proxy;
            while (true)
            {
                TreeNode<T> parent = currentNode.getParent();
                if (parent == null) break;

                nodePath.Add(parent);
                currentNode = parent;
            }

            nodePath.Reverse();

            return nodePath;
        }

        
        
        
        
        public List<TreeNode<T>> FindRoot()
        {
            return getRoot();
        }

        
        
        
        
        
        public List<TreeNode<T>> FindChildren(string id)
        {
            TreeNode<T> proxy = getById(id);
            if (proxy == null) return new List<TreeNode<T>>();
            return proxy.getChildren();
        }

        
        
        
        
        public List<TreeNode<T>> FindAllOrdered()
        {
            if (_allOrdered == null)
            {
                var results = new List<TreeNode<T>>();
                List<TreeNode<T>> roots = FindRoot();
                foreach (var TreeNode in roots)
                {
                    addSubProxyNodes(results, TreeNode);
                }

                _allOrdered = results;
            }
            return _allOrdered;
        }


        
        
        
        
        
        
        public T GetById(string id)
        {
            TreeNode<T> proxy = getById(id);
            if (proxy != null) return proxy.getNode();
            return default(T);
        }

        
        
        
        
        
        public int GetDepth(string id)
        {
            TreeNode<T> proxy = getById(id);
            if (proxy != null) return proxy.getDepth();
            return 0;
        }

        
        
        
        
        
        public T GetParent(string id)
        {
            TreeNode<T> proxy = getById(id);
            if (proxy == null) return default(T);
            TreeNode<T> parentProxy = proxy.getParent();
            if (parentProxy != null) return parentProxy.getNode();
            return default(T);
        }

        
        
        
        
        
        public List<T> GetPath(string id)
        {
            var nodePath = new List<T>();

            TreeNode<T> proxy = getById(id);
            if (proxy == null) return nodePath;

            nodePath.Add(proxy.getNode());

            TreeNode<T> tempNode = proxy;
            while (true)
            {
                TreeNode<T> parent = tempNode.getParent();
                if (parent == null) break;

                nodePath.Add(parent.getNode());
                tempNode = parent;
            }

            nodePath.Reverse();

            return nodePath;
        }

        
        
        
        
        
        public List<T> GetChildren(string id)
        {
            TreeNode<T> proxy = getById(id);
            if (proxy == null) return new List<T>();
            List<TreeNode<T>> children = proxy.getChildren();
            var results = new List<T>();
            foreach (var px in children) results.Add(px.getNode());
            results.Sort();

            return results;
        }

        public List<T> GetRoot()
        {
            List<TreeNode<T>> nodes = FindRoot();
            var results = new List<T>();
            foreach (var px in nodes)
            {
                results.Add(px.getNode());
            }
            return results;
        }


        public List<T> GetAllOrdered()
        {
            if (_allOrderedNode == null)
            {
                List<TreeNode<T>> nodes = FindAllOrdered();
                var results = new List<T>();
                foreach (var px in nodes)
                {
                    results.Add(px.getNode());
                }

                _allOrderedNode = results;
            }
            return _allOrderedNode;
        }

        private TreeNode<T> getById(string id)
        {
            if (getIdCache().ContainsKey(id) == false) return default(TreeNode<T>);

            return getIdCache()[id];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var root in _roots)
            {
                RecursiveRenderString(sb, root);
            }

            return sb.ToString();
        }

        private static string Ctrem(int count)
        {
            string a = "-";
            for (int i = 0; i < count; i++)
            {
                a += "-";
            }

            return a;
        }

        private static void RecursiveRenderString(StringBuilder sb, TreeNode<T> node)
        {
            sb.AppendLine(string.Format("{2} 节点 {0}({1})", node.getNode().Name, node.getNode().Id,
                                        Ctrem(node.getDepth())));

            if (node.getChildren().Count > 0)
            {
                foreach (var treeNode in node.getChildren())
                {
                    RecursiveRenderString(sb, treeNode);
                }
            }
        }

        #region private method

        private Dictionary<string, TreeNode<T>> _idcache;

        
        
        
        private List<TreeNode<T>> _proxyList;

        private List<TreeNode<T>> _roots;

        
        
        
        private void initProxyList()
        {
            _proxyList = new List<TreeNode<T>>();
            _idcache = new Dictionary<string, TreeNode<T>>();
            _roots = new List<TreeNode<T>>();

            
            foreach (T TreeNode in _rawList)
            {
                var proxy = new TreeNode<T>(TreeNode);
                proxy.setTree(this);
                _proxyList.Add(proxy);
                _idcache.Add(TreeNode.Id, proxy);
            }

            _proxyList.Sort();

            
            foreach (var TreeNode in _proxyList)
            {
                if (string.IsNullOrEmpty(TreeNode.getNode().ParentId) || TreeNode.getParent() == null) 
                {
                    _roots.Add(TreeNode);
                }
                else
                {
                    
                    TreeNode.getParent().addChildren(TreeNode);
                }
            }

            
            List<TreeNode<T>> orderedList = FindAllOrdered();

            for (int i = 0; i < orderedList.Count; i++)
            {
                if (i == 0) continue;

                TreeNode<T> TreeNode = orderedList[i];
                TreeNode<T> pre = orderedList[i - 1];

                TreeNode.setPrev(pre);
                pre.setNext(TreeNode);
            }
        }

        
        
        
        
        
        private void addSubProxyNodes(List<TreeNode<T>> results, TreeNode<T> parentnode)
        {
            results.Add(parentnode);
            List<TreeNode<T>> subnodes = parentnode.getChildren();
            subnodes.Sort();
            foreach (var TreeNode in subnodes)
            {
                addSubProxyNodes(results, TreeNode);
            }
        }

        private List<TreeNode<T>> getNodeList()
        {
            return _proxyList;
        }

        private Dictionary<string, TreeNode<T>> getIdCache()
        {
            return _idcache;
        }

        private List<TreeNode<T>> getRoot()
        {
            return _roots;
        }

        #endregion

        #region json output

        
        
        
        
        
        
        
        
        public string ToJsonString(string childrenMark, string idmark, string namemark, bool expandall)
        {
            var sb = new StringBuilder();
            sb.Append("[ ");
            foreach (var root in _roots)
            {
                RecursiveRenderJsonStr(sb, childrenMark, idmark, namemark, expandall, root);
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
        }

        public string ToJsonStringWithRoot(string childrenMark, string idmark, string namemark, bool expandall)
        {
            var sb = new StringBuilder();
            sb.Append("[{name:'根', Status:'5', children:[ ");
            foreach (var root in _roots)
            {
                RecursiveRenderJsonStr(sb, childrenMark, idmark, namemark, expandall, root);
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]}]");
            return sb.ToString();
        }

        private static void RecursiveRenderJsonStr(StringBuilder sb, string childrenMark, string idmark, string namemark,
                                                   bool expandall, TreeNode<T> node)
        {
            sb.Append("{ id:\"" + node.getNode().Id + "\"," + namemark + ":\"" + node.getNode().Name + "\", " + idmark +
                      ":\"" + node.getNode().Id + "\"");
            if (expandall)
                sb.Append(",open:true ");

            if (node.getChildren().Count <= 0)
            {
                sb.Append("},");
                return;
            }
            sb.Append("," + childrenMark + ":[ ");
            foreach (var treeNode in node.getChildren())
            {
                RecursiveRenderJsonStr(sb, childrenMark, idmark, namemark, expandall, treeNode);
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");

            sb.Append("},");
        }

        #endregion

        
    }

    #endregion
}