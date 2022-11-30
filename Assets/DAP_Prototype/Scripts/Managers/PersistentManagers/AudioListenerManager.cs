using System.Collections.Generic;
using RPG.Managers.PersistentManagers.ClientSequences;
using static RPG.Core.Shared.Utils;
using RPG.Utils;

namespace RPG.Managers.PersistentManagers
{
    [System.Serializable]
    public class AudioListenerManager : PersistentManager<AudioListenerManager, IAudioListenerManager>, IAudioListenerManager
    {
        [UnityEngine.SerializeField] private List<UnityEngine.AudioListener> activeAudioListeners = new List<UnityEngine.AudioListener>();
        protected override void Awake()
        {
            base.Awake();
            foreach (var _go in gameObject.scene.GetRootGameObjects())
            {
                var _al = _go.GetComponent<UnityEngine.AudioListener>();
                if (_al != null && _al.enabled) RegisterActiveAudioListener(_al);
            }
        }
        public void RegisterActiveAudioListener(UnityEngine.AudioListener al)
        {
            if (!al.isActiveAndEnabled)
            {
                string _e = "AudioListener must be active and enabled in order to be registered as active!";
                UnityEngine.Debug.LogError(_e, al);
                throw new System.Exception(_e);
            }
            // @neuro: It's unclear at this point if AudioListener-s could traverse
            // sequence boundaries (e.g. move from one scene to another), and how
            // would we know if a specific AudioListener could do so. So for now
            // we treat situations when o is already in the list as programmer errors.
            if (activeAudioListeners.Contains(al))
            {
                string _e = "AudioListener already registered as active!";
                UnityEngine.Debug.LogError(_e, al);
                throw new System.Exception(_e);
            }
            if (activeAudioListeners.Count > 0)
            {
                if (!activeAudioListeners[0].isActiveAndEnabled)
                {
                    string _e = "AudioListener was registered as active but was deactivated or disabled without calling UnregisterActiveAudioListener first!";
                    UnityEngine.Debug.LogError(_e, activeAudioListeners[0]);
                    throw new System.Exception(_e);
                }
                activeAudioListeners[0].enabled = false;
            }
            activeAudioListeners.Insert(0, al);
        }
        public void UnregisterActiveAudioListener(UnityEngine.AudioListener al)
        {
            if (!activeAudioListeners.Contains(al))
            {
                string _e = "AudioListener was never registered as active!";
                UnityEngine.Debug.LogError(_e, al);
                throw new System.Exception(_e);
            }
            // @neuro: at this point we don't know if al is enabled since it may
            // have been disabled due to newer AudioListener-s activating, but
            // we do know that it must be active.
            if (!al.gameObject.activeInHierarchy)
            {
                string _e = "AudioListener was registered as active but was deactivated without calling UnregisterActiveAudioListener first!";
                UnityEngine.Debug.LogError(_e, al);
                throw new System.Exception(_e);
            }
            activeAudioListeners.Remove(al);
            // @neuro: setting this back to true since we got an active and enabled
            // AudioListener back when RegisterActiveAudioListener was called.
            // The calling code must take care of disabling al's GO right after
            // this returns.
            al.enabled = true;
            if (activeAudioListeners.Count > 0)
            {
                if (!activeAudioListeners[0].gameObject.activeInHierarchy)
                {
                    string _e = "AudioListener was registered as active but was deactivated without calling UnregisterActiveAudioListener first!";
                    UnityEngine.Debug.LogError(_e, activeAudioListeners[0]);
                    throw new System.Exception(_e);
                }
                if (activeAudioListeners[0].enabled)
                {
                    string _e = "AudioListener was registered as active and disabled by the manager due to newer listeners activating, but then was enabled manually!";
                    UnityEngine.Debug.LogError(_e, activeAudioListeners[0]);
                    throw new System.Exception(_e);
                }
                activeAudioListeners[0].enabled = true;
            }
        }
    }
}