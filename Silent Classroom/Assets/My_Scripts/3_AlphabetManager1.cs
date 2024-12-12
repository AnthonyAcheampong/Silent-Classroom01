
/*

using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AlphabetManager : MonoBehaviour
{
    // Prefabs for ASL and English alphabets
    public GameObject[] aslPrefabs;
    public GameObject[] englishPrefabs;

    // Placeholders for ASL and English alphabets
    public Transform aslPlaceholder;
    public Transform[] pokeVisualPlaceholders;

    // Timer cubes
    public GameObject[] timerCubes; // Assign Cube 1 to Cube 10 in the Inspector

    // Treasure and Life placeholders
    public Transform treasurePlaceholder; // Assign a placeholder for treasure spawning
    public Transform[] lifePlaceholders; // Assign placeholders for each life sphere

    // Prefabs
    public GameObject treasurePrefab;
    public GameObject lifeSpherePrefab;

    // Timer settings
    public float initialTimeInterval = 1.5f;
    public float minTimeInterval = 0.5f;
    public float speedIncrement = 0.1f;
    private float timeInterval;

    // Treasure settings
    public float treasureDisplayTime = 3f; // Time treasure remains visible (set in Inspector)

    // UI elements
    public TextMeshProUGUI scoreText;

    // Score and life tracking
    private int score = 0;
    private int wrongAnswerCount = 0;
    private int livesRemaining = 3;

    private GameObject currentASLAlphabet;
    private GameObject correctEnglishAlphabet;
    private int correctIndex;

    private bool answerSelected = false;
    private Coroutine timerCoroutine;
    private bool isPaused = false; // Game pause state
    private List<GameObject> lifeSpheres = new List<GameObject>(); // Active life spheres

    void Start()
    {
        timeInterval = initialTimeInterval;
        SpawnLifeSpheres(); // Spawn life spheres at the start of the game
        UpdateScoreText();
        SpawnAlphabets();
    }

    private void SpawnLifeSpheres()
    {
        // Spawn life spheres at their designated placeholders
        foreach (var placeholder in lifePlaceholders)
        {
            GameObject lifeSphere = Instantiate(lifeSpherePrefab, placeholder.position, placeholder.rotation);
            lifeSpheres.Add(lifeSphere);
        }
    }

    public void SpawnAlphabets()
    {
        if (isPaused) return; // Do not spawn if the game is paused

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        answerSelected = false;

        // Clean up previous objects
        if (currentASLAlphabet != null)
            Destroy(currentASLAlphabet);

        foreach (var placeholder in pokeVisualPlaceholders)
        {
            if (placeholder.childCount > 0)
                Destroy(placeholder.GetChild(0).gameObject);
        }

        foreach (var cube in timerCubes)
        {
            cube.SetActive(false);
        }

        // Spawn the ASL alphabet
        int aslIndex = Random.Range(0, aslPrefabs.Length);
        currentASLAlphabet = Instantiate(
            aslPrefabs[aslIndex],
            aslPlaceholder.position,
            aslPlaceholder.rotation,
            aslPlaceholder
        );

        // Determine the correct English alphabet
        correctEnglishAlphabet = englishPrefabs[aslIndex];
        correctIndex = Random.Range(0, pokeVisualPlaceholders.Length);

        // Track used indices to prevent duplicate alphabets
        List<int> usedIndices = new List<int> { aslIndex };

        // Spawn English alphabets
        for (int i = 0; i < pokeVisualPlaceholders.Length; i++)
        {
            GameObject alphabetToSpawn;

            if (i == correctIndex)
            {
                alphabetToSpawn = correctEnglishAlphabet;
            }
            else
            {
                int randomIndex;
                do
                {
                    randomIndex = Random.Range(0, englishPrefabs.Length);
                } while (usedIndices.Contains(randomIndex));

                usedIndices.Add(randomIndex);
                alphabetToSpawn = englishPrefabs[randomIndex];
            }

            Instantiate(
                alphabetToSpawn,
                pokeVisualPlaceholders[i].position,
                pokeVisualPlaceholders[i].rotation,
                pokeVisualPlaceholders[i]
            );
        }

        // Start the timer
        timerCoroutine = StartCoroutine(StartTimer());
    }

    public void CheckAnswer(GameObject selectedPokeVisual)
    {
        if (answerSelected || isPaused) return;

        answerSelected = true;

        // Stop the timer
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        Transform prefabChild = selectedPokeVisual.transform.GetChild(0);
        if (prefabChild == null)
        {
            Debug.LogError("No child prefab found!");
            return;
        }

        string selectedName = prefabChild.name.Replace("(Clone)", "").Trim();
        string correctName = correctEnglishAlphabet.name.Replace("(Clone)", "").Trim();

        if (selectedName == correctName)
        {
            score++;
            wrongAnswerCount = 0;

            if (IsMilestone(score))
            {
                StartCoroutine(DisplayTreasure());
            }
        }
        else
        {
            score--;
            wrongAnswerCount++;

            if (wrongAnswerCount >= 3)
            {
                LoseLife();
                wrongAnswerCount = 0; // Reset wrong answer count after losing a life
            }
            else if (score < 0)
            {
                LoseLife();
                score = 0; // Reset score to prevent multiple deductions
            }
        }

        UpdateScoreText();
        SpawnAlphabets();
    }

    private IEnumerator DisplayTreasure()
    {
        isPaused = true;

        // Remove ASL and English alphabets
        if (currentASLAlphabet != null) Destroy(currentASLAlphabet);
        foreach (var placeholder in pokeVisualPlaceholders)
        {
            if (placeholder.childCount > 0)
                Destroy(placeholder.GetChild(0).gameObject);
        }

        // Spawn the treasure at the placeholder
        GameObject treasure = Instantiate(treasurePrefab, treasurePlaceholder.position, treasurePlaceholder.rotation);
        Debug.Log("Treasure spawned!");

        // Wait for the specified time
        yield return new WaitForSeconds(treasureDisplayTime);

        // Remove the treasure and resume the game
        Destroy(treasure);
        isPaused = false;
        SpawnAlphabets();
    }

    private IEnumerator StartTimer()
    {
        for (int i = 0; i < timerCubes.Length; i++)
        {
            if (isPaused) yield break; // Stop if the game is paused
            timerCubes[i].SetActive(true);
            yield return new WaitForSeconds(timeInterval);
        }

        score--;
        wrongAnswerCount++;

        if (wrongAnswerCount >= 3)
        {
            LoseLife();
            wrongAnswerCount = 0; // Reset wrong answer count after losing a life
        }
        else if (score < 0)
        {
            LoseLife();
            score = 0; // Reset score to prevent multiple deductions
        }

        UpdateScoreText();
        SpawnAlphabets();

        timeInterval = Mathf.Max(minTimeInterval, timeInterval - speedIncrement);
    }

    private void LoseLife()
    {
        if (livesRemaining > 0)
        {
            livesRemaining--;

            // Deactivate the last remaining life sphere
            lifeSpheres[livesRemaining].SetActive(false);
            Debug.Log($"Life {livesRemaining + 1} lost!");

            if (livesRemaining == 0)
            {
                Debug.Log("Game Over!");
            }
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    private bool IsMilestone(int score)
    {
        int[] milestones = { 10, 20, 40, 60, 80, 100 };
        foreach (int milestone in milestones)
        {
            if (score == milestone) return true;
        }
        return false;
    }
}


*/
