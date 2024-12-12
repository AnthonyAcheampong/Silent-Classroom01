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

    // Answer tiles
    public GameObject[] answerTiles; // Assign tiles corresponding to pokeVisualPlaceholders in the Inspector

    // Timer cubes
    public GameObject[] timerCubes; // Assign Cube 1 to Cube 10 in the Inspector

    // Treasure and Life placeholders
    public Transform treasurePlaceholder; // Assign a placeholder for treasure spawning
    public Transform[] lifePlaceholders; // Assign placeholders for each life sphere

    // Prefabs
    public GameObject treasurePrefab; // Treasure prefab to spawn
    public GameObject lifeSpherePrefab; // Life sphere prefab to spawn

    // Tile Colors (configurable in the Inspector)
    public Color correctColor = Color.green; // Color for correct answers
    public Color wrongColor = Color.red;    // Color for wrong answers
    public Color defaultColor = Color.white; // Default reset color for tiles

    // Timer settings
    public float initialTimeInterval = 1.5f;
    public float minTimeInterval = 0.5f;
    public float speedIncrement = 0.1f;
    private float timeInterval;

    // Treasure display time
    public float treasureDisplayTime = 3f; // Time treasure remains visible (set in Inspector)

    // UI Elements
    public TextMeshProUGUI scoreText; // Assign the score text UI in the Inspector

    // Score and life tracking
    private int score = 0;
    private int wrongAnswerCount = 0;
    private int livesRemaining = 3;

    private GameObject currentASLAlphabet;
    private GameObject correctEnglishAlphabet;
    private int correctIndex;

    private bool answerSelected = false; // Prevent multiple selections
    private Coroutine timerCoroutine;
    private bool isPaused = false; // Game pause state
    private bool isGameStopped = false; // Stop button state
    private List<GameObject> lifeSpheres = new List<GameObject>(); // Active life spheres

    void Start()
    {
        timeInterval = initialTimeInterval; // Set the initial timer interval
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
        if (isPaused || isGameStopped) return; // Do not spawn if the game is paused or stopped

        answerSelected = false; // Reset the selection lock for the new round

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        // Reset tile colors
        ResetTileColors();

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
        if (answerSelected || isPaused || isGameStopped) return; // Prevent multiple selections

        answerSelected = true; // Lock input after one selection

        // Stop the timer
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        // Find the index of the selected poke visual
        int selectedIndex = System.Array.IndexOf(pokeVisualPlaceholders, selectedPokeVisual.transform);

        // Validate index and associated tile
        if (selectedIndex < 0 || selectedIndex >= answerTiles.Length)
        {
            Debug.LogError("Invalid tile index for the selected poke visual.");
            return;
        }

        GameObject selectedTile = answerTiles[selectedIndex];

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
            wrongAnswerCount = 0;

            // Set the tile to the correct color
            SetTileColor(selectedTile, correctColor);

            if (IsMilestone(score))
            {
                StartCoroutine(DisplayTreasure());
            }
        }
        else
        {
            Debug.Log("Wrong!");
            score--;
            wrongAnswerCount++;

            // Set the tile to the wrong color
            SetTileColor(selectedTile, wrongColor);

            // Check if player loses a life
            if (wrongAnswerCount >= 3 || score < 0)
            {
                LoseLife();
                wrongAnswerCount = 0; // Reset wrong answer count after losing a life
            }
        }

        UpdateScoreText();
        SpawnAlphabets();
    }

    public void SetTileColor(GameObject tile, Color color)
    {
        if (tile.TryGetComponent<Renderer>(out Renderer renderer))
        {
            renderer.material.color = color;
        }
    }

    private void ResetTileColors()
    {
        foreach (var tile in answerTiles)
        {
            if (tile.TryGetComponent<Renderer>(out Renderer renderer))
            {
                renderer.material.color = defaultColor;
            }
        }
    }

    public void ToggleGamePause()
    {
        isGameStopped = !isGameStopped; // Toggle the stop state

        if (isGameStopped)
        {
            Debug.Log("Game Paused!");

            // Stop the timer coroutine
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }

            // Deactivate ASL and English alphabets
            if (currentASLAlphabet != null)
                currentASLAlphabet.SetActive(false);

            foreach (var placeholder in pokeVisualPlaceholders)
            {
                if (placeholder.childCount > 0)
                {
                    placeholder.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        else
        {
            Debug.Log("Game Resumed!");

            // Reactivate ASL and English alphabets and spawn new questions
            SpawnAlphabets();
        }
    }

    private IEnumerator DisplayTreasure()
    {
        isPaused = true; // Pause the game during treasure display

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

        // Wait for the specified treasure display time
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
            if (isPaused || isGameStopped) yield break;

            timerCubes[i].SetActive(true);
            yield return new WaitForSeconds(timeInterval);
        }

        Debug.Log("Time's up!");
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

        // Gradually increase timer speed
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
        scoreText.text = score.ToString(); // Only display the numeric score
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