using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RPG.Managers;

namespace RPG.DiagSystem
{
    public class PlayerConvo : MonoBehaviour
    {
        [SerializeField] Dialogue debugDialogue;
        Dialogue currentDialogue;
        DialogueNode currentNode = null;
        bool isChoice = false;
        private bool dialogueActive = false;

        public event Action onConversationUpdated;

        private void Start()
        {
            GameEvents.current.onFinalScene += Quit;
        }
        public void StartDialogue(Dialogue newDialogue)
        {
            Debug.Log("Dialogue Started");
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            onConversationUpdated();
            dialogueActive = true;
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool isChoosing()
        {
            return isChoice;
        }
        public string GetText()
        {
            if(currentNode == null)
            {
                return "";
            }

            return currentNode.GetText();
        }

        public string GetSpeaker()
        {
            if (currentNode == null)
            {
                return "";
            }

            return currentNode.GetSpeaker();
        }
        public bool GetFinalScene()
        {
            if(currentNode == null)
            {
                return false;
            }
            return currentNode.GetFinal();
        }
        public IEnumerable<DialogueNode> GetChoices()
        {
            return currentDialogue.GetResponses(currentNode);
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            isChoice = false;
            Next();
        }

        public void Next()
        {
            int numResponses = currentDialogue.GetResponses(currentNode).Count();
            if(numResponses > 0)
            {
                isChoice = true;
                onConversationUpdated();
                return;
            }

            DialogueNode[] children = currentDialogue.GetAIChildren(currentNode).ToArray();
            currentNode = children[0];
            onConversationUpdated();
        }

        public bool HasNext()
        {
            return currentDialogue.GetAllChildren(currentNode).Count() > 0;
        }
        
        public void Quit()
        {
            if(currentNode.GetFinal())
            {
                SceneLoader.current.LoadFinalScene();
            }
            currentDialogue = null;
            currentNode = null;
            isChoice = false;
            onConversationUpdated();
            dialogueActive = false;
        }
        public bool CheckDialogueActive()
        {
            return dialogueActive;
        }
    }
}
