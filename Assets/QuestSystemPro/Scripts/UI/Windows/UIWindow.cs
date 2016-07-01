using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.QuestSystemPro.UI
{
    using UnityEngine.Serialization;

    [RequireComponent(typeof(Animator))]
    public partial class UIWindow : MonoBehaviour
    {
        [System.Serializable]
        public class UIWindowActionEvent : UnityEngine.Events.UnityEvent
        {

        }


        #region Events & Delegates

        /// <summary>
        /// Event is fired when the window is hidden.
        /// </summary>
        public event Action OnHide;

        /// <summary>
        /// Event is fired when the window becomes visible.
        /// </summary>
        public event Action OnShow;

        #endregion


        #region Variables 


        [Header("Behavior")]
        public string windowName = "MyWindow";

        /// <summary>
        /// Should the window be hidden when the game starts?
        /// </summary>
        public bool hideOnStart = true;

        /// <summary>
        /// Set the position to 0,0 when the game starts
        /// </summary>
        public bool resetPositionOnStart = true;



        /// <summary>
        /// The animation played when showing the window, if null the item will be shown without animation.
        /// </summary>
        [Header("Audio & Visuals")]
        [SerializeField]
        private AnimationClip _showAnimation;
        public int showAnimationHash { get; protected set; }

        /// <summary>
        /// The animation played when hiding the window, if null the item will be hidden without animation. 
        /// </summary>
        [SerializeField]
        private AnimationClip _hideAnimation;
        public int hideAnimationHash { get; protected set; }

        public AudioClip showAudioClip;
        public AudioClip hideAudioClip;



        [Header("Actions")]
        public UIWindowActionEvent onShowActions = new UIWindowActionEvent();
        public UIWindowActionEvent onHideActions = new UIWindowActionEvent();


        /// <summary>
        /// Is the window visible or not? Used for toggling.
        /// </summary>
        public bool isVisible { get; protected set; }

        private IEnumerator _animationCoroutine;


        private Animator _animator;
        public Animator animator
        {
            get
            {
                if (_animator == null)
                    _animator = GetComponent<Animator>();

                return _animator;
            }
        }

        private RectTransform _rectTransform;
        protected RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }



        #endregion




        protected virtual void Awake()
        {
            if (_showAnimation != null)
                showAnimationHash = Animator.StringToHash(_showAnimation.name);

            if (_hideAnimation != null)
                hideAnimationHash = Animator.StringToHash(_hideAnimation.name);

            LevelStart();

            if (resetPositionOnStart)
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }

        protected virtual void Start()
        {
#if UNITY_5_4_OR_NEWER
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadMode) =>
            {
                LevelStart();
            };
#endif
        }

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3

        public void OnLevelWasLoaded(int level)
        {
            LevelStart();
        }

#endif

        protected virtual void LevelStart()
        {
            if (hideOnStart)
            {
                HideFirst();
            }
            else
            {
                isVisible = true;
            }
        }


        #region Notifies

        public void NotifyWindowHidden()
        {
            onHideActions.Invoke();

            if (OnHide != null)
                OnHide();

        }

        public void NotifyWindowShown()
        {
            onShowActions.Invoke();

            if (OnShow != null)
                OnShow();

        }

        #endregion



        protected virtual void SetChildrenActive(bool active)
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(active);
            }

            var img = gameObject.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
                img.enabled = active;
        }

        public void PlayAudio(AudioClip clip)
        {
            var source = GetComponent<AudioSource>();
            if (source != null)
            {
                source.PlayOneShot(clip);
            }
        }

        private void PlayAnimation(AnimationClip clip, int hash, Action callback)
        {
            if (clip != null)
            {
                if (_animationCoroutine != null)
                {
                    StopCoroutine(_animationCoroutine);
                }

                _animationCoroutine = _PlayAnimationAndDisableAnimator(clip.length + 0.1f, hash, callback);
                StartCoroutine(_animationCoroutine);
            }
            else
            {
                animator.enabled = false;
                if (callback != null)
                    callback();

            }
        }

        public virtual void Toggle()
        {
            if (isVisible)
                Hide();
            else
                Show();
        }

        public virtual void Show()
        {
            _DoShow(true);
        }

        public virtual void Show(bool fireEvents)
        {
            _DoShow(fireEvents);
        }

        public void Show(float waitTime)
        {
            Show(waitTime, true);
        }

        public void Show(float waitTime, bool fireEvents)
        {
            StartCoroutine(_Show(waitTime, fireEvents));
        }

        protected virtual IEnumerator _Show(float waitTime, bool fireEvents = true)
        {
            if (isVisible)
                yield break;

            yield return StartCoroutine(CustomWait(waitTime));

            _DoShow(fireEvents);
        }

        private void _DoShow(bool fireEvents = true)
        {
            if (isVisible)
            {
                return;
            }

            isVisible = true;
            SetChildrenActive(true);
            PlayAnimation(_showAnimation, showAnimationHash, null);
            PlayAudio(showAudioClip);

            if (fireEvents)
                NotifyWindowShown();
        }

        public virtual void HideFirst()
        {
            isVisible = false;
            animator.enabled = false;

            SetChildrenActive(false);
        }

        /// <summary>
        /// Convenience method for easy upgrading...
        /// </summary>
        public virtual void Hide()
        {
            _DoHide(true);
        }

        public virtual void Hide(bool fireEvents)
        {
            _DoHide(fireEvents);
        }

        public void Hide(float waitTime)
        {
            Hide(waitTime, true);
        }

        public void Hide(float waitTime, bool fireEvents)
        {
            StartCoroutine(_WaitAndStartHide(waitTime, fireEvents));
        }

        protected virtual IEnumerator _WaitAndStartHide(float waitTime, bool fireEvents = true)
        {
            if (isVisible == false)
                yield break;

            yield return StartCoroutine(CustomWait(waitTime));
 
            _DoHide(fireEvents);
        }

        private void _DoHide(bool fireEvents)
        {
            if (isVisible == false)
            {
                return;
            }

            isVisible = false;
            PlayAnimation(_hideAnimation, hideAnimationHash, () =>
            {
                // Still invisible? Maybe it got shown while we waited.
                if (isVisible == false)
                    SetChildrenActive(false);
            });

            PlayAudio(hideAudioClip);

            if (fireEvents)
                NotifyWindowHidden();
        }


        /// <summary>
        /// Hides object after animation is completed.
        /// </summary>
        protected virtual IEnumerator _PlayAnimationAndDisableAnimator(float waitTime, int hash, Action callback)
        {
            yield return null; // Needed for some reason, Unity bug??

            var before = _animationCoroutine;
            animator.enabled = true;
            animator.Play(hash);

            yield return StartCoroutine(CustomWait(waitTime));

            // If action completed without any other actions overriding isVisible should be true. It could be hidden before the coroutine finished.
            animator.enabled = false;
            if (callback != null)
                callback();

            if (before == _animationCoroutine)
            {
                // Didn't change curing coroutine
                _animationCoroutine = null;
            }
        }

        protected IEnumerator CustomWait(float waitTime)
        {
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + waitTime)
            {
                yield return null;
            }
        }

        public static UIWindow FindByName(string name)
        {
            return FindByName<UIWindow>(name);
        }

        public static T FindByName<T>(string name) where T : UIWindow
        {
            return FindObjectsOfType<T>().FirstOrDefault(o => o.windowName == name);
        }
    }
}
