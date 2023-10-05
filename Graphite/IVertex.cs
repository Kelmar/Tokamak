namespace Graphite
{
    /// <summary>
    /// Holds 2D coordinates as well as UV information.
    /// </summary>
    public interface IVertex
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float U { get; set; }

        public float V { get; set; }
    }
}
