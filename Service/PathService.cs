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

namespace MiMFa.Service
{
    public class PathService
    {
        public static bool IsUsingByProccess(string fileName)
        {
            try { File.ReadAllBytes(fileName); return false; } catch { return true; }
        }
        public static string ReturnExistAddress(ref string CheckAddress, string ReplaceAddress = null)
        {
            if (File.Exists(CheckAddress)) return CheckAddress;
            else return CheckAddress = ReplaceAddress;
        }
        public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs=true)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            
            if (!dir.Exists)
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            foreach (FileInfo file in dir.EnumerateFiles())
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            if (copySubDirs)
                foreach (DirectoryInfo subdir in dir.EnumerateDirectories())
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    CopyDirectory(subdir.FullName, temppath, copySubDirs);
                }
        }
        public static void MoveDirectory(string sourceDirName, string destDirName)
        {
            if (Directory.Exists(sourceDirName))
            {
                if (sourceDirName != destDirName)
                {
                    if (Directory.Exists(destDirName)) DeleteDirectory(destDirName);
                    Directory.Move(sourceDirName, destDirName);
                }
            }
        }
        public static void MoveAllInDirectory(string sourceDirName, string destDirName,bool overrideIfExists = true, bool files = true, bool dirs = true)
        {
            destDirName = destDirName.TrimEnd(Path.DirectorySeparatorChar)+ Path.DirectorySeparatorChar;
           if(files)
                foreach (var item in Directory.GetFiles(sourceDirName))
                    MoveFile(item, destDirName + Path.GetFileName(item), overrideIfExists);
            if (dirs)
                foreach (var item in Directory.GetDirectories(sourceDirName))
                    MoveDirectory(item, destDirName + GetDirectoryName(item));
        }
        public static void CopyAllInDirectory(string sourceDirName, string destDirName, bool overrideIfExists = true, bool files = true, bool dirs = true)
        {
            destDirName = destDirName.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if (files)
                foreach (var item in Directory.GetFiles(sourceDirName))
                    CopyFile(item, destDirName + Path.GetFileName(item), overrideIfExists); if (files)
                if (dirs)
                    foreach (var item in Directory.GetDirectories(sourceDirName))
                        CopyDirectory(item, destDirName + GetDirectoryName(item), overrideIfExists);
        }

        public static bool CopyFile(string path, string newPath, bool overrideIfExists)
        {
            if (File.Exists(path))
            {
                if (path != newPath)
                {
                    File.Copy(path, newPath, overrideIfExists);
                }
                return true;
            }
            return false;
        }
        public static bool MoveFile(string path, string newPath, bool overrideIfExists)
        {

            if (File.Exists(path))
            {
                if (path != newPath)
                {
                    if (File.Exists(newPath) && overrideIfExists) File.Delete(newPath);
                    File.Move(path, newPath);
                }
                return true;
            }
            return false;
        }
        public static void DeleteAllFilesInAllDirectoriesInPath(string DirectoryAddress)
        {
            if (Directory.Exists(DirectoryAddress))
            {
                string[] sa = Directory.GetFiles(DirectoryAddress);
                foreach (var item in sa)
                    try { File.Delete(item); } catch { }
                string[] sda = Directory.GetDirectories(DirectoryAddress);
                foreach (var item in sda)
                    DeleteAllFilesInAllDirectoriesInPath(item);
            }
        }
        public static void DeleteAllFilesInDirectory(string DirectoryAddress)
        {
            if (Directory.Exists(DirectoryAddress))
            {
                string[] sa = Directory.GetFiles(DirectoryAddress);
                foreach (var item in sa)
                    File.Delete(item);
            }
        }
        public static void TryDeleteAllFilesInDirectory(string DirectoryAddress)
        {
            if (Directory.Exists(DirectoryAddress))
            {
                string[] sa = Directory.GetFiles(DirectoryAddress);
                foreach (var item in sa)
                    try { File.Delete(item); } catch { }
            }
        }
        public static void DeleteAllDirectoriesInPath(string DirectoryAddress, bool reclcive = true)
        {
            if (reclcive) DeleteAllFilesInAllDirectoriesInPath(DirectoryAddress);
            if (Directory.Exists(DirectoryAddress))
            {
                string[] sa = Directory.GetDirectories(DirectoryAddress);
                foreach (var item in sa)
                    Directory.Delete(item, reclcive);
            }
        }
        public static void DeleteAllDirectories(bool deleteFiles = true,params string[] DirectoryAddresses)
        {
            foreach (var item in DirectoryAddresses)
                DeleteDirectory(item, deleteFiles);
        }
        public static void DeleteDirectory(string DirectoryAddress, bool deleteFiles = true)
        {
            Directory.Delete(DirectoryAddress, deleteFiles);
        }
        public static void DeleteAllFilesInAllDirectories(params string[] DirectoryAddresses)
        {
            foreach (var item in DirectoryAddresses)
                 DeleteFiles(Directory.GetFiles(item));
        }
        public static void DeleteFile(string FileAddress)
        {
            while (File.Exists(FileAddress))
                    File.Delete(FileAddress);
        }
        public static void DeleteFiles(params string[] FileAddress)
        {
            foreach (var item in FileAddress)
                DeleteFile(item);
        }
        public static void CreateDirectories(params string[] DirectoryAddresses)
        {
            foreach (var item in DirectoryAddresses)
                CreateDirectory(item);
        }
        public static void CreateFiles(params string[] FileAddress)
        {
            foreach (var item in FileAddress)
                CreateFile(item);
        }
        public static IEnumerable<string> GetAllDirectories(string DirectoryAddress, bool reclcive = true)
        {
            if (Directory.Exists(DirectoryAddress))
            {
                if (!reclcive)
                    foreach (var dir in Directory.GetDirectories(DirectoryAddress))
                        yield return dir;
                else
                    foreach (var dir in Directory.GetDirectories(DirectoryAddress))
                        foreach (var dir2 in GetAllDirectories(dir, reclcive))
                            yield return dir2;
            }
        }
        public static IEnumerable<string> GetAllFilesInAllDirectoriesInPath(string DirectoryAddress)
        {
           return GetAllFiles(DirectoryAddress,true);
        }
        public static IEnumerable<string> GetAllFiles(string DirectoryAddress, bool reclcive = true, string extention = "")
        {
            if (Directory.Exists(DirectoryAddress))
            {
               if(string.IsNullOrEmpty(extention))
                    foreach (var file in Directory.GetFiles(DirectoryAddress))
                        yield return file;
                else
                    foreach (var file in Directory.GetFiles(DirectoryAddress, "*."+extention.TrimStart('*','.'), SearchOption.TopDirectoryOnly))
                        yield return file;
                if (reclcive)
                    foreach (var dir in Directory.GetDirectories(DirectoryAddress))
                        foreach (var file in GetAllFiles(dir, reclcive, extention))
                            yield return file;
            }
        }
        public static IEnumerable<string> GetAllFiles(string DirectoryAddress,Func<string,bool> func, bool reclcive = true)
        {
            if (Directory.Exists(DirectoryAddress))
            {
                foreach (var file in Directory.GetFiles(DirectoryAddress))
                    if (func(file)) yield return file;
                if (reclcive)
                    foreach (var dir in Directory.GetDirectories(DirectoryAddress))
                        foreach (var file in GetAllFiles(dir, func, reclcive))
                            yield return file;
            }
        }
        public static bool CreateFile(string FileAddress)
        {
            if (!File.Exists(FileAddress))
            {
                File.Create(FileAddress);
                return true;
            }
            return false;
        }
        public static bool CreateDirectory(string DirectoryAddress)
        {
            if (!Directory.Exists(DirectoryAddress))
            {
                Directory.CreateDirectory(DirectoryAddress);
                return true;
            }
            return false;
        }
        public static void CreateAllDirectories(string DirectoryAddress)
        {
            if (Directory.Exists(DirectoryAddress)) return;
            string[] stra = DirectoryAddress.Split(new char[] { '\\', '/' },StringSplitOptions.RemoveEmptyEntries);
            string str = "";
            if (stra.Length > 0)
            {
                str = stra[0];
                CreateDirectory(str);
            }
            for (int i = 1; i < stra.Length; i++)
                CreateDirectory(str += "\\" + stra[i]);
        }
        public static string CreateValidDirectoryName(string DirectoryAddress, string DirectoryName, bool withparentsis = false)
        {
            string fn = DirectoryName = NormalizeForFileAndDirectoryName(DirectoryName);
            DirectoryAddress = DirectoryAddress.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string address = DirectoryAddress + fn + Path.DirectorySeparatorChar;
            int i = 1;
           if(withparentsis)
                while (Directory.Exists(address))
                {
                    fn = DirectoryName + "(" + i++ + ")";
                    address = DirectoryAddress + fn + "\\";
                }
           else
                while (Directory.Exists(address))
                {
                    fn = DirectoryName +  i++ ;
                    address = DirectoryAddress + fn + "\\";
                }
            return address;
        }
        public static string NormalizeForFileAndDirectoryName(string oldFileAndFolderName,short maxchar = 50)
        {
            string newName = System.Text.RegularExpressions.Regex.Replace(oldFileAndFolderName, @"[\~\#\%\&\*\{\}\\\:\<\>\?\/\+\|]", " ").Trim();
            return newName.Length > maxchar ? newName.Substring(0, maxchar) : newName;
        }
        public static string NormalizeForAddressPath(string oldAddress)
        {
            string[] signs = { "~", "!", "@", "#", "$", "%", "^", "&", "*", "-", ":", ";", "'", "\"", "<", ">", "|"};
            foreach (var item in signs)
               oldAddress=  oldAddress.Replace(item, "_");
            return oldAddress;
        }
        public static string PathCreator(string DirectoryAddress, string VariableFileName, string extension)
        {

            foreach (var item in Path.GetInvalidFileNameChars())
                VariableFileName = VariableFileName.Replace(item + "", "");
            string address = DirectoryAddress + VariableFileName + extension;
            int i = 1;
            while (File.Exists(address))
                address = DirectoryAddress + VariableFileName + "(" + i++ + ")" + extension;
            return address;
        }
        public static string CreateValidPathName(string DirectoryAddress, string VariableFileName, string extension, bool withparentsis = false)
        {
            DirectoryAddress = (DirectoryAddress??"").TrimEnd('\\')+"\\";
            CreateAllDirectories(DirectoryAddress);
            extension = "."+extension.TrimStart('.');
            string fn = VariableFileName;
            string address = DirectoryAddress + fn + extension;
            int i = 1;
            if (withparentsis)
                while (File.Exists(address))
                {
                    fn = VariableFileName + "(" + i++ + ")";
                    address = DirectoryAddress + fn + extension;
                }
            else
                while (File.Exists(address))
                {
                    fn = VariableFileName + i++;
                    address = DirectoryAddress + fn + extension;
                }
            return address;
        }
        public static Dictionary<string, string> GetPathRestoreList(string pathFileAddress, string defaultPathDirectory)
        {
            FileStream fs = new FileStream(pathFileAddress, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Dictionary<string, string> list = new Dictionary<string, string>();
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                Dictionary<string, string> ls = new Dictionary<string, string>();
                try { ls = (Dictionary<string, string>)bf.Deserialize(fs); }
                catch { }
                fs.Close();
                foreach (var item in ls)
                    if (File.Exists(item.Key)) list.Add(item.Key, Path.GetFileNameWithoutExtension(item.Key));
                fs = new FileStream(pathFileAddress, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                bf = new BinaryFormatter();
                bf.Serialize(fs, list);
                fs.Close();
                string[] dpFiles = Directory.GetFiles(defaultPathDirectory);
                foreach (var item in dpFiles)
                    try { list.Add(item, Path.GetFileNameWithoutExtension(item)); }
                    catch { }

            }
            catch { return null; }
            finally { fs.Close(); }
            return list;
        }
        public static Dictionary<string, string> GetPathRestoreList(string pathFileAddress)
        {
            FileStream fs = new FileStream(pathFileAddress, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Dictionary<string, string> list = new Dictionary<string, string>();
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                Dictionary<string, string> ls = new Dictionary<string, string>();
                try { ls = (Dictionary<string, string>)bf.Deserialize(fs); }
                catch { }
                fs.Close();
                foreach (var item in ls)
                    if (File.Exists(item.Key)) list.Add(item.Key, Path.GetFileNameWithoutExtension(item.Key));
                fs = new FileStream(pathFileAddress, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                bf = new BinaryFormatter();
                bf.Serialize(fs, list);
                fs.Close();
            }
            catch { return null; }
            finally { fs.Close(); }
            return list;
        }
        public static Dictionary<string, string> AddPathRestoreToList(string pathFileAddress, string newpath)
        {
            FileStream fs = new FileStream(pathFileAddress, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Dictionary<string, string> list = new Dictionary<string, string>();
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                Dictionary<string, string> ls = new Dictionary<string, string>();
                try { ls = (Dictionary<string, string>)bf.Deserialize(fs); }
                catch { }
                fs.Close();
                try { list.Add(newpath, Path.GetFileNameWithoutExtension(newpath)); }
                catch { }
                foreach (var item in ls)
                    if (File.Exists(item.Key)) try { list.Add(item.Key, Path.GetFileNameWithoutExtension(item.Key)); }
                        catch { continue; }
                fs = new FileStream(pathFileAddress, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                bf = new BinaryFormatter();
                bf.Serialize(fs, list);
                fs.Close();
            }
            catch { return null; }
            finally { fs.Close(); }
            return list;
        }
        public static string GetFullAddress(string path)
        {
            path = Path.GetFullPath(path);
            if (Directory.Exists(path)) path = path.TrimEnd(Path.DirectorySeparatorChar)+ Path.DirectorySeparatorChar;
            return path;
        }
        public static string GetDirectoryName(string dir)
        {
            return dir.Split(new string[] { System.IO.Path.DirectorySeparatorChar.ToString(),":" },StringSplitOptions.RemoveEmptyEntries).Last();
        }
        public static string GetParentDirectory(string dir)
        {
            string[] str = dir.TrimEnd(System.IO.Path.DirectorySeparatorChar).Split(new string[] { System.IO.Path.DirectorySeparatorChar.ToString() }, StringSplitOptions.None);
            return string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), str.Take(str.Length-1))+ System.IO.Path.DirectorySeparatorChar.ToString();
        }
        public static string GetFileName(string path)
        {
           return Path.GetFileName(path);
        }
        public static string GetFileNameWithoutExtension(string path)
        {
           return Path.GetFileNameWithoutExtension(path);
        }
    }
}
