using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiMFa.Exclusive.Accessibility
{
    public class PropertyToContentInjection
    {
        public char SignChar = '§';
        public object[] Objects = { };

        public PropertyToContentInjection(char signChar, params object[] objects)
        {
            try
            {
                ObjectSource(signChar, objects);
            }
            catch { }
        }
        public PropertyToContentInjection()
        { }
        public void ObjectSource(char signChar, params object[] objects)
        {
            try
            {
                SignChar = signChar;
                Objects = objects;
            }
            catch { }
        }
        public string[] CheckStringArray(params string[] contents)
        {
            try
            {
                for (int i = 1; i < contents.Length; i++)
                    contents[i] = CheckString(contents[i]);
            }
            catch { }
            return contents;
        }
        public string CheckString(string content)
        {

            string[] stra = content.Split(SignChar);
            string str = stra[0];
            try
            {
                int m = -1;
                for (int i = 1; i < stra.Length; i++)
                {
                    m = -1;
                    try { m = Convert.ToInt32(stra[i].Trim().Substring(0, 2)); }
                    catch { m = -1; }
                    if (m > -1 && Objects.Length > m)
                        str += Objects[m].ToString() + stra[i].Substring(2);
                    else { str += stra[i]; }
                }
            }
            catch { }
            return str;
        }
        public object CheckObject(object content)
        {
            string[] stra = content.ToString().Split(SignChar);
            string str = stra[0];
            try
            {
                int m = -1;
                for (int i = 1; i < stra.Length; i++)
                {
                    m = -1;
                    try { m = Convert.ToInt32(stra[i].Trim().Substring(0, 2)); }
                    catch { m = -1; }
                    if (m > -1 && Objects.Length > m)
                        str += Objects[m].ToString() + stra[i].Substring(2);
                    else { str += stra[i]; }
                }
            }
            catch { }
            return (object)str;
        }
    }
}
