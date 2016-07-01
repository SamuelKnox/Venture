#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{

    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Adds currency to a collection.")]
    public class AddCurrencyToCollectionFsmObject : FsmStateAction
    {
        //public ItemCollectionBase collection;
        public FsmInt currencyID;
        public FsmFloat amount = 1f;
        public FsmObject collection;

        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            var c = collection.Value as ItemCollectionBase;
            if (c == null)
            {
                Debug.Log("Can't add currency, given type is not an ItemCollectionBase type");
                Finish();
                return;
            }

            c.AddCurrency(amount.Value, (uint)currencyID.Value);
            Finish();
        }
    }
}

#endif