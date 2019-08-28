using Pixama.ViewModels.Services;
using System;
using Windows.UI.Xaml.Controls;

namespace Pixama.App.Services
{
    public class FrameAdapter : IFrameAdapter
    {
        private Frame _internalFrame;

        public void SetFrame(object frame)
        {
            _internalFrame = frame as Frame;
        }

        public bool Navigate(Type pageType, object parameters)
        {
            //TODO: Add animation
            return _internalFrame.Navigate(pageType, pageType);
        }

        public bool CanNavigate => _internalFrame != null;
    }
}
