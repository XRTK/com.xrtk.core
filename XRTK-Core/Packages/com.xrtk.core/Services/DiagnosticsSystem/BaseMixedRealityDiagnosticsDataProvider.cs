using System.Collections.Generic;
using XRTK.Interfaces.DiagnosticsSystem;
using XRTK.Interfaces.DiagnosticsSystem.Handlers;
using XRTK.Definitions.DiagnosticsSystem;

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// Abstract base implementation for diagnostics data providers. Provides needed implementations to register and unregister
    /// diagnostics handlers.
    /// </summary>
    public abstract class BaseMixedRealityDiagnosticsDataProvider : BaseDataProvider, IMixedRealityDiagnosticsDataProvider
    {
        private readonly List<IMixedRealityDiagnosticsHandler> handlers = new List<IMixedRealityDiagnosticsHandler>();

        /// <summary>
        /// Gets currently registered diagnostics handlers.
        /// </summary>
        protected IReadOnlyList<IMixedRealityDiagnosticsHandler> Handlers => handlers;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the data provider as assigned in configuration.</param>
        /// <param name="priority">The priority of the data provider.</param>
        /// <param name="profile">The provider configuration profile assigned.</param>
        public BaseMixedRealityDiagnosticsDataProvider(string name, uint priority, MixedRealityDiagnosticsDataProviderProfile profile)
            : base(name, priority) { }

        /// <inheritdoc />
        public void Register(IMixedRealityDiagnosticsHandler handler)
        {
            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        /// <inheritdoc />
        public void Unregister(IMixedRealityDiagnosticsHandler handler)
        {
            if (handlers.Contains(handler))
            {
                handlers.Remove(handler);
            }
        }
    }
}