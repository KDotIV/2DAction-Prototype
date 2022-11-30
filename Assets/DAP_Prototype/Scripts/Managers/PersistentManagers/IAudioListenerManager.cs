using RPG.Utils;

namespace RPG.Managers.PersistentManagers
{
    public interface IAudioListenerManager
    {
        /// <summary>
        ///     Call this when an AudioListener becomes active and enabled.
        ///     IAudioListenerManager will make sure only one AudioListener is
        ///     active at a time.
        /// </summary>
        /// <param name="al">The AudioListener in question.</param>
        void RegisterActiveAudioListener(UnityEngine.AudioListener al);
        /// <summary>
        ///     Call this when an AudioListener is about to become inactive or disabled.
        ///     IAudioListenerManager will make sure the AudioListener that was
        ///     previously active, if any, will be reactivated.
        /// </summary>
        /// <param name="al">The AudioListener in question.</param>
        void UnregisterActiveAudioListener(UnityEngine.AudioListener al);
    }
}