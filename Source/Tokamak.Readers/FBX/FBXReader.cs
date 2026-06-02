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

            /*
             * First pass:
             * 
             * Simplify FBXObjects into C# objects for second pass, this will flatten
             * out some of the FBX indirect references into (mostly) concrete or at
             * least easy to resolve references for our second pass.
             */
            ReadTextures(state);
            ReadMaterials(state);
            ReadModels(state);
            ReadMeshes(state);

            /*
             * Second pass:
             * In some cases (e.g. mesh index of model) we need details of the first
             * pass to do a final lookup for import.
             * 
             * Here we'll make calls to the IAssetBuilder to actually start constructing
             * the imports.  Note that for textures that refer to external files; then
             * they will get deferred to those files.
             */
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

        private void ReadTypes<T>(ReadState state, string type, Func<FBXObject, T> reader)
            where T : ResultRecord
        {
            var items = state.ObjectGraph
                .GetObjectsOfType(type)
                .ToList();

            foreach (var item in items)
            {
                var record = reader(item);

                state.Results.Add(new ImportResult
                {
                    InternalId = record.Id,
                    Name = record.Name,
                    ResourceType = record.Type,
                    Result = record
                });
            }
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

        private ResultModel ReadModel(FBXObject obj)
        {
            var materialIds = obj.Children
                .WithFBXType("material")
                .Select(o => o.Id)
                .ToList();

            var meshIds = obj.Children
                .WithFBXType("mesh")
                .Select(o => o.Id)
                .ToList();

            return new ResultModel
            {
                Id = obj.Id,
                Name = obj.Name,
                MaterialIds = materialIds,
                MeshIds = meshIds
            };
        }

        private void ReadTextures(ReadState state)
        {
            //ReadTypes(state, "texture", o => {});
        }

        private void ReadMaterials(ReadState state) => ReadTypes(state, "material", ReadMaterial);

        private void ReadModels(ReadState state) => ReadTypes(state, "model", ReadModel);

        private void ReadMeshes(ReadState state)
        {
            var meshBuilder = new MeshBuilder(state);
            //ReadTypes(state, "geometry", meshBuilder.ReadMesh);
        }
    }
}
