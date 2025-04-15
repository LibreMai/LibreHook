using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SgHook
{
    public class MemoryDump
    {
        [Flags]
        public enum DumpType
        {
            MiniDumpNormal = 0x00000000,
            MiniDumpWithDataSegs = 0x00000001,
            MiniDumpWithFullMemory = 0x00000002,
            MiniDumpWithHandleData = 0x00000004,
            MiniDumpFilterMemory = 0x00000008,
            MiniDumpScanMemory = 0x00000010,
            MiniDumpWithUnloadedModules = 0x00000020,
            MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
            MiniDumpFilterModulePaths = 0x00000080,
            MiniDumpWithProcessThreadData = 0x00000100,
            MiniDumpWithPrivateReadWriteMemory = 0x00000200,
            MiniDumpWithoutOptionalData = 0x00000400,
            MiniDumpWithFullMemoryInfo = 0x00000800,
            MiniDumpWithThreadInfo = 0x00001000,
            MiniDumpWithCodeSegs = 0x00002000
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct MiniDumpExceptionInformation
        {
            public uint ThreadId;
            public IntPtr ExceptionPointers;
            [MarshalAs(UnmanagedType.Bool)]
            public bool ClientPointers;
        }

        [DllImport("dbghelp.dll",
            EntryPoint = "MiniDumpWriteDump",
            CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            ExactSpelling = true,
            SetLastError = true)]
        public static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            uint processId,
            SafeHandle hFile,
            DumpType dumpType,
            ref MiniDumpExceptionInformation expParam,
            IntPtr userStreamParam,
            IntPtr callbackParam);

        public static void CreateDump(DumpType option, string dumpName)
        {
            using (FileStream fs = new FileStream($"{AppDomain.CurrentDomain.FriendlyName}_{dumpName}_{DateTime.Now:yyyyMMddHHmmss}.dmp", FileMode.Create))
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
                MiniDumpExceptionInformation exp;
                exp.ThreadId = GetCurrentThreadId();
                exp.ClientPointers = false;
                exp.ExceptionPointers = IntPtr.Zero;
                if (!MiniDumpWriteDump(process.Handle, (uint)process.Id, fs.SafeFileHandle, option, ref exp, IntPtr.Zero, IntPtr.Zero))
                {
                    throw new System.ComponentModel.Win32Exception();
                }
            }
        }

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();
    }
}
