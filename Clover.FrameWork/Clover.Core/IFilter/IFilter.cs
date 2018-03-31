using System;
using System.Text;
using System.Runtime.InteropServices;




namespace Clover.Core.IFilter
{
  [StructLayout(LayoutKind.Sequential)]
  public struct FULLPROPSPEC 
  {
    public Guid guidPropSet;
    public PROPSPEC psProperty;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct FILTERREGION 
  {
    public int idChunk;
    public int cwcStart;
    public int cwcExtent;
  }

  [StructLayout(LayoutKind.Explicit)]
  public struct PROPSPEC
  {
    [FieldOffset(0)] public int ulKind;     
    [FieldOffset(4)] public int propid;    
    [FieldOffset(4)] public IntPtr lpwstr;
  }

  [Flags]
  internal enum IFILTER_FLAGS 
  {
    
    
    
    
    
    
    IFILTER_FLAGS_OLE_PROPERTIES = 1
  }

  
  
  
  
  [Flags]
  internal enum IFILTER_INIT
  {
    NONE = 0,
    
    
    
    
    CANON_PARAGRAPHS = 1,

    
    
    
    
    
    
    
    
    HARD_LINE_BREAKS = 2,

    
    
    
    
    
    
    
    
    CANON_HYPHENS = 4,

    
    
    
    
    
    
    CANON_SPACES = 8,

    
    
    
    
    APPLY_INDEX_ATTRIBUTES = 16,

    
    
    
    
    APPLY_CRAWL_ATTRIBUTES = 256,

    
    
    
    
    APPLY_OTHER_ATTRIBUTES = 32,

    
    
    
    
    
    
    INDEXING_ONLY = 64,

    
    
    
    
    
    
    SEARCH_LINKS = 128,

    
    
    
    FILTER_OWNED_VALUE_OK = 512
  }

  public struct STAT_CHUNK 
  {
    
    
    
    
    
    
    
    
    
    
    public int idChunk;

    
    
    
    
    [MarshalAs(UnmanagedType.U4)]
    public CHUNK_BREAKTYPE breakType;

    
    
    
    
    
    
    
    
    
    
    
    [MarshalAs(UnmanagedType.U4)]
    public CHUNKSTATE flags;

    
    
    
    
    
    
    public int locale;

    
    
    
    
    
    public FULLPROPSPEC attribute;

    
    
    
    
    
    
    
    
    
    
    public int idChunkSource;

    
    
    
    
    public int cwcStartSource;

    
    
    
    
    
    
    
    
    public int cwcLenSource;
  }

  
  
  
  
  public enum CHUNK_BREAKTYPE
  {
    
    
    
    
    CHUNK_NO_BREAK = 0,
    
    
    
    
    
    
    
    CHUNK_EOW = 1,
    
    
    
    
    CHUNK_EOS = 2,
    
    
    
    
    CHUNK_EOP = 3,
    
    
    
    
    CHUNK_EOC = 4
  }


  public enum CHUNKSTATE 
  {
    
    
    
    CHUNK_TEXT = 0x1,
    
    
    
    CHUNK_VALUE = 0x2,
    
    
    
    CHUNK_FILTER_OWNED_VALUE = 0x4
  }

  internal enum IFilterReturnCode : uint 
  {
    
    
    
    S_OK = 0,
    
    
    
    E_ACCESSDENIED = 0x80070005,
    
    
    
    
    E_HANDLE = 0x80070006,
    
    
    
    E_INVALIDARG = 0x80070057,
    
    
    
    E_OUTOFMEMORY = 0x8007000E,
    
    
    
    E_NOTIMPL = 0x80004001,
    
    
    
    E_FAIL = 0x80000008,
    
    
    
    FILTER_E_PASSWORD = 0x8004170B,
    
    
    
    FILTER_E_UNKNOWNFORMAT = 0x8004170C,
    
    
    
    FILTER_E_NO_TEXT = 0x80041705,
    
    
    
    FILTER_E_END_OF_CHUNKS = 0x80041700,
    
    
    
    FILTER_E_NO_MORE_TEXT = 0x80041701,
    
    
    
    FILTER_E_NO_MORE_VALUES = 0x80041702,
    
    
    
    FILTER_E_ACCESS = 0x80041703,
    
    
    
    FILTER_W_MONIKER_CLIPPED = 0x00041704,
    
    
    
    FILTER_E_EMBEDDING_UNAVAILABLE = 0x80041707,
    
    
    
    FILTER_E_LINK_UNAVAILABLE = 0x80041708,
    
    
    
    FILTER_S_LAST_TEXT = 0x00041709,
    
    
    
    FILTER_S_LAST_VALUES = 0x0004170A
  }

  [ComImport, Guid("89BCB740-6119-101A-BCB7-00DD010655AF")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface IFilter
  {
    
    
    
    [PreserveSig]
    IFilterReturnCode Init(
      
      
      
      IFILTER_INIT grfFlags,

      
      
      
      
      
      
      
      
      
      
      
      
      
      
      int cAttributes,

      
      
      
      
      IntPtr aAttributes,

      
      
      out IFILTER_FLAGS pdwFlags);

    
    
    
    
    
    
    [PreserveSig]
    IFilterReturnCode GetChunk(out STAT_CHUNK pStat);

    
    
    
    
    
    [PreserveSig]
    IFilterReturnCode GetText(
      
      
      
      
      ref uint pcwcBuffer,

      
      
      [Out(), MarshalAs(UnmanagedType.LPArray)] 
      char[] awcBuffer);

    
    
    
    
    
    [PreserveSig]
    int GetValue(
      
      
      
      
      
      
      
      
      ref IntPtr PropVal);

    
    
    
    
    
    [PreserveSig]
    int BindRegion(ref FILTERREGION origPos,
      ref Guid riid, ref object ppunk);
  }


}
