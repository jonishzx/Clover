using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Clover.Core.IFilter
{
  [ComVisible(false)]
  [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000001-0000-0000-C000-000000000046")]
  internal interface IClassFactory
  {
    void CreateInstance([MarshalAs(UnmanagedType.Interface)] object pUnkOuter, ref Guid refiid, [MarshalAs(UnmanagedType.Interface)] out object ppunk);
    void LockServer(bool fLock);
  }

  
  
  
  
  internal static class ComHelper
  {
    
    private delegate int DllGetClassObject(ref Guid ClassId, ref Guid InterfaceId, [Out, MarshalAs(UnmanagedType.Interface)] out object ppunk);

    
    private class Win32NativeMethods
    {
      [DllImport("kernel32.dll", CharSet=CharSet.Ansi)]
      public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

      [DllImport("kernel32.dll")]
      public static extern bool FreeLibrary(IntPtr hModule);

      [DllImport("kernel32.dll")]
      public static extern IntPtr LoadLibrary(string lpFileName);
    }

    
    
    
    
    private class DllList
    {
      private List<IntPtr> _dllList=new List<IntPtr>();
      public void AddDllHandle(IntPtr dllHandle)
      {
        lock (_dllList)
        {
          _dllList.Add(dllHandle);
        }
      }

      ~DllList()
      {
        foreach (IntPtr dllHandle in _dllList)
        {
          try
          {
            Win32NativeMethods.FreeLibrary(dllHandle);
          }
          catch { };
        }
      }
    }

    static DllList _dllList=new DllList();

    
    
    
    
    
    
    internal static IClassFactory GetClassFactory(string dllName, string filterPersistClass)
    {
      
      IClassFactory classFactory=GetClassFactoryFromDll(dllName, filterPersistClass);
      return classFactory;
    }

    private static IClassFactory GetClassFactoryFromDll(string dllName, string filterPersistClass)
    {
      
      IntPtr dllHandle=Win32NativeMethods.LoadLibrary(dllName);
      if (dllHandle==IntPtr.Zero)
        return null;

      
      _dllList.AddDllHandle(dllHandle);

      
      IntPtr dllGetClassObjectPtr=Win32NativeMethods.GetProcAddress(dllHandle, "DllGetClassObject");
      if (dllGetClassObjectPtr==IntPtr.Zero)
        return null;

      
      DllGetClassObject dllGetClassObject=(DllGetClassObject)Marshal.GetDelegateForFunctionPointer(dllGetClassObjectPtr, typeof(DllGetClassObject));

      
      Guid filterPersistGUID=new Guid(filterPersistClass);
      Guid IClassFactoryGUID=new Guid("00000001-0000-0000-C000-000000000046"); 
      Object unk;
      if (dllGetClassObject(ref filterPersistGUID, ref IClassFactoryGUID, out unk)!=0)
        return null;

      
      return (unk as IClassFactory);
    }
  }
}
