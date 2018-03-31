
 
 
﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Collections;
using System.Data.SqlClient;
using System.Globalization;
using System.Configuration;
using System.Data.Common;

namespace Clover.I18n
{
    public class DatabaseResourceReader : IResourceReader
    {
        private string dsn;
        private string language;
        private string sp;

        public DatabaseResourceReader(string dsn, CultureInfo culture)
        {
            this.dsn = dsn;
            this.language = culture.Name;
        }

        public DatabaseResourceReader(string dsn, CultureInfo culture, string sp)
        {
            this.sp = sp;
            this.dsn = dsn;
            this.language = culture.Name;
        }

        public System.Collections.IDictionaryEnumerator GetEnumerator()
        {
            Hashtable dict = new Hashtable();

            var conn = new SqlConnection(dsn);
            SqlCommand command = conn.CreateCommand();
            if (language == "")
                language = CultureInfo.InvariantCulture.Name;

            
            if (string.IsNullOrEmpty(sp))
            {
                command.CommandText = string.Format("SELECT msgid, msgstr FROM i18nMessage WHERE Culture = '{0}'", language);
            }
            else
            {
                command.CommandText = sp;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@culture", language);
            }

            try
            {

                conn.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetValue(1) != System.DBNull.Value)
                        {
                            dict[reader.GetString(0)] = reader.GetString(1);
                        }
                    }
                }

            }
            catch
            {
                bool raise = false;
                if (bool.TryParse(ConfigurationManager.AppSettings["Gettext.Throw"], out raise) && raise)
                {
                    throw;
                }
            }
            finally
            {
                conn.Close();
            }

            return dict.GetEnumerator();
        }

        public void Close()
        {
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        void IDisposable.Dispose()
        {
        }
    }
}
