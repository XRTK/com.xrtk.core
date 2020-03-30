using XRTK.Interfaces.PlatformSystem;

namespace XRTK.Services.PlatformSystem.Platforms
{
    /// <summary>
    /// Base platform class to derive all platforms from.
    /// </summary>
    public abstract class BasePlatform : BaseDataProvider, IMixedRealityPlatform
    {
        protected BasePlatform(string name, uint priority) : base(name, priority)
        {
        }

        /// <inheritdoc />
        public virtual bool IsAvailable => false;
    }
}