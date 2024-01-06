using UnityEngine;

public static class TransformExtensions
{
    public static Transform FindChildRecursiveExact(this Transform parent, string name)
    {
        if (parent.childCount > 0)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    return child;
                }

                var result = child.FindChildRecursiveExact(name);
                if (result != null)
                {
                    return result;
                }
            }
        }

        return null;
    }
}
