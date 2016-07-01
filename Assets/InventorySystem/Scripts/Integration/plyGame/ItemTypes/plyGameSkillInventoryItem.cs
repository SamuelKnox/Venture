#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem;
using plyCommon;
using plyGame;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame
{
    public partial class plyGameSkillInventoryItem : InventoryItemBase
    {

        [InventoryRequired]
        public Skill skill;


        public override void NotifyItemUsed(uint amount, bool alsoNotifyCollection)
        {
            base.NotifyItemUsed(amount, alsoNotifyCollection);
            InventoryItemUtility.SetItemProperties(InventoryPlayerManager.instance.currentPlayer, properties, InventoryItemUtility.SetItemPropertiesAction.Use);
        }

        public override int Use()
        {
            int used = base.Use();
            if (used < 0)
                return used;

            if (Player.Instance.actor.actorClass.currLevel < requiredLevel)
            {
                InventoryManager.langDatabase.itemCannotBeUsedLevelToLow.Show(name, description, requiredLevel);
                return -1;
            }

            // Use plyGame skill
            if (skill != null)
            {
                Player.Instance.actor.QueueSkillForExecution(skill, true);
            }

            NotifyItemUsed(1, true);
            return 1;
        }
    }
}

#endif