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

    public void EnableFlip()
    {
        canFlip = true;
    }

    public void ShowFront()
    {
        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);
        frontImage.gameObject.SetActive(true);
        backImage.gameObject.SetActive(false);
        IsFlipped = true;
        transform.localScale = Vector3.one;
    }

    public void HideFront()
    {
        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);
        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
        IsFlipped = false;
        transform.localScale = Vector3.one;
    }

    public void FlipFront()
    {
        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);
        flipCoroutine = StartCoroutine(FlipCard(true));
    }

    public void FlipBack()
    {
        if (IsMatched) return;

        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);
        flipCoroutine = StartCoroutine(FlipCard(false));
    }

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canFlip || IsFlipped || IsMatched) return;

        if (matchingManager != null)
            matchingManager.CardClicked(this);
    }
}
