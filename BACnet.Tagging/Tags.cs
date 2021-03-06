﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BACnet.Types;

namespace BACnet.Tagging
{
    public static class Tags
    {
        /// <summary>
        /// Encodes a value into a byte array
        /// </summary>
        /// <typeparam name="T">The type of value to encode</typeparam>
        /// <param name="value">The value to encode</param>
        /// <returns>The byte array</returns>
        public static byte[] EncodeBytes<T>(T value)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                TagWriter writer = new TagWriter(ms);
                TagWriterSink sink = new TagWriterSink(writer, Value<T>.Schema);
                Value<T>.Save(sink, value);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Encodes a value into a byte array
        /// </summary>
        /// <typeparam name="T">The type of value to encode</typeparam>
        /// <param name="buffer">The buffer to encode into</param>
        /// <param name="offset">The offset to begin encoding</param>
        /// <param name="value">The value to encode</param>
        /// <returns>The next offset</returns>
        public static int Encode<T>(byte[] buffer, int offset, T value)
        {
            using (var ms = new MemoryStream(buffer, offset, buffer.Length, true))
            {
                TagWriter writer = new TagWriter(ms);
                TagWriterSink sink = new TagWriterSink(writer, Value<T>.Schema);
                Value<T>.Save(sink, value);
                offset = (int)ms.Position;
            }
            return offset;
        }

        /// <summary>
        /// Encodes a value to a generic object
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="value">The value to encode</param>
        /// <returns>The generic object</returns>
        public static GenericValue Encode<T>(T value)
        {
            byte[] tag;

            using (var ms = new MemoryStream())
            {
                TagWriter writer = new TagWriter(ms);
                TagWriterSink sink = new TagWriterSink(writer, Value<T>.Schema);
                Value<T>.Save(sink, value);
                tag = ms.ToArray();
            }

            return new TaggedGenericValue(tag, 0, tag.Length);
        }

        /// <summary>
        /// Decodes a value from a byte array
        /// </summary>
        /// <typeparam name="T">The type of value to decode</typeparam>
        /// <param name="buffer">The buffer to decode from</param>
        /// <param name="offset">The offset to decode at</param>
        /// <param name="value">The decoded value</param>
        /// <returns>The next offset</returns>
        public static int Decode<T>(byte[] buffer, int offset, out T value)
        {
            return Decode(buffer, offset, buffer.Length, out value);
        }

        /// <summary>
        /// Decodes a value from a byte array
        /// </summary>
        /// <typeparam name="T">The type of value to decode</typeparam>
        /// <param name="buffer">The buffer to decode from</param>
        /// <param name="offset">The offset to decode at</param>
        /// <param name="end">The last offset</param>
        /// <param name="value">The decoded value</param>
        /// <returns>The next offset</returns>
        public static int Decode<T>(byte[] buffer, int offset, int end, out T value)
        {
            using (var ms = new MemoryStream(buffer, offset, end - offset, false))
            {
                TagReader reader = new TagReader(ms);
                TagReaderStream stream = new TagReaderStream(reader, Value<T>.Schema);
                value = Value<T>.Load(stream);
                offset = (int)ms.Position;
            }
            return offset;
        }

        /// <summary>
        /// Decodes a value from a byte array
        /// </summary>
        /// <typeparam name="T">The type of value to decode</typeparam>
        /// <param name="buffer">The buffer to decode from</param>
        /// <param name="offset">The offset to decode at</param>
        /// <returns>The decoded value</returns>
        public static T Decode<T>(byte[] buffer, int offset)
        {
            T ret;
            Decode(buffer, offset, out ret);
            return ret;
        }

        /// <summary>
        /// Decodes a value from a byte array
        /// </summary>
        /// <typeparam name="T">The type of value to decode</typeparam>
        /// <param name="buffer">The buffer to decode from</param>
        /// <param name="offset">The offset to decode at</param>
        /// <param name="end">The last offset</param>
        /// <returns>The decoded value</returns>
        public static T Decode<T>(byte[] buffer, int offset, int end)
        {
            T ret;
            Decode(buffer, offset, end, out ret);
            return ret;
        }
    }
}
