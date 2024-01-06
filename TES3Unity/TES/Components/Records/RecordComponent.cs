using TES3Unity.ESM;
using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity.Components.Records
{
    public interface IUsableComponent
    {
        void Use();
    }

    public interface IPickableComponent
    {
        void Pick();
    }

    public class RecordComponent : MonoBehaviour
    {
        public class ObjectData
        {
            public Texture2D icon;
            public string interactionPrefix;
            public string name;
            public string weight;
            public string value;
        }

        protected Transform m_transform = null;

        public RefObjDataGroup refObjDataGroup = null;
        public Record record;
        public ObjectData objData = new ObjectData();
        public bool usable = false;
        public bool pickable = true;

        protected virtual void Awake()
        {
            m_transform = GetComponent<Transform>();
        }

        public virtual void Interact()
        {
        }

        protected void TryAddScript(string scriptName)
        {
            if (scriptName != null)
            {
                var scriptRecord = TES3Engine.Inst.DataReader.FindScript(scriptName);

                if (scriptRecord != null)
                {
                    var script = gameObject.AddComponent<TESScript>();
                    script.record = scriptRecord;
                }
            }
        }

        public static RecordComponent Create(GameObject gameObject, Record record, string tag)
        {
            gameObject.tag = tag;

            var transform = gameObject.GetComponent<Transform>();

            for (int i = 0, l = transform.childCount; i < l; i++)
            {
                transform.GetChild(i).tag = tag;
            }

            RecordComponent component = null;

            if (record is DOORRecord)
            {
                component = gameObject.AddComponent<Door>();
            }
            else if (record is LIGHRecord)
            {
                component = gameObject.AddComponent<TESLight>();
            }
            else if (record is BOOKRecord)
            {
                component = gameObject.AddComponent<Book>();
            }
            else if (record is CONTRecord)
            {
                component = gameObject.AddComponent<Container>();
            }
            else if (record is MISCRecord)
            {
                component = gameObject.AddComponent<MiscObject>();
            }
            else if (record is WEAPRecord)
            {
                component = gameObject.AddComponent<Weapon>();
            }
            else if (record is ARMORecord)
            {
                component = gameObject.AddComponent<Armor>();
            }
            else if (record is INGRRecord)
            {
                component = gameObject.AddComponent<Ingredient>();
            }
            else if (record is ACTIRecord)
            {
                component = gameObject.AddComponent<Activator>();
            }
            else if (record is LOCKRecord)
            {
                component = gameObject.AddComponent<Lock>();
            }
            else if (record is PROBRecord)
            {
                component = gameObject.AddComponent<ProbeItem>();
            }
            else if (record is REPARecord)
            {
                component = gameObject.AddComponent<Repaire>();
            }
            else if (record is CLOTRecord)
            {
                component = gameObject.AddComponent<TESCloth>();
            }
            else if (record is ALCHRecord)
            {
                component = gameObject.AddComponent<Alchemy>();
            }
            else if (record is APPARecord)
            {
                component = gameObject.AddComponent<AlchemyApparatus>();
            }
            else if (record is CREARecord)
            {
                component = gameObject.AddComponent<Creature>();
            }
            else if (record is NPC_Record)
            {
                component = gameObject.AddComponent<NPC>();
            }
            else
            {
                component = gameObject.AddComponent<RecordComponent>();
            }

            component.record = record;

            return component;
        }
    }
}