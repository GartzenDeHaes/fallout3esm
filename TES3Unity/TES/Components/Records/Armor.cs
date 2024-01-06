using TES3Unity.ESM.Records;

namespace TES3Unity.Components.Records
{
    public class Armor : RecordComponent
    {
        private void Start()
        {
            var ARMO = (ARMORecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = ARMO.Name;
            objData.weight = ARMO.Data.Weight.ToString();
            objData.value = ARMO.Data.Value.ToString();
            objData.interactionPrefix = "Take ";

            TryAddScript(ARMO.Script);
        }
    }
}