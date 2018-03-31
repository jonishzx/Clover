namespace Clover.Core.Collection
{
    #region 引用的命名空间.
    

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Collections;
    using System.Diagnostics;

    
    #endregion

    

    
    
    
    
    
    
    
    
    [Serializable]
    [DebuggerDisplay(@"Count = {Count}")]
    public class Set<T> :
        ICollection<T>,
        IEnumerable<T>,
        ICollection,
        IEnumerable
    {
        #region 私有 dummy 类.
        

        
        
        
        [DebuggerDisplay(@"")]
        private struct Dummy
        {
        }

        
        #endregion

        #region 私有变量.
        

        
        
        
        private static Dummy dummy = new Dummy();

        
        
        
        private Dictionary<T, Dummy> data;

        
        #endregion

        #region 公共方法.
        

        
        
        
        public Set()
        {
            data = new Dictionary<T, Dummy>();
        }

        
        
        
        
        public Set(
            int capacity)
        {
            data = new Dictionary<T, Dummy>(capacity);
        }

        
        
        
        
        public Set(
            Set<T> original)
        {
            if (original == null)
            {
                data = new Dictionary<T, Dummy>();
            }
            else
            {
                data = new Dictionary<T, Dummy>(original.data);
            }
        }

        
        
        
        
        public void Sort()
        {
            List<T> temp = new List<T>(this);

            temp.Sort();

            this.Clear();
            this.AddRange(temp);
        }

        
        
        
        
        
        public void Sort(
            Comparison<T> comparison)
        {
            List<T> temp = new List<T>(this);

            temp.Sort(comparison);

            this.Clear();
            this.AddRange(temp);
        }

        
        
        
        
        public void Sort(
            IComparer<T> comparer)
        {
            List<T> temp = new List<T>(this);

            temp.Sort(comparer);

            this.Clear();
            this.AddRange(temp);
        }

        
        
        
        
        
        
        
        public void Sort(
            int index,
            int count,
            IComparer<T> comparer)
        {
            List<T> temp = new List<T>(this);

            temp.Sort(index, count, comparer);

            this.Clear();
            this.AddRange(temp);
        }

        
        
        
        
        
        public T[] ToArray()
        {
            T[] result = new T[data.Keys.Count];

            data.Keys.CopyTo(result, 0);

            return result;
        }

        
        
        
        
        public Set(
            IEnumerable<T> original)
        {
            data = new Dictionary<T, Dummy>();

            if (original != null)
            {
                AddRange(original);
            }
        }

        
        
        
        
        public void Add(
            T a)
        {
            data[a] = dummy;
        }

        
        
        
        
        public void AddRange(
            IEnumerable<T> range)
        {
            if (range != null)
            {
                foreach (T a in range)
                {
                    Add(a);
                }
            }
        }

        
        
        
        
        public void RemoveRange(
            IEnumerable<T> range)
        {
            if (range != null)
            {
                foreach (T a in range)
                {
                    Remove(a);
                }
            }
        }

        
        
        
        
        
        public Set<U> ConvertAll<U>(
            Converter<T, U> converter)
        {
            Set<U> result = new Set<U>(this.Count);
            foreach (T element in this)
            {
                result.Add(converter(element));
            }

            return result;
        }

        
        
        
        
        
        public bool TrueForAll(
            Predicate<T> predicate)
        {
            foreach (T element in this)
            {
                if (!predicate(element))
                {
                    return false;
                }
            }
            return true;
        }

        
        
        
        
        
        public Set<T> FindAll(
            Predicate<T> predicate)
        {
            Set<T> result = new Set<T>();

            foreach (T element in this)
            {
                if (predicate(element))
                {
                    result.Add(element);
                }
            }
            return result;
        }

        
        
        
        
        
        
        
        public T Find(
            Predicate<T> predicate)
        {
            foreach (T element in this)
            {
                if (predicate(element))
                {
                    return element;
                }
            }

            return default(T);
        }

        
        
        
        
        
        
        
        
        
        
        public bool Find(
            out T foundElement,
            Predicate<T> predicate)
        {
            foreach (T element in this)
            {
                if (predicate(element))
                {
                    foundElement = element;
                    return true;
                }
            }

            foundElement = default(T);
            return false;
        }

        
        
        
        
        public void ForEach(
            Action<T> action)
        {
            foreach (T element in this)
            {
                action(element);
            }
        }

        
        
        
        
        
        public void Clear()
        {
            data.Clear();
        }

        
        
        
        
        
        
        
        public bool Contains(
            T a)
        {
            return data.ContainsKey(a);
        }

        
        
        
        
        
        public void CopyTo(
            T[] array,
            int index)
        {
            data.Keys.CopyTo(array, index);
        }

        
        
        
        
        
        public bool Remove(
            T a)
        {
            return data.Remove(a);
        }

        
        
        
        
        
        
        public IEnumerator<T> GetEnumerator()
        {
            return data.Keys.GetEnumerator();
        }

        
        
        
        
        
        
        public static Set<T> operator |(
            Set<T> a,
            Set<T> b)
        {
            if (a == null)
            {
                return b;
            }
            else if (b == null)
            {
                return a;
            }
            else
            {
                Set<T> result = new Set<T>(a);
                result.AddRange(b);
                return result;
            }
        }

        
        
        
        
        
        public Set<T> Union(
            IEnumerable<T> b)
        {
            if (b == null)
            {
                return this;
            }
            else
            {
                return this | new Set<T>(b);
            }
        }

        
        
        
        
        
        
        
        public override bool Equals(
            object obj)
        {
            Set<T> a = this;
            Set<T> b = obj as Set<T>;

            if ((object)b == null)
            {
                return false;
            }
            else
            {
                return a == b;
            }
        }

        
        
        
        
        
        
        public override int GetHashCode()
        {
            int hashcode = 0;
            foreach (T element in this)
            {
                hashcode ^= element.GetHashCode();
            }

            return hashcode;
        }

        
        
        
        
        
        
        
        
        
        void ICollection.CopyTo(
            Array array,
            int index)
        {
            ((ICollection)data.Keys).CopyTo(array, index);
        }

        
        
        
        
        
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)data.Keys).GetEnumerator();
        }

        
        
        
        
        
        public Set<T> Intersection(
            IEnumerable<T> b)
        {
            if (b == null)
            {
                return null;
            }
            else
            {
                return this & new Set<T>(b);
            }
        }

        
        
        
        
        
        public Set<T> Difference(
            IEnumerable<T> b)
        {
            if (b == null)
            {
                return this;
            }
            else
            {
                return this - new Set<T>(b);
            }
        }

        
        
        
        
        
        public Set<T> SymmetricDifference(
            IEnumerable<T> b)
        {
            if (b == null)
            {
                return this;
            }
            else
            {
                return this ^ new Set<T>(b);
            }
        }

        
        #endregion

        #region 公共属性.
        

        
        
        
        
        
        
        
        public int Count
        {
            get
            {
                return data.Count;
            }
        }

        
        
        
        
        public bool IsEmpty
        {
            get
            {
                return data == null || data.Count <= 0;
            }
        }

        
        
        
        
        
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        
        
        
        
        public static Set<T> Empty
        {
            get
            {
                return new Set<T>(0);
            }
        }

        
        
        
        
        
        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)data.Keys).SyncRoot;
            }
        }

        
        
        
        
        
        bool ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)data.Keys).IsSynchronized;
            }
        }

        
        #endregion

        #region 公共操作符.
        

        
        
        
        public static Set<T> operator &(
            Set<T> a,
            Set<T> b)
        {
            Set<T> result = new Set<T>();

            if (a != null)
            {
                foreach (T element in a)
                {
                    if (b != null && b.Contains(element))
                    {
                        result.Add(element);
                    }
                }
            }

            return result;
        }

        
        
        
        
        
        
        public static Set<T> operator -(
            Set<T> a,
            Set<T> b)
        {
            Set<T> result = new Set<T>();

            if (a != null)
            {
                foreach (T element in a)
                {
                    if (b == null || !b.Contains(element))
                    {
                        result.Add(element);
                    }
                }
            }

            return result;
        }

        
        
        
        
        
        
        public static Set<T> operator ^(
            Set<T> a,
            Set<T> b)
        {
            Set<T> result = new Set<T>();

            if (a != null)
            {
                foreach (T element in a)
                {
                    if (b == null || !b.Contains(element))
                    {
                        result.Add(element);
                    }
                }
            }

            if (a != null)
            {
                foreach (T element in b)
                {
                    if (a == null || !a.Contains(element))
                    {
                        result.Add(element);
                    }
                }
            }

            return result;
        }

        
        
        
        public static bool operator <=(
            Set<T> a,
            Set<T> b)
        {
            if (a == null && b == null)
            {
                return false;
            }
            else if (a == null)
            {
                return true;
            }
            else
            {
                foreach (T element in a)
                {
                    if (b == null || !b.Contains(element))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        
        
        
        public static bool operator <(
            Set<T> a,
            Set<T> b)
        {
            if (a == null && b == null)
            {
                return false;
            }
            else if (a == null)
            {
                return true;
            }
            else if (b == null)
            {
                return false;
            }
            else
            {
                return (a.Count < b.Count) && (a <= b);
            }
        }

        
        
        
        
        
        
        public static bool operator ==(
            Set<T> a,
            Set<T> b)
        {
            if ((a as object) == null && (b as object) == null)
            {
                return true;
            }
            else if ((a as object) == null)
            {
                return false;
            }
            else if ((b as object) == null)
            {
                return false;
            }
            else
            {
                return (a.Count == b.Count) && (a <= b);
            }
        }

        
        
        
        
        
        
        public static bool operator >(Set<T> a, Set<T> b)
        {
            return b < a;
        }

        
        
        
        
        
        
        public static bool operator >=(Set<T> a, Set<T> b)
        {
            return (b <= a);
        }

        
        
        
        
        
        
        public static bool operator !=(Set<T> a, Set<T> b)
        {
            return !(a == b);
        }

        
        #endregion
    }

    
}
