using System;

namespace Tokamak.Readers.FBX.Mappers
{
    internal class Connection
    {
        private Connection() { } // Force use of Build() method.

        public required string Type { get; init; }

        public long From { get; init; }

        public long To { get; init; }

        public required string PropertyName { get; init; }

        public static Connection? Build(Node node)
        {
            if (node.Properties.Count < 3)
                return null;

            string propName = (node.Properties.Count >= 4) ?
                node.Properties[3].AsString() :
                String.Empty;

            return new Connection
            {
                Type = node.Properties[0].AsString().ToUpper(),
                From = node.Properties[1].AsLong(),
                To = node.Properties[2].AsLong(),
                PropertyName = propName
            };
        }
    }
}
