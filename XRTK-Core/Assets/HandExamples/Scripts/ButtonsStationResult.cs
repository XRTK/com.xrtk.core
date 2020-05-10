using TMPro;
using UnityEngine;

public class ButtonsStationResult : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro resultText = null;

    public void SetResult(string text)
    {
        resultText.text = $"You selected: {text}";
    }
}
