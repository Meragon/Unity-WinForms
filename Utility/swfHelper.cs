using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public static class swfHelper
    {
        public static float GetDeltaTime()
        {
            if (ApiHolder.Timing == null)
                return 1f / 60f;
            return ApiHolder.Timing.DeltaTime;
        }
    }
}
