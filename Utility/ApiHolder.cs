using System;
using System.Collections.Generic;
using System.Drawing.API;
using System.Linq;
using System.Text;
using Unity.API;

namespace System.Windows.Forms
{
    /// <summary>
    /// Replace with your own implementation if needed.
    /// </summary>
    public static class ApiHolder
    {
        public static IApiGraphics Graphics = new UnityGdi(ApplicationBehaviour.DefaultSprite);
        public static IApiTiming Timing = new UnityTiming();
    }
}
