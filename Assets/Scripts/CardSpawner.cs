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
    public int Columns = 2;
    public int Rows = 2;

    private void Start()
    {
        SpawnCards();
    }

    private void SpawnCards()
    {
        int totalCards = Columns * Rows;

        // Ensure even number of cards for pairing
        if (totalCards % 2 != 0)
        {
            totalCards += 1;
        }

        // Configure grid layout
        ConfigureGridLayout();

        // Prepare paired and shuffled card data
        List<CardData> pairedCards = GetShuffledCardPairs(totalCards);

        // Clear existing cards 
        ClearExistingCards();

        // Spawn new cards
        foreach (CardData data in pairedCards)
        {
            SpawnCard(data);
        }
    }

    private void ConfigureGridLayout()
    {
        GridLayoutGroup grid = gridParent.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = Columns;
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

        // Shuffle the paired cards
        for (int i = 0; i < pairedCards.Count; i++)
        {
            CardData temp = pairedCards[i];
            int randIndex = Random.Range(i, pairedCards.Count);
            pairedCards[i] = pairedCards[randIndex];
            pairedCards[randIndex] = temp;
        }

        return pairedCards;
    }

    private void ClearExistingCards()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void SpawnCard(CardData data)
    {
        GameObject cardObj = Instantiate(cardPrefab, gridParent);
        CardDisplay display = cardObj.GetComponent<CardDisplay>();
        if (display != null)
        {
            display.SetUp(data);
        }
    }
}
