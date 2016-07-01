#if INVENTORY_PRO

using UnityEngine;
using System.Collections.Generic;
using Devdog.InventorySystem;
using Devdog.QuestSystemPro.Integration.InventoryPro;
using Devdog.QuestSystemPro.UI;
using UnityEngine.UI;

namespace Devdog.QuestSystemPro.Integration.InventoryPro.UI
{
    public class InventoryProStatRewardRowUI : RewardRowUI
    {
        [Header("Options")]
        public bool overrideColor = false;
        public Color positiveColor = Color.green;
        public Color negativeColor = Color.red;

        [Header("UI Elements")]
        public Image icon;
        public Text statName;
        public Text statValue;

        public override void Repaint(IRewardGiver rewardGiver, Quest quest)
        {
            var r = (InventoryProStatRewardGiver) rewardGiver;
            var prop = r.property.property;

            if (icon != null)
            {
                icon.sprite = prop.icon;
            }

            if (statName != null)
            {
                statName.text = prop.name;
                statName.color = prop.color;
            }

            if(statValue != null)
            {
                statValue.text = r.property.ToString();
                if (overrideColor)
                {
                    statValue.color = r.property.floatValue >= 0f ? positiveColor : negativeColor;
                }
                else
                {
                    statValue.color = prop.color;
                }
            }
        }
    }
}

#endif