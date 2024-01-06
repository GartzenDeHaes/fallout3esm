using System.Collections;
using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity.Components.Records
{
    public class Door : RecordComponent
    {
        public class DoorData
        {
            public string doorName;
            public string doorExitName;
            public bool leadsToAnotherCell;
            public bool leadsToInteriorCell;
            public Vector3 doorExitPos;
            public Quaternion doorExitOrientation;

            public bool isOpen;
            public Quaternion closedRotation;
            public Quaternion openRotation;
            public bool moving = false;
        }

        private AudioClip m_OpenSound;
        private AudioClip m_CloseSound;

        public DoorData doorData = null;

        private void Start()
        {
            usable = true;
            pickable = false;

            doorData = new DoorData();
            doorData.closedRotation = transform.rotation;
            doorData.openRotation = doorData.closedRotation * Quaternion.Euler(Vector3.up * 90f);
            doorData.moving = false;

            var DOOR = record as DOORRecord;
            if (DOOR.Name != null)
            {
                doorData.doorName = DOOR.Name;
            }

            doorData.leadsToAnotherCell = (refObjDataGroup.DNAM != null) || (refObjDataGroup.DODT != null);
            doorData.leadsToInteriorCell = (refObjDataGroup.DNAM != null);

            if (doorData.leadsToInteriorCell)
            {
                doorData.doorExitName = refObjDataGroup.DNAM.value;
            }

            if (doorData.leadsToAnotherCell && !doorData.leadsToInteriorCell)
            {
                var doorExitCell = TES3Engine.Inst.DataReader.FindExteriorCellRecord(TES3Engine.Inst.CellManager.GetExteriorCellIndices(doorData.doorExitPos));

                if (doorExitCell != null)
                {
                    doorData.doorExitName = doorExitCell.RGNN?.value ?? "Unknown Region";
                }
                else
                {
                    doorData.doorExitName = doorData.doorName;
                }
            }

            if (refObjDataGroup.DODT != null)
            {
                doorData.doorExitPos = NIFUtils.NifPointToUnityPoint(refObjDataGroup.DODT.position);
                doorData.doorExitOrientation = NIFUtils.NifEulerAnglesToUnityQuaternion(refObjDataGroup.DODT.eulerAngles);
            }

            objData.name = doorData.leadsToAnotherCell ? doorData.doorExitName : "Use " + doorData.doorName;

            m_OpenSound = SoundManager.GetAudioClip(DOOR.OpenSound);
            m_CloseSound = SoundManager.GetAudioClip(DOOR.CloseSound);

            TryAddScript(DOOR.Script);
        }

        public override void Interact()
        {
            if (doorData != null)
            {
                if (doorData.isOpen)
                {
                    Close();
                }
                else
                {
                    Open();
                }
            }
        }

        private void Open()
        {
            if (!doorData.moving)
            {
                StartCoroutine(c_Open());
            }
        }

        private void Close()
        {
            if (!doorData.moving)
            {
                StartCoroutine(c_Close());
            }
        }

        private IEnumerator c_Open()
        {
            doorData.moving = true;

            if (m_OpenSound != null)
            {
                AudioSource.PlayClipAtPoint(m_OpenSound, transform.position);
            }

            while (Quaternion.Angle(transform.rotation, doorData.openRotation) > 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, doorData.openRotation, Time.deltaTime * 5f);
                yield return new WaitForEndOfFrame();
            }

            doorData.isOpen = true;
            doorData.moving = false;
        }

        private IEnumerator c_Close()
        {
            doorData.moving = true;

            if (m_CloseSound != null)
            {
                AudioSource.PlayClipAtPoint(m_CloseSound, transform.position);
            }

            while (Quaternion.Angle(transform.rotation, doorData.closedRotation) > 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, doorData.closedRotation, Time.deltaTime * 5f);
                yield return new WaitForEndOfFrame();
            }

            doorData.isOpen = false;
            doorData.moving = false;
        }
    }
}