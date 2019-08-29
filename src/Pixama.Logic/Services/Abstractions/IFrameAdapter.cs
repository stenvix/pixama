using System;

namespace Pixama.Logic.Services
{
    public interface IFrameAdapter
    {
        bool CanNavigate { get; }

        void SetFrame(object frame);
        bool Navigate(Type pageType, object parameters);
    }
}
