using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Dialogs;
using Devdog.InventorySystem.UI;
using Devdog.InventorySystem.Models;
using UnityEngine.Serialization;

namespace Devdog.InventorySystem
{
    /// <summary>
    /// Used to trigger a physical object such as vendor, treasure chests etc.
    /// </summary>
    [AddComponentMenu("InventorySystem/Triggers/Object triggererHandler")]
    public partial class ObjectTriggerer : ObjectTriggererBase
    {

        [Serializable]
        public struct ItemBoolPair
        {
            public InventoryItemAmountRow item;
            public bool remove;
        }


        #region Events

        public delegate void TriggerAction(InventoryPlayer player);

        public event TriggerAction OnTriggerUsed;
        public event TriggerAction OnTriggerUnUsed;

        #endregion

        public string triggerName
        {
            get { return objectInfo != null ? objectInfo.name : string.Empty; }
        }

        [SerializeField]
        private bool _triggerMouseClick = true;
        public override bool triggerMouseClick
        {
            get { return _triggerMouseClick; }
            set { _triggerMouseClick = value; }

        }

        [SerializeField]
        private KeyCode _triggerKeyCode = KeyCode.None;
        public override KeyCode triggerKeyCode
        {
            get { return _triggerKeyCode; }
            set { _triggerKeyCode = value; }
        }

        public override InventoryCursorIcon cursorIcon
        {
            get { return InventorySettingsManager.instance.useCursor; }
            set { InventorySettingsManager.instance.useCursor = value; }
        }

        public override Sprite uiIcon
        {
            get { return InventorySettingsManager.instance.objectTriggererFPSUseSprite; }
            set { InventorySettingsManager.instance.objectTriggererFPSUseSprite = value; }
        }

        /// <summary>
        /// When true the window will be triggered directly, if false, a 2nd party will have to handle it through events.
        /// </summary>
        [HideInInspector]
        [NonSerialized]
        public bool handleWindowDirectly = true;

        /// <summary>
        /// Toggle when triggered
        /// </summary>
        public bool toggleWhenTriggered = true;

        /// <summary>
        /// Only required if handling the window directly
        /// </summary>
        [Header("The window")]
        [FormerlySerializedAs("window")]
        [SerializeField]
        private UIWindowField _window;
        public UIWindowField window
        {
            get
            {
                return _window;
            }
            set
            {
                _window = value;
            }
        }


        [Header("Requirements")]
        public ItemBoolPair[] requiredItems = new ItemBoolPair[0];
        public InventoryItemPropertyRequirementLookup[] propertyRequirements = new InventoryItemPropertyRequirementLookup[0];

        [Header("Animations & Audio")]
        public AnimationClip useAnimation;
        public AnimationClip unUseAnimation;

        public InventoryAudioClip useAudioClip;
        public InventoryAudioClip unUseAudioClip;


        public Animator animator { get; protected set; }
        public SelectableObjectInfo objectInfo { get; protected set; }

        /// <summary>
        /// Check if there's a other component on this object that can handle the triggerer.
        /// (Called by editor, don't remove)
        /// </summary>
        public bool isControlledByOther
        {
            get
            {
                return GetComponent(typeof(IObjectTriggerUser)) != null;
            }
        }

        protected static ObjectTriggerer previousTriggerer { get; set; }


        protected override void Start()
        {
            base.Start();

            isActive = false;
            animator = GetComponent<Animator>();
            objectInfo = GetComponent<SelectableObjectInfo>();
        }

        private void WindowOnHide()
        {
            UnUse();
        }

        private void WindowOnShow()
        {

        }

        public override void NotifyCameInRange(InventoryPlayer player)
        {
            
        }

        public override void NotifyWentOutOfRange(InventoryPlayer player)
        {
            UnUse();
        }
        


        public override void OnMouseDown()
        {
            if (enabled == false)
                return;

            if (triggerMouseClick && InventoryUIUtility.isHoveringUIElement == false)
            {
                if (toggleWhenTriggered)
                    Toggle();
                else
                    Use();
            }
        }


        public override bool Toggle(InventoryPlayer player, out bool removeSource, bool fireEvents = true)
        {
            if (window.window != null && window.window.isVisible && isActive)
            {
                removeSource = false; // Never destroy an ObjectTriggerer, they're static.
                return UnUse(player, fireEvents);
            }

            return Use(player, out removeSource, fireEvents);
        }

        public void DoVisuals()
        {
            if (useAnimation != null && animator != null)
                animator.Play(useAnimation.name);

            InventoryAudioManager.AudioPlayOneShot(useAudioClip);
        }

        public void UndoVisuals()
        {
            if (unUseAnimation != null && animator != null)
                animator.Play(unUseAnimation.name);

            InventoryAudioManager.AudioPlayOneShot(unUseAudioClip);
        }

        protected override void NotifyTriggererUsed(InventoryPlayer usedByPlayer)
        {
            base.NotifyTriggererUsed(usedByPlayer);

            if (OnTriggerUsed != null)
                OnTriggerUsed(usedByPlayer);

            InventoryTriggererManager.instance.NotifyTriggererUsed(this);
        }

        protected override void NotifyTriggererUnUsed(InventoryPlayer unUsedByPlayer)
        {
            base.NotifyTriggererUnUsed(unUsedByPlayer);

            if (OnTriggerUnUsed != null)
                OnTriggerUnUsed(unUsedByPlayer);

            InventoryTriggererManager.instance.NotifyTriggererUnUsed(this);
        }


        public sealed override bool Use(bool fireEvents = true)
        {
            return base.Use(fireEvents);
        }

        public sealed override bool Use(out bool removeSource, bool fireEvents = true)
        {
            return Use(InventoryPlayerManager.instance.currentPlayer, out removeSource, fireEvents);
        }

        public override bool Use(InventoryPlayer player, out bool removeSource, bool fireEvents = true)
        {
            removeSource = false; // ObjectTriggers are "static" and aren't moved / changed after usage.

            if (enabled == false || inRange == false)
                return false;

            if (isActive)
                return true;

            if (InventoryUIUtility.CanReceiveInput(gameObject) == false)
                return false;

            // Has requireid items?
            var required = requiredItems.Select(o => o.item);
            bool canRemoveItems = InventoryManager.CanRemoveItems(required.ToList());
            if (canRemoveItems == false)
            {
                InventoryManager.langDatabase.triggererCantBeUsedMissingItems.Show(triggerName, required.Select(o => o.item.name).Aggregate((a, b) => a + ", " + b));
                return false;
            }

            foreach (var req in propertyRequirements)
            {
                if (req.CanUse(player) == false)
                {
                    InventoryManager.langDatabase.triggererCantBeUsedFailedPropertyRequirements.Show(triggerName, req.property.name);
                    return false;
                }
            }

            if (previousTriggerer != null)
            {
                previousTriggerer.UnUse(player, fireEvents);
            }

            if (window.window != null)
            {
                window.window.OnShow += WindowOnShow;
                window.window.OnHide += WindowOnHide;

                if (handleWindowDirectly && fireEvents)
                {
                    if (toggleWhenTriggered)
                        window.window.Toggle();
                    else if (window.window.isVisible == false)
                        window.window.Show();
                }
            }

            DoVisuals();
            isActive = true;

            foreach (var requiredItem in requiredItems)
            {
                if (requiredItem.remove)
                {
                    InventoryManager.RemoveItem(requiredItem.item.item.ID, requiredItem.item.amount, false);
                }
            }

            if (fireEvents)
            {
                NotifyTriggererUsed(player);
            }

            previousTriggerer = this;

            return true;
        }

        public override bool UnUse(bool fireEvents = true)
        {
            return UnUse(InventoryPlayerManager.instance.currentPlayer, fireEvents);
        }

        public override bool UnUse(InventoryPlayer player, bool fireEvents = true)
        {
            if (enabled == false || inRange == false)
                return false;

            if (isActive == false)
                return true;

            if (InventoryUIUtility.CanReceiveInput(gameObject) == false)
                return false;

            if (handleWindowDirectly && fireEvents && window.window != null)
            {
                window.window.Hide();
            }

            UndoVisuals();


            isActive = false;

            if (window.window != null)
            {
                window.window.OnShow -= WindowOnShow;
                window.window.OnHide -= WindowOnHide;
            }

            if (OnTriggerUnUsed != null && fireEvents)
                OnTriggerUnUsed(player);

            InventoryTriggererManager.instance.NotifyTriggererUnUsed(this);

            previousTriggerer = null;
            return true;
        }
    }
}