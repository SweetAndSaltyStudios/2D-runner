using System.Collections;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public Sprite[] sprites;

    private SpriteRenderer spriteRenderer;
    private float spriteSwapSpeed = 0.05f;
    private int currentSpriteIndex;

    private bool animationRunning;

    public Sprite GetNextSprite
    {
        get
        {
            return sprites[currentSpriteIndex + 1 >= sprites.Length ? currentSpriteIndex = 0 : currentSpriteIndex += 1];       
        }
    }

    public Sprite GetRandomSprite
    {
        get
        {
            return sprites[Random.Range(0, sprites.Length - 1)];
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        spriteRenderer.sprite = GetRandomSprite;

        if (animationRunning == false)
            StartCoroutine(IAnimateCollectable());
    }

    private void OnDisable()
    {
        animationRunning = false;
    }

    private IEnumerator IAnimateCollectable()
    {
        animationRunning = true;

        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(spriteSwapSpeed);

            spriteRenderer.sprite = GetNextSprite;
        }
    }
}
