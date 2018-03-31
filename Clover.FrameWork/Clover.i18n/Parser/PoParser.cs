
 
 
﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Clover.I18n
{
    
    
    
    public class PoParser
    {
        
        
        
        public void Parse(TextReader reader, IGettextParserRequestor requestor)
        {
            const int Finkey = 2, FindPluralKey = 4, FindMessage = 8;
            string line;
            bool isMsgId = false, isMsgPluralId = false, isMsgStr = false;
            string currentKey = null, pluralKey = null, piece = null;
            StringBuilder msgstrs = new StringBuilder();
            int state = 0, emptycount = 0;

            while (true)
            {
                line = reader.ReadLine();
                line = line == null ? null : line.Trim();
                if (string.IsNullOrEmpty(line))
                {

                    
                    emptycount++;
                    if (emptycount > 5)
                        break;

                    if ((state != Finkey && state != FindPluralKey) && state != FindMessage)
                        continue;

                    
                    string[] values = msgstrs.ToString().Split(new string[] { "|~|" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = values[i].Replace("\\n", "\n").Replace("\\\"", "\"");
                    }

                    if (values.Length >= 1) 
                    {
                        requestor.Handle(currentKey, values[0]);
                    }

                    if (!string.IsNullOrEmpty(pluralKey)) 
                    {
                        requestor.Handle(pluralKey, values);
                    }

                    
                    isMsgPluralId = isMsgId = isMsgStr = false;
                    currentKey = pluralKey = null;
                    state = 0;
                    if (msgstrs.Length > 0)
                        msgstrs.Remove(0, msgstrs.Length);
                    continue;
                }
                else if (line[0] == '#')
                {
                    continue;
                }

                
                isMsgId = line.StartsWith("msgid ");

                isMsgPluralId = line.StartsWith("msgid_plural ");

                isMsgStr = !isMsgId && System.Text.RegularExpressions.Regex.IsMatch(line, "^msgstr(\\[\\d\\])?(\\s)");

                if (isMsgId || isMsgPluralId || isMsgStr)
                {
                    
                    int firstQuote = line.IndexOf('"');
                    if (firstQuote == -1)
                        continue;

                    int secondQuote = line.IndexOf('"', firstQuote + 1);
                    while (secondQuote != -1 && line[secondQuote - 1] == '\\')
                        secondQuote = line.IndexOf('"', secondQuote + 1);
                    if (secondQuote == -1)
                        continue;

                    piece = line.Substring(firstQuote + 1, secondQuote - firstQuote - 1);
                    
                    if (isMsgId) 
                    {
                        currentKey = piece;
                        state = Finkey;
                        emptycount = 0;
                    }
                    else if (isMsgPluralId) 
                    {
                        pluralKey = piece;
                        state = FindPluralKey;
                    }
                    else if (isMsgStr) 
                    {
                        msgstrs.Append(piece);
                        msgstrs.Append("|~|");
                        state = FindMessage;
                    }
                }
            }
        }

        
        
        
        public void Parse(string text, IGettextParserRequestor requestor)
        {
            Parse(new StringReader(text), requestor);
        }

        
        
        
        public Dictionary<String, object> ParseIntoDictionary(TextReader reader)
        {
            var requestor = new DictionaryGettextParserRequestor();
            Parse(reader, requestor);
            return requestor;
        }
    }
}
