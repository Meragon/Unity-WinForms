namespace Unity.API
{
    using System.Drawing.API;

    public class UnityTiming : IApiTiming
    {
        public float DeltaTime { get { return UnityEngine.Time.deltaTime; } }
    }
}
