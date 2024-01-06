using TES3Unity.ESM.Records;

namespace TES3Unity.Components.Records
{
    public class Activator : RecordComponent
    {
        private void Start()
        {
            usable = true;
            pickable = false;
            var ACTI = (ACTIRecord)record;
            objData.name = ACTI.Name;
            objData.interactionPrefix = "Use ";

            TryAddScript(ACTI.Script);
        }
    }
}