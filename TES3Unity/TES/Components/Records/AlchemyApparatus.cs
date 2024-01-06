using TES3Unity.ESM.Records;

namespace TES3Unity.Components.Records
{
    public class AlchemyApparatus : RecordComponent
    {
        private void Start()
        {
            var APPA = (APPARecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = APPA.Name;
            objData.weight = APPA.Data.Weight.ToString();
            objData.value = APPA.Data.Value.ToString();
            objData.interactionPrefix = "Take ";

            TryAddScript(APPA.Script);
        }
    }
}