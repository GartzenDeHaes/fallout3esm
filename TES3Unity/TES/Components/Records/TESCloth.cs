using TES3Unity.ESM.Records;

namespace TES3Unity.Components.Records
{
    public class TESCloth : RecordComponent
    {
        private void Start()
        {
            var CLOT = (CLOTRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = CLOT.Name;
            objData.weight = CLOT.Data.Weight.ToString();
            objData.value = CLOT.Data.Value.ToString();
            objData.interactionPrefix = "Take ";

            TryAddScript(CLOT.Script);
        }
    }
}