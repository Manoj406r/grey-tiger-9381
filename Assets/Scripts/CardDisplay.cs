using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardDisplay : MonoBehaviour, IPointerClickHandler
{
    public Image frontImage;
    public Image backImage;

    public CardData CardData { get; private set; }
    public bool IsFlipped { get; private set; }
    public bool IsMatched { get; private set; }

    private MatchingManager matchingManager;
    private bool canFlip = false;

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
        frontImage.gameObject.SetActive(true);
        backImage.gameObject.SetActive(false);
        IsFlipped = true;
    }

    public void HideFront()
    {
        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
        IsFlipped = false;
    }


    public void FlipFront()
    {
        frontImage.gameObject.SetActive(true);
        backImage.gameObject.SetActive(false);
        IsFlipped = true;
    }

    public void FlipBack()
    {
        if (IsMatched) return;

        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
        IsFlipped = false;
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
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canFlip || IsFlipped || IsMatched) return;

        if (matchingManager != null)
            matchingManager.CardClicked(this);
    }
}