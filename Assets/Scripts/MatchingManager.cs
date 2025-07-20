using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

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
            CardDisplay first = flippedCards[0];
            CardDisplay second = flippedCards[1];
            StartCoroutine(CheckMatchRoutine(first, second));
        }
    }

    private IEnumerator CheckMatchRoutine(CardDisplay firstCard, CardDisplay secondCard)
    {
        yield return new WaitForSeconds(0.3f);

        if (firstCard.CardData.CardSprite == secondCard.CardData.CardSprite)
        {
            PlaySound(matchSound);
            firstCard.SetMatched();
            secondCard.SetMatched();

            score++;
            comboCount++; // increase combo count

            if (comboCount > maxCombo)
                maxCombo = comboCount;

            UpdateScoreUI();
            UpdateComboUI();
        }
        else
        {
            PlaySound(mismatchSound);
            firstCard.FlipBack();
            secondCard.FlipBack();

            comboCount = 0; // reset combo on mismatch
            UpdateComboUI();
        }

        flippedCards.Remove(firstCard);
        flippedCards.Remove(secondCard);

        if (IsGameOver())
        {
            PlaySound(gameOverSound);
            Debug.Log("Game Over! Final Score: " + score);

            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = "Matches Completed - " + score.ToString();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
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

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score : " + score.ToString();
        }
    }

    private void UpdateComboUI()
    {
        if (comboText != null)
        {
            if (comboCount > 1)
                comboText.text = "Combo x - " + comboCount.ToString();

        }
    }
}