using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CardDisplay : MonoBehaviour, IPointerClickHandler
{
    public Image frontImage;
    public Image backImage;

    public CardData CardData { get; private set; }
    public bool IsFlipped { get; private set; }
    public bool IsMatched { get; private set; }

    private MatchingManager matchingManager;
    private bool canFlip = false;
    private Coroutine flipCoroutine;

    // Setup card with data and initialize state
    public void SetupCard(CardData data)
    {
        CardData = data;
        frontImage.sprite = CardData.CardSprite;

        frontImage.gameObject.SetActive(true);
        backImage.gameObject.SetActive(false);

        IsFlipped = true;
        IsMatched = false;
        canFlip = false;

        if (matchingManager == null)
            matchingManager = FindObjectOfType<MatchingManager>();

        if (matchingManager == null)
            Debug.LogError("MatchingManager not found in scene!");
    }

    // Enable card flipping interaction
    public void EnableFlip()
    {
        canFlip = true;
    }

    // Immediately show front of card (no animation)
    public void ShowFront()
    {
        StopFlipCoroutineIfRunning();

        frontImage.gameObject.SetActive(true);
        backImage.gameObject.SetActive(false);
        IsFlipped = true;

        transform.localScale = Vector3.one;
    }

    // Immediately show back of card (no animation)
    public void HideFront()
    {
        StopFlipCoroutineIfRunning();

        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
        IsFlipped = false;

        transform.localScale = Vector3.one;
    }

    // Start animated flip to front
    public void FlipFront()
    {
        StopFlipCoroutineIfRunning();
        flipCoroutine = StartCoroutine(FlipCard(true));
    }

    // Start animated flip to back (only if not matched)
    public void FlipBack()
    {
        if (IsMatched)
            return;

        StopFlipCoroutineIfRunning();
        flipCoroutine = StartCoroutine(FlipCard(false));
    }

    // Coroutine for smooth flip animation
    private IEnumerator FlipCard(bool showFront)
    {
        float duration = 0.3f;
        float halfTime = duration / 2f;
        float time = 0f;

        
        while (time < halfTime)
        {
            float scaleX = Mathf.Lerp(1f, 0f, time / halfTime);
            transform.localScale = new Vector3(scaleX, 1f, 1f);
            time += Time.deltaTime;
            yield return null;
        }

        // Switch visible side
        if (showFront)
        {
            frontImage.gameObject.SetActive(true);
            backImage.gameObject.SetActive(false);
            IsFlipped = true;
        }
        else
        {
            frontImage.gameObject.SetActive(false);
            backImage.gameObject.SetActive(true);
            IsFlipped = false;
        }

        time = 0f;

        
        while (time < halfTime)
        {
            float scaleX = Mathf.Lerp(0f, 1f, time / halfTime);
            transform.localScale = new Vector3(scaleX, 1f, 1f);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    // Mark card as matched and disable interactions and visuals
    public void SetMatched()
    {
        IsMatched = true;

        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(false);

        Image mainImage = GetComponent<Image>();
        if (mainImage != null)
            mainImage.enabled = false;

        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.enabled = false;

        canFlip = false;

        StartCoroutine(MatchEffect());
    }

    // Simple scale pulsate effect for matched card
    private IEnumerator MatchEffect()
    {
        float t = 0f;
        Vector3 original = transform.localScale;

        while (t < 0.2f)
        {
            transform.localScale = original + Vector3.one * Mathf.Sin(t * 20f) * 0.1f;
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    // Handle user click input on card
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canFlip || IsFlipped || IsMatched)
            return;

        matchingManager?.CardClicked(this);
    }

    // Helper method to stop ongoing flip coroutine if any
    private void StopFlipCoroutineIfRunning()
    {
        if (flipCoroutine != null)
        {
            StopCoroutine(flipCoroutine);
            flipCoroutine = null;
        }
    }
}
