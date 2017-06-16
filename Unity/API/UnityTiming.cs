using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.API;
using System.Linq;
using System.Text;

namespace Unity.API
{
    public class UnityTiming : IApiTiming
    {
        public float DeltaTime { get { return UnityEngine.Time.deltaTime; } }
    }
}
