using TES3Unity.ESM.Records;

namespace TES3Unity.Components.Records
{
    public class MiscObject : RecordComponent
    {
        private void Start()
        {
            var MISC = (MISCRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(MISC.ITEX.value, "icons"); 
            objData.name = MISC.Name;
            objData.weight = MISC.Data.Weight.ToString();
            objData.value = MISC.Data.Value.ToString();
            objData.interactionPrefix = "Take ";

            TryAddScript(MISC.Script);
        }

        public override void Interact()
        {

        }
    }
}
