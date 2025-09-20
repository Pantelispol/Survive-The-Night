using UnityEngine;

namespace Script.Controller
{
    public class Consumable : Item
    {
        public override void ConsumeItem()
        {
            base.ConsumeItem();


            Debug.Log("Consumable used");
        }
    }
}