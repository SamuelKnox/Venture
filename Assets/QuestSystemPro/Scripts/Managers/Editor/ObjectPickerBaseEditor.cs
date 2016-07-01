using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.QuestSystemPro.Editors
{
    public abstract class ObjectPickerBaseEditor<T> : EditorWindow where T : class
    {
        public Action<T> OnPickObject { get; set; }

        public virtual EditorWindow window { get; protected set; }
        public string windowTitle { get; set; }
        
        // Not a list to add serialization
        protected IList<T> objects;
        protected Vector2 scrollPosition;
        public string searchQuery;

        private bool focusOnInput = true;
        public int selectionIndex;
        public Vector2 minSizeVec { get; set; }
        public bool isUtility { get; protected set; }

        public bool isSearching
        {
            get { return string.IsNullOrEmpty(searchQuery) == false && searchQuery != "Search..."; }
        }


        public ObjectPickerBaseEditor() : base()
        {
            objects = new List<T>();
        }

        protected void NotifyPickedObject(T obj)
        {
            if (OnPickObject != null)
                OnPickObject(obj);

            Close();
        }


        /// <summary>
        /// Find objects of type in asset database.
        /// </summary>
        /// <returns></returns>
        protected abstract IList<T> FindObjects(bool searchProjectFolder);


        public new virtual void Show()
        {
            Show(true);
        }

        /// <summary>
        /// Show the window, searches all available items in the asset database.
        /// </summary>
        public new virtual void Show(bool searchProjectFolder)
        {
            if (searchProjectFolder)
            {
                Show(FindObjects(searchProjectFolder));
            }
        }

        public virtual void Show(IList<T> objectsToPickFrom)
        {
            window = GetWindow(GetType(), isUtility);

            objects = objectsToPickFrom;
            selectionIndex = 0;
            focusOnInput = true;
            window.titleContent = new GUIContent(windowTitle);
            window.minSize = minSizeVec;

            window.Show();
        }

        public new virtual void Close()
        {
            if (window != null)
                window.Close();
        }


        public virtual void OnGUI()
        {
            searchQuery = EditorStyles.SearchBar(searchQuery, this, isSearching);
            if (focusOnInput)
            {
                EditorGUI.FocusTextInControl("SearchField");
                focusOnInput = false;
            }

            ShowInfoBox();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            int resultCount = 0;
            T selectedObject = null;
            foreach (var obj in objects)
            {
                EditorGUILayout.BeginHorizontal();

                if (isSearching)
                {
                    string search = searchQuery.ToLower();
                    if (MatchesSearch(obj, search))
                    {
                        if (resultCount == selectionIndex)
                        {
                            GUI.color = Color.green;
                            selectedObject = obj;
                        }

                        DrawObjectButton(obj);
                        resultCount++;
                    }
                }
                else
                {
                    if (resultCount == selectionIndex)
                    {
                        GUI.color = Color.green;
                        selectedObject = obj;
                    }

                    DrawObjectButton(obj);
                    resultCount++;
                }

                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();


            if (Event.current.isKey)
            {
                selectionIndex = Mathf.Clamp(selectionIndex, 0, resultCount - 1);

                // Keyboard movement
                if (Event.current.keyCode == KeyCode.DownArrow)
                {
                    selectionIndex++;
                    Repaint();
                }
                else if (Event.current.keyCode == KeyCode.UpArrow)
                {
                    selectionIndex--;
                    Repaint();
                }


                // When pressing enter, the selected item
                if (Event.current.keyCode == KeyCode.Return)
                {
                    if (resultCount > 0 && selectedObject != null)
                    {
                        NotifyPickedObject(selectedObject);
                    }
                    else
                    {
                        EditorGUI.FocusTextInControl("SearchField");
                        Repaint();
                    }
                }
            }
        }


        protected virtual void DrawObjectButton(T obj)
        {
            if (GUILayout.Button(obj.ToString()))
                NotifyPickedObject(obj);
        }


        protected abstract bool MatchesSearch(T obj, string search);

        protected virtual void ShowInfoBox()
        {
            EditorGUILayout.HelpBox("Use the up and down arrow keys to select an item.\nHit enter to pick the highlighted item.", MessageType.Info);            
        }
    }
}