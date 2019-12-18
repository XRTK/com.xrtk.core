using XRTK.Interfaces.PlatformSystem;

namespace XRTK.Services.PlatformSystem.Platforms
{
    public class EditorPlatform : BaseDataProvider, IMixedRealityPlatform
    {
        public EditorPlatform(string name, uint priority) : base(name, priority)
        {
        }
    }
}