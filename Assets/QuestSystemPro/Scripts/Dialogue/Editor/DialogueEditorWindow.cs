using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Devdog.QuestSystemPro.Editors;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;
using EditorStyles = UnityEditor.EditorStyles;
using EditorUtility = UnityEditor.EditorUtility;

namespace Devdog.QuestSystemPro.Dialogue.Editors
{
    public class DialogueEditorWindow : EditorWindowBase
    {
        private Dialogue _dialogue;
        public Dialogue dialogue
        {
            get { return _dialogue; }
            set
            {
                if (ReferenceEquals(_dialogue, value))
                {
                    return; // Same, no need to change
                }

                if(_dialogue != null)
                {
                    _dialogue.OnCurrentNodeChanged -= Dialogue_OnCurrentNodeChanged;
                }

                _dialogue = value;

                if (_dialogue != null)
                {
                    _dialogue.OnCurrentNodeChanged += Dialogue_OnCurrentNodeChanged;
                }

                NotifyNodeEditorsChanged();
                DialogueManager.instance.SetCurrentDialogue(_dialogue, dialogueOwner);
            }
        }

        private IDialogueOwner _dialogueOwner;
        public IDialogueOwner dialogueOwner
        {
            get { return _dialogueOwner; }
            set
            {
                _dialogueOwner = value;
                dialogue = _dialogueOwner != null ? _dialogueOwner.dialogue : null;

                DialogueManager.instance.SetCurrentDialogue(dialogue, _dialogueOwner);
            }
        }

        public List<NodeEditorBase> nodeEditors = new List<NodeEditorBase>();

        private List<NodeEditorBase> _selectedNodeEditors = new List<NodeEditorBase>();
        public List<NodeEditorBase> selectedNodeEditors
        {
            get { return _selectedNodeEditors; }
            protected set
            {
                foreach (var node in _selectedNodeEditors)
                {
                    node.UnSelect();
                }

                value.RemoveAll(o => o == null);
                _selectedNodeEditors = value;

                foreach (var node in _selectedNodeEditors)
                {
                    node.Select();
                }
            }
        }

        public static DialogueEditorWindow window { get; protected set; }
        private Vector2 _dragStartPosition;

        private bool _isDraggingNode = false;
        private bool _isDraggingEdge = false;
        private bool _isDraggingSidebarWidth = false;
        private NodeEditorBase _dragEdgeStartNode;
        private uint _dragEdgeStartEdgeConnectorIndex;
        private bool _isDrawingSelectionGrid;
        private Vector2 _propertiesScrollPos;

        public Rect sidebarResizableAreaRect
        {
            get { return new Rect(window.position.width - sidebarWidth - 20f, 0f, 20f, window.position.height); }
        }

        private const string SidebarWidthPrefsKey = "QuestSystemPro_SidebarWidth";
        private static int _sidebarWidth = 350;
        public static int sidebarWidth
        {
            get { return _sidebarWidth; }
            set
            {
                _sidebarWidth = value;
                _sidebarWidth = Mathf.Clamp(_sidebarWidth, MinSidebarWidth, MaxSidebarWidth);

                EditorPrefs.SetInt(SidebarWidthPrefsKey, _sidebarWidth);
            }
        }

        public const int MinSidebarWidth = 250;
        public const int MaxSidebarWidth = 450;

        public const float ZoomSpeed = 0.03f;
        public const float MinZoomAmount = 0.4f;
        public const float MaxZoomAmount = 1f;

        public float zoomAmount = 1f;
        public List<NodeBase> clipboard = new List<NodeBase>();
        public List<Edge> selectedEdges = new List<Edge>();

        public static List<DialogueEditorSidebarBase> sidebars = new List<DialogueEditorSidebarBase>();


        private const string SelectedSidebarPrefsKey = "QuestSystemPro_SelectedSidebarName";
        private static DialogueEditorSidebarBase _selectedSidebar;
        public static DialogueEditorSidebarBase selectedSidebar
        {
            get { return _selectedSidebar; }
            protected set
            {
                _selectedSidebar = value;
                EditorPrefs.SetString("SelectedSidebarPrefsKey", _selectedSidebar != null ? _selectedSidebar.name : "");
            }
        }

        private static bool _isListeningForDialogueChange = false;

        private static bool _isDirty;
        private static bool _isDialogueEventRegistered;
        public Vector2 nodesOffset { get; protected set; }


        [MenuItem("Tools/Quest System Pro/Dialogue Editor")]
        protected static void Init()
        {
            window = GetWindow<DialogueEditorWindow>();
            window.minSize = new Vector2(400,400);
            window.autoRepaintOnSceneChange = true;
            window.wantsMouseMove = false;

            var icon = (Texture)Resources.Load("QuestSystemPro_DialogueEditorIcon");
            window.titleContent = new GUIContent("Dialogue editor", icon);

            CreateSidebars();

            _sidebarWidth = EditorPrefs.GetInt(SidebarWidthPrefsKey, 350);
            
            DialogueNodesSidebarEditor.Init();
        }

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            if (window != null && window.dialogue != null)
            {
                _isDialogueEventRegistered = false;
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void ReloadedScripts()
        {
//            Debug.Log("Scripts reloaded");
            _isDirty = true;
        }
        
        private static void CreateSidebars()
        {
            sidebars.Clear();
            sidebars.Add(new DialogueEditorSidebarProperties("Properties"));
            sidebars.Add(new DialogueEditorSidebarVariables("Variables"));

            _selectedSidebar = sidebars.FirstOrDefault(o => o.name == EditorPrefs.GetString(SelectedSidebarPrefsKey));
            if (_selectedSidebar == null)
            {
                _selectedSidebar = sidebars[0];
            }
        }

        private static void Dialogue_OnCurrentNodeChanged(NodeBase before, NodeBase after)
        {
            var selectedEditor = window.nodeEditors.FirstOrDefault(o => o.node == after);
            if (selectedEditor != null)
            {
                selectedEditor.PingDebugView(DebugNodeVisualType.Debug);
            }
        }

        private void NotifyNodeEditorsChanged()
        {
            if (dialogue == null)
            {
                return;
            }

            if (dialogue.nodes.FirstOrDefault() is EntryNode == false)
            {
                dialogue.AddNode(NodeFactory.Create<EntryNode>());
            }

            nodeEditors.Clear();
            foreach (NodeBase node in dialogue.nodes)
            {
                nodeEditors.Add(CreateEditorForNode(node));
            }

            window.Repaint();
        }

        protected virtual NodeEditorBase CreateEditorForNode(NodeBase node)
        {
            var type = DialogueReflectionUtility.GetCustomNodeEditorFor(node.GetType());
            if (type != null)
            {
                var editor = (NodeEditorBase)Activator.CreateInstance(type);
                editor.Init(node, this);

                return editor;
            }

            var editor2 = new DefaultNodeEditor();
            editor2.Init(node, this);

            return editor2;
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            if (window == null)
            {
                Init();
                return;
            }

//            dialogue = GetDialogueFromGameObjects(Selection.gameObjects);
            if (dialogue == null)
            {
                GUI.Label(new Rect(0, 0, 400, EditorGUIUtility.singleLineHeight), "No dialogue selected");
                return;
            }

            if (_isDirty)
            {
                _isDirty = false;
                NotifyNodeEditorsChanged();
            }


            if (_isDialogueEventRegistered == false)
            {
                _isDialogueEventRegistered = true;
                _dialogue.OnCurrentNodeChanged += Dialogue_OnCurrentNodeChanged;
            }

            // TODO: Consider adding a check to see if 'something' changed. reflection drawers can detect change with NotifyValueChanged (could mark dirty) + dragging nodes.
            EditorUtility.SetDirty(dialogue); // Always serialize..

            if (GUI.Button(new Rect(0, 0, 100, 24), "Save"))
            {
                dialogue.Save();
            }

            if (GUI.Button(new Rect(100, 0, 100, 24), "Load"))
            {
                dialogue.Load();
                NotifyNodeEditorsChanged();
            }

            if (GUI.Button(new Rect(200, 0, 100, 24), "Open sidebar"))
            {
                DialogueNodesSidebarEditor.Init();
            }


            if (EditorApplication.isPlaying)
            {
                if (_isListeningForDialogueChange)
                {
                    if (GUI.Button(new Rect(300, 0, 150, 24), new GUIContent("[Stop] auto change")))
                    {
                        _isListeningForDialogueChange = false;
                        DialogueManager.instance.OnCurrentDialogueChanged -= OnCurrentDialogueChangedAutoChange;
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(300, 0, 150, 24), new GUIContent("[Start] auto change", "Automatically change the dialogue when the player uses one in-game.")))
                    {
                        _isListeningForDialogueChange = true;
                        DialogueManager.instance.OnCurrentDialogueChanged += OnCurrentDialogueChangedAutoChange;
                    }
                }
            }


            if (_isDraggingEdge)
            {
                DialogueEditorUtility.DrawCurves(_dragStartPosition, Event.current.mousePosition, Color.white);
            }
            else if (_isDrawingSelectionGrid && _isDraggingSidebarWidth == false)
            {
                DrawSelectionRect();
            }


            var nodeBoxRect = GetNodeBoxRect();
            Matrix4x4 trs = Matrix4x4.TRS(new Vector3(nodeBoxRect.width / 2, nodeBoxRect.height / 2f - 21f, 0f), Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoomAmount, zoomAmount, zoomAmount));
            using (new GUIMatrixBlock(trs * scale * trs.inverse))
            {
                using (new GroupBlock(nodeBoxRect))
                {
                    DrawAllEdges();
                    DrawAllNodes();
                }
            }

            DrawZoomIndicator();
            DrawMinimap();

            // Resizable sidebar
            EditorGUIUtility.AddCursorRect(sidebarResizableAreaRect, MouseCursor.ResizeHorizontal);

            using (new ScrollableBlock(
                    new Rect(window.position.width - sidebarWidth, 0f, sidebarWidth, window.position.height),
                    ref _propertiesScrollPos,
                    200))
            {
                using (new GroupBlock(new Rect(0f, 0f, sidebarWidth, window.position.height), GUIContent.none, EditorStyles.helpBox))
                {
                    DrawSidebar();
                }
            }
        }

        private void OnCurrentDialogueChangedAutoChange(Dialogue before, Dialogue after, IDialogueOwner owner)
        {
            this.dialogueOwner = owner;
        }

        protected override void OnMouseDrag(Vector2 delta)
        {
            if (GetNodeBoxRect().Contains(Event.current.mousePosition))
            {
                OnTryDragNode(delta);
                OnPerformDrag(delta);
            }
        }

        private Rect GetNodeBoxRect()
        {
            return new Rect(0, 0, (window.position.width - sidebarWidth), window.position.height);
        }

        private void DrawMinimap()
        {
            var zoomRect = new Rect(window.position.width - sidebarWidth - 160f, 50f, 140f, 100f);

            GUI.color = new Color(1f, 1f, 1f, 0.5f);

            using (new GroupBlock(zoomRect, GUIContent.none, "CN Box"))
            {
                var allNodesInRect = nodeEditors;

                foreach (var n in allNodesInRect)
                {
                    var r = GetNodeBoxRect();
                    r.x += r.width;
                    r.y += r.height;

                    var scaleMult = 160f / r.width;
                    scaleMult *= 0.4f;

                    var scaledRect = n.GetNodeRect();
                    scaledRect.x += r.width/2f;
                    scaledRect.y += r.height/2f;
                    scaledRect.x -= scaledRect.width;
                    scaledRect.y -= scaledRect.height;

                    scaledRect.position *= scaleMult;
                    scaledRect.size *= 0.1f;
                    scaledRect = ToZoomSpaceRect(scaledRect);

                    EditorGUI.LabelField(scaledRect, GUIContent.none, (GUIStyle)"ColorPickerBox");
                }
            }

            GUI.color = Color.white;
        }

        private void DrawZoomIndicator()
        {
            var zoomRect = new Rect(window.position.width - sidebarWidth - 160f, 10f, 140f, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(zoomRect, "Zoom: " + zoomAmount * 100f + "%");

            GUI.color = new Color(0, 0, 0, 0.5f);

            zoomRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(zoomRect, GUIContent.none, (GUIStyle)"MiniMinMaxSliderHorizontal");

            GUI.color = Color.white;

            zoomRect.width *= zoomAmount;
            EditorGUI.LabelField(zoomRect, GUIContent.none, (GUIStyle)"MiniMinMaxSliderHorizontal");
        }

        private void CopyNodesToClipboard(List<NodeEditorBase> nodes)
        {
            clipboard = nodes.Select(o => o.node).ToList();
            Debug.Log("Copied " + clipboard.Count + " nodes to clipboard");
        }

        private void PasteNodesFromClipboard()
        {
            if (clipboard.Count == 0)
            {
                return;
            }

            var clones = new List<NodeBase>();
            foreach (var node in clipboard)
            {
                clones.Add((NodeBase)ReflectionUtility.CreateDeepClone(node));
            }

            if (window.position.Contains(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)))
            {
                // If the cursor is inside the window paste the new nodes near the cursor.
                var delta = Event.current.mousePosition - clones.First().editorPosition;
                foreach (var node in clones)
                {
                    node.editorPosition = node.editorPosition + delta - nodesOffset;
                }
            }
            else
            {
                foreach (var node in clones)
                {
                    node.editorPosition = new Vector2(node.editorPosition.x + 100f, node.editorPosition.y) - nodesOffset;
                }
            }

            foreach (var node in clones)
            {
                AddNode(node);
            }
        }

        private void DrawSelectionRect()
        {
            if (GetNodeBoxRect().Contains(Event.current.mousePosition))
            {

                var start = _dragStartPosition;
                var end = Event.current.mousePosition - start;

                var r = new Rect(start, end);
                SelectNodes(GetNodesInRect(r));

                GUI.Label(r, GUIContent.none, "SelectionRect");
            }
        }


        private void OnTryDragNode(Vector2 delta)
        {
            if (GetNodeBoxRect().Contains(Event.current.mousePosition))
            {
                if (_isDrawingSelectionGrid == false && _isDraggingNode == false && _isDraggingEdge == false && _isDraggingSidebarWidth == false)
                {
                    if (Event.current.button == 0)
                    {
                        foreach (var node in nodeEditors)
                        {
                            var edgeRects = node.GetEdgeConnectorRects();
                            for (uint i = 0; i < edgeRects.Length; i++)
                            {
                                if (ToZoomSpaceRect(edgeRects[i]).Contains(Event.current.mousePosition))
                                {
                                    _dragStartPosition = Event.current.mousePosition;
                                    _dragEdgeStartNode = node;
                                    _dragEdgeStartEdgeConnectorIndex = i;
                                    _isDraggingEdge = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void OnMouseDown(int button)
        {
            if (GetNodeBoxRect().Contains(Event.current.mousePosition))
            {
                if (button == 0)
                {
                    var edge = GetEdgeAtPosition(Event.current.mousePosition);
                    SelectEdges(edge);
                }
            }
        }

        protected override void OnMouseUp(int button)
        {
            if (GetNodeBoxRect().Contains(Event.current.mousePosition))
            {
                if (button == 0)
                {
                    if (_isDraggingEdge)
                    {
                        foreach (var connectToNode in nodeEditors)
                        {
                            if (connectToNode.node.index == _dragEdgeStartNode.node.index)
                            {
                                continue; // We can never connect to ourselves...
                            }

                            foreach (var rect in connectToNode.GetReceivingEdgeConnectorRects())
                            {
                                if (connectToNode.CanConnectEdgeFromNode(_dragEdgeStartNode.node))
                                {
                                    if (ToZoomSpaceRect(rect).Contains(Event.current.mousePosition))
                                    {
                                        _dragEdgeStartNode.SetEdge(_dragEdgeStartEdgeConnectorIndex, connectToNode.node);
                                    }
                                }
                            }
                        }
                    }
                    else if (_isDraggingNode == false)
                    {
                        var hoveringNode = GetNodeAtPosition(Event.current.mousePosition);
                        if (hoveringNode == null)
                        {
                            GUI.FocusControl("NON_EXISTING");
                        }

                        if (selectedNodeEditors.Contains(hoveringNode))
                        {
                            if (Event.current.control)
                            {
                                hoveringNode.UnSelect();
                                selectedNodeEditors.Remove(hoveringNode);
                            }
                        }
                        else
                        {
                            SelectNodes(new List<NodeEditorBase>() { hoveringNode });
                        }
                    }
                    else
                    {
                        OnEndDrag();
                    }
                }
                else if (button == 1)
                {
                    RemoveEdgeAtPosition(Event.current.mousePosition);
                }
            }

            if (_isDraggingNode)
            {
                OnEndDrag();
            }
        }

        protected override void OnKeyDown(KeyCode keyCode)
        {
            base.OnKeyDown(keyCode);

//            if (keyCode == KeyCode.LeftControl || keyCode == KeyCode.RightControl)
//            {
//                GUI.FocusControl("QuestSystemPro_NonExisting"); // Remove focus from node to avoid missing key events like ctrl + c / v
//            }

//            // Space to quickly toggle zoomed out and zoomed in
//            if (keyCode == KeyCode.Space)
//            {
//                if (Mathf.Approximately(_zoomAmount, MinZoomAmount))
//                {
//                    Zoom(1f);
//                }
//                else
//                {
//                    Zoom(MinZoomAmount);
//                }
//            }

//            // TODO: Create setting to enable / disable arrow key movement behavior.
//            if (selectedNodeEditors.Count > 0)
//            {
//                var move = Vector2.zero;
//                if (keyCode == KeyCode.RightArrow)
//                    move.x += 1f;
//                else if (keyCode == KeyCode.LeftArrow)
//                    move.x -= 1f;
//                else if (keyCode == KeyCode.UpArrow)
//                    move.y -= 1f;
//                else if (keyCode == KeyCode.DownArrow)
//                    move.y += 1f;
//
//                if (Event.current.shift)
//                {
//                    move *= 20f;
//                }
//
//                move /= _zoomAmount;
//
//                foreach (var node in selectedNodeEditors)
//                {
//                    node.Drag(move);
//                }
//            }
        }

        private void SelectEdges(params Edge[] edges)
        {
            selectedEdges.Clear();
            selectedEdges.AddRange(edges.Where(o => o != null));
        }

        protected override void OnKeyUp(KeyCode keyCode)
        {
            base.OnKeyUp(keyCode);

            if (Event.current.control && keyCode == KeyCode.C)
            {
                // Copy
                CopyNodesToClipboard(selectedNodeEditors);
            }
            else if (Event.current.control && keyCode == KeyCode.V)
            {
                // Paste
                PasteNodesFromClipboard();
            }
            else if (Event.current.control && keyCode == KeyCode.D)
            {
                // Duplicate
                CopyNodesToClipboard(selectedNodeEditors);
                PasteNodesFromClipboard();
            }


            if (keyCode == KeyCode.Delete ||
                (((Event.current.modifiers & EventModifiers.Command) != 0) && keyCode == KeyCode.Backspace) ||
                (((Event.current.modifiers & EventModifiers.Control) != 0) && keyCode == KeyCode.Backspace))
            {
                if (selectedNodeEditors.Count > 0 && EditorGUIUtility.editingTextField == false)
                {
                    if (EditorUtility.DisplayDialog("Deleting nodes",
                    "Are you sure you want to delete " + selectedNodeEditors.Count + " nodes?", "Yes", "Cancel"))
                    {
                        RemoveNodes(selectedNodeEditors);
                    }
                }
            }
        }

        protected override void OnScroll(Vector2 delta)
        {
            base.OnScroll(delta);

//            Zoom(_zoomAmount + -delta.y * ZoomSpeed);
        }

        private void Zoom(float amount)
        {
            zoomAmount = amount;
            zoomAmount = Mathf.Clamp(zoomAmount, MinZoomAmount, MaxZoomAmount);
            zoomAmount = (float)System.Math.Round((decimal)zoomAmount, 1);
        }

        private void RemoveEdgeAtPosition(Vector2 position)
        {
            foreach (var node in nodeEditors)
            {
                var connectorRects = node.GetEdgeConnectorRects();
                for (uint i = 0; i < connectorRects.Length; i++)
                {
                    if (ToZoomSpaceRect(connectorRects[i]).Contains(position))
                    {
                        node.RemoveEdge(i);
                        return;
                    }
                }
            }
        }


        public void AddNode(NodeBase node)
        {
            dialogue.AddNode(node);
            nodeEditors.Add(CreateEditorForNode(node));

            EditorUtility.SetDirty(dialogue);
            GUI.changed = true;
            window.Repaint();
        }

        public void RemoveNode(NodeBase node)
        {
            dialogue.RemoveNode(node);
            nodeEditors.RemoveAt((int)node.index);

            EditorUtility.SetDirty(dialogue);
            GUI.changed = true;
            window.Repaint();
        }


        private void RemoveNodes(List<NodeEditorBase> nodes)
        {
            foreach (var node in nodes.Where(o => o.CanDelete()))
            {
                RemoveNode(node.node);
            }
        }

        private void DrawSidebar()
        {
            // Create some tabs
            using (new GroupBlock(new Rect(0, 0, sidebarWidth, EditorGUIUtility.singleLineHeight), GUIContent.none, "Toolbar"))
            {
                int buttonWidth = sidebarWidth / sidebars.Count;
                for (int i = 0; i < sidebars.Count; i++)
                {
                    var sidebar = sidebars[i];
                    if (GUI.Button(new Rect(buttonWidth * i, 0, buttonWidth, EditorGUIUtility.singleLineHeight), sidebar.name,
                        "toolbarbutton"))
                    {
                        // Select page
                        Debug.Log("Select " + sidebar.name);
                        selectedSidebar = sidebar;
                    }
                }
            }

            GUI.Label(new Rect(10, 30f, sidebarWidth - 20f, EditorGUIUtility.singleLineHeight), _selectedSidebar.name, EditorStyles.boldLabel);
            using (new GroupBlock(new Rect(10f, 50f, sidebarWidth - 20f, window.position.height - 20f)))
            {
                var r = new Rect(0, 0, sidebarWidth - 20f, EditorGUIUtility.singleLineHeight);
                if (selectedSidebar != null)
                {
                    selectedSidebar.Draw(r, this);
                }
                else
                {
                    GUI.Label(r, "Couldn't find page to load...");
                }
            }
        }

        private void DrawAllEdges()
        {
            foreach (var node in nodeEditors)
            {
                if (node == null)
                {
                    continue;
                }

                node.DrawEdges();
            }
        }


        private void OnPerformDrag(Vector2 delta)
        {
            delta /= zoomAmount;

            if (_isDraggingNode == false && _isDraggingSidebarWidth == false)
            {
                if (Event.current.button == 0)
                {
                    var hoveringNode = GetNodeAtPosition(Event.current.mousePosition);
                    if (hoveringNode != null)
                    {
                        if (selectedNodeEditors.Contains(hoveringNode) == false)
                        {
                            // No nodes selected + started drag on a node, select it directly.
                            SelectNodes(new List<NodeEditorBase>() { hoveringNode });
                        }
                    }
                    else
                    {
                        _isDrawingSelectionGrid = true;
                    }
                }

                // Start dragging
                _dragStartPosition = Event.current.mousePosition;
                _isDraggingNode = true;
            }
            else
            {
                if (sidebarResizableAreaRect.Contains(Event.current.mousePosition))
                {
                    _isDraggingSidebarWidth = true;

                    sidebarWidth -= (int)delta.x;
                    return;
                }

                if (Event.current.button == 2 || Event.current.button == 1)
                {
                    // Scrollwheel or righ click drag
                    nodesOffset += delta;
                    return;
                }

                if (selectedNodeEditors.Count > 0 && _isDrawingSelectionGrid == false)
                {
                    if (Event.current.button == 0)
                    {
                        foreach (var selectedNode in selectedNodeEditors)
                        {
                            selectedNode.Drag(delta);
                        }
                    }
                }
            }
        }

        private void OnEndDrag()
        {
            _isDraggingNode = false;
            _isDraggingEdge = false;
            _isDrawingSelectionGrid = false;
            _isDraggingSidebarWidth = false;
            _dragEdgeStartEdgeConnectorIndex = 0;
        }

        private void SelectNodes(List<NodeEditorBase> nodes)
        {
            nodes.RemoveAll(o => o == null);

            if (Event.current.control)
            {
                foreach (var node in nodes)
                {
                    if (selectedNodeEditors.Contains(node) == false)
                    {
                        selectedNodeEditors.Add(node);
                        node.Select();
                    }
                }
            }
            else
            {
                foreach (var node in selectedNodeEditors)
                {
                    node.UnSelect();
                }

                selectedNodeEditors.Clear();
                selectedNodeEditors.AddRange(nodes);

                foreach (var node in selectedNodeEditors)
                {
                    node.Select();
                }


                //                if (nodes.Count == 0)
                //                {
                //                    GUI.FocusControl("QuestSystemPro_NonExisting");
                //                }
            }
        }

        private Rect ToZoomSpaceRect(Rect rect)
        {
            rect.position *= zoomAmount;
            rect.size *= zoomAmount;

            return rect;
        }

        private List<NodeEditorBase> GetNodesInRect(Rect rect)
        {
            var selectedNodes = new List<NodeEditorBase>();
            foreach (var node in nodeEditors)
            {
                if (rect.Overlaps(ToZoomSpaceRect(node.GetNodeRect()), true))
                {
                    selectedNodes.Add(node);
                }
            }

            return selectedNodes;
        }

        private NodeEditorBase GetNodeAtPosition(Vector2 pos)
        {
            // Objects are drawn front to back, so last drawn item is on top; Reversed for to grab from back to front (top to bottom).
            for (int i = nodeEditors.Count - 1; i >= 0; i--)
            {
                var node = nodeEditors[i];
                if (ToZoomSpaceRect(node.GetNodeRect()).Contains(pos))
                {
                    return node;
//                    return new[] { node };
                }
            }

            return null;
//            return new NodeEditorBase[0];
        }

        private Edge GetEdgeAtPosition(Vector2 pos)
        {
            foreach (var node in nodeEditors)
            {
                for (uint i = 0; i < node.node.edges.Length; i++)
                {
                    if (i >= node.node.edges.Length || node.node.edges[i] == null)
                    {
                        continue;
                    }

                    var edge = node.node.edges[i];

                    var fromRect = ToZoomSpaceRect(node.GetEdgeConnectorRects()[i]);
                    var fromPos = fromRect.position;
                    fromPos.x += fromRect.width / 2;
                    fromPos.y += fromRect.height / 2;

                    var to = nodeEditors[(int)edge.toNodeIndex];
                    var toRect = ToZoomSpaceRect(to.GetReceivingEdgeConnectorRects()[0]);
                    var toPos = toRect.position;
                    toPos.x += toRect.width / 2;
                    toPos.y += toRect.height / 2;

                    var dist = Vector3.Distance(fromPos, toPos);
                    Vector3 startTangent = fromPos + Vector2.up * (dist / 3f);
                    Vector3 endTangent = toPos + Vector2.down * (dist / 3f);
                    if (HandleUtility.DistancePointBezier(pos, fromPos, toPos, startTangent, endTangent) < 10f)
                    {
//                        Debug.Log("Select edge " + edge.fromNodeIndex);
                        return edge;
                    }
                }
            }

            return null;
        }

        private void DrawAllNodes()
        {
            foreach (var node in nodeEditors)
            {
                var before = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 60f;

                // Only draw visible nodes
                var viewingRect = GetNodeBoxRect();
                viewingRect.x -= node.nodeSize.x * 2;
                viewingRect.width += node.nodeSize.x * 2;
                viewingRect.y -= node.nodeSize.y * 2;
                viewingRect.height += node.nodeSize.y * 2;
                viewingRect.position -= nodesOffset;

                if (viewingRect.Contains(node.node.editorPosition))
                {
                    if (zoomAmount < 0.7f)
                    {
                        node.Draw(false);
                    }
                    else
                    {
                        node.Draw(true);
                    }
                }

                EditorGUIUtility.labelWidth = before;
            }
        }

//        private Dialogue GetDialogueFromGameObjects(GameObject[] gameObjects)
//        {
//            foreach (var obj in gameObjects)
//            {
//                var dialogueUser = obj.GetComponent<IDialogueUser>();
//                if (dialogueUser != null)
//                {
//                    var d = dialogueUser.dialogues.FirstOrDefault();
//                    return d;
//                }
//            }
//
//            return null;
//        }

        public static T CreateAndAddNodeToCurrentDialog<T>() where T : NodeBase
        {
            return (T)CreateAndAddNodeToCurrentDialog(typeof (T));
        }

        public static NodeBase CreateAndAddNodeToCurrentDialog(Type type)
        {
            if (window != null && window.dialogue != null)
            {
                var inst = NodeFactory.Create(type);
                var pos = -window.nodesOffset + (window.position.size/2);
                pos.x -= sidebarWidth/2f;
                pos.x -= 100;
                pos.y -= 60;
                inst.editorPosition = pos;
                window.AddNode(inst);

                return inst;
            }

            return null;
        }

        public static void Edit(IDialogueOwner owner)
        {
            Init();
            if (window != null)
            {
                window.dialogueOwner = owner;
            }
        }

        public static void Edit(Dialogue dialogue)
        {
            Init();
            if (window != null)
            {
                window.dialogue = dialogue;
            }
        }
    }
}