using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingManager : MonoBehaviour
{
    private List<CardDisplay> flippedCards = new List<CardDisplay>();

    public void CardClicked(CardDisplay clickedCard)
    {
        if (clickedCard.IsMatched || clickedCard.IsFlipped)
            return;

        clickedCard.FlipFront();
        flippedCards.Add(clickedCard);

        if (flippedCards.Count >= 2)
        {
            
            CardDisplay first = flippedCards[0];
            CardDisplay second = flippedCards[1];
            StartCoroutine(CheckMatchRoutine(first, second));
        }
    }

    private IEnumerator CheckMatchRoutine(CardDisplay firstCard, CardDisplay secondCard)
    {
        yield return new WaitForSeconds(1f);

        if (firstCard.CardData.CardSprite == secondCard.CardData.CardSprite)
        {
            firstCard.SetMatched();
            secondCard.SetMatched();
        }
        else
        {
            firstCard.FlipBack();
            secondCard.FlipBack();
        }

        flippedCards.Remove(firstCard);
        flippedCards.Remove(secondCard);
    }
}
