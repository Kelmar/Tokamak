using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Tokamak.Assets;

using Tokamak.Readers.FBX.DOM;
using Tokamak.Readers.FBX.Readers;

namespace Tokamak.Readers.FBX
{
    public sealed class FBXImportDirector(IAssetBuilder builder)
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

            //ReadTypes(state, new TextureReader(state));

            ReadTypes(state, new MaterialReader(state));
            ReadTypes(state, new ModelReader(state));
            ReadTypes(state, new MeshReader(state));
            ReadTypes(state, new DeformerReader(state));

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
                    // TODO: Add material details here when we figure out a good interface for that.
                );
            }

            foreach (var mesh in state.Meshes)
            {
                m_builder.NewMesh(cfg =>
                {
                    cfg.WithName(mesh.Name);
                    var polyBuilder = cfg.GetPolygonBuilder();

                    foreach (var p in mesh.Polygons)
                    {
                        polyBuilder
                            .AddVertices(p.Vectors)
                            .AddNormals(p.Normals)
                            .AddUVs(p.TexCoord)
                            .AddColors(p.Material)
                            .Close();
                    }
                });
            }

            foreach (var model in state.SceneObjects)
            {
                var meshNames = state.Meshes
                    .Where(m => model.MeshIds.Contains(m.Id))
                    .Select(m => m.Name);

                m_builder.NewModel(cfg => cfg
                    .WithName(model.Name)
                    .AddMeshes(meshNames)
                );
            }

            m_builder.BuildAll();
        }

        private static string ReadString(Stream input, Encoding encoding, int length)
        {
            byte[] buffer = new byte[length];
            input.ReadExactly(buffer);
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

        private static void ReadTypes(ReadState state, IFBXObjectReader reader)
        {
            state.ObjectGraph
                .GetObjectsOfType(reader.ObjectType)
                .ToList()
                .ForEach(reader.ReadObject);
        }
    }
}
