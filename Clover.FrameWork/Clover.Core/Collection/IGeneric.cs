#if NET1
#else

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Clover.Core.Collection
{
    public interface IGenericCollection<T> : ICollection<T>, IComparable
    {
        
        
        
        int FixedSize { get;}

        
        
        
        bool IsEmpty { get;}

        
        
        
        bool IsFull { get;}

        
        
        
        string Version { get;}

        
        
        
        string Author { get;}
    }

    public interface IBinFenVisitor<T>
    {
        
        
        
        bool HasDone { get; }

        
        
        
        void Visit(T obj);
    }


}


#endif