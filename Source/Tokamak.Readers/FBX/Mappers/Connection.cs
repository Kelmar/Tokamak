namespace Tokamak.Readers.FBX.Mappers
{
    internal class Connection
    {
        private Connection()
        {
        }

        public required string Type { get; init; }

        public long From { get; init; }

        public long To { get; init; }

        public static Connection? Build(Node node)
        {
            if (node.Properties.Count != 3)
                return null;

            return new Connection
            {
                Type = node.Properties[0].AsString().ToUpper(),
                From = node.Properties[1].AsLong(),
                To = node.Properties[2].AsLong()
            };
        }
    }
}
