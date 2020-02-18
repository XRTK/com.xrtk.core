using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;

namespace XRTK.Definitions
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Registered Service Profile", fileName = "MixedRealityRegisteredServiceProfile", order = (int)CreateProfileMenuItemIndices.RegisteredServiceProviders)]
    public class BaseMixedRealityServiceProfile : BaseMixedRealityProfile
    {
        /// <summary>
        /// Currently registered system and manager configurations.
        /// </summary>
        public virtual IMixedRealityServiceConfiguration[] ServiceConfigurations { get; internal set; }
    }
}