using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


namespace Devdog.QuestSystemPro.Editors
{
    public class SetupWizardEditor : EditorWindow
    {
        internal class SetupIssue
        {
            public string saveName { get; set; }
            private string finalSaveName
            {
                get { return QuestSystemPro.ProductName + saveName; }
            }

            public string message { get; set; }
            public MessageType messageType { get; set; }
            public List<IssueAction> actions { get; set; }


            public bool ignore
            {
                get
                {
                    return EditorPrefs.GetBool(finalSaveName, false);
                }
                set
                {
                    EditorPrefs.SetBool(finalSaveName, value);
                }
            }



            public SetupIssue(string saveName, string message, MessageType messageType, System.Action fixAction)
                : this(saveName, message, messageType, new IssueAction("Fix", fixAction))
            {

            }

            public SetupIssue(string saveName, string message, MessageType messageType, params IssueAction[] action)
            {
                this.saveName = saveName;
                this.message = message;
                this.messageType = messageType;

                this.actions = action.ToList();
                if (this.actions == null)
                    this.actions = new List<IssueAction>();
            }
        }

        internal class IssueAction
        {
            public System.Action action;
            public string name;

            public IssueAction(string name, System.Action action)
            {
                this.name = name;
                this.action = action;
            }
        }



        private static List<SetupIssue> _setupIssues;
        internal static List<SetupIssue> setupIssues
        {
            get
            {
                if (_setupIssues == null)
                    _setupIssues = new List<SetupIssue>();

                return _setupIssues;
            }
            set
            {
                _setupIssues = value;
            }
        }

        private Vector2 scrollPos { get; set; }


        internal delegate void IssuesUpdated(List<SetupIssue> issues);
        internal static event IssuesUpdated OnIssuesUpdated;


        [MenuItem(QuestSystemPro.ToolsMenuPath + "Setup wizard", false, 2)] // Always at bottom
        public static void ShowWindow()
        {
            var window = GetWindow<SetupWizardEditor>(true, QuestSystemPro.ProductName + " - Setup wizard", true);
            window.minSize = new Vector2(400, 500);
            //window.maxSize = new Vector2(400, 500);

            CheckScene();
            window.Repaint();
        }


        [UnityEditor.Callbacks.DidReloadScripts]
        private static void DidReloadScripts()
        {
            CheckScene();
        }

        public static void CheckScene()
        {
            setupIssues.Clear();

            CheckManagers();

            if (OnIssuesUpdated != null)
                OnIssuesUpdated(setupIssues);
        }

        private static void CheckManagers()
        {
            var managers = GameObject.Find("_Managers");
            if (managers == null)
            {
                setupIssues.Add(new SetupIssue("managers_obj", "No managers object found", MessageType.Error, () =>
                {
                    var m = new GameObject("_Managers");
                    m.AddComponent<QuestManager>(); // Adds the other managers
                }));

                return;
            }
        }


        private static T GetOrAddComponent<T>(GameObject obj, string saveName, string message, MessageType error) where T : Component
        {
            var comp = obj.GetComponent<T>();
            if (comp == null)
            {
                setupIssues.Add(new SetupIssue(saveName, message, error, () =>
                {
                    obj.AddComponent<T>();
                }));
            }

            return comp;
        }


        public void OnGUI()
        {
            //CheckScene();

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            GUILayout.BeginHorizontal("Toolbar");

            if (setupIssues.Sum(o => o.ignore ? 1 : 0) > 0)
            {
                if (GUILayout.Button("Clear ignore list", "toolbarbutton"))
                {
                    var i = setupIssues.FindAll(o => o.ignore);
                    foreach (var issue in i)
                    {
                        issue.ignore = false;
                    }

                    CheckScene();
                }
            }

            GUI.color = Color.green;
            if (GUILayout.Button("Force rescan", "toolbarbutton"))
            {
                CheckScene();
                Repaint();
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();


            if (setupIssues.Sum(o => o.ignore ? 0 : 1) == 0)
            {
                EditorGUILayout.HelpBox("No problems found...", MessageType.Info);
            }


            bool shouldUpdate = false;
            foreach (var issue in setupIssues)
            {
                if (issue.ignore)
                    continue;

                EditorGUILayout.HelpBox(issue.message, issue.messageType);


                GUILayout.BeginHorizontal("Toolbar");
                foreach (var action in issue.actions)
                {
                    if (action.name == "Fix")
                        GUI.color = Color.green;

                    if (GUILayout.Button(action.name, "toolbarbutton"))
                    {
                        action.action();
                        shouldUpdate = true;
                    }

                    GUI.color = Color.white;
                }


                GUI.color = Color.yellow;
                if (GUILayout.Button("Ignore", "toolbarbutton"))
                {
                    issue.ignore = true;
                    shouldUpdate = true;
                }
                GUI.color = Color.white;

                GUILayout.EndHorizontal();
            }

            // To avoid editing the list while itterating.
            if (shouldUpdate)
            {
                CheckScene();
                Repaint();
            }

            GUILayout.EndScrollView();
        }
    }
}