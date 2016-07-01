using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Devdog.QuestSystemPro.UI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(UIWindow))]
    public class AchievementCompletedUI : MonoBehaviour
    {
        public QuestRowModelUI uiModel = new QuestRowModelUI();

        [Header("Audio & Visuals")]
        public float showForSeconds = 2f;

        protected Queue<TaskPreviousProgressPair> queue = new Queue<TaskPreviousProgressPair>();
        protected UIWindow window;
        protected Animator animator;

        private Coroutine _coroutine;

        protected virtual void Awake()
        {
            window = GetComponent<UIWindow>();
            animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            QuestManager.instance.OnAchievementStatusChanged += OnAchievementStatusChanged;
        }

        private void OnAchievementStatusChanged(Achievement self)
        {
            if (self.status == QuestStatus.Completed)
            {
                uiModel.Repaint(self);
                window.Show();

                if(_coroutine != null)
                {
                    StopCoroutine(_coroutine);   
                }
                _coroutine = StartCoroutine(_WaitAndHideWindow());
            }
        }

        private IEnumerator _WaitAndHideWindow()
        {

            yield return new WaitForSeconds(showForSeconds);
            window.Hide();

        }
    }
}