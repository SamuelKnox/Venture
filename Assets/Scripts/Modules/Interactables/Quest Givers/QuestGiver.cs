using CustomUnityLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuestGiver : Interactable
{
    [Tooltip("Container used to store quests")]
    [SerializeField]
    private Transform questContainer;

    [Tooltip("Text where quest is going to be displayed")]
    [SerializeField]
    private TextMeshPro questText;

    [Tooltip("What the quest giver will say when they have no more quests to give")]
    [SerializeField]
    private string outOfQuestsDialog;

    [Tooltip("What the quest giver will say if the player is not qualified to perform the quest")]
    [SerializeField]
    private string notQualifiedDialog;

    [Tooltip("Whether or not this is a main questline questgiver.  If yes, the quests will be saved across playthroughs.")]
    [SerializeField]
    private bool mainQuestGiver = false;

    private Player player;
    private Quest activeQuest;

    protected override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<Player>();
        if (!player)
        {
            Debug.LogError("Could not find player!", gameObject);
        }
        questText.enabled = false;
    }

    void Start()
    {
        SetUpQuests();
    }

    /// <summary>
    /// Fetches the quest
    /// </summary>
    public override void OnInteractionEnter()
    {
        questText.enabled = true;
        var quests = questContainer.GetComponentsInChildren<Quest>().ToList();
        var questsToRemove = new List<Quest>();
        foreach (var quest in quests)
        {
            if (quest.transform == questContainer)
            {
                continue;
            }
            foreach (var playerQuest in player.GetQuests())
            {
                if (quest.name == playerQuest.name)
                {
                    if (playerQuest.IsComplete())
                    {
                        questsToRemove.Add(quest);
                        Destroy(quest.gameObject);
                    }
                    else
                    {
                        activeQuest = playerQuest;
                    }
                }
            }
        }
        foreach(var quest in questsToRemove)
        {
            quests.Remove(quest);
        }
        if (quests.Count() == 0 && !activeQuest)
        {
            questText.text = outOfQuestsDialog;
            return;
        }
        ActivateQuest(quests.ToArray());
    }

    /// <summary>
    /// Ends the quest fetch
    /// </summary>
    public override void OnInteractionExit()
    {
        questText.enabled = false;
    }

    /// <summary>
    /// Activates the quest and has the quest giver present it
    /// </summary>
    public void ActivateQuest(Quest[] quests)
    {
        if (!activeQuest)
        {
            activeQuest = quests.Where(q => q.IsQualified()).OrderBy(q => q.GetDifficulty()).ThenBy(q => UnityEngine.Random.Range(0.0f, 1.0f)).FirstOrDefault();
        }
        if (!activeQuest)
        {
            questText.text = notQualifiedDialog;
            return;
        }
        activeQuest.enabled = true;
        if (activeQuest.IsComplete())
        {
            Debug.LogError("Attempting to active quest " + activeQuest + ", but this quest has already been completed!", activeQuest.gameObject);
            return;
        }
        activeQuest.SetLongTermQuest(mainQuestGiver);
        player.AddQuest(activeQuest);
        activeQuest.OnQuestComplete += OnQuestComplete;
        questText.text = activeQuest.GetDescription();
    }

    /// <summary>
    /// Called when quest is complete
    /// </summary>
    /// <param name="quest">Quest which was completed</param>
    private void OnQuestComplete(Quest quest)
    {
        if (quest != activeQuest)
        {
            Debug.LogError("Completed a quest that was not the active quest!", quest.gameObject);
            return;
        }
        quest.OnQuestComplete -= OnQuestComplete;
        activeQuest = null;
    }

    /// <summary>
    /// Sorts the quests as desired and disables them
    /// </summary>
    private void SetUpQuests()
    {
        var quests = questContainer.GetComponentsInChildren<Quest>();
        foreach (var quest in quests)
        {
            quest.enabled = false;
        }
    }
}