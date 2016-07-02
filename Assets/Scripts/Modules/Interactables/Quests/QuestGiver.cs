﻿using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuestGiver : Interactable
{
    [Tooltip("Text where quest is going to be displayed")]
    [SerializeField]
    private TextMeshPro questText;

    [Tooltip("What the quest giver will say when they have no more quests to give")]
    [SerializeField]
    private string outOfQuestsDialog;

    [Tooltip("What the quest giver will say if the player is not qualified to perform the quest")]
    [SerializeField]
    private string notQualifiedDialog;

    [Tooltip("Quests that can be given")]
    [SerializeField]
    private List<Quest> quests = new List<Quest>();
    private Player player;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        if (!player)
        {
            Debug.LogError("Could not find player!", gameObject);
        }
        questText.enabled = false;
    }

    void Start()
    {
        SortQuests();
    }

    /// <summary>
    /// Fetches the quest
    /// </summary>
    public override void Interact()
    {
        ActivateQuest();
    }

    /// <summary>
    /// Ends the quest fetch
    /// </summary>
    public override void EndInteraction()
    {
        questText.enabled = false;
    }

    /// <summary>
    /// Activates the quest and has the quest giver present it
    /// </summary>
    public void ActivateQuest()
    {
        questText.enabled = true;
        if (quests.Count == 0)
        {
            questText.text = outOfQuestsDialog;
            return;
        }
        var activeQuest = quests.Where(q => q.IsQualified()).FirstOrDefault();
        if (!activeQuest)
        {
            questText.text = notQualifiedDialog;
            return;
        }
        if (activeQuest.IsComplete())
        {
            Debug.LogError("Attempting to active quest " + activeQuest + ", but this quest has already been completed!", activeQuest.gameObject);
            return;
        }
        questText.text = activeQuest.GetDescription();
        player.AddQuest(activeQuest);
        activeQuest.OnQuestComplete += OnQuestComplete;
    }

    /// <summary>
    /// Called when quest is complete
    /// </summary>
    /// <param name="quest">Quest which was completed</param>
    private void OnQuestComplete(Quest quest)
    {
        quests.Remove(quest);
        quest.OnQuestComplete -= OnQuestComplete;
    }

    /// <summary>
    /// Sorts the quests as desired
    /// </summary>
    private void SortQuests()
    {
        quests = quests.OrderBy(q => q.GetDifficulty()).ThenBy(q => UnityEngine.Random.Range(0.0f, 1.0f)).ToList();
    }
}