#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{

    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Adds currency to a collection.")]
    public class AddCurrencyToCollection : FsmStateAction
    {
        //public ItemCollectionBase collection;
        public FsmInt currencyID;
        public FsmFloat amount = 1f;
        public ItemCollectionBase collection;

        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            collection.AddCurrency(amount.Value, (uint)currencyID.Value);
            Finish();
        }
    }
}

#endif