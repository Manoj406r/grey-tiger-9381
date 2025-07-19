using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardDisplay : MonoBehaviour
{
    public Image frontImage;

    public CardData cardData;
    
    public void SetUp(CardData data)
    {
        cardData = data;
        if(frontImage != null && data != null)
        {
            frontImage.sprite = data.CardSprite;
        }
    }
}
