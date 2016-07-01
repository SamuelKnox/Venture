#if REWIRED

using UnityEngine;
using System.Collections;
using System.Linq;
using Devdog.InventorySystem.UI;
using Rewired;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem.Integration.RewiredIntegration
{
    [AddComponentMenu("InventorySystem/Integration/Rewired/Rewired UI Element Key Actions")]
    public partial class RewiredUIElementKeyActions : MonoBehaviour
    {
        [System.Serializable]
        public class KeyAction : UnityEngine.Events.UnityEvent
        {

        }

        [Header("Action")]
        public bool useSystemPlayer = false;
        public int rewiredPlayerID = 0;
        public string rewiredActionName;



        public KeyAction keyActions = new KeyAction();

        /// <summary>
        /// The time the action has to be active before invoking the action.
        /// </summary>
        [Header("Timers")]
        public float activationTime = 0.0f;
        public bool continous = false;

        [Header("Visuals")]
        public UIShowValueModel visualizer = new UIShowValueModel();



        /// <summary>
        /// The time (duration) the action has been activated.
        /// </summary>
        private bool _firedInActiveTime;
        private UIWindow _window;
        private Rewired.Player _player;


        protected virtual void Awake()
        {
            if (useSystemPlayer)
            {
                _player = Rewired.ReInput.players.GetSystemPlayer();
            }
            else
            {
                _player = Rewired.ReInput.players.GetPlayer(rewiredPlayerID);
            }

            Assert.IsNotNull(_player, "Rewired player with ID " + rewiredPlayerID + " could not be found!");
            Assert.IsTrue(Rewired.ReInput.mapping.Actions.Any(o => o.name == rewiredActionName), "No rewired action found with name: " + rewiredActionName);

            _window = GetComponent<UIWindow>();
        }

        protected virtual void Update()
        {
            if (gameObject.activeInHierarchy == false)
                return;

            if (_window != null && _window.isVisible == false)
                return;


            var rewiredButton = _player.GetButton(rewiredActionName);
            var rewiredButtonDown = _player.GetButtonDown(rewiredActionName);
            if (activationTime <= 0.01f)
            {
                if (continous)
                {
                    if (rewiredButton)
                    {
                        Activate();
                    }
                }
                else
                {
                    if (rewiredButtonDown)
                    {
                        Activate();
                    }
                }

                return;
            }

            // Got a timer.

            float rewiredTimeDown = _player.GetButtonTimePressed(rewiredActionName);
            visualizer.Repaint(rewiredTimeDown, 1);


            // Timer
            if (rewiredButton == false)
            {
                _firedInActiveTime = false;
            }

            // Timer check
            if (rewiredTimeDown < activationTime)
                return;

            // Time set, it's activated...
            if (continous)
            {
                if (rewiredButton)
                {
                    keyActions.Invoke();
                }
            }
            else
            {
                // Already fired / invoked events?
                if (_firedInActiveTime)
                    return;

                if (rewiredButton)
                {
                    keyActions.Invoke();
                    _firedInActiveTime = true;
                }
            }
        }

        protected virtual void Activate()
        {
            if (InventoryUIUtility.CanReceiveInput(gameObject) == false)
                return;

            keyActions.Invoke();
            visualizer.Activate();
        }
    }
}

#endif