using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Ryujinx.Common
{
    public static class BinaryReaderExtensions
    {
        public unsafe static T ReadStruct<T>(this BinaryReader reader)
            where T : struct
        {
            int size;
            if (typeof(T).IsEnum)
            {
                size = Marshal.SizeOf(Enum.GetUnderlyingType(typeof(T)));
            }
            else
            {
                size = Marshal.SizeOf<T>();
            }

            byte[] data = reader.ReadBytes(size);

            fixed (byte* ptr = data)
            {
                if(typeof(T).IsEnum)
                {
                    var rawValue = Marshal.PtrToStructure((IntPtr)ptr, Enum.GetUnderlyingType(typeof(T)));
                    return (T)rawValue;
                }
                else
                {
                    return Marshal.PtrToStructure<T>((IntPtr)ptr);
                }
            }
        }

        public unsafe static T[] ReadStructArray<T>(this BinaryReader reader, int count)
            where T : struct
        {
            int size = Marshal.SizeOf<T>();

            T[] result = new T[count];

            for (int i = 0; i < count; i++)
            {
                byte[] data = reader.ReadBytes(size);

                fixed (byte* ptr = data)
                {
                    result[i] = Marshal.PtrToStructure<T>((IntPtr)ptr);
                }
            }

            return result;
        }

        public unsafe static void WriteStruct<T>(this BinaryWriter writer, T value)
            where T : struct
        {
            long size;
            if (typeof(T).IsEnum)
            {
                size = Marshal.SizeOf(Enum.GetUnderlyingType(typeof(T)));
            }
            else
            {
                size = Marshal.SizeOf<T>();
            }

            byte[] data = new byte[size];
            
            fixed (byte* ptr = data)
            {
                if (typeof(T).IsEnum)
                {
                    var convertValue = Convert.ChangeType(value, Enum.GetUnderlyingType(typeof(T)));
                    Marshal.StructureToPtr((object)convertValue, (IntPtr)ptr, false);
                }
                else
                {
                    Marshal.StructureToPtr(value, (IntPtr)ptr, false);
                }
            }

            writer.Write(data);
        }
    }
}
