using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers
{
    /// <summary>
    /// Base speech data provider to inherit from when implementing <see cref="IMixedRealitySpeechDataProvider"/>s
    /// </summary>
    public abstract class BaseSpeechDataProvider : BaseControllerDataProvider, IMixedRealitySpeechDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        protected BaseSpeechDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile)
            : base(name, priority)
        {
        }

        /// <inheritdoc />
        public virtual bool IsRecognitionActive { get; protected set; } = false;

        /// <inheritdoc />
        public virtual void StartRecognition() { }

        /// <inheritdoc />
        public virtual void StopRecognition() { }
    }
}