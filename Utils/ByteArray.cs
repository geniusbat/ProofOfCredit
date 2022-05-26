using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace ProofOfCredit.Utils
{
    class ByteArray
    {
        public byte[] Bytes;
        public int Length;
        public ByteArray(byte[] inputBytes)
        {
            Bytes = inputBytes;
            Length = inputBytes.Length;
        }
        public ByteArray()
        {

        }
        public ByteArray(string minerData)
        {
            Deserialize(minerData);
        }
        public void FillRandomly(int maxLength)
        {
            Bytes = new byte[maxLength];
            Random rd = new Random();
            rd.NextBytes(Bytes);
            Length = Bytes.Length;
        }
        public void Set(byte[] inputBytes)
        {
            Bytes = inputBytes;
            Length = inputBytes.Length;
        }
        public ByteArray Sum(ByteArray bytesB)
        {
            byte[] a = Bytes;
            byte[] b = bytesB.Bytes;
            byte[] ret = new byte[Math.Max(a.Length,b.Length)];
            byte[] whoeverWasSmallest;
            byte[] whoeverWasLargest;
            if (a.Length < b.Length)
            {
                whoeverWasLargest = b;
                whoeverWasSmallest = a;
            }
            else
            {
                whoeverWasLargest = a;
                whoeverWasSmallest = b;
            }
            List<byte> result = new List<byte>();
            int carry = 0;
            //Sum until smallest one was used
            for (int pos = 0; pos < whoeverWasSmallest.Length; ++pos)
            {
                int sum = whoeverWasLargest[pos] + whoeverWasSmallest[pos] + carry;
                ret[pos] = (byte)(sum & 0xFF);
                carry = sum >> 8;
            }
            //Fill with the rest
            for (int pos = whoeverWasSmallest.Length; pos < whoeverWasLargest.Length; ++pos)
            {
                int sum = whoeverWasLargest[pos] + carry;
                ret[pos] = (byte)(sum & 0xFF);
                carry = sum >> 8;
            }
            if (carry > 0)
            {
                Array.Resize<byte>(ref ret, ret.Length + 1);
                ret[ret.Length - 1] = (byte)(carry);
            }
            return new ByteArray(ret);
        }
        public ByteArray Sum(byte[] b)
        {
            byte[] a = Bytes;
            byte[] ret = new byte[Math.Max(a.Length, b.Length)];
            byte[] whoeverWasSmallest;
            byte[] whoeverWasLargest;
            if (a.Length<b.Length)
            {
                whoeverWasLargest = b;
                whoeverWasSmallest = a;
            }
            else
            {
                whoeverWasLargest = a;
                whoeverWasSmallest = b;
            }
            List<byte> result = new List<byte>();
            int carry = 0;
            //Sum until smallest one was used
            for (int pos = 0; pos < whoeverWasSmallest.Length; ++pos)
            {
                int sum = whoeverWasLargest[pos] + whoeverWasSmallest[pos] + carry;
                ret[pos] = (byte)(sum & 0xFF);
                carry = sum >> 8;
            }
            //Fill with the rest
            for (int pos = whoeverWasSmallest.Length; pos < whoeverWasLargest.Length; ++pos)
            {
                int sum = whoeverWasLargest[pos] + carry;
                ret[pos] = (byte)(sum & 0xFF);
                carry = sum >> 8;
            }
            if (carry > 0)
            {
                Array.Resize<byte>(ref ret, ret.Length + 1);
                ret[ret.Length - 1] = (byte)(carry);
            }
            return new ByteArray(ret);
        }
        static public ByteArray Sum(byte[] a, byte[] b)
        {
            byte[] ret = new byte[Math.Max(a.Length, b.Length)];
            byte[] whoeverWasSmallest;
            byte[] whoeverWasLargest;
            if (a.Length < b.Length)
            {
                whoeverWasLargest = b;
                whoeverWasSmallest = a;
            }
            else
            {
                whoeverWasLargest = a;
                whoeverWasSmallest = b;
            }
            List<byte> result = new List<byte>();
            int carry = 0;
            //Sum until smallest one was used
            for (int pos = 0; pos < whoeverWasSmallest.Length; ++pos)
            {
                int sum = whoeverWasLargest[pos] + whoeverWasSmallest[pos] + carry;
                ret[pos] = (byte)(sum & 0xFF);
                carry = sum >> 8;
            }
            //Fill with the rest
            for (int pos = whoeverWasSmallest.Length; pos < whoeverWasLargest.Length; ++pos)
            {
                int sum = whoeverWasLargest[pos] + carry;
                ret[pos] = (byte)(sum & 0xFF);
                carry = sum >> 8;
            }
            if (carry > 0)
            {
                Array.Resize<byte>(ref ret, ret.Length + 1);
                ret[ret.Length - 1] = (byte)(carry);
            }
            return new ByteArray(ret);
        }
        public void PrettyPrint()
        {
            Console.WriteLine(ToString());
        }
        public override string ToString()
        {
            String ret = "[";
            foreach (byte item in Bytes)
            {
                ret += item.ToString() + " ";
            }
            ret += "]";
            return ret;
        }
        public bool Equals(ByteArray b)
        {
            if (b.Length!=Length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < Length; i++)
                {
                    byte aB = Bytes[i];
                    byte bB = b.Bytes[i];
                    if (aB!=bB)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public ByteArray Copy()
        {
            return new ByteArray((Byte[])Bytes.Clone());
        }
        public string Serialize()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["bytes"] = "";
            bool first = true;
            foreach (byte item in Bytes)
            {
                if (first)
                {
                    first = false;
                    dic["bytes"] += item.ToString();
                }
                else
                {
                    dic["bytes"] += "," + item.ToString();
                }
            }
            dic["length"] = Length.ToString();
            return JsonConvert.SerializeObject(dic);
        }
        public void Deserialize(string data)
        {
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            List<string> bytesList = dic["bytes"].Split(",").ToList();
            if (bytesList.Count()== int.Parse(dic["length"]))
            {
                byte[] bts = new byte[bytesList.Count()];
                for (int i = 0; i < bytesList.Count(); i++)
                {
                    bts[i] = BitConverter.GetBytes(int.Parse(bytesList[i]))[0];
                }
                Set(bts);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}
