using TES3Unity.ESM.Records;

namespace TES3Unity.Components.Records
{
    public class Character : RecordComponent
    {
        public CLASRecord Class { get; protected set; }
        public FACTRecord Faction { get; protected set; }
        public RACERecord Race { get; protected set; }
        public SKILRecord Skills { get; protected set; }
        public BSGNRecord BirthSign { get; protected set; }
    }
}
