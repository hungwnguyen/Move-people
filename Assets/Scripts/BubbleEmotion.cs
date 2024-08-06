using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class BubbleEmotion : MonoBehaviour
{
    public Sprite[] allEmoji;
    private Image sRenderer = default;

    private void OnEnable()
    {
        if (sRenderer == null)
        {
            sRenderer = GetComponent<Image>();
        }

        sRenderer.sprite = allEmoji[Random.Range(0, allEmoji.Length)];
    }
}