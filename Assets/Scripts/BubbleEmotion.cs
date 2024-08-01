using UnityEngine;
using Random = UnityEngine.Random;

public class BubbleEmotion : MonoBehaviour
{
    public Sprite[] allEmoji;
    private SpriteRenderer sRenderer = default;

    private void OnEnable()
    {
        if (sRenderer == null)
        {
            sRenderer = GetComponent<SpriteRenderer>();
        }

        sRenderer.sprite = allEmoji[Random.Range(0, allEmoji.Length)];
    }
}