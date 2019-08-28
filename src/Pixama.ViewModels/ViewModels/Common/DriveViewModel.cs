namespace Pixama.ViewModels.Common
{
    public class DriveViewModel : BaseViewModel
    {
        public string Name { get; }
        public string Path { get; }
        public string Glyph { get; }

        public DriveViewModel(string name, string path, string glyph)
        {
            Name = name;
            Path = path;
            Glyph = glyph;
        }
    }
}
