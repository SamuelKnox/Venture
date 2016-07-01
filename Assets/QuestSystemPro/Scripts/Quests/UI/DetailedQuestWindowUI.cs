using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Devdog.QuestSystemPro.UI
{
    [RequireComponent(typeof(UIWindow))]
    public class DetailedQuestWindowUI : MonoBehaviour
    {
        [Header("UI References")]
        public QuestWindowUIBase questDetailsWindow;
        public RectTransform questButtonContainer;
        public RectTransform noQuestSelected;

        public QuestSidebarUI questSidebarUI;


        [Header("Prefabs")]
        public QuestButtonUI questButtonUIPrefab;


        protected Dictionary<Quest, QuestButtonUI> uiCache = new Dictionary<Quest, QuestButtonUI>();
        protected Quest selectedQuest;

        protected virtual void Awake()
        {
            
        }

        protected virtual void Start()
        {
            SetQuestWindowVisible(false);
            QuestManager.instance.OnQuestStatusChanged += OnQuestStatusChanged;
        }

        private void SetQuestWindowVisible(bool b)
        {
            questDetailsWindow.gameObject.SetActive(b);

            if (noQuestSelected != null)
            {
                noQuestSelected.gameObject.SetActive(!b);
            }
        }

        protected virtual void OnQuestStatusChanged(Quest quest)
        {
            switch (quest.status)
            {
                case QuestStatus.InActive:
                case QuestStatus.Completed:
                case QuestStatus.Cancelled:

                    if (selectedQuest == quest)
                    {
                        SetQuestWindowVisible(false);
                    }

                    if (uiCache.ContainsKey(quest))
                    {
                        var a = uiCache[quest];
                        uiCache.Remove(quest);
                        Destroy(a.gameObject);
                    }

                    break;
                case QuestStatus.Active:

                    SaveQuestToggledState(quest, PlayerPrefs.HasKey(QuestUtility.GetQuestCheckedSaveKey(quest)));
                    Repaint(quest);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual void Repaint(Quest quest)
        {
            if (uiCache.ContainsKey(quest) == false)
            {
                uiCache[quest] = CreateUIButtonForQuest(quest);
            }

            SetQuestWindowVisible(true);
            uiCache[quest].Repaint(quest);
        }

        protected virtual QuestButtonUI CreateUIButtonForQuest(Quest quest)
        {
            var uiInst = Instantiate<QuestButtonUI>(questButtonUIPrefab);
            uiInst.transform.SetParent(questButtonContainer);
            UIUtility.ResetTransform(uiInst.transform);

            var questTemp = quest;
            uiInst.button.onClick.AddListener(() =>
            {
                OnQuestButtonClicked(questTemp);
            });

            if (uiInst.toggle != null)
            {
                uiInst.toggle.onValueChanged.AddListener((isOn) =>
                {
                    OnQuestToggleValueChanged(questTemp, isOn);
                });
            }

            return uiInst;
        }

        protected virtual void OnQuestButtonClicked(Quest quest)
        {
            selectedQuest = quest;
            questDetailsWindow.Repaint(quest); // Repaint the window's details.
        }

        protected virtual void OnQuestToggleValueChanged(Quest quest, bool isOn)
        {
            SaveQuestToggledState(quest, isOn);
        }

        private void SaveQuestToggledState(Quest quest, bool isOn)
        {
            var playerPrefsKey = QuestUtility.GetQuestCheckedSaveKey(quest);
            if (isOn)
            {
                PlayerPrefs.SetInt(playerPrefsKey, 1);
                if (questSidebarUI != null && questSidebarUI.ContainsQuest(quest) == false)
                {
                    questSidebarUI.AddQuest(quest);
                }
            }
            else
            {
                if (PlayerPrefs.HasKey(playerPrefsKey))
                {
                    PlayerPrefs.DeleteKey(playerPrefsKey);
                    questSidebarUI.RemoveQuest(quest);
                }
            }
        }
    }
}