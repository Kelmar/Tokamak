using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;

using Tokamak.Readers.FBX;

using ReadersTests.Support;

namespace ReadersTests
{
    [TestFixture]
    public class FBXReaderTests
    {
        private static string ResourceDir
            => Path.Combine(AppContext.BaseDirectory, "resources");

        public static IEnumerable<string> SampleFiles()
        {
            if (!Directory.Exists(ResourceDir))
                yield break;

            foreach (var file in Directory.GetFiles(ResourceDir, "*.fbx"))
                yield return file;
        }

        [Test]
        public void Import_NonBinaryFile_ThrowsNotImplemented()
        {
            // The text-format parser is not written yet; a non-matching magic must
            // surface that clearly rather than silently mis-parsing.
            byte[] bogus = Encoding.ASCII.GetBytes("This is definitely not an FBX file at all");
            using var stream = new MemoryStream(bogus);

            var reader = new FBXImportDirector(new RecordingAssetBuilder());

            Assert.That(() => reader.Import(stream, "Bogus.FBX"), Throws.TypeOf<NotImplementedException>());
        }

        [Test]
        public void Import_NullStream_Throws()
        {
            var reader = new FBXImportDirector(new RecordingAssetBuilder());
            Assert.That(() => reader.Import((Stream)null, "Bad.FBX"), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Import_BlankFilename_Throws()
        {
            var reader = new FBXImportDirector(new RecordingAssetBuilder());
            Assert.That(() => reader.Import("   "), Throws.TypeOf<ArgumentException>());
        }

        [TestCaseSource(nameof(SampleFiles))]
        public void Import_SampleModel_ParsesWithoutError(string path)
        {
            var builder = new RecordingAssetBuilder();
            var reader = new FBXImportDirector(builder);

            using var stream = File.OpenRead(path);

            Assert.That(() => reader.Import(stream, path), Throws.Nothing,
                $"Importing '{Path.GetFileName(path)}' should not throw.");

            TestContext.Out.WriteLine(
                $"{Path.GetFileName(path)}: materials={builder.Materials.Count}, " +
                $"meshes={builder.Meshes.Count}, models={builder.Models.Count}");
        }
    }
}
