using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GG40.Core.Utils
{
    public class StringUtils
    {
        public static string Capitalize(string text)
        {
            if (text.Length == 0) return text;
                
            if (text.Length == 1) return text.ToUpper();
            
            var textSplit = text.Split(' ');
            var resString = new List<string>();
            foreach ( var item in textSplit )
            {
                if (item.Length == 0) continue;
                try
                {
                    if (item.Length == 1)
                    {
                        resString.Add(item.ToUpper());
                    }
                    else
                    {
                        var txt = char.ToUpper(item[0]) + item.Substring(1).ToLower();
                        resString.Add(txt);
                    }
                }
                catch (Exception ex )
                {
                    continue;
                }
            }
            return string.Join(' ', resString);
        }
    }
}
