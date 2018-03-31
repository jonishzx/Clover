using System;
using System.Data;

namespace Clover.Data
{
    
    
    
    public class SafeDataReader : IDataReader
    {
        private readonly IDataReader _dataReader;

        
        
        
        
        
        public SafeDataReader(IDataReader dataReader)
        {
            _dataReader = dataReader;
        }

        
        
        
        
        
        protected IDataReader DataReader
        {
            get { return _dataReader; }
        }

        
        
        
        
        
        
        
        public virtual string GetString(int i)
        {
            if (_dataReader.IsDBNull(i))
                return string.Empty;
            return _dataReader.GetString(i);
        }


        
        
        
        
        public virtual object GetValue(int i)
        {
            if (_dataReader.IsDBNull(i))
                return null;
            return _dataReader.GetValue(i);
        }

        
        
        
        
        
        
        
        public virtual int GetInt32(int i)
        {
            if (_dataReader.IsDBNull(i))
                return 0;
            return _dataReader.GetInt32(i);
        }

        
        
        
        
        
        
        
        public virtual double GetDouble(int i)
        {
            if (_dataReader.IsDBNull(i))
                return 0;
            return _dataReader.GetDouble(i);
        }

        
        
        
        
        
        
        
        public virtual Guid GetGuid(int i)
        {
            if (_dataReader.IsDBNull(i))
                return Guid.Empty;
            return _dataReader.GetGuid(i);
        }

        
        
        
        public bool Read()
        {
            return _dataReader.Read();
        }

        
        
        
        public bool NextResult()
        {
            return _dataReader.NextResult();
        }

        
        
        
        public void Close()
        {
            _dataReader.Close();
        }

        
        
        
        public int Depth
        {
            get { return _dataReader.Depth; }
        }

        
        
        
        public int FieldCount
        {
            get { return _dataReader.FieldCount; }
        }

        
        
        
        
        
        
        
        public virtual bool GetBoolean(int i)
        {
            if (_dataReader.IsDBNull(i))
                return false;
            return _dataReader.GetBoolean(i);
        }

        
        
        
        
        
        
        
        public virtual byte GetByte(int i)
        {
            if (_dataReader.IsDBNull(i))
                return 0;
            return _dataReader.GetByte(i);
        }

        
        
        
        
        
        
        
        
        
        
        
        public virtual Int64 GetBytes(int i, Int64 fieldOffset,
            byte[] buffer, int bufferOffset, int length)
        {
            if (_dataReader.IsDBNull(i))
                return 0;
            return _dataReader.GetBytes(i, fieldOffset, buffer, bufferOffset, length);
        }

        
        
        
        
        
        
        
        public virtual char GetChar(int i)
        {
            if (_dataReader.IsDBNull(i))
                return char.MinValue;
            var myChar = new char[1];
            _dataReader.GetChars(i, 0, myChar, 0, 1);
            return myChar[0];
        }

        
        
        
        
        
        
        
        
        
        
        
        public virtual Int64 GetChars(int i, Int64 fieldOffset,
            char[] buffer, int bufferOffset, int length)
        {
            if (_dataReader.IsDBNull(i))
                return 0;
            return _dataReader.GetChars(i, fieldOffset, buffer, bufferOffset, length);
        }

        
        
        
        
        public virtual IDataReader GetData(int i)
        {
            return _dataReader.GetData(i);
        }

        
        
        
        
        public virtual string GetDataTypeName(int i)
        {
            return _dataReader.GetDataTypeName(i);
        }

        
        
        
        
        
        
        
        public virtual DateTime GetDateTime(int i)
        {
            if (_dataReader.IsDBNull(i))
                return DateTime.MinValue;
            return _dataReader.GetDateTime(i);
        }

        
        
        
        
        
        
        
        public virtual decimal GetDecimal(int i)
        {
            if (_dataReader.IsDBNull(i))
                return 0;
            return _dataReader.GetDecimal(i);
        }

        
        
        
        
        public virtual Type GetFieldType(int i)
        {
            return _dataReader.GetFieldType(i);
        }

        
        
        
        
        
        
        
        public virtual float GetFloat(int i)
        {
            if (_dataReader.IsDBNull(i))
                return 0;
            return _dataReader.GetFloat(i);
        }

        
        
        
        
        
        
        
        public virtual short GetInt16(int i)
        {
            if (_dataReader.IsDBNull(i))
                return 0;
            return _dataReader.GetInt16(i);
        }

        
        
        
        
        
        
        
        public virtual Int64 GetInt64(int i)
        {
            if (_dataReader.IsDBNull(i))
                return 0;
            return _dataReader.GetInt64(i);
        }

        
        
        
        
        public virtual string GetName(int i)
        {
            return _dataReader.GetName(i);
        }

        
        
        
        
        public int GetOrdinal(string name)
        {
            return _dataReader.GetOrdinal(name);
        }

        
        
        
        public DataTable GetSchemaTable()
        {
            return _dataReader.GetSchemaTable();
        }


        
        
        
        
        
        
        
        public int GetValues(object[] values)
        {
            return _dataReader.GetValues(values);
        }

        
        
        
        public bool IsClosed
        {
            get { return _dataReader.IsClosed; }
        }

        
        
        
        
        public virtual bool IsDBNull(int i)
        {
            return _dataReader.IsDBNull(i);
        }

        
        
        
        
        public object this[string name]
        {
            get
            {
                object val = _dataReader[name];
                if (DBNull.Value.Equals(val))
                    return null;
                return val;
            }
        }

        
        
        
        
        public virtual object this[int i]
        {
            get
            {
                if (_dataReader.IsDBNull(i))
                    return null;
                return _dataReader[i];
            }
        }

        
        
        
        public int RecordsAffected
        {
            get { return _dataReader.RecordsAffected; }
        }

        #region IDisposable Support

        private bool _disposedValue; 

        
        
        
        public void Dispose()
        {
            
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        
        
        
        
        
        
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    
                    _dataReader.Dispose();
                }

                
            }
            _disposedValue = true;
        }

        
        
        
        ~SafeDataReader()
        {
            Dispose(false);
        }

        #endregion

        
        
        
        
        
        
        
        public string GetString(string name)
        {
            return GetString(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        public object GetValue(string name)
        {
            return GetValue(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        
        
        
        public int GetInt32(string name)
        {
            return GetInt32(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        
        
        
        public double GetDouble(string name)
        {
            return GetDouble(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        
        
        
        
        public SmartDate GetSmartDate(string name)
        {
            return GetSmartDate(_dataReader.GetOrdinal(name), true);
        }

        
        
        
        
        
        
        
        
        public virtual SmartDate GetSmartDate(int i)
        {
            return GetSmartDate(i, true);
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        public SmartDate GetSmartDate(string name, bool minIsEmpty)
        {
            return GetSmartDate(_dataReader.GetOrdinal(name), minIsEmpty);
        }

        
        
        
        
        
        
        
        
        public virtual SmartDate GetSmartDate(
            int i, bool minIsEmpty)
        {
            if (_dataReader.IsDBNull(i))
                return new SmartDate(minIsEmpty);
            return new SmartDate(
                _dataReader.GetDateTime(i), minIsEmpty);
        }

        
        
        
        
        
        
        
        public Guid GetGuid(string name)
        {
            return GetGuid(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        
        
        
        public bool GetBoolean(string name)
        {
            return GetBoolean(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        
        
        
        public byte GetByte(string name)
        {
            return GetByte(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        
        
        
        
        
        
        
        public Int64 GetBytes(string name, Int64 fieldOffset,
            byte[] buffer, int bufferOffset, int length)
        {
            return GetBytes(_dataReader.GetOrdinal(name), fieldOffset, buffer, bufferOffset, length);
        }

        
        
        
        
        
        
        
        public char GetChar(string name)
        {
            return GetChar(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        
        
        
        
        
        
        
        public Int64 GetChars(string name, Int64 fieldOffset,
            char[] buffer, int bufferOffset, int length)
        {
            return GetChars(_dataReader.GetOrdinal(name), fieldOffset, buffer, bufferOffset, length);
        }

        
        
        
        
        public IDataReader GetData(string name)
        {
            return GetData(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        public string GetDataTypeName(string name)
        {
            return GetDataTypeName(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        
        
        
        public virtual DateTime GetDateTime(string name)
        {
            return GetDateTime(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        
        
        
        public decimal GetDecimal(string name)
        {
            return GetDecimal(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        public Type GetFieldType(string name)
        {
            return GetFieldType(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        
        
        
        public float GetFloat(string name)
        {
            return GetFloat(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        
        
        
        public short GetInt16(string name)
        {
            return GetInt16(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        
        
        
        public Int64 GetInt64(string name)
        {
            return GetInt64(_dataReader.GetOrdinal(name));
        }

        
        
        
        
        public virtual bool IsDBNull(string name)
        {
            int index = GetOrdinal(name);
            return IsDBNull(index);
        }
    }
}