using XRTK.Interfaces;

namespace XRTK.Definitions
{
    public abstract class BaseMixedRealityServiceProfile : BaseMixedRealityProfile
    {
        /// <summary>
        /// Currently registered system and manager <see cref="IMixedRealityServiceConfiguration"/>s.
        /// </summary>
        public virtual IMixedRealityServiceConfiguration[] RegisteredServiceConfigurations { get; internal set; }
    }
}