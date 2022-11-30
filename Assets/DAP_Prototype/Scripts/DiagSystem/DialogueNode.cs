using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.DiagSystem
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private Sprite speaker;
        [SerializeField] string speakerName;
        [SerializeField] private bool isPlayer = false;
        [SerializeField] private bool isFinal = false;
        [SerializeField] private string text;
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private Rect rect = new Rect(55, 150, 200, 150);

        public Rect GetRect()
        {
            return rect;
        }
        public string GetSpeaker()
        {
            return speakerName;
        }
        public Sprite GetSpeakerSprite()
        {
            return speaker;
        }
        public string GetText()
        {
            return text;
        }
        public bool GetFinal()
        {
            return isFinal;
        }
        public List<string> GetChildren()
        {
            return children;
        }
        public bool IsPlayerSpeaking()
        {
            return isPlayer;
        }
#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }
        public void SetSpeaker(string newSpeaker)
        {
            if(newSpeaker != speakerName)
            {
                Undo.RecordObject(this, "Update Speaker Text");
                speakerName = newSpeaker;
                EditorUtility.SetDirty(this);
            }
        }
        public void SetText(string newText)
        {
            if(newText != text)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                text = newText;
                EditorUtility.SetDirty(this);
            }
        }
        public void AddChildren(string childID)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }
        public void RemoveChildren(string childID)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }

        public void SetPlayerSpeaking(bool newIsPlayerSpeaking)
        {
            Undo.RecordObject(this, "Remove Dialogue Speaker");
            isPlayer = newIsPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
