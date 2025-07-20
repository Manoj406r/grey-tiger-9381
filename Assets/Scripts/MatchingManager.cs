using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchingManager : MonoBehaviour
{
    

    private List<CardDisplay> flippedCards = new List<CardDisplay>();

    [Header("Audio Clips")]
    public AudioClip flipSound;
    public AudioClip matchSound;
    public AudioClip mismatchSound;
    public AudioClip gameOverSound;

    private AudioSource audioSource;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    private int score = 0;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Combo UI")]
    public TextMeshProUGUI comboText;
    private int comboCount = 0;
    private int maxCombo = 0;

    

    

    private void Start()
    {
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        UpdateScoreUI();
        UpdateComboUI();
    }

    
    public void CardClicked(CardDisplay clickedCard)
    {
        if (clickedCard.IsMatched || clickedCard.IsFlipped)
            return;

        PlaySound(flipSound);

        clickedCard.FlipFront();
        flippedCards.Add(clickedCard);

        if (flippedCards.Count >= 2)
        {
            StartCoroutine(CheckMatchRoutine(flippedCards[0], flippedCards[1]));
        }
    }

   

    

    private IEnumerator CheckMatchRoutine(CardDisplay firstCard, CardDisplay secondCard)
    {
        // Small delay to allow the player to see the flipped cards
        yield return new WaitForSeconds(0.3f);

        if (IsMatch(firstCard, secondCard))
        {
            HandleMatch(firstCard, secondCard);
        }
        else
        {
            HandleMismatch(firstCard, secondCard);
        }

        flippedCards.Remove(firstCard);
        flippedCards.Remove(secondCard);

        if (IsGameOver())
        {
            HandleGameOver();
        }
    }

    private bool IsMatch(CardDisplay firstCard, CardDisplay secondCard)
    {
        return firstCard.CardData.CardSprite == secondCard.CardData.CardSprite;
    }

    private void HandleMatch(CardDisplay firstCard, CardDisplay secondCard)
    {
        PlaySound(matchSound);

        firstCard.SetMatched();
        secondCard.SetMatched();

        score++;
        comboCount++;

        if (comboCount > maxCombo)
            maxCombo = comboCount;

        UpdateScoreUI();
        UpdateComboUI();
    }

    private void HandleMismatch(CardDisplay firstCard, CardDisplay secondCard)
    {
        PlaySound(mismatchSound);

        firstCard.FlipBack();
        secondCard.FlipBack();

        comboCount = 0;
        UpdateComboUI();
    }

    private bool IsGameOver()
    {
        CardDisplay[] allCards = FindObjectsOfType<CardDisplay>();
        foreach (var card in allCards)
        {
            if (!card.IsMatched)
                return false;
        }
        return true;
    }

    private void HandleGameOver()
    {
        PlaySound(gameOverSound);
        Debug.Log($"Game Over! Final Score: {score}");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = $"Matches Completed - {score}";
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score : {score}";
    }

    private void UpdateComboUI()
    {
        if (comboText == null)
            return;

        if (comboCount > 1)
            comboText.text = $"Combo x - {comboCount}";
        else
            comboText.text = string.Empty;  
    }

    
}
