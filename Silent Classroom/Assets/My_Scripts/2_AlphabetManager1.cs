
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

    // Player lives
    public GameObject[] lifeSpheres; // Assign 3 spheres in the Inspector

    // Timer settings
    public float initialTimeInterval = 1.5f; // Slowest interval (start speed)
    public float minTimeInterval = 0.5f; // Fastest interval (max speed)
    public float speedIncrement = 0.1f; // Speed increase per round
    private float timeInterval; // Current timer interval

    // UI elements
    public TextMeshProUGUI scoreText;

    // Score and life tracking
    private int score = 0;
    private int wrongAnswerCount = 0; // Tracks consecutive wrong answers
    private int livesRemaining = 3; // Total player lives

    private GameObject currentASLAlphabet;
    private GameObject correctEnglishAlphabet;
    private int correctIndex;

    private bool answerSelected = false; // Prevent multiple triggers
    private Coroutine timerCoroutine;

    void Start()
    {
        timeInterval = initialTimeInterval; // Start with the slowest interval
        UpdateScoreText();
        UpdateLivesDisplay();
        SpawnAlphabets();
    }

    public void SpawnAlphabets()
    {
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
                Destroy(placeholder.GetChild(0).gameObject); // Destroy any previous prefab child
        }

        foreach (var cube in timerCubes)
        {
            cube.SetActive(false); // Reset timer cubes
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
        List<int> usedIndices = new List<int> { aslIndex }; // Start with the correct index

        // Spawn English alphabets into Poke Visual placeholders
        for (int i = 0; i < pokeVisualPlaceholders.Length; i++)
        {
            GameObject alphabetToSpawn;

            if (i == correctIndex)
            {
                // Place the correct alphabet in the correct placeholder
                alphabetToSpawn = correctEnglishAlphabet;
            }
            else
            {
                // Get a random wrong answer that hasn't been used yet
                int randomIndex;
                do
                {
                    randomIndex = Random.Range(0, englishPrefabs.Length);
                } while (usedIndices.Contains(randomIndex)); // Ensure it's unique

                usedIndices.Add(randomIndex); // Mark this index as used
                alphabetToSpawn = englishPrefabs[randomIndex];
            }

            // Instantiate the alphabet prefab
            Instantiate(
                alphabetToSpawn,
                pokeVisualPlaceholders[i].position,
                pokeVisualPlaceholders[i].rotation,
                pokeVisualPlaceholders[i]
            );
        }

        // Start the timer for the round
        timerCoroutine = StartCoroutine(StartTimer());
    }

    public void CheckAnswer(GameObject selectedPokeVisual)
    {
        if (answerSelected)
        {
            Debug.Log("Answer already selected!");
            return;
        }

        answerSelected = true; // Lock input for this round

        // Stop the timer
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        // Find the child prefab under the selected Poke Visual
        Transform prefabChild = selectedPokeVisual.transform.GetChild(0); // Get the first child (dynamically spawned prefab)
        if (prefabChild == null)
        {
            Debug.LogError("No prefab child found under the selected Poke Visual!");
            return;
        }

        // Clean up prefab names for comparison
        string selectedName = prefabChild.name.Replace("(Clone)", "").Trim();
        string correctName = correctEnglishAlphabet.name.Replace("(Clone)", "").Trim();

        Debug.Log($"Selected Alphabet: {selectedName}");
        Debug.Log($"Correct Alphabet: {correctName}");

        if (selectedName == correctName)
        {
            Debug.Log("Correct!");
            score++;
            wrongAnswerCount = 0; // Reset wrong answer count
        }
        else
        {
            Debug.Log("Wrong!");
            score--;
            wrongAnswerCount++;

            // Check if player loses a life
            if (wrongAnswerCount >= 3 || score < 0)
            {
                LoseLife();
            }
        }

        UpdateScoreText();
        SpawnAlphabets();
    }

    private IEnumerator StartTimer()
    {
        for (int i = 0; i < timerCubes.Length; i++)
        {
            timerCubes[i].SetActive(true); // Activate the current cube
            yield return new WaitForSeconds(timeInterval); // Wait for the interval
        }

        // Time ran out, deduct a point and spawn a new question
        Debug.Log("Time's up!");
        score--;
        wrongAnswerCount++;

        // Check if player loses a life
        if (wrongAnswerCount >= 3 || score < 0)
        {
            LoseLife();
        }

        UpdateScoreText();
        SpawnAlphabets();

        // Gradually speed up the timer
        timeInterval = Mathf.Max(minTimeInterval, timeInterval - speedIncrement);
    }

    private void LoseLife()
    {
        if (livesRemaining > 0)
        {
            livesRemaining--;
            UpdateLivesDisplay();

            if (livesRemaining == 0)
            {
                Debug.Log("Game Over!");
                // You can add additional Game Over logic here
            }
        }
    }

    private void UpdateLivesDisplay()
    {
        for (int i = 0; i < lifeSpheres.Length; i++)
        {
            lifeSpheres[i].SetActive(i < livesRemaining); // Activate spheres based on remaining lives
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString(); // Only display the numeric score
    }
}



*/