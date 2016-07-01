using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.QuestSystemPro.Dialogue.Editors;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Devdog.QuestSystemPro.Editors.ReflectionDrawers
{
    public abstract class ObjectPickerBase : EditorWindowBase
    {
        public enum SearchType
        {
            ObjectTypes,
            Components,
        }

        protected const int Columns = 4;
        protected const int InnerPadding = 3;

        public SearchType searchType;

        private List<UnityEngine.Object> _foundObjects = new List<UnityEngine.Object>();
        public List<UnityEngine.Object> foundObjects
        {
            get { return _foundObjects; }
            set { _foundObjects = value; }
        }

        public Dictionary<string, List<UnityEngine.Object>> foundObjectsDict
        {
            get { return CreateDictionary(foundObjects); }
        }

        public string currentSearchQuery = "";

        public Action<Object> callback;
        public Type type;
        public bool allowInherited;
        private Vector2 _scrollPos;


        public bool isSearching
        {
            get
            {
                return string.IsNullOrEmpty(currentSearchQuery) == false;
            }
        }

        public virtual void Init()
        {
            if (searchType == SearchType.Components)
            {
                foundObjects.AddRange(FindAssetsWithComponent(type, allowInherited));
            }
            else if (searchType == SearchType.ObjectTypes)
            {
                foundObjects.AddRange(FindAssetsOfType(type, allowInherited));
            }
        }

        protected virtual IEnumerable<UnityEngine.Object> FindAssetsWithComponent(Type type, bool allowInherited)
        {
            var l = new List<UnityEngine.Object>();
            if (typeof (UnityEngine.Component).IsAssignableFrom(type) == false)
            {
                return l; // Can't search, not a component type.
            }

            foreach (var guid in AssetDatabase.FindAssets("t:GameObject"))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);

                // Check for components
                var asset = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                var c = asset.GetComponent(type);
                if (c != null)
                {
                    if (IsObjectValidType(c, type, allowInherited))
                    {
                        l.Add(c);
                    }
                }
            }

            return l;
        }

        protected virtual IEnumerable<UnityEngine.Object> FindAssetsOfType(Type type, bool allowInherited)
        {
            var l = new List<UnityEngine.Object>();
            foreach (var guid in AssetDatabase.FindAssets("t:" + type.FullName))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);

                // Check for types
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, type);
                if (asset != null)
                {
                    if (IsObjectValidType(asset, type, allowInherited))
                    {
                        l.Add(asset);
                    }
                }
            }

            return l;
        }

        public virtual bool IsObjectValidType(Object asset, Type type, bool allowInherited)
        {
            if (asset.GetType() == type)
            {
                return true;
            }

            if (allowInherited)
            {
                if (type.IsInstanceOfType(asset))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool IsSearchMatch(Object asset, string searchQuery)
        {
            return asset.name.ToLower().Contains(currentSearchQuery.ToLower()) ||
                   asset.GetType().Name.ToLower().Contains(currentSearchQuery.ToLower());
        }

        protected override void OnGUI()
        {
            var cellSize = position.width/Columns - InnerPadding;

            using (new ScrollableBlock(new Rect(0, 0, position.width, position.height), ref _scrollPos, GetInternalHeight(cellSize)))
            {
                base.OnGUI();

                DrawSearchBar();
                DrawObjects(cellSize);
            }
        }

        protected virtual int GetInternalHeight(float cellSize)
        {
            return (int)(35 + foundObjectsDict.Sum(kvp => GetCategoryHeight(kvp.Key, cellSize)));
        }

        protected virtual float GetCategoryHeight(string category, float cellSize)
        {
            float height = EditorGUIUtility.singleLineHeight + 30;
            int rows;
            if (isSearching)
            {
                rows = Mathf.CeilToInt((float) foundObjectsDict[category].Count(o => IsSearchMatch(o, currentSearchQuery)) / Columns);
            }
            else
            {
                rows = Mathf.CeilToInt((float)foundObjectsDict[category].Count / Columns);
            }

            return height + (rows * cellSize + rows * InnerPadding);
        }

        protected virtual void DrawObjects(float cellSize)
        {
            float yOffset = 0f;

            foreach (var kvp in foundObjectsDict)
            {
                float categoryHeight = GetCategoryHeight(kvp.Key, cellSize);
                using (new GroupBlock(new Rect(0, yOffset, position.width, categoryHeight)))
                {
                    DrawCategory(kvp.Key, kvp.Value, cellSize);
                }

                yOffset += categoryHeight;
            }
        }

        protected virtual void DrawCategory(string categoryName, IEnumerable<UnityEngine.Object> objs, float cellSize)
        {
            const float topPadding = 20f;

            int row = 0;
            int col = 0;

            EditorGUI.LabelField(new Rect(InnerPadding, topPadding + InnerPadding + 10f, 200f, EditorGUIUtility.singleLineHeight), categoryName, UnityEditor.EditorStyles.boldLabel);
            float yOffset = EditorGUIUtility.singleLineHeight + 10f;

            foreach (var obj in objs)
            {
                if (isSearching)
                {
                    if (IsSearchMatch(obj, currentSearchQuery) == false)
                    {
                        continue;
                    }
                }

                var r = new Rect(InnerPadding + (col * (cellSize + InnerPadding)), topPadding + InnerPadding + (row * (cellSize + InnerPadding)) + yOffset, cellSize, cellSize);
                r.x = Mathf.RoundToInt(r.x); // Avoid ugly anti-aliasing in editor.
                r.y = Mathf.RoundToInt(r.y);

                GUI.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
                if (r.Contains(Event.current.mousePosition))
                {
                    GUI.color = Color.white;

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        callback(obj);
                        Close();
                    }
                }

                DrawObject(r, obj);
                GUI.color = Color.white;

                col = (col + 1) % Columns;
                if (col % Columns == 0)
                {
                    row++;
                }
            }
        }

        private Dictionary<string, List<UnityEngine.Object>> CreateDictionary(List<UnityEngine.Object> objs)
        {
            var d = new Dictionary<string, List<UnityEngine.Object>>();
            foreach (var o in objs)
            {
                if (d.ContainsKey(o.GetType().Name) == false)
                {
                    d[o.GetType().Name] = new List<UnityEngine.Object>();
                }

                d[o.GetType().Name].Add(o);
            }

            return d;
        }

        private void DrawSearchBar()
        {
            currentSearchQuery = EditorStyles.SearchBar(new Rect(InnerPadding, InnerPadding, position.width - InnerPadding*2, 30f), currentSearchQuery, this, isSearching);
            if (GUI.GetNameOfFocusedControl() != "SearchField")
            {
                GUI.FocusControl("SearchField");
            }
        }

        protected virtual void DrawObject(Rect r, Object obj)
        {
            using (new GroupBlock(r, GUIContent.none, "box"))
            {
                var cellSize = r.width;

                var labelRect = new Rect(0, 0, cellSize, EditorGUIUtility.singleLineHeight);
                GUI.Label(labelRect, obj.name);
                labelRect.y += EditorGUIUtility.singleLineHeight;
                GUI.Label(labelRect, obj.GetType().Name);

                var iconSize = Mathf.RoundToInt(cellSize * 0.6f);
                GUI.DrawTexture(new Rect(cellSize * 0.2f, cellSize * 0.4f - InnerPadding, iconSize, iconSize), AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(obj)));
            }
        }
    }
}
