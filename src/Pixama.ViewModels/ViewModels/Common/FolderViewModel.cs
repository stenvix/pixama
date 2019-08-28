namespace Pixama.ViewModels.Common
{
    public class FolderViewModel
    {
        public string Name { get; }
        public string Path { get; }
        public string Glyph { get; }

        public FolderViewModel(string name, string path, string glyph)
        {
            Name = name;
            Path = path;
            Glyph = glyph;
        }
    }
}
