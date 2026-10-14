using TMPro;
using UnityEngine;

public static class InteractPrompt
{
    public static GameObject CreateOrUpdate(Transform parent, GameObject existing, string message, Vector3 localOffset)
    {
        GameObject prompt = existing;

        if (prompt == null)
        {
            prompt = new GameObject("InteractPrompt");
            prompt.transform.SetParent(parent, false);
            prompt.transform.localPosition = localOffset;
        }
        else
        {
            prompt.transform.localPosition = localOffset;
        }

        TextMeshPro tmp = prompt.GetComponent<TextMeshPro>();
        if (tmp == null)
            tmp = prompt.GetComponentInChildren<TextMeshPro>(true);
        if (tmp == null)
            tmp = prompt.AddComponent<TextMeshPro>();

        ConfigureTmp(tmp);

        if (prompt.GetComponent<WorldPromptBillboard>() == null)
            prompt.AddComponent<WorldPromptBillboard>();

        SetText(prompt, message);
        return prompt;
    }

    private static void ConfigureTmp(TextMeshPro tmp)
    {
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 6f;
        tmp.enableAutoSizing = false;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        tmp.outlineWidth = 0.25f;
        tmp.outlineColor = Color.black;
        tmp.sortingOrder = 200;
        tmp.rectTransform.sizeDelta = new Vector2(8f, 3f);
        tmp.rectTransform.localScale = Vector3.one;
    }

    public static void SetText(GameObject prompt, string message)
    {
        if (prompt == null)
            return;

        TextMeshPro tmp = prompt.GetComponent<TextMeshPro>();
        if (tmp == null)
            tmp = prompt.GetComponentInChildren<TextMeshPro>(true);
        if (tmp != null)
        {
            tmp.text = message;
            return;
        }

        TextMeshProUGUI tmpUi = prompt.GetComponent<TextMeshProUGUI>();
        if (tmpUi == null)
            tmpUi = prompt.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmpUi != null)
        {
            tmpUi.text = message;
            return;
        }

        TextMesh legacy = prompt.GetComponent<TextMesh>();
        if (legacy == null)
            legacy = prompt.GetComponentInChildren<TextMesh>(true);
        if (legacy != null)
            legacy.text = message;
    }
}
