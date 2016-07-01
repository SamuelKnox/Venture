using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Devdog.QuestSystemPro.UI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(UIWindow))]
    public class AchievementTaskProgressUI : MonoBehaviour
    {
        public QuestRowModelUI uiModel = new QuestRowModelUI();
        public bool showOverAchievement = false;

        [Header("Progress value")]
        public bool hideAfterSeconds = true;
        public float showForSeconds = 5f;
        public float interpSpeed = 1f;
        public AnimationCurve interpCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        protected Queue<TaskPreviousProgressPair> queue = new Queue<TaskPreviousProgressPair>();
        protected UIWindow window;

        protected virtual void Awake()
        {
            window = GetComponent<UIWindow>();
        }

        protected virtual void Start()
        {
            QuestManager.instance.OnAchievementTaskProgressChanged += OnAchievementTaskProgressChanged;
            StartCoroutine(ShowAchievementProgress());
        }

        protected virtual void OnAchievementTaskProgressChanged(float taskProgressBefore, Task task, Achievement achievement)
        {
            if (task.progress <= task.progressCap || showOverAchievement)
            {
                queue.Enqueue(new TaskPreviousProgressPair(taskProgressBefore, task));
            }
        }

        private IEnumerator ShowAchievementProgress()
        {
            while (true)
            {
                if (queue.Count > 0)
                {
                    bool alreadyVisible = window.isVisible;
                    window.Show();

                    while (queue.Count > 0)
                    {
                        var a = queue.Dequeue();
                        uiModel.Repaint(a.task);

                        // To avoid playing the audio twice (on show and here)
                        if (alreadyVisible)
                        {
                            window.PlayAudio(window.showAudioClip);
                        }

                        yield return StartCoroutine(InterpolateValueTo(a));
                    }

                    float timer = 0f;
                    while (timer < showForSeconds)
                    {
                        if (queue.Count != 0)
                        {
                            // An item got added while we were waiting, abort and go back to showing.
                            break;
                        }

                        timer += Time.deltaTime;
                        yield return null;
                    }

                    // We waited the entire time, show out animation.
                    if (timer >= showForSeconds && hideAfterSeconds)
                    {
                        window.Hide();
                    }
                }

                yield return null;
            }
        }

        private IEnumerator InterpolateValueTo(TaskPreviousProgressPair to)
        {
            float timer = 0f;
            while (timer < interpSpeed)
            {
                timer += Time.deltaTime;

                var from = to.taskProgressBefore / to.task.progressCap;
                var nValue = interpCurve.Evaluate(timer) * (to.task.progressNormalized - from);
                uiModel.progress.Repaint(from + nValue, 1f);

                yield return null;
            }
        }
    }
}