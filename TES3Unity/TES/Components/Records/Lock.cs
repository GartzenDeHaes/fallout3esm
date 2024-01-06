using TES3Unity.ESM.Records;

namespace TES3Unity.Components.Records
{
    public class Lock : RecordComponent
    {
        private void Start()
        {
            usable = true;
            pickable = false;
            var LOCK = (LOCKRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = LOCK.Name;
            objData.weight = LOCK.Data.Weight.ToString();
            objData.value = LOCK.Data.Value.ToString();

            TryAddScript(LOCK.Script);
        }
    }
}