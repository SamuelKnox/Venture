using System.Linq;
using TMPro;
using UnityEngine;

public class QuestsView : MonoBehaviour
{
    [Tooltip("Quest description")]
    [SerializeField]
    private TextMeshProUGUI description;

    [Tooltip("Text to be displayed when there are no active quests")]
    [SerializeField]
    private string noQuestDescription;

    private int questIndex = 0;
    private Quest[] activeQuests;

    void Awake()
    {
        UpdateQuests();
    }

    void Start()
    {
        UpdateQuestDescription(null);
    }

    /// <summary>
    /// Updates the quest list to show only the active quests
    /// </summary>
    public void UpdateQuests()
    {
        var player = FindObjectOfType<Player>();
        if (!player)
        {
            Debug.LogError("Could not find player!", gameObject);
            return;
        }
        activeQuests = player.GetQuests().Where(q => !q.IsComplete()).ToArray();
        if (activeQuests.Length > 0)
        {
            UpdateQuestDescription(activeQuests[questIndex]);
        }
        else
        {
            UpdateQuestDescription(null);
        }
    }

    /// <summary>
    /// Switches the quest being displayed by moving its index
    /// </summary>
    /// <param name="movement">How far to the left or right to switch the quest being viewed</param>
    public void SwitchQuest(int movement)
    {
        UpdateQuests();
        if (activeQuests.Length == 0)
        {
            UpdateQuestDescription(null);
            return;
        }
        questIndex = (questIndex + movement) % activeQuests.Length;
        if (questIndex < 0)
        {
            questIndex = activeQuests.Length - 1;
        }
        UpdateQuestDescription(activeQuests[questIndex]);
    }

    /// <summary>
    /// Changes the display to show the specified quest
    /// </summary>
    /// <param name="quest">Quest to display</param>
    private void UpdateQuestDescription(Quest quest)
    {
        description.text = quest ? quest.GetDescription() : noQuestDescription;
    }
}