using CustomUnityLibrary;
using System.Collections.Generic;
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

    [Tooltip("Whether or not this is a main questline questgiver.  If yes, the quests will be saved across playthroughs.")]
    [SerializeField]
    private bool mainQuestGiver = false;

    private Player player;

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
        if (quests.Count == 0)
        {
            questText.text = outOfQuestsDialog;
            return;
        }
        var questsToRemove = new List<Quest>();
        foreach (var quest in quests)
        {
            foreach (var playerQuest in player.GetQuests())
            {
                if (quest.name == playerQuest.name.TrimEnd(GameObjectUtility.CloneSuffix))
                {
                    if (!playerQuest.IsComplete())
                    {
                        questText.text = playerQuest.GetDescription();
                        return;
                    }
                    else
                    {
                        questsToRemove.Add(quest);
                    }
                }
            }
        }
        foreach (var quest in questsToRemove)
        {
            quests.Remove(quest);
        }
        ActivateQuest();
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
    public void ActivateQuest()
    {
        var activeQuest = quests.Where(q => q.IsQualified()).FirstOrDefault();
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
        questText.text = activeQuest.GetDescription();
        activeQuest.SetLongTermQuest(mainQuestGiver);
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
    /// Sorts the quests as desired and disables them
    /// </summary>
    private void SetUpQuests()
    {
        quests = quests.OrderBy(q => q.GetDifficulty()).ThenBy(q => Random.Range(0.0f, 1.0f)).ToList();
        foreach (var quest in quests)
        {
            quest.enabled = false;
        }
    }
}