using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.DiagSystem
{
    public class AIConvo : MonoBehaviour
    {
        [SerializeField] Dialogue dialogue = null;
        [SerializeField] Dialogue commonDialogue = null;

        public Dialogue GetDialogue()
        {
            return dialogue;
        }
        public Dialogue GetCommonDialogue()
        {
            return commonDialogue;
        }
    }
}
