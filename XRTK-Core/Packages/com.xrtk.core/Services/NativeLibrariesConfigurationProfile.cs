using UnityEngine;
using XRTK.Definitions;

namespace XRTK.Services
{
    public class NativeLibrariesConfigurationProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private NativeDataModelConfiguration[] nativeDataModelConfigurations = new NativeDataModelConfiguration[0];

        public NativeDataModelConfiguration[] NativeDataModelConfigurations => nativeDataModelConfigurations;
    }
}