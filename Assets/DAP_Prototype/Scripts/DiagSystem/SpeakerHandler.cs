using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Managers;
using RPG.Attributes;

namespace RPG.DiagSystem
{
    public class SpeakerHandler : MonoBehaviour
    {
        [SerializeField] public static List<SpeakerLog> speakerLogs;
        public static SpeakerHandler speakerHandler;

        private void Awake()
        {
            speakerHandler = this;
            speakerLogs = new List<SpeakerLog>();
        }
        public static bool CreateSpeakerLog(GameObject _speaker)
        {
            if(CheckSpeakerLog(_speaker.GetComponent<CoreAttributes>()))
            {
                Debug.Log("Already Spoken...Count +1");
                return false;
            } else {
                Debug.Log("Creating new speaker log....");
                SpeakerLog newlog = new SpeakerLog();
                newlog.speaker = _speaker.GetComponent<CoreAttributes>();
                newlog.count = 1;
                speakerLogs.Add(newlog);
                Debug.Log("Log Created...." + newlog.speaker);
                return true;
            }
        }
        private static bool CheckSpeakerLog(CoreAttributes _speaker)
        {
            foreach(SpeakerLog _checked in speakerLogs)
            {
                if(_checked.speaker.GetName() == _speaker.GetName())
                {
                    Debug.Log("Speaker exists...");
                    _checked.count += 1;
                    return true;
                } else { Debug.Log("Speaker does not exists...."); return false;}
            }
            return false;
        }
    }
}