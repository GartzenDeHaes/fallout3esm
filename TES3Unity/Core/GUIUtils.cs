using UnityEngine;
using UnityEngine.UI;

public static class GUIUtils
{
    private static GameObject mainCanvas;

    public static Sprite CreateSprite(Texture2D texture)
    {
        if (texture == null)
        {
            return null;
        }

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector3.zero);
    }

    public static GameObject MainCanvas
    {
        get
        {
            if (mainCanvas == null)
            {
                var canvas = MonoBehaviour.FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    mainCanvas = canvas.gameObject;
                }
            }
            return mainCanvas;
        }
    }

    public static void SetCanvasToWorldSpace(Canvas canvas, Transform parent, float depth, float scale, float y = 0.0f)
    {
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        var canvasTransform = canvas.GetComponent<RectTransform>();
        canvasTransform.SetParent(parent);

        var canvasScaler = canvas.GetComponent<CanvasScaler>();
        if (canvasScaler != null)
        {
            canvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvasScaler.referenceResolution.x);
            canvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvasScaler.referenceResolution.y);
        }

        canvasTransform.localPosition = new Vector3(0.0f, y, depth);
        canvasTransform.localRotation = Quaternion.identity;
        canvasTransform.localScale = new Vector3(scale, scale, scale);
    }
}