using TES3Unity.ESM.Records;

namespace TES3Unity.Components.Records
{
    public class ProbeItem : RecordComponent
    {
        private void Start()
        {
            var PROB = (PROBRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = PROB.Name;
            objData.weight = PROB.Data.Weight.ToString();
            objData.value = PROB.Data.Value.ToString();
            objData.interactionPrefix = "Take ";

            TryAddScript(PROB.Script);
        }
    }
}