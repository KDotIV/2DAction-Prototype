using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.DiagSystem
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Node/Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField]
        private Vector2 newNodeOffset = new Vector2(250, 0);

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

        private void OnEnable()
        {
            nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetResponses(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if(node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if (!node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.GetChildren())
            {
                if(nodeLookup.ContainsKey(childID))
                {
                    yield return (nodeLookup[childID]);
                }
            }
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode _newNode = MakeNode(parentNode);
            Undo.RegisterCreatedObjectUndo(_newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "added Dialogue Node");
            AddNode(_newNode);
        }

        private void AddNode(DialogueNode _newNode)
        {
            nodes.Add(_newNode);
            OnEnable();
        }

        private DialogueNode MakeNode(DialogueNode parentNode)
        {
            DialogueNode _newNode = CreateInstance<DialogueNode>();
            _newNode.name = Guid.NewGuid().ToString();
            if (parentNode != null)
            {
                parentNode.AddChildren(_newNode.name);
                _newNode.SetPlayerSpeaking(!parentNode.IsPlayerSpeaking());
                _newNode.SetPosition(parentNode.GetRect().position + newNodeOffset);
            }

            return _newNode;
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "deleted Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnEnable();
            RemoveChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void RemoveChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChildren(nodeToDelete.name);
            }
        }
#endif
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                DialogueNode _newNode = MakeNode(null);
                AddNode(_newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}