using System.Collections.Generic;
using XRTK.Definitions;
using XRTK.Interfaces.DiagnosticsSystem;
using XRTK.Interfaces.DiagnosticsSystem.Handlers;

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// Abstract base implementation for diagnostics data providers. Provides needed implementations to register and unregister
    /// diagnostics handlers.
    /// </summary>
    public abstract class BaseMixedRealityDiagnosticsDataProvider<T> : BaseDataProvider, IMixedRealityGenericDiagnosticsDataProvider<T> where T : IMixedRealityDiagnosticsHandler
    {
        private readonly List<T> handlers = new List<T>();

        /// <summary>
        /// Gets currently registered diagnostics handlers.
        /// </summary>
        protected IReadOnlyList<T> Handlers => handlers;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the data provider as assigned in configuration.</param>
        /// <param name="priority">The priority of the data provider.</param>
        /// <param name="profile">The provider configuration profile assigned.</param>
        public BaseMixedRealityDiagnosticsDataProvider(string name, uint priority, BaseMixedRealityProfile profile)
            : base(name, priority) { }

        /// <inheritdoc />
        public void Register(T handler)
        {
            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        /// <inheritdoc />
        public void Unregister(T handler)
        {
            if (handlers.Contains(handler))
            {
                handlers.Remove(handler);
            }
        }
    }
}