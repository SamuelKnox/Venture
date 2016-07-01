using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Net;
using UnityEditor.Callbacks;

namespace Devdog.QuestSystemPro.Editors
{
    [InitializeOnLoad]
    public class GettingStartedEditor : EditorWindow
    {
        private struct ProductInfo
        {
            public string productName { get; set; }

            public string assetStoreUrl { get; set; }

            public Texture icon { get; set; }
        }

        private static GettingStartedEditor _window;
        public static GettingStartedEditor window
        {
            get
            {
                if (_window == null)
                {
                    _window = GetWindow<GettingStartedEditor>(true, ProductName + " - Getting started", true);
                    _window.minSize = new Vector2(SingleColWidth * 2 + (SingleColWidth / 25), 500);
                    _window.maxSize = _window.minSize;
                }

                return _window;
            }
        }


        private const int SingleColWidth = 400;
        private static Texture _documentationIcon;
        private static Texture _videoTutorialsIcon;
        private static Texture _reviewIcon;
        private static Texture _forumIcon;
        private static Texture _integrationsIcon;
        private static Texture _newsletterIcon;

        private static bool _showOnStart = true;
        private static int _heightExtra;

        private const string SignupNewsletterApiUrl = "http://devdog.io/unity/mailchimpsignup.php";
        private const string ProductsFetchApiUrl = "http://devdog.io/unity/editorproducts.php?product=" + ProductName;
        private const string ProductName = QuestSystemPro.ProductName;
        private const string MenuItemPath = QuestSystemPro.ToolsMenuPath +  "Getting started";
        private const string DocumentationUrl = QuestSystemPro.ProductUrl;
        private const string YoutubeUrl = "https://www.youtube.com/watch?v=kWeXmVIgqO4&list=PL_HIoK0xBTK4R3vX9eIT1QUl-fn78eyIM"; // TODO: Set correct URL
        private const string ForumUrl = "http://forum.devdog.io";
        private const string ReviewProductUrl = "https://www.assetstore.unity3d.com/en/content/31226"; // TODO: Set correct URL

        private const string IconRootPath = QuestSystemPro.ProductPath + "EditorStyles/";
        private const string DocumentationIconUri = IconRootPath + "Documentation.png";
        private const string VideoTutorialsIconUri = IconRootPath + "Youtube.png";
        private const string ReviewIconUri = IconRootPath + "Star.png";
        private const string ForumIconUri = IconRootPath + "Forum.png";
        private const string IntegrationIconUri = IconRootPath + "Integration.png";
        private const string NewsletterIconUri = IconRootPath + "MailNotSignedUp.png";
        private const string NewsletterSubscribedIconUri = IconRootPath + "MailSignedUp.png";

        private static readonly List<ProductInfo> _productInfo = new List<ProductInfo>();

        private static string editorPrefsKey
        {
            get { return "SHOW_" + ProductName +  "_GETTING_STARTED_WINDOW" + QuestSystemPro.Version; }
        }

        private static string _email;
        private const string DevdogNewsletterSignupKey = "DEVDOG_SIGNUP_EMAIL";
        private static string subscribedWithEmail
        {
            get
            {
                if (string.IsNullOrEmpty(_email) && EditorPrefs.HasKey(DevdogNewsletterSignupKey))
                {
                    _email = EditorPrefs.GetString(DevdogNewsletterSignupKey);
                }

                return _email;
            }
            set
            {
                EditorPrefs.SetString(DevdogNewsletterSignupKey, value);
                _email = value;
            }
        }

        private static bool _didReloadScripts = false;

        private static GUIStyle _boxStyle;
        public static GUIStyle boxStyle
        {
            get
            {
                if (_boxStyle == null)
                {
                    _boxStyle = new GUIStyle("HelpBox");
                    _boxStyle.padding = new RectOffset(10, 10, 10, 10);
                }

                return _boxStyle;
            }
        }

        [MenuItem(MenuItemPath, false, 1)] // Always at bottom
        public static void ShowWindow()
        {
            _showOnStart = EditorPrefs.GetBool(editorPrefsKey, true);
            GetImages();
            GetProducts();

            window.Show();
        }

        [InitializeOnLoadMethod]
        protected static void InitializeOnLoadMethod()
        {
            GetImages();
            GetProducts();

            EditorApplication.update += Update;
        }

        private static void Update()
        {
            if (_didReloadScripts == false)
            {
                ShowWindowIfRequired();
                _didReloadScripts = false;
            }

            EditorApplication.update -= Update;
        }

        [DidReloadScripts]
        protected static void DidReloadScripts()
        {
            _didReloadScripts = true;
        }

        private static void GetImages()
        {
            _documentationIcon = AssetDatabase.LoadAssetAtPath<Texture>(DocumentationIconUri);
            _videoTutorialsIcon = AssetDatabase.LoadAssetAtPath<Texture>(VideoTutorialsIconUri);
            _reviewIcon = AssetDatabase.LoadAssetAtPath<Texture>(ReviewIconUri);
            _forumIcon = AssetDatabase.LoadAssetAtPath<Texture>(ForumIconUri);
            _integrationsIcon = AssetDatabase.LoadAssetAtPath<Texture>(IntegrationIconUri);

            _newsletterIcon = AssetDatabase.LoadAssetAtPath<Texture>(NewsletterIconUri);
            if (EditorPrefs.HasKey(DevdogNewsletterSignupKey))
            {
                _newsletterIcon = AssetDatabase.LoadAssetAtPath<Texture>(NewsletterSubscribedIconUri);
            }
        }

        private static void GetProducts()
        {
            _productInfo.Clear();
            using (var www = new WWW(ProductsFetchApiUrl))
            {
                var startTime = EditorApplication.timeSinceStartup;
                while (www.isDone == false || EditorApplication.timeSinceStartup - startTime > 5f)
                {
                    // Wait...
                }

                var arr = www.text.Split(new string[] { ">>>>" }, StringSplitOptions.None);
                foreach (var s in arr)
                {
                    if (string.IsNullOrEmpty(s))
                    {
                        continue;
                    }

                    _productInfo.Add(ParseProductInfo(s));
                }
            }
        }

        private static ProductInfo ParseProductInfo(string s)
        {
            var info = s.Split('|');
            using (var www = new WWW(info[1]))
            {
                var startTime = EditorApplication.timeSinceStartup;
                while (www.isDone == false || EditorApplication.timeSinceStartup - startTime > 5f)
                {
                    // Wait...    
                }
                var icon = www.texture;

                return new ProductInfo()
                {
                    productName = info[0],
                    icon = icon,
                    assetStoreUrl = info[2],
                };
            }
        }

        public static void ShowWindowIfRequired()
        {
            if (EditorPrefs.GetBool(editorPrefsKey, true))
            {
                ShowWindow();
            }
        }
        
        public void OnGUI()
        {
            _heightExtra = 0;

            GUILayout.BeginHorizontal("Toolbar", GUILayout.Width(window.position.size.x));
            GUILayout.Label(ProductName + " Version: " + QuestSystemPro.Version);
            GUILayout.EndVertical();

            GUILayout.BeginArea(new Rect(0, 0, SingleColWidth, window.position.height));
            DrawGettingStarted();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(SingleColWidth + (SingleColWidth / 50), 0, SingleColWidth, 260));
            DrawMailSignupForm();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(SingleColWidth + (SingleColWidth / 50), 260, SingleColWidth, window.position.height - 260));
            DrawOtherProducts();
            GUILayout.EndArea();
        }

        private void DrawOtherProducts()
        {
            GUILayout.Space(30);

            EditorGUILayout.LabelField("Other great products you should check out.", UnityEditor.EditorStyles.boldLabel);

            for (int i = 0; i < _productInfo.Count; i++)
            {
                DrawProduct(i, _productInfo[i].icon, _productInfo[i].assetStoreUrl);
            }
        }

        private void DrawProduct(int index, Texture icon, string url)
        {
            var rect = new Rect(70 * index, 64, 64, 64);
            if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            {
                Application.OpenURL(url);
            }

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            GUI.DrawTexture(rect, icon);
        }

        private void DrawMailSignupForm()
        {
            GUI.DrawTexture(new Rect(SingleColWidth / 2 - 32, 30, 64, 64), _newsletterIcon);
            GUILayout.Space(100);
            
            EditorGUILayout.LabelField("Never miss a thing, sign up for the newsletter.", UnityEditor.EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Get notified about updates, upgrade guides and new products.");

            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();

//            EditorGUILayout.LabelField("Name");
//            _name = EditorGUILayout.TextField(_name);

            EditorGUILayout.LabelField("Email");
            _email = EditorGUILayout.TextField(_email);

            GUILayout.Space(10);

            if (EditorPrefs.HasKey(DevdogNewsletterSignupKey))
            {
                EditorGUILayout.LabelField("Subscribed with " + EditorPrefs.GetString(DevdogNewsletterSignupKey), UnityEditor.EditorStyles.wordWrappedLabel);
            }

            if (string.IsNullOrEmpty(_email))
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.color = Color.green;
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Subscribe", "LargeButton"))
            {
                SubscribeToMailingList(_email);
            }

            GUI.enabled = true;
            GUI.color = Color.white;
        }

        private void SubscribeToMailingList(string email)
        {
            string result = "";
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                result = client.UploadString(SignupNewsletterApiUrl + "?email_address=" + email, "POST");
            }

            if (result.Contains("\"status\":\"subscribed\""))
            {
                subscribedWithEmail = email;
                Debug.Log("Successfully subscribed to mailing list :)");

                Repaint();
            }
            else
            {
                Debug.Log("Whoops something went wrong while subscribing");
                Debug.Log(result);
            }

            GetImages(); // Update images for mail
        }

        private void DrawGettingStarted()
        {
            DrawBox(0, 0, "Documentation", "The official documentation has a detailed description of all components and code examples.", _documentationIcon, () =>
            {
                Application.OpenURL(DocumentationUrl);
            });

            DrawBox(1, 0, "Video tutorials", "The video tutorials cover all interfaces and a complete set up.", _videoTutorialsIcon, () =>
            {
                Application.OpenURL(YoutubeUrl);
            });

            DrawBox(2, 0, "Forums", "Check out the " + ProductName + " forums for some community power.", _forumIcon, () =>
            {
                Application.OpenURL(ForumUrl);
            });

            DrawBox(3, 0, "Integrations", "Combine the power of assets and enable integrations.", _integrationsIcon, () =>
            {
                IntegrationHelperEditor.ShowWindow();
            });

            DrawBox(4, 0, "Rate / Review", "Like " + ProductName + "? Share the experience :)", _reviewIcon, () =>
            {
                Application.OpenURL(ReviewProductUrl);
            });

            var toggle = GUI.Toggle(new Rect(10, window.minSize.y - 20, SingleColWidth - 10, 20), _showOnStart, "Show " + ProductName + " getting started on start.");
            if (toggle != _showOnStart)
            {
                _showOnStart = toggle;
                EditorPrefs.SetBool(editorPrefsKey, toggle);
            }
        }

        private void DrawBox(int index, int extraHeight, string title, string desc, Texture texture, Action action)
        {
            _heightExtra += extraHeight;

            const int spacing = 10;
            const int offset = 30;
            int offsetY = offset + (spacing * index) + (64 * index);

            GUI.BeginGroup(new Rect(10, offsetY, SingleColWidth - 20, 64 + _heightExtra), boxStyle);

            var rect = new Rect(0, 0, SingleColWidth - 20, 64 + _heightExtra);
            if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            {
                action();
            }

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

            var iconRect = new Rect(15, 7, 50, 50);
            GUI.DrawTexture(iconRect, texture);
            
            rect.x = 74;
            rect.y += 5;
            rect.width = SingleColWidth - 90;

            GUI.Label(rect, title, UnityEditor.EditorStyles.boldLabel);

            rect.y += 20;
            rect.height = 44;
            GUI.Label(rect, desc, UnityEditor.EditorStyles.wordWrappedLabel);
            
            GUI.EndGroup();
        }

        private bool Button(GUIContent content)
        {
            return GUILayout.Button(content, GUILayout.Width(128), GUILayout.Height(128));
        }
    }
}