namespace Tokamak.Quill
{
    /// <summary>
    /// Maps a character to a glyph index.
    /// </summary>
    public interface ICharacterMapper
    {
        int MapChar(char c);
    }
}
