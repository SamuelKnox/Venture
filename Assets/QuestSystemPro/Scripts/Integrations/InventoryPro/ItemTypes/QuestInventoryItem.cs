#if INVENTORY_PRO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;

namespace Devdog.QuestSystemPro.Integration.InventoryPro
{
    public class QuestInventoryItem : InventoryItemBase
    {
        [InventoryRequired]
        public Quest quest;
        public bool useQuestWindow = true;
        public bool removeItemWhenUsed = true;
        
        /// <summary>
        /// When the item is used, play this sound.
        /// </summary>
        public InventoryAudioClip audioClipWhenUsed;

        public override LinkedList<InventoryItemInfoRow[]> GetInfo()
        {
            var list = base.GetInfo();

            list.AddFirst(new InventoryItemInfoRow[]{
                new InventoryItemInfoRow("Quest", quest.name),
            });

            return list;
        }

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

            if (currentStackSize <= 0)
                return -2;

            if (quest.CanActivate().status == false)
                return -2;

            // Do something with item
            InventoryAudioManager.AudioPlayOneShot(audioClipWhenUsed);

            if (useQuestWindow)
            {
                QuestManager.instance.questWindowUI.acceptCallback.AddListener(ActivateQuest);
                QuestManager.instance.questWindowUI.Repaint(quest);
            }
            else
            {
                if (quest.status == QuestStatus.InActive || quest.status == QuestStatus.Cancelled)
                {
                    ActivateQuest(quest);
                }
            }

            return 1;
        }

        public virtual void ActivateQuest(Quest q)
        {
            q.Activate();

            if (removeItemWhenUsed)
            {
                currentStackSize--; // Remove 1
                NotifyItemUsed(1, true);
            }
        }
    }
}

#endif