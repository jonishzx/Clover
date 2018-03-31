
using System;
using System.Text.RegularExpressions;

namespace Clover.Net
{
	
	
	
	public class RFC2822DateTime
	{
		
		
		
		
		
		public static DateTime Parse(String date)
		{
			string tmp;
			string[] resp;
			string dayName;
			string dpart;
			string hour, minute;
			string timeZone;
			System.DateTime dt = System.DateTime.Now;
			
			
			tmp = Regex.Replace(date, "(\\([^(].*\\))", "");

			
			tmp = Regex.Replace(tmp, "\\s+", " ");
			tmp = Regex.Replace(tmp, "^\\s+", "");
			tmp = Regex.Replace(tmp, "\\s+$", "");

			
			resp = tmp.Split(new char[] { ',' }, 2);
			if (resp.Length == 2)
			{
				
				dayName = resp[0];
				tmp = resp[1];
			}
			else dayName = "";

			try
			{
				
				int pos = tmp.LastIndexOf(" ");
				if (pos < 1) throw new FormatException("probably not a date");
				dpart = tmp.Substring(0, pos - 1);
				timeZone = tmp.Substring(pos + 1);
				dt = Convert.ToDateTime(dpart);

				
				
				if (dayName != string.Empty)
				{
					if ((dt.DayOfWeek == DayOfWeek.Friday && dayName != "Fri") ||
						(dt.DayOfWeek == DayOfWeek.Monday && dayName != "Mon") ||
						(dt.DayOfWeek == DayOfWeek.Saturday && dayName != "Sat") ||
						(dt.DayOfWeek == DayOfWeek.Sunday && dayName != "Sun") ||
						(dt.DayOfWeek == DayOfWeek.Thursday && dayName != "Thu") ||
						(dt.DayOfWeek == DayOfWeek.Tuesday && dayName != "Tue") ||
						(dt.DayOfWeek == DayOfWeek.Wednesday && dayName != "Wed")
						)
						throw new FormatException("invalid week of day");
				}

				
				if (Regex.IsMatch(timeZone, "[+\\-][0-9][0-9][0-9][0-9]"))
				{
					
					int factor = 0;
					hour = timeZone.Substring(1, 2);
					minute = timeZone.Substring(3, 2);
					if (timeZone.Substring(0, 1) == "+") factor = 1;
					else if (timeZone.Substring(0, 1) == "-") factor = -1;
					else throw new FormatException("incorrect time zone");
					dt = dt.AddHours(factor * Convert.ToInt32(hour));
					dt = dt.AddMinutes(factor * Convert.ToInt32(minute));
				}
				else
				{
					
					switch (timeZone)
					{
						case "A": dt = dt.AddHours(1); break;
						case "B": dt = dt.AddHours(2); break;
						case "C": dt = dt.AddHours(3); break;
						case "D": dt = dt.AddHours(4); break;
						case "E": dt = dt.AddHours(5); break;
						case "F": dt = dt.AddHours(6); break;
						case "G": dt = dt.AddHours(7); break;
						case "H": dt = dt.AddHours(8); break;
						case "I": dt = dt.AddHours(9); break;
						case "K": dt = dt.AddHours(10); break;
						case "L": dt = dt.AddHours(11); break;
						case "M": dt = dt.AddHours(12); break;
						case "N": dt = dt.AddHours(-1); break;
						case "O": dt = dt.AddHours(-2); break;
						case "P": dt = dt.AddHours(-3); break;
						case "Q": dt = dt.AddHours(-4); break;
						case "R": dt = dt.AddHours(-5); break;
						case "S": dt = dt.AddHours(-6); break;
						case "T": dt = dt.AddHours(-7); break;
						case "U": dt = dt.AddHours(-8); break;
						case "V": dt = dt.AddHours(-9); break;
						case "W": dt = dt.AddHours(-10); break;
						case "X": dt = dt.AddHours(-11); break;
						case "Y": dt = dt.AddHours(-12); break;
						case "Z":
						case "UT":
						case "GMT": break;    
						case "EST": dt = dt.AddHours(5); break;
						case "EDT": dt = dt.AddHours(4); break;
						case "CST": dt = dt.AddHours(6); break;
						case "CDT": dt = dt.AddHours(5); break;
						case "MST": dt = dt.AddHours(7); break;
						case "MDT": dt = dt.AddHours(6); break;
						case "PST": dt = dt.AddHours(8); break;
						case "PDT": dt = dt.AddHours(7); break;
						default: throw new FormatException("invalid time zone");
					}
				}
			}
			catch (Exception e)
			{
				throw new FormatException(string.Format("Invalid date:{0}:{1}",	e.Message, date));
			}
			return dt;
		}
	}
}
