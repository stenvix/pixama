﻿using System;
using Windows.UI.Xaml.Controls;

namespace Pixama.Logic.Services
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
            return _internalFrame.Navigate(pageType, parameters);
        }

        public bool CanNavigate => _internalFrame != null;
    }
}
