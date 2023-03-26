using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Collections;
using MiMFa.General;
using MiMFa.Service;

namespace MiMFa.Exclusive.Security
{
    public class Cryptography
    {
        private char InterfaceChar = '☼';//مورد استفاده در الگوریتم های خاص
        private char[] SplitorChar = { ',', '/', '*' };//مورد استفاده در الگوریتم های خاص
        //----------------------------------------------------// 

        public byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA1.Create();  // SHA1.Create()
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
        public string GetHashString(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return inputString;
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        #region Cryptography Algorithms

        public string ECBPlaintoCipher(string plain, string key, bool auto = true)
        {
            return ECBPlaintoCipher(new string[] { plain }, key,auto)[0];
        }
        public string ECBCiphertoPlain(string cipher, string key, bool auto = true)
        {
            return ECBCiphertoPlain(new string[] { cipher }, key, auto)[0];
        }
        public string[] ECBPlaintoCipher(string[] plain, string key, bool auto = true)//تبدیل متن به کد
        {
            List<BitArray> Cipherlb = new List<BitArray>();
            string[] Cipher = new string[plain.Length];
            byte[] keyArray;
            byte[] toEncryptArray;
            int i = 0;
            string k = key;
            int m = 1;
            if (k.Length != 0)
                if (k.Length > 24)
                    k = (k.Substring(k.Length - 1) + k).Substring(0, 24);
                else if (k.Length < 24)
                    for (int j = k.Length; j < 24; j++)
                        if (m == 1) { k += "a"; m++; }
                        else if (m == 2) { k += "b"; m++; }
                        else if (m == 3) { k += "c"; m++; }
                        else if (m == 4) { k += "d"; m++; }
                        else { k += "e"; m = 1; }
            //
            foreach (var item in plain)
            {
                toEncryptArray = UTF8Encoding.UTF8.GetBytes(item);
                if (auto)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(k));
                    hashmd5.Clear();
                }
                else
                {
                    keyArray = UTF8Encoding.UTF8.GetBytes(k);
                }

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock
                        (toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();
                Cipher[i++] = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            return Cipher;
        }
        public string[] ECBCiphertoPlain(string[] cipher, string key, bool auto = true)//تبدیل کد به متن
        {
            List<BitArray> Cipherlb = new List<BitArray>();
            string[] Plain = new string[cipher.Length];
            byte[] keyArray;
            byte[] toEncryptArray;
            int i = 0;
            int m = 1;
            string k = key;
            if (k.Length != 0)
                if (k.Length > 24)
                    k = (k.Substring(k.Length - 1) + k).Substring(0, 24);
                else if (k.Length < 24)
                    for (int j = k.Length; j < 24; j++)
                        if (m == 1) { k += "a"; m++; }
                        else if (m == 2) { k += "b"; m++; }
                        else if (m == 3) { k += "c"; m++; }
                        else if (m == 4) { k += "d"; m++; }
                        else { k += "e"; m = 1; }
            //
            foreach (var item in cipher)
            {
                toEncryptArray = Convert.FromBase64String(item);

                if (auto)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(k));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(k);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;

                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock
                        (toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();
                Plain[i++] = UTF8Encoding.UTF8.GetString(resultArray);
            }
            return Plain;
        }

        public string CaesarPlaintoCipher(string plain, int scope = 4)
        {
            return CaesarPlaintoCipher(new string[] { plain }, scope)[0];
        }
        public string CaesarCiphertoPlain(string cipher, int scope = 4)
        {
            return CaesarCiphertoPlain(new string[] { cipher }, scope)[0];
        }
        public string[] CaesarPlaintoCipher(string[] plain, int scope = 4)
        {
            CharBank cb = new CharBank();
            List<char[]> Plainlc = new List<char[]>();
            List<char[]> Cipherlc = new List<char[]>();
            string[] Cipher = new string[plain.Length];
            char mc;
            bool Find = false;
            //
            foreach (var item in plain)
            {
                Plainlc.Add(item.ToCharArray());
                Cipherlc.Add(item.ToCharArray());
            }
            //
            for (int i = 0; i < Plainlc.Count; i++)
            {
                for (int j = 0; j < Plainlc[i].Length; j++)
                {
                    if (!Find &&
                        (mc = FindInCharArrayToCipher(
                        ref cb.EnglishCharacters,
                        ref Plainlc[i][j],
                        scope)) != InterfaceChar)
                    {
                        Cipherlc[i][j] = mc;
                        Find = true;
                    }
                    if (!Find &&
                        (mc = FindInCharArrayToCipher(
                        ref cb.FarsiCharacters,
                        ref Plainlc[i][j],
                        scope)) != InterfaceChar)
                    {
                        Cipherlc[i][j] = mc;
                        Find = true;
                    }
                    if (!Find &&
                        (mc = FindInCharArrayToCipher(
                        ref cb.NumberCharacters,
                        ref Plainlc[i][j],
                        scope)) != InterfaceChar)
                    {
                        Cipherlc[i][j] = mc;
                        Find = true;
                    }
                    if (!Find &&
                        (mc = FindInCharArrayToCipher(
                        ref cb.SignCharacters,
                        ref Plainlc[i][j],
                        scope)) != InterfaceChar)
                    {
                        Cipherlc[i][j] = mc;
                        Find = true;
                    }
                    Find = false;
                }
            }
            //
            int l = 0;
            foreach (var item in Cipherlc)
                Cipher[l++] = new string(item);
            return Cipher;
        }
        public string[] CaesarCiphertoPlain(string[] cipher, int scope = 4)
        {
            CharBank cb = new CharBank();
            List<char[]> Cipherlc = new List<char[]>();
            List<char[]> Plainlc = new List<char[]>();
            string[] Plain = new string[cipher.Length];
            char mc;
            bool Find = false;
            //
            foreach (var item in cipher)
            {
                Cipherlc.Add(item.ToCharArray());
                Plainlc.Add(item.ToCharArray());
            }
            //
            for (int i = 0; i < Cipherlc.Count; i++)
            {
                for (int j = 0; j < Cipherlc[i].Length; j++)
                {
                    if (!Find &&
                        (mc = FindInCharArrayToPlain(
                        ref cb.EnglishCharacters,
                        ref Cipherlc[i][j],
                        scope)) != InterfaceChar)
                    {
                        Plainlc[i][j] = mc;
                        Find = true;
                    }
                    if (!Find &&
                        (mc = FindInCharArrayToPlain(
                        ref cb.FarsiCharacters,
                        ref Cipherlc[i][j],
                        scope)) != InterfaceChar)
                    {
                        Plainlc[i][j] = mc;
                        Find = true;
                    }
                    if (!Find &&
                        (mc = FindInCharArrayToPlain(
                        ref cb.NumberCharacters,
                        ref Cipherlc[i][j],
                        scope)) != InterfaceChar)
                    {
                        Plainlc[i][j] = mc;
                        Find = true;
                    }
                    if (!Find &&
                        (mc = FindInCharArrayToPlain(
                        ref cb.SignCharacters,
                        ref Cipherlc[i][j],
                        scope)) != InterfaceChar)
                    {
                        Plainlc[i][j] = mc;
                        Find = true;
                    }
                    Find = false;
                }
            }
            //
            int l = 0;
            foreach (var item in Plainlc)
                Plain[l++] = new string(item);
            return Plain;
        }

        public string MCBACPlaintoCipher(string plain, int scope = 4)
        {
            return MCBACPlaintoCipher(new string[] { plain }, scope)[0];
        }
        public string MCBACCiphertoPlain(string cipher, int scope = 4)
        {
            return MCBACCiphertoPlain(new string[] { cipher }, scope)[0];
        }
        public string[] MCBACPlaintoCipher(string[] plain, int scope = 4)
        {
            CharBank cb = new CharBank();
            char[] allchar = AllChars();
            List<char[]> Plainlc = new List<char[]>();
            List<char[]> Cipherlc = new List<char[]>();
            string[] Cipher = new string[plain.Length];
            char mc;
            //
            foreach (var item in plain)
            {
                Plainlc.Add(item.ToCharArray());
                Cipherlc.Add(item.ToCharArray());
            }
            //
            for (int i = 0; i < Plainlc.Count; i++)
            {
                for (int j = 0; j < Plainlc[i].Length; j++)
                {
                    if ((mc = FindInCharArrayToCipher(
                        ref allchar,
                        ref Plainlc[i][j],
                        scope)) != InterfaceChar)
                        Cipherlc[i][j] = mc;
                }
            }
            //
            int l = 0;
            foreach (var item in Cipherlc)
                Cipher[l++] = new string(item);
            return Cipher;
        }
        public string[] MCBACCiphertoPlain(string[] Cipher, int scope = 4)
        {
            CharBank cb = new CharBank();
            char[] allchar = AllChars();
            List<char[]> Cipherlc = new List<char[]>();
            List<char[]> Plainlc = new List<char[]>();
            string[] Plain = new string[Cipher.Length];
            char mc;
            //
            foreach (var item in Cipher)
            {
                Cipherlc.Add(item.ToCharArray());
                Plainlc.Add(item.ToCharArray());
            }
            //
            for (int i = 0; i < Cipherlc.Count; i++)
            {
                for (int j = 0; j < Plainlc[i].Length; j++)
                {
                    if ((mc = FindInCharArrayToPlain(
                        ref allchar,
                        ref Cipherlc[i][j],
                        scope)) != InterfaceChar)
                        Plainlc[i][j] = mc;
                }
            }
            //
            int l = 0;
            foreach (var item in Plainlc)
                Plain[l++] = new string(item);
            return Plain;
        }

        public int[] RSAGetKeys()
        {
            int[] primes = { 3, 5, 7, 11, 13, 17, 19, 23, 29, 31 };
            Random r = new Random();
            int p = primes[r.Next(0, 9)];
            int q = primes[r.Next(0, 9)];
            if (p == q) q = 34;
            int n = p * q;
            int fn = (p - 1) * (q - 1);
            int e = 5;
            int i = 1;
            bool gcd = false;
            while (e < fn && !gcd)
                while (i++ <= e && !gcd)
                    if (fn % i == 0 && e % i == 0)
                    {
                        e = e + 2;
                        i = 1;
                    }
                    else gcd = i == e;
            int d = 1;
            while ((e * d) % fn != 1) d++;
            return new int[] { e, d, n };
        }
        public int RSAPlaintoCipher(int plain, int e,int n)
        {
            return  (int)MathService.ModularPower(plain, e, n);
        }
        public int RSACiphertoPlain(int cipher, int d, int n)
        {
            return (int)MathService.ModularPower(cipher, d, n);
        }

        #endregion

        #region Private Algorithms
        private char[] AllChars()
        {
            CharBank cb = new CharBank();
            return CollectionService.Concat<char>(
                cb.EnglishCharacters,
                cb.FarsiCharacters,
                cb.NumberCharacters,
                cb.SignCharacters,
                cb.SymbolCharacter);
        }
        private char FindInCharArrayToCipher(ref char[] ca, ref char c, int key)
        {
            int pi, ci;
            char mc = InterfaceChar;
            for (int i = 0; i < ca.Length; i++)
            {
                if (ca[i] == c)
                {
                    pi = i;
                    ci = i + key;
                    for (; ; )
                    {
                        if (ci >= ca.Length)
                            ci -= ca.Length;
                        else if (ci < 0)
                            ci = ca.Length + ci;
                        else break;
                    }
                    mc = ca[ci];
                    return mc;
                }
            }
            return mc;
        }
        private char FindInCharArrayToPlain(ref char[] ca, ref char c, int key)
        {
            int pi, ci;
            char mc = InterfaceChar;
            for (int i = 0; i < ca.Length; i++)
            {
                if (ca[i] == c)
                {
                    pi = i;
                    ci = i - key;
                    for (; ; )
                    {
                        if (ci >= ca.Length)
                            ci -= ca.Length;
                        else if (ci < 0)
                            ci = ca.Length + ci;
                        else break;
                    }
                    mc = ca[ci];
                    return mc;
                }
            }
            return mc;
        }
        #endregion
    }
}
