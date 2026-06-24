using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Tokamak.Import.Builders;

using Tokamak.Readers.FBX.DOM;
using Tokamak.Readers.FBX.Passes;
using Tokamak.Readers.FBX.SubFormat;

namespace Tokamak.Readers.FBX
{
    public sealed class FBXImportDirector(IAssetBuilder builder)
    {
        internal const string BINARY_MAGIC = "Kaydara FBX Binary  ";

        private readonly IAssetBuilder m_builder = builder;

        public void Import(string fileName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fileName, nameof(fileName));

            using var input = File.OpenRead(fileName);
            Import(input, fileName);
        }

        public void Import(Stream input, string fileName)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));

            var rootNode = ParseStream(input);
            var state = new ReadState(fileName, rootNode);

            List<IReadPass> passes =
            [
                new ParseDataPass(state),
                new ResolvePass(state),
                new BuildPass(m_builder, state)
            ];

            foreach (var pass in passes)
                pass.Execute();
        }

        #region Basic Reading

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

        #endregion
    }
}
