using UnityEngine;
using System.Collections;
using Devdog.QuestSystemPro.Dialogue.UI;
using Devdog.QuestSystemPro.UI;


namespace Devdog.QuestSystemPro
{
    [CreateAssetMenu(menuName = QuestSystemPro.ProductName + "/Settings Database")]
    public partial class SettingsDatabase : ScriptableObject
    {
        [Category("Quests"), Header("Quests")]
        public int playerMaxActiveQuests = 10;

        [Category("Triggers"), Header("Triggers")]
        public float objectUseDistance = 10f;

        [Category("UI Prefabs"), Header("UI Prefabs")]
        public RewardRowUI defaultRewardRowUI;
        public TaskProgressRowUI defaultTaskRowUI;

        [Header("Node UI Prefabs")]
        public DefaultNodeUI defaultNodeUIPrefab;
        public DefaultNodeUI playerDecisionNodeUI;
        public DefaultNodeUI playerInputNodeUIPrefab;

        [Header("Dialogue UI")]
        public Sprite playerDialogueIcon;

    }
}