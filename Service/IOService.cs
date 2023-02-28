using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MiMFa.General;
using System.Drawing;
using MiMFa.Model;

namespace MiMFa.Service

{
    public class IOService
    {
        public static object Ensure { get; private set; }
        #region serialize
        public static byte[] Serialize(object obj)
        {
            byte[] arrayData = null;
            if (obj != null)
                if (obj is byte[]) return (byte[])obj;
                else if (obj is Image) return ConvertService.ToByteArray((Image)obj);
                else
                    using (MemoryStream stream = new MemoryStream())
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, obj);
                        arrayData = stream.ToArray();
                        stream.Close();
                    }
            return arrayData;
        }
        public static dynamic TryDeserialize(byte[] byteArray,bool elseReturnoriginal = true)
        {
            try { return Deserialize(byteArray); } catch { }
            try { return ConvertService.ToImage(byteArray); } catch { }
            if(!elseReturnoriginal) try { return ConvertService.ToString(byteArray); } catch { }
            return byteArray;
        }
        public static T TryDeserialize<T>(byte[] byteArray, T defVal = default)
        {
            try { var o = Deserialize(byteArray); if (o is T) return (T)o; } catch { }
            return defVal;
        }
        public static object Deserialize(byte[] byteArray)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Seek(0, SeekOrigin.Begin);
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream);
            }
        }
        public static void SaveSerializeFile<T>(string FileAddress, T Case, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.ReadWrite)
        {
            FileStream fs = new FileStream(FileAddress, filemode, fileaccess);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Case);
            }
            finally{ fs.Close(); }
        }
        public static void OpenDeserializeFile<T>(string FileAddress, ref T Case, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.ReadWrite)
        {
            FileStream fs = new FileStream(FileAddress, filemode, fileaccess);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                Case = (T)bf.Deserialize(fs);
            }
            finally { fs.Close(); }
        }
        public static string OpenDeserializeFile(string FileAddress, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.ReadWrite)
        {
            FileStream fs = new FileStream(FileAddress, filemode, fileaccess);
            string Case = "";
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                Case = (string)bf.Deserialize(fs);
            }
            finally
            {
                fs.Close();
            }
            return Case;
        }
        #endregion

        #region file
        public static string ReadTrimedText(string FileAddress, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Read)
        {
            return ReadTrimedText(FileAddress,Encoding.UTF8,  filemode , fileaccess);
        }
        public static string ReadTrimedText(string FileAddress, int[] lines, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Read)
        {
            return ReadTrimedText(FileAddress,lines, Encoding.UTF8, filemode, fileaccess);

        }
        public static string ReadTrimedText(string FileAddress, int index, int count, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Read)
        {
            return ReadTrimedText(FileAddress,index,count, Encoding.UTF8, filemode, fileaccess);

        }
        public static string ReadTrimedText(string FileAddress,Encoding encoding, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Read)
        {
            string str = "";
            FileStream fs = new FileStream(FileAddress, filemode, fileaccess);
            StreamReader sr = new StreamReader(fs,encoding);
            try
            {
                str += sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception ex) { return ex.Message; }
            finally { sr.Close(); }
            return str;
        }
        public static string ReadTrimedText(string FileAddress, int[] lines, Encoding encoding, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Read)
        {
            string str = "";
            List<int> li = CollectionService.Sort(lines.ToList());
            FileStream fs = new FileStream(FileAddress, filemode, fileaccess);
            StreamReader sr = new StreamReader(fs,encoding);
            try
            {
                int i = 0;
                while (sr.Peek() > 0 && i <= li.Last())
                    if (li.Exists((n) => n == i++)) str += sr.ReadLine();
                    else sr.ReadLine();
                sr.Close();
            }
            catch (Exception ex) { return ex.Message; }
            finally { sr.Close(); }
            return str;
        }
        public static string ReadTrimedText(string FileAddress, int index, int count, Encoding encoding, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Read)
        {
            string str = "";
            FileStream fs = new FileStream(FileAddress, filemode, fileaccess);
            StreamReader sr = new StreamReader(fs,encoding);
            try
            {
                int i = 0;
                while (sr.Peek() > 0 && count > 0)
                    if (i++ >= index && count-- > 0) str += sr.ReadLine();
                    else sr.ReadLine();
                sr.Close();
            }
            catch (Exception ex) { return ex.Message; }
            finally { sr.Close(); }
            return str;
        }
        public static string ReadText(string FileAddress, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Read)
        {
            return ReadText(FileAddress, Encoding.UTF8, filemode, fileaccess);
        }
        public static string ReadText(string FileAddress,int[] lines, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Read)
        {
            string str = "";
            List<int> li = CollectionService.Sort(lines.ToList());
            FileStream fs = new FileStream(FileAddress, filemode, fileaccess);
            StreamReader sr = new StreamReader(fs,Encoding.UTF8, true);
            try
            {
                int i = 0;
                while (sr.Peek() > 0 && i <= li.Last())
                    if (li.Exists((n) => n == i++)) str += sr.ReadLine() + "\n";
                    else sr.ReadLine();
                sr.Close();
            }
            catch (Exception ex) { return ex.Message; }
            finally { sr.Close(); }
            return str;
        }
        public static string ReadText(string FileAddress,int index,int count, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Read)
        {
            string str = "";
            FileStream fs = new FileStream(FileAddress, filemode, fileaccess);
            StreamReader sr = new StreamReader(fs,Encoding.UTF8, true);
            try
            {
                int i = 0;
                while (sr.Peek() > 0 && count > 0)
                    if (i++ >= index && count-- > 0) str += sr.ReadLine() + "\n";
                    else sr.ReadLine();
                sr.Close();
            }
            catch (Exception ex) { return ex.Message; }
            finally { sr.Close(); }
            return str;
        }
        public static IEnumerable<string> ReadLines(string FileAddress, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Read)
        {
            FileStream fs = new FileStream(FileAddress, filemode, fileaccess);
            StreamReader sr = new StreamReader(fs, Encoding.UTF8, true);
            try
            {
                while (sr.Peek() > 0)
                    yield return (sr.ReadLine());
                sr.Close();
            }
            finally { sr.Close(); }
            yield break;
        }


        public static void ClearText(string FileAddress)
        {
            FileStream fs = new FileStream(FileAddress, FileMode.Truncate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            try
            {
                sw.Flush();
            }
            catch { }
            finally { sw.Close(); }
        }
        public static void ClearText(string FileAddress, string text)
        {
            try
            {
                string str = ReadText(FileAddress);
                str = str.Replace(text, "");
                AppendText(FileAddress, str, FileMode.Truncate);
            }
            catch { }
        }
        public static void SaveText(string FileAddress, string text, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Write)
        {
            try { System.IO.File.WriteAllText(FileAddress, string.Empty); } catch { }
            AppendText(FileAddress, text, filemode, fileaccess);
        }
        public static void WriteLine(string FileAddress, string text, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Write)
        {
            if (File.Exists(FileAddress)) filemode = FileMode.Append;
            FileStream fs = new FileStream(FileAddress, filemode, fileaccess);
            StreamWriter sw = new StreamWriter(fs);
            try
            {
                sw.WriteLine(text);
                sw.Close();
            }
            catch { }
            finally { sw.Close(); }
        }
        public static void AppendText(string FileAddress, string text, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Write)
        {
            if (File.Exists(FileAddress)) filemode = FileMode.Append;
            FileStream fs = new FileStream(FileAddress, filemode, fileaccess);
            StreamWriter sw = new StreamWriter(fs);
            try
            {
                sw.Write(text);
                sw.Close();
            }
            catch { }
            finally { sw.Close(); }
        }
        public static void WriteLines(string FileAddress, IEnumerable<string> strarr, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Write)
        {
            try { System.IO.File.WriteAllText(FileAddress, string.Empty); } catch { }
            FileStream fs = new FileStream(FileAddress, filemode, fileaccess);
            StreamWriter sw = new StreamWriter(fs);
            try
            {
                foreach (var text in strarr)
                    sw.WriteLine(text);
                sw.Close();
            }
            catch { }
            finally { sw.Close(); }
        }

        public static void WriteDictionary<T, F>(string FileAddress, Dictionary<T, F> Dic, string SplitChar = "|", Encoding encoding = null)
        {
            try { File.WriteAllLines(FileAddress, ConvertService.ToStrings(Dic, SplitChar), encoding ?? Encoding.UTF8); } catch { }
        }
        public static Dictionary<string, string> ReadDictionary(string FileAddress, string SplitChar = "|", bool keyToLower = false, Encoding encoding = null)
        {
            return ConvertService.ToDictionary(File.ReadLines(FileAddress,encoding?? Encoding.UTF8), SplitChar, keyToLower);
        }
        public static SmartDictionary<string, string> ReadSmartDictionary(string FileAddress, string SplitChar = "|", bool keyToLower = false, Encoding encoding = null)
        {
            return ConvertService.ToSmartDictionary(File.ReadLines(FileAddress, encoding ?? Encoding.UTF8), SplitChar, keyToLower);
        }
        public static void WriteKeyValues<T, F>(string FileAddress, IEnumerable<KeyValuePair<T, F>> Dic, string SplitChar = "|", Encoding encoding = null)
        {
            try { File.WriteAllLines(FileAddress, ConvertService.ToStrings(Dic, SplitChar), encoding ?? Encoding.UTF8); } catch { }
        }
        public static IEnumerable<KeyValuePair<string, string>> ReadKeyValues(string path, string SplitChar = "|", Encoding encoding = null)
        {
            return ConvertService.ToKeyValuePairs(File.ReadLines(path, encoding ?? Encoding.UTF8), SplitChar);
        }

        public static int WriteLines(string path, IEnumerable<string> enumerable, Encoding encoding, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Write)
        {
            int numbers = 0;
            try { System.IO.File.WriteAllText(path, string.Empty); } catch { }
            using (FileStream fs = new FileStream(path, filemode, fileaccess))
            using (StreamWriter sw = new StreamWriter(fs, encoding))
                try
                {
                    foreach (var text in enumerable)
                    { sw.WriteLine(text); numbers++; }
                }
                finally { sw.Close(); }
            return numbers;
        }
        public static void WriteText(string path, string text, Encoding encoding, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Write)
        {
            try { System.IO.File.WriteAllText(path, string.Empty); } catch { }
            using (FileStream fs = new FileStream(path, filemode, fileaccess))
                using(StreamWriter sw = new StreamWriter(fs, encoding))
                try
                {
                    sw.Write(text);
                }
                finally { sw.Close(); }
        }
        public static int AppendLines(string path, IEnumerable<string> enumerable, Encoding encoding, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Write)
        {
            int i = 0;
            File.AppendAllLines(path, from v in enumerable where ++i>0 select v, encoding);
            return i;
            //using (FileStream fs = new FileStream(path, filemode, fileaccess))
            //using (StreamWriter sw = new StreamWriter(fs, encoding))
            //    try
            //    {
            //        foreach (var text in enumerable)
            //            sw.WriteLine(text);
            //    }
            //    finally { sw.Close(); }
        }
        public static void AppendText(string path, string text, Encoding encoding, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Write)
        {
            File.AppendAllText(path, text, encoding);
            //using (FileStream fs = new FileStream(path, filemode, fileaccess))
            //using (StreamWriter sw = new StreamWriter(fs, encoding))
            //    try
            //    {
            //        sw.Write(text);
            //    }
            //    finally { sw.Close(); }
        }

        public static IEnumerable<string> ReadLines(string path, Encoding encoding, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Read)
        {
            using (FileStream fs = new FileStream(path, filemode, fileaccess))
                using (StreamReader sr = new StreamReader(fs, encoding, true))
                    try
                    {
                        while (sr.Peek() > 0)
                            yield return sr.ReadLine();
                    }
                    finally { sr.Close(); }
        }
        public static string ReadText(string path, Encoding encoding, FileMode filemode = FileMode.OpenOrCreate, FileAccess fileaccess = FileAccess.Read)
        {
            using (FileStream fs = new FileStream(path, filemode, fileaccess))
                using (StreamReader sr = new StreamReader(fs, encoding, true))
                    try
                    {
                        if (sr.Peek() > 0)
                            return sr.ReadToEnd();
                    }
                    finally { sr.Close(); }
            return null;
        }

        public static long LinesCount(string path)
        {
            FileStream stream = new FileStream(path,FileMode.OpenOrCreate,FileAccess.Read);

            long lineCount = 0L;

            var byteBuffer = new byte[1024 * 1024];
            char bR = '\r';
            char bN = '\n';
            char latestChar = ' ';
            char currentChar = ' ';

            int bytesRead;
            while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
                for (var i = 0; i < bytesRead; i++)
                {
                    if ((currentChar = (char)byteBuffer[i]) == bN && latestChar == bR)
                        lineCount++;
                    latestChar = currentChar;
                }
            stream.Close();
            return lineCount;
        }
        public static long LinesCount(string path, Func<long,bool> callBack)
        {
            FileStream stream = new FileStream(path,FileMode.OpenOrCreate,FileAccess.Read);

            long lineCount = 1L;

            byte[] byteBuffer = new byte[1024 * 1024];
            char bR = '\r';
            char bN = '\n';
            char latestChar = ' ';
            char currentChar = ' ';

            int bytesRead;
            while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
                for (var i = 0; i < bytesRead; i++)
                {
                    if ((currentChar = (char)byteBuffer[i]) == bN && latestChar == bR)
                        if (!callBack(lineCount++)) return lineCount;
                    latestChar = currentChar;
                }
            stream.Close();
            return lineCount;
        }
        #endregion

        public static T ValueToObject<T>(T obj, string valueName, object Value, bool caseSens = false)
        {
            PropertyInfo[] pi = InfoService.GetProperties(obj, valueName, caseSens);
            FieldInfo[] fi = InfoService.GetFields(obj, valueName, caseSens);
            Type typ = null;
            if (pi.Length > 0)
                typ = pi[0].PropertyType;
            else if (fi.Length > 0)
                typ = fi[0].FieldType;
            else return obj;
            string vs = Value + "";
            if (Value != null)
                if (typ == typeof(bool) && (vs == "1" || vs == "0" || vs == "true" || vs == "false")) Value = Convert.ToBoolean(Value);
                else if (typ == typeof(int)) Value = Convert.ToInt32(Value);
                else if (typ == typeof(float)) Value = Convert.ToSingle(Value);
                else if (typ == typeof(decimal)) Value = Convert.ToDecimal(Value);
                else if (typ == typeof(double)) Value = Convert.ToDouble(Value);
                else if (typ == typeof(long)) Value = Convert.ToInt64(Value);
                else if (typ == typeof(Image) || typ == typeof(Bitmap))
                    if (Value is byte[]) Value = ConvertService.ToImage((byte[])Value);
                    else try { Value = (Bitmap)Value; } catch { }
            if (pi.Length > 0)
                pi[0].SetValue(obj, Value);
            else if (fi.Length > 0)
                fi[0].SetValue(obj, Value);
            return obj;
        }

    }
}
