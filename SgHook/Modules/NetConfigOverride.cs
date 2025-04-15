using Net;
using HarmonyLib;
using Main;
using Manager;
using Manager.Operation;
using MelonLoader;
using Monitor;
using System;
using System.Reflection;
using System.IO;
using System.IO.Compression;

namespace SgHook.Modules
{
    public class NetConfigOverride : ISgHookBase
    {
        private static NetConfigOverrideConfig config;
        public void InitConfig()
        {
            config = Config.Conf.netConfigOverride;
        }
        public bool IsEnabled()
        {
            return config.enable;
        }

        public void Run()
        {
            if (config.filterUnCompressed)
            {
                var origin = typeof(NetHttpClient).GetMethod("Decompress", BindingFlags.NonPublic | BindingFlags.Instance);
                var prefix = typeof(NetConfigOverride).GetMethod(nameof(Fix_Decompress));
                SgHook.H0.Patch(origin, prefix: new HarmonyMethod(prefix));
            }
        }

        public static bool Fix_Decompress(
            ref MemoryStream ____memoryStream,
            ref MemoryStream ____temporaryStream,
            ref byte[] ____buffer
            )
        {
            ____memoryStream.SetLength(0L);
            if (____temporaryStream.Length < 6)
            {
                return false;
            }

            // 读取前两个字节并检查是否是0x78 0x9c
            ____temporaryStream.Position = 0L;
            byte[] buffer = new byte[2];
            ____temporaryStream.Read(buffer, 0, 2);
            bool isCompressed = buffer[0] == 0x78 && buffer[1] == 0x9c;

            // 复制流
            ____temporaryStream.Position = 0L;  // 重置位置
            ____temporaryStream.CopyTo(____memoryStream);
            ____temporaryStream.SetLength(0L);
            if (!isCompressed)
            {
                MelonLogger.Msg("Skiped Z-Lib decompress");
            }
            // 如果是压缩的，则进行解压缩操作
            if (isCompressed)
            {
                ____memoryStream.Position = 2L;
                ____memoryStream.SetLength(____memoryStream.Length - 4);
                using (DeflateStream deflateStream = new DeflateStream(____memoryStream, CompressionMode.Decompress, leaveOpen: true))
                {
                    ____temporaryStream.SetLength(0L);  // 清空临时流，准备写入解压后的数据
                    int readBytes;
                    while ((readBytes = deflateStream.Read(____buffer, 0, 1024)) > 0)
                    {
                        ____temporaryStream.Write(____buffer, 0, readBytes);
                    }
                }
                // 将解压后的数据复制回____memoryStream
                ____temporaryStream.Position = 0L;
                ____memoryStream.SetLength(0L);  // 清空____memoryStream，准备写入解压后的数据
                ____temporaryStream.CopyTo(____memoryStream);
                ____temporaryStream.SetLength(0L);
            }

            // 将____memoryStream的位置重置为0，以便从头开始读取
            ____memoryStream.Position = 0L;
            return false;
        }
    }
}