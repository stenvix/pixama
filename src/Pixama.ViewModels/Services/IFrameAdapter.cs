using System;

namespace Pixama.ViewModels.Services
{
    public interface IFrameAdapter
    {
        bool CanNavigate { get; }

        void SetFrame(object frame);
        bool Navigate(Type pageType, object parameters);
    }
}
