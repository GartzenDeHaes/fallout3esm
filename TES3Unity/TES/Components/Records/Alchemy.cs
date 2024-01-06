using TES3Unity.ESM.Records;

namespace TES3Unity.Components.Records
{
    public class Alchemy : RecordComponent
    {
        private void Start()
        {
            var ALCH = (ALCHRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = ALCH.Name;
            objData.weight = ALCH.Data.Weight.ToString();
            objData.value = ALCH.Data.Value.ToString();
            objData.interactionPrefix = "Take ";

            TryAddScript(ALCH.Script);
        }
    }
}
