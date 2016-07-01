using System;
using UnityEngine;
using System.Collections.Generic;
using Devdog.QuestSystemPro.UI;
using UnityEngine.Serialization;

namespace Devdog.QuestSystemPro
{
    public partial class ObjectTrigger : ObjectTriggerBase
    {
        /// <summary>
        /// When true the window will be triggered directly, if false, a 2nd party will have to handle it through events.
        /// </summary>
        [HideInInspector]
        [NonSerialized]
        public bool handleWindowDirectly = true;

        public override float useDistance
        {
            get
            {
                return QuestManager.instance.settingsDatabase.objectUseDistance;
            }
        }

        /// <summary>
        /// Only required if handling the window directly
        /// </summary>
        [Header("The window")]
        [SerializeField]
        private UIWindowField _window;
        public UIWindowField window
        {
            get { return _window; }
            set { _window = value; }
        }

        [Header("Animations & Audio")]
        public AnimationClip useAnimation;
        public AnimationClip unUseAnimation;

        public AudioClipInfo useAudioClip;
        public AudioClipInfo unUseAudioClip;


        public Animator animator { get; protected set; }

        protected static ObjectTrigger previousTriggerer { get; set; }


        protected virtual void Awake()
        {
            isInUse = false;
            animator = GetComponent<Animator>();
        }

        private void WindowOnHide()
        {
            UnUse();
        }

        private void WindowOnShow()
        {

        }

        public override void NotifyCameInRange(QuestSystemPlayer player)
        {
            base.NotifyCameInRange(player);
        }

        public override void NotifyWentOutOfRange(QuestSystemPlayer player)
        {
            base.NotifyWentOutOfRange(player);
            UnUse();
        }

        public void DoVisuals()
        {
            if (useAnimation != null && animator != null)
                animator.Play(useAnimation.name);

            AudioManager.AudioPlayOneShot(useAudioClip);
        }

        public void UndoVisuals()
        {
            if (unUseAnimation != null && animator != null)
                animator.Play(unUseAnimation.name);

            AudioManager.AudioPlayOneShot(unUseAudioClip);
        }

        public sealed override bool Use(bool fireEvents = true)
        {
            return Use(QuestManager.instance.currentPlayer, fireEvents);
        }

        public override bool Use(QuestSystemPlayer player, bool fireEvents = true)
        {
            if (enabled == false || inRange == false)
                return false;

            if (isInUse)
                return true;

//            if (UIUtility.CanReceiveInput(gameObject) == false)
//                return false;

            if (previousTriggerer != null)
            {
                previousTriggerer.UnUse(player, fireEvents);
            }

            if (window.window != null)
            {
                window.window.OnShow += WindowOnShow;
                window.window.OnHide += WindowOnHide;

                if (handleWindowDirectly)
                {
                    window.window.Show();
                }
            }

            DoVisuals();
            isInUse = true;

            if (fireEvents)
            {
                NotifyTriggerUsed(player);
            }

            previousTriggerer = this;

            return true;
        }

        public override bool UnUse(bool fireEvents = true)
        {
            return UnUse(QuestManager.instance.currentPlayer, fireEvents);
        }

        public override bool UnUse(QuestSystemPlayer player, bool fireEvents = true)
        {
            if (enabled == false || inRange == false)
                return false;

            if (isInUse == false)
                return true;

//            if (UIUtility.CanReceiveInput(gameObject) == false)
//                return false;

            UndoVisuals();
            isInUse = false;

            if (window.window != null)
            {
                window.window.OnShow -= WindowOnShow;
                window.window.OnHide -= WindowOnHide;

                if (handleWindowDirectly)
                {
                    window.window.Hide();
                }
            }

            if (fireEvents)
            {
                NotifyTriggerUnUsed(player);
            }

            previousTriggerer = null;
            return true;
        }
    }
}