#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Get a stats value and store it in a FSM variable.")]
    public class GetStatFsmObject : FsmStateAction
    {
        public FsmObject player;

        public FsmString statCategoryName = "Default";
        public FsmString statName;

        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("The result (final value) after adding")]
        public FsmFloat result;


        public bool everyFrame;


        public override void Reset()
        {

        }

        public override void OnUpdate()
        {
            DoGetStat();

            if (!everyFrame)
                Finish();
        }

        public override void OnEnter()
        {
            DoGetStat();
        }

        private void DoGetStat()
        {
            if (player.Value == null)
            {
                player.Value = InventoryPlayerManager.instance.currentPlayer;
            }

            var p = player.Value as InventoryPlayer;
            if (p == null)
            {
                Finish();
                return;
            }

            if (p.characterCollection == null)
            {
                LogWarning("No character collection set on player.");
                return;
            }

            var stat = p.characterCollection.stats.Get(statCategoryName.Value, statName.Value);
            if (stat == null)
            {
                LogWarning("Stat in category " + statCategoryName + " with name " + statName + " does not exist.");
                return;
            }

            result.Value = stat.currentValue;
        }
    }
}

#endif