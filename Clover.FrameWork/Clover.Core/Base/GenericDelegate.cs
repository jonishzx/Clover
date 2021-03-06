﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Core.Base
{

    
    
    
    
    
    public delegate TResult TFunc<TResult>();
    public delegate TResult TFunc<T, TResult>(T arg);
    public delegate TResult TFunc<T1, T2, TResult>(T1 arg1, T2 arg2);
    public delegate TResult TFunc<T1, T2, T3, TResult>(T1 arg1, T2 arg2,T3 arg3);
    public delegate TResult TFunc<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2,T3 arg3,T4 arg4);
    public delegate TResult TFunc<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2,T3 arg3,T4 arg4,T5 arg5);
    public delegate TResult TFunc<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2,T3 arg3,T4 arg4,T5 arg5,T6 arg6);
    public delegate TResult TFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 arg1, T2 arg2,T3 arg3,T4 arg4,T5 arg5,T6 arg6,T7 arg7);
    public delegate TResult TFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 arg1, T2 arg2,T3 arg3,T4 arg4,T5 arg5,T6 arg6,T7 arg7,T8 arg8);
    public delegate TResult TFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(T1 arg1, T2 arg2,T3 arg3,T4 arg4,T5 arg5,T6 arg6,T7 arg7,T8 arg8,T9 arg9);

    
    
    
    public delegate void TAction();
    public delegate void TAction<T>(T arg);
    public delegate void TAction<T1, T2>(T1 arg1, T2 arg2);
    public delegate void TAction<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate void TAction<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate void TAction<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate void TAction<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    public delegate void TAction<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    public delegate void TAction<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    public delegate void TAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);

}
