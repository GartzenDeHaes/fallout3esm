using UnityEngine;

namespace TES3Unity
{
    public sealed class PlayerSkin : MonoBehaviour
    {
        private void Start()
        {
            var player = GameSettings.GetPlayerRecord();

            /*var obj = NPCFactory.InstanciateNPC(nifManager, player, false, false);
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.SetActive(false); // For now*/
        }

        public static (Transform, Transform) AddHands(Transform leftHand, Transform rightHand)
        {
            // Loading hands.
            var nifManager = NIFManager.Instance;

            if (nifManager == null)
            {
                Debug.LogError("The NIFManager is not ready.");
                return (null, null);
            }

            var player = GameSettings.GetPlayerRecord();

            var race = player.Race.ToLower().Replace("_", " ");
            var gender = player.IsFemale ? "f" : "m";
            var hands1st = $"b_n_{race}_{gender}_hands.1st";

            if (race == "breton" && gender == "m")
            {
                // The breton male hands have not a "s" in the name...
                hands1st = hands1st.Replace("hands", "hand");
            }

            var hands = nifManager.InstantiateNIF($"meshes\\b\\{hands1st}.NIF", false);

            var meshColliders = hands.GetComponentsInChildren<MeshCollider>(true);
            foreach (var collider in meshColliders)
            {
                Destroy(collider);
            }

            var leftHandObject = CreateHand(hands, leftHand, rightHand, true);
            var rightHandObject = CreateHand(hands, leftHand, rightHand, false);

            Destroy(hands.gameObject);

            return (leftHandObject, rightHandObject);
        }

        private static Transform CreateHand(GameObject hands, Transform leftHand, Transform rightHand, bool left)
        {
            var hand = hands.transform.FindChildRecursiveExact($"{(left ? "Left" : "Right")} Hand");
            hand.gameObject.isStatic = false;
            hand.parent = left ? leftHand : rightHand;
            hand.localPosition = Vector3.zero;
            hand.localRotation = Quaternion.Euler(left ? -90.0f : 90.0f, 0.0f, left ? 90.0f : -90.0f);

            var anchor = new GameObject($"{(left ? "Left" : "Right")} Hand Socket");
            var anchorTransform = anchor.transform;
            anchorTransform.parent = hand;
            anchorTransform.localPosition = new Vector3(left ? 0.03f : -0.03f, 0, 0);
            anchorTransform.localRotation = Quaternion.Euler(0, left ? 180 : -180, 0);

            return anchorTransform;
        }
    }
}
