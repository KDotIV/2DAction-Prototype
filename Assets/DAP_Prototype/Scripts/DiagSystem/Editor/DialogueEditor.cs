using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

namespace RPG.DiagSystem.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        [NonSerialized]
        GUIStyle nodeStyle;
        [NonSerialized]
        GUIStyle speakerStyle;
        [NonSerialized]
        GUIStyle playerStyle;
        [NonSerialized]
        DialogueNode draggedNode = null;
        [NonSerialized]
        Vector2 dragOffset;
        [NonSerialized]
        DialogueNode createNode = null;
        [NonSerialized]
        DialogueNode deleteNode = null;
        [NonSerialized]
        DialogueNode linkNode = null;
        [NonSerialized]
        bool dragCanvas = false;
        [NonSerialized]
        Vector2 dragCanvasOffset;
        Vector2 scrollPosition;

        const float canvasSize = 4000;
        const float backgroundSize = 50;

        [MenuItem("Editors/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAssetAttribute(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }
            return false;
        }
        
        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            speakerStyle = new GUIStyle();
            speakerStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            speakerStyle.normal.textColor = Color.white;
            speakerStyle.padding = new RectOffset(20, 20, 20, 20);
            speakerStyle.border = new RectOffset(12, 12, 12, 12);

            playerStyle = new GUIStyle();
            playerStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerStyle.normal.textColor = Color.white;
            playerStyle.padding = new RectOffset(20, 20, 20, 20);
            playerStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if(newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if(selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            } 
            else 
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(canvasSize,canvasSize);
                Texture2D backgroundTexture = Resources.Load("background") as Texture2D;

                GUI.DrawTextureWithTexCoords(canvas,backgroundTexture,new Rect(0,0,canvasSize / backgroundSize,canvasSize / backgroundSize));

                foreach (DialogueNode nodes in selectedDialogue.GetAllNodes())
                {
                    DrawNode(nodes);
                    DrawConnections(nodes);
                }

                EditorGUILayout.EndScrollView();

                if(createNode != null)
                {
                    selectedDialogue.CreateNode(createNode);
                    createNode = null;
                }
                if(deleteNode != null)
                {
                    selectedDialogue.DeleteNode(deleteNode);
                    deleteNode = null;
                }
            }
        }

        private void ProcessEvents()
        {
            if(Event.current.type == EventType.MouseDown && draggedNode == null)
            {
                draggedNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if(draggedNode != null)
                {
                    dragOffset = draggedNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggedNode;
                }
                else
                {
                    dragCanvas = true;
                    dragCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggedNode != null) 
            {
                draggedNode.SetPosition(Event.current.mousePosition + dragOffset);
                GUI.changed = true; 
            }
            else if (Event.current.type == EventType.MouseDrag && dragCanvas)
            {
                scrollPosition = dragCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggedNode != null)
            { 
                draggedNode = null; 
            }
            else if (Event.current.type == EventType.MouseUp && dragCanvas)
            {
                dragCanvas = false;
            }
        }
        
        private void DrawNode(DialogueNode nodes)
        {
            GUIStyle style = nodeStyle;
            if(nodes.IsPlayerSpeaking())
            {
                style = playerStyle;
            }
            GUILayout.BeginArea(nodes.GetRect(), style);
            nodes.SetSpeaker(EditorGUILayout.TextField(nodes.GetSpeaker()));
            nodes.SetText(EditorGUILayout.TextField(nodes.GetText()));

            if (GUILayout.Button("Create New Node"))
            {
                createNode = nodes;
            }
            DrawLinkButtons(nodes);
            if (GUILayout.Button("Delete Node"))
            {
                deleteNode = nodes;
            }
            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode nodes)
        {
            if (linkNode == null)
            {
                if (GUILayout.Button("Link Node"))
                {
                    linkNode = nodes;
                }
            }
            else if (linkNode == nodes)
            {
                if(GUILayout.Button("cancel"))
                {
                    linkNode = null;
                }
            }
            else if (linkNode.GetChildren().Contains(nodes.name))
            {
                if (GUILayout.Button("unlink"))
                {
                    linkNode.RemoveChildren(nodes.name);
                    linkNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                    linkNode.AddChildren(nodes.name);
                    linkNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode nodes)
        {
            Vector3 _startPosition = new Vector2(nodes.GetRect().xMax, nodes.GetRect().center.y);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(nodes))
            {
                Vector3 _endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                Vector3 _controlPointOffSet = _endPosition - _startPosition;
                _controlPointOffSet.y = 0;
                _controlPointOffSet.x *= 0.8f;
                Handles.DrawBezier(
                    _startPosition, _endPosition,
                    _startPosition + _controlPointOffSet,
                    _endPosition - _controlPointOffSet, 
                    Color.white, null, 4f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 mousePosition)
        {
            DialogueNode _foundNode = null;
            foreach (DialogueNode nodes in selectedDialogue.GetAllNodes())
            {
                if(nodes.GetRect().Contains(mousePosition))
                {
                    _foundNode = nodes;
                }
            }
            return _foundNode;
        }
    }
}
