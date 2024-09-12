using System;
using System.IO;
using System.Numerics;

using FreeTypeSharp;

using static FreeTypeSharp.FT;

namespace FreeTypeWrapper
{
    public unsafe class FTLibrary : IDisposable
    {
        private readonly FT_LibraryRec_ *m_handle;
        private bool m_disposed = false;

        public unsafe FTLibrary()
        {
            fixed (FT_LibraryRec_** handle = &m_handle)
            {
                FT_Error err = FT_Init_FreeType(handle);

                if (err != FT_Error.FT_Err_Ok)
                    throw new FreeTypeException(err);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && m_handle != null)
            {
                // Maybe log this if there's a problem, but not much we can do if it fails....
                FT_Done_FreeType(m_handle);
            }

            m_disposed = true;
        }

        internal FT_LibraryRec_ *Handle => m_handle;

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

#if false
        /// <summary>
        /// Load a font face by file name
        /// </summary>
        /// <param name="filename">The file path to load.</param>
        /// <returns></returns>
        public FTFace GetFace(IMountSystem mountSys, string filename, float size, in Vector2 dpi)
        {
            var bytes = mountSys.ReadAllBytes(filename);
            return GetFace(bytes, size, dpi);
        }
#endif

        /// <summary>
        /// Load a font face by file name
        /// </summary>
        /// <param name="filename">The file path to load.</param>
        /// <returns></returns>
        public FTFace GetFace(string filename, float size, in Vector2 dpi)
        {
            var bytes = File.ReadAllBytes(filename);
            return GetFace(bytes, size, dpi);
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
            return GetFace(ms.ToArray(), size, dpi);
        }

        /// <summary>
        /// Load a font face from a raw byte array.
        /// </summary>
        /// <param name="data">The byte array to read from.</param>
        /// <param name="length">The length of the data to read.</param>
        /// <param name="offset">Starting offset into the array to read.</param>
        /// <returns></returns>
        public FTFace GetFace(in ReadOnlySpan<byte> data, float size, in Vector2 dpi)
        {
            return new FTFace(data.ToArray(), this, size, dpi);
        }
    }
}