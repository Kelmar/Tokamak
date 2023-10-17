using System;
using System.IO;
using System.Numerics;
using System.Runtime;
using System.Runtime.InteropServices;

using FreeTypeSharp;
using FreeTypeSharp.Native;

using static FreeTypeSharp.Native.FT;

namespace FreeTypeWrapper
{
    public unsafe class FTLibrary : IDisposable
    {
        private readonly IntPtr m_handle;
        private bool m_disposed = false;

        public FTLibrary()
        {
            IntPtr handle = IntPtr.Zero;

            SafeExecute(() => FT_Init_FreeType(out handle));

            m_handle = handle;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && m_handle != IntPtr.Zero)
            {
                // Maybe log this if there's a problem, but not much we can do if it fails....
                FT_Done_FreeType(m_handle);
            }

            m_disposed = true;
        }

        internal IntPtr Handle => m_handle;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void SafeExecute(Func<FT_Error> func)
        {
            if (m_disposed)
                throw new ObjectDisposedException(nameof(FTLibrary));

            FT_Error err = func();

            if (err != FT_Error.FT_Err_Ok)
                throw new FreeTypeException(err);
        }

        /// <summary>
        /// Load a font face by file name
        /// </summary>
        /// <param name="filename">The file path to load.</param>
        /// <returns></returns>
        public FTFace GetFace(string filename, float size, in Vector2 dpi)
        {
            var data = File.ReadAllBytes(filename);

            return GetFace(size, dpi, data);
        }

        /// <summary>
        /// Load a font face from a file stream
        /// </summary>
        /// <param name="file">File to load from.</param>
        /// <returns></returns>
        public FTFace GetFace(Stream file, float size, in Vector2 dpi)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            return GetFace(size, dpi, ms.ToArray());
        }

        /// <summary>
        /// Load a font face from a raw byte array.
        /// </summary>
        /// <param name="data">The byte array to read from.</param>
        /// <param name="length">The length of the data to read.</param>
        /// <param name="offset">Starting offset into the array to read.</param>
        /// <returns></returns>
        public FTFace GetFace(float size, in Vector2 dpi, in ReadOnlySpan<byte> data)
        {
            return new FTFace(data.ToArray(), this, size, dpi);
        }
    }
}