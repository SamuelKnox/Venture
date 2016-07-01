using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;
using Devdog.InventorySystem.UI;
using UnityEngine;


namespace Devdog.InventorySystem.Integration.RewiredIntegration
{
    [RequireComponent(typeof(UIWindow))]
    [AddComponentMenu("InventorySystem/Integration/Rewired Object trigger Range UI")]
    public partial class RewiredObjectTriggererRangeUI : MonoBehaviour
    {
        public UnityEngine.UI.Image imageIcon;
        public UnityEngine.UI.Text shortcutText;
        public bool moveToTriggererLocation = true;

        private UIWindow _window;
        protected virtual void Awake()
        {
            _window = GetComponent<UIWindow>();
        }

        protected virtual void Start()
        {
            InventoryPlayerManager.instance.OnPlayerChanged += OnPlayerChanged;
            if (InventoryPlayerManager.instance.currentPlayer != null)
            {
                OnPlayerChanged(null, InventoryPlayerManager.instance.currentPlayer);
            }
        }

        protected void LateUpdate()
        {
            if (moveToTriggererLocation && InventoryPlayerManager.instance.currentPlayer != null)
            {
                UpdatePosition(InventoryPlayerManager.instance.currentPlayer.rangeHelper.bestTriggerer);
            }
        }
        
        private void OnPlayerChanged(InventoryPlayer oldPlayer, InventoryPlayer newPlayer)
        {
            if (oldPlayer != null)
            {
                oldPlayer.rangeHelper.OnChangedBestTriggerer -= BestTriggererChanged;
            }

            newPlayer.rangeHelper.OnChangedBestTriggerer += BestTriggererChanged;

            // First time
            Repaint(newPlayer.rangeHelper.bestTriggerer);
        }

        private void BestTriggererChanged(ObjectTriggererBase old, ObjectTriggererBase newBest)
        {
            if (newBest != null)
            {
                _window.Show();
                Repaint(newBest);
            }
            else
            {
                _window.Hide();
            }
        }
        
        protected virtual void UpdatePosition(ObjectTriggererBase triggerer)
        {
            if(triggerer != null)
                transform.position = Camera.main.WorldToScreenPoint(triggerer.transform.position);
        }

        public virtual void Repaint(ObjectTriggererBase triggerer)
        {
            if(_window.isVisible == false)
                _window.Show();
            
            if (shortcutText != null)
            {
                if (triggerer != null)
                {
                    if (shortcutText.text != triggerer.triggerKeyCode.ToString())
                    {
                        shortcutText.text = triggerer.triggerKeyCode.ToString();
                    }
                    else
                    {
                        shortcutText.text = "";
                    }
                }
            }
        }
    }    
}
