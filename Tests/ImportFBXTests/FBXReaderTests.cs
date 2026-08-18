using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;

using Stashbox;

using Tokamak.Import.FBX;

using ImportFBXTests.Support;

using TestUtilities;

namespace ImportFBXTests
{
    [TestFixture]
    public class FBXReaderTests
    {
        private static string ResourceDir
            => Path.Combine(AppContext.BaseDirectory, "resources");

        protected IStashboxContainer InitContainer()
        {
            var container = new StashboxContainer();

            container.AddLogging();
            
            return container;
        }

        private static IEnumerable<string> SampleFiles()
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
            
            using var container = InitContainer();
            using var scope = container.BeginScope();

            var reader = container.Activate<FBXImportDirector>([new RecordingAssetBuilder()]);

            Assert.That(() => reader.Import(stream, "Bogus.FBX"), Throws.TypeOf<NotImplementedException>());
        }

        [Test]
        public void Import_NullStream_Throws()
        {
            using var container = InitContainer();
            using var scope = container.BeginScope();
            
            var reader = container.Activate<FBXImportDirector>([new RecordingAssetBuilder()]);

            // We know we're passing a NULL here, we are checking that it fails correctly.
            Assert.That(() => reader.Import((Stream)null!, "Bad.FBX"), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Import_BlankFilename_Throws()
        {
            using var container = InitContainer();
            using var scope = container.BeginScope();
            
            var reader = container.Activate<FBXImportDirector>([new RecordingAssetBuilder()]);
            Assert.That(() => reader.Import("   "), Throws.TypeOf<ArgumentException>());
        }

        [TestCaseSource(nameof(SampleFiles))]
        public void Import_SampleModel_ParsesWithoutError(string path)
        {
            using var container = InitContainer();
            using var scope = container.BeginScope();
            
            var builder = new RecordingAssetBuilder();
            var reader = container.Activate<FBXImportDirector>([builder]);

            using var stream = File.OpenRead(path);

            Assert.That(() => reader.Import(stream, path), Throws.Nothing,
                $"Importing '{Path.GetFileName(path)}' should not throw.");

            TestContext.Out.WriteLine(
                $"{Path.GetFileName(path)}: materials={builder.Materials.Count}, " +
                $"meshes={builder.Meshes.Count}, models={builder.Models.Count}");
        }
    }
}
