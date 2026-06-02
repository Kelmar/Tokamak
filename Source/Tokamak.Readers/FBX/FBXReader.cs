using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

using Tokamak.Assets;
using Tokamak.Readers.FBX.Builders;
using Tokamak.Readers.FBX.DOM;
using Tokamak.Tritium.Geometry;
using Tokamak.Tritium.Scene;

namespace Tokamak.Readers.FBX
{
    public sealed class FBXReader(IAssetBuilder builder)
    {
        internal const string BINARY_MAGIC = "Kaydara FBX Binary  ";

        private readonly IAssetBuilder m_builder = builder;

#if false
        private List<FBXModel> GetModels(ReadState state)
        {
            var reader = new ModelBuilder(state);
            return reader.Models;
        }

        private List<FBXMaterial> GetMaterials(ReadState state)
        {
            var reader = new MaterialBuilder(this);
            return reader.Materials;
        }

        private List<FBXMesh> GetMeshes(ReadState state)
        {
            var reader = new MeshBuilder(this);
            return reader.Meshes;
        }

        public SceneMeshObject? ImportModel(string name)
        {
            var model = Models.WithName(name);

            if (model == null)
                return null;

            SceneMeshObject? result = new SceneMeshObject();

            try
            {
                foreach (var mesh in model.Meshes)
                {
                    var outMesh = new Mesh();

                    if (!ImportMesh(outMesh, model, mesh))
                        return null;

                    result.AddMesh(outMesh);
                }

                var r = result;
                result = null;

                return r;
            }
            finally
            {
                // Only gets disposed if we don't return a success.
                result?.Dispose();
            }
        }

        private bool ImportMesh(Mesh outMesh, FBXModel? parent, FBXMesh mesh)
        {
            // Fall back to global materials if no parent.
            List<FBXMaterial> materialObjects = parent == null ? Materials : parent.Materials;
            List<MaterialParameters> materials;

            if (materialObjects == null || materialObjects.Count == 0)
                materials = [new MaterialParameters()]; // Add a basic default material at least.
            else
                materials = materialObjects.Select(m => m.Parameters).ToList();

            var outPolys = new List<Polygon>(mesh.Polygons.Count);
            int lastMaterialID = 0;

            foreach (var p in mesh.Polygons)
            {
                var poly = new Polygon();

                poly.Vectors.AddRange(p.Vectors);
                poly.Normals.AddRange(p.Normals);
                poly.TexCoord.AddRange(p.TexCoord);

                poly.Colors.AddRange(p.Material.Select(mid =>
                {
                    if (mid >= materials.Count)
                        mid = lastMaterialID;

                    lastMaterialID = mid;

                    return materials[mid].DiffuseColor;
                }));

                outPolys.AddRange(poly.SplitIntoTriangles());
            }

            outMesh.SetData(outPolys);

            return true;
        }

        public bool ImportMesh(Mesh outMesh, string name)
        {
            var mesh = Meshes.WithName(name);

            if (mesh == null)
                return false;

            var model = Models.FirstOrDefault(m => m.MeshIds.Contains(mesh.ID));

            return ImportMesh(outMesh, model, mesh);
        }
#endif

        public void Import(string filename)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filename, nameof(filename));

            using var input = File.OpenRead(filename);
            Import(input);
        }

        public void Import(Stream input)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));

            var state = new ReadState(ParseStream(input));

            BuildTextures(state);
            BuildMaterials(state);
            BuildModels(state);
            BuildMeshes(state);
        }

        private string ReadString(Stream input, Encoding encoding, int length)
        {
            byte[] buffer = new byte[length];

            int rd = input.Read(buffer);

            if (rd != buffer.Length)
                throw new Exception($"Unable to read string of length {length}");

            return encoding.GetString(buffer).TrimEnd('\0');
        }

        private Node ParseStream(Stream input)
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
                Children = children.ToLookup(c => c.Name, c => c)
            };
        }

        private IEnumerable<T> ParseType<T>(ReadState state, string type, Func<FBXObject, T> reader)
        {
            var items = state.ObjectGraph
                .GetObjectsOfType(type)
                .ToList();

            foreach (var item in items)
                yield return reader(item);
        }

        private void BuildTextures(ReadState state)
        {
            //ParseType(state, "texture", o => {});
        }

        private MaterialParameters ReadMaterial(FBXObject obj)
        {
            var result = obj.MapTo<MaterialParameters>();

            result.Id = obj.Id;
            result.Name = obj.Name;

            string? shading = obj.Node.Children["ShadingModel"].FirstOrDefault()?.Properties[0].AsString();

            if (!String.IsNullOrWhiteSpace(shading))
                result.ShadingModel = shading.ToLower();

            return result;
        }

        private void BuildMaterials(ReadState state)
        {
            var materials = ParseType(state, "material", ReadMaterial).ToList();

            state.Results.AddRange(materials.Select(m => new ImportResult
            {
                InternalId = m.Id,
                Name = m.Name,
                ResourceType = ImportType.Material,
                Result = m
            }));
        }

        private void BuildModels(ReadState state)
        {
            //ParseType(state, "model", o => {});
        }

        private void BuildMeshes(ReadState state)
        {
            //ParseType(state, "geometry", o => {});
        }
    }
}
