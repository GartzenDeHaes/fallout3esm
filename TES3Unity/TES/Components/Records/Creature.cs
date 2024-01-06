using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity.Components.Records
{
    public class Creature : RecordComponent
    {
        protected CREARecord _creature;

        private void Start()
        {
            _creature = (CREARecord)record;

            transform.rotation = Quaternion.Euler(-70, 0, 0);

            TryAddScript(_creature.Script);
        }
    }
}
