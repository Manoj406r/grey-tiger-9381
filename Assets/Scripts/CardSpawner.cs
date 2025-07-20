using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSpawner : MonoBehaviour
{
    [Header("Card Setup")]
    public GameObject cardPrefab;
    public Transform gridParent;
    public List<CardData> cardDataList;

    [Header("Grid Settings")]
    public int columns = 2;
    public int rows = 2;

    private List<CardDisplay> spawnedCards = new List<CardDisplay>();

    private void Start()
    {
        SpawnCards();
    }

    private void SpawnCards()
    {
        int totalCards = columns * rows;

        if (totalCards % 2 != 0) totalCards++;

        ConfigureGridLayout();

        List<CardData> pairedCards = GetShuffledCardPairs(totalCards);

        ClearExistingCards();

        foreach (CardData data in pairedCards)
        {
            GameObject cardObj = Instantiate(cardPrefab, gridParent);
            CardDisplay display = cardObj.GetComponent<CardDisplay>();
            if (display != null)
            {
                display.SetupCard(data);
                spawnedCards.Add(display);
            }
        }

        SetButtonsActive(false);

        StartCoroutine(InitialPreview());
    }

    private void ConfigureGridLayout()
    {
        GridLayoutGroup grid = gridParent.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
        }
    }

    private List<CardData> GetShuffledCardPairs(int totalCards)
    {
        List<CardData> pairedCards = new List<CardData>();

        for (int i = 0; i < totalCards / 2; i++)
        {
            CardData data = cardDataList[i % cardDataList.Count];
            pairedCards.Add(data);
            pairedCards.Add(data);
        }

        for (int i = 0; i < pairedCards.Count; i++)
        {
            CardData temp = pairedCards[i];
            int rand = Random.Range(i, pairedCards.Count);
            pairedCards[i] = pairedCards[rand];
            pairedCards[rand] = temp;
        }

        return pairedCards;
    }

    private void ClearExistingCards()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
        spawnedCards.Clear();
    }

    private IEnumerator InitialPreview()
    {
        
        foreach (CardDisplay card in spawnedCards)
        {
            card.FlipFront();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2f); // Show front for 2 seconds

        
        foreach (CardDisplay card in spawnedCards)
        {
            card.FlipBack();
            yield return new WaitForSeconds(0.1f);
        }

        SetButtonsActive(true);

        foreach (CardDisplay card in spawnedCards)
        {
            card.EnableFlip();
        }
    }

    private void SetButtonsActive(bool state)
    {
        foreach (CardDisplay card in spawnedCards)
        {
            Button btn = card.GetComponent<Button>();
            if (btn == null)
                btn = card.GetComponentInChildren<Button>();

            if (btn != null)
                btn.enabled = state;
        }
    }
}