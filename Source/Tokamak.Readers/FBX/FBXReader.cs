using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Tokamak.Assets;

using Tokamak.Utilities;

using Tokamak.Readers.FBX.Readers;
using Tokamak.Readers.FBX.Mappers;

namespace Tokamak.Readers.FBX
{
    public sealed class FBXReader(IAssetBuilder builder)
    {
        internal const string BINARY_MAGIC = "Kaydara FBX Binary  ";

        private readonly IAssetBuilder m_builder = builder;

        public void Import(string filename)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filename, nameof(filename));

            using var input = File.OpenRead(filename);
            Import(input);
        }

        public void Import(Stream input)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));

            var rootNode = ParseStream(input);
            var state = new ReadState(rootNode);

            /*
             * First pass:
             * 
             * Simplify FBXObjects into C# objects for second pass, this will flatten
             * out some of the FBX indirect references into (mostly) concrete or at
             * least easy to resolve references for our second pass.
             */

            //var textures = ReadTextures(state);

            state.Materials = ReadMaterials(state);
            state.Models = ReadModels(state);
            state.Meshes = ReadMeshes(state);

            /*
             * Second pass:
             * In some cases (e.g. mesh index of model) we need details of the first
             * pass to do a final lookup for import.
             * 
             * Here we'll make calls to the IAssetBuilder to actually start constructing
             * the imports.  Note that for textures that refer to external files; then
             * they will get deferred to those files.
             */

            foreach (var material in state.Materials)
            {
                m_builder.NewMaterial(cfg => cfg
                    .WithName(material.Name)
                );
            }

            foreach (var mesh in state.Meshes)
            {
                m_builder.NewMesh(cfg => cfg
                    .WithName(mesh.Name)
                );
            }

            foreach (var model in state.Models)
            {
                var meshNames = state.Meshes
                    .Where(m => model.MeshIds.Contains(m.Id))
                    .Select(m => m.Name);

                m_builder.NewModel(cfg => cfg
                    .WithName(model.Name)
                    .AddMeshes(meshNames)
                );
            }
        }

        private static string ReadString(Stream input, Encoding encoding, int length)
        {
            byte[] buffer = new byte[length];

            int rd = input.Read(buffer);

            if (rd != buffer.Length)
                throw new Exception($"Unable to read string of length {length}");

            return encoding.GetString(buffer).TrimEnd('\0');
        }

        private static Node ParseStream(Stream input)
        {
            List<Node> children = [];

            if (!input.CanRead)
                throw new ArgumentException("Stream not open for reading", nameof(input));

            input.Seek(0, SeekOrigin.Begin);

            Encoding encoding = Encoding.UTF8; // Use ASCII by default?

            string magic = ReadString(input, encoding, 21);

            IParser parser = (magic == BINARY_MAGIC) ?
                new BinaryFormatReader(input, encoding) :
                throw new NotImplementedException("Text based parser for FBX not written yet."); // TODO: Create a text parser

            for (;;)
            {
                Node? node = parser.ReadNode();

                if (node == null)
                    break;

                children.Add(node);
            }

            return new Node
            {
                Name = String.Empty,
                Properties = [],
                Children = children
            };
        }

        private static List<T> ReadTypes<T>(ReadState state, string type, Func<FBXObject, T?> reader)
            where T : ResultRecord
        {
            return state.ObjectGraph
                .GetObjectsOfType(type)
                .Select(reader)
                .NotNull()
                .ToList();
        }

        private MaterialInfo ReadMaterial(FBXObject obj)
        {
            var result = obj.MapTo<MaterialInfo>();

            result.Id = obj.Id;
            result.Name = obj.Name;

            string? shading = obj.Node.Children
                .WithName("ShadingModel")
                .FirstOrDefault()
                ?.Properties[0].AsString();

            if (!String.IsNullOrWhiteSpace(shading))
                result.ShadingModel = shading.ToLower();

            return result;
        }

        private List<object> ReadTextures(ReadState state)
        {
            //ReadTypes(state, "texture", o => {});
            return [];
        }

        private List<MaterialInfo> ReadMaterials(ReadState state) 
            => ReadTypes(state, "material", ReadMaterial);

        private List<ModelInfo> ReadModels(ReadState state)
        {
            var modelReader = new ModelReader(state);
            return ReadTypes(state, "model", modelReader.ReadModel);
        }

        private List<MeshInfo> ReadMeshes(ReadState state)
        {
            var meshBuilder = new MeshReader(state);
            return ReadTypes(state, "geometry", meshBuilder.ReadMesh);
        }
    }
}
