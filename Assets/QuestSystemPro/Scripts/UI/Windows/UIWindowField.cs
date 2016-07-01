using System;
using Devdog.QuestSystemPro.UI;
using UnityEngine;

namespace Devdog.QuestSystemPro.UI
{
    [System.Serializable]
    public class UIWindowField : UIWindowField<UIWindow>
    { }

    [System.Serializable]
    public class UIWindowField<T> where T : UIWindow
    {
        public bool useDynamicSearch = false;
        public string name;

        [SerializeField]
        private T _window;
        public T window
        {
            get
            {
                if (_window == null && useDynamicSearch)
                {
                    _window = UIWindow.FindByName<T>(name);
                }

                return _window;
            }
        }
    }
}