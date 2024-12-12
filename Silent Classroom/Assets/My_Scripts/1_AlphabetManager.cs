
/*

using UnityEngine;
using TMPro;
using System.Collections.Generic; // Required for List

public class AlphabetManager : MonoBehaviour
{
    // Prefabs for ASL and English alphabets
    public GameObject[] aslPrefabs;
    public GameObject[] englishPrefabs;

    // Placeholders for ASL and English alphabets
    public Transform aslPlaceholder;
    public Transform[] pokeVisualPlaceholders; // Assign Poke Visual 1, 2, 3 in the Inspector

    // UI elements
    public TextMeshProUGUI scoreText;

    // Score tracking
    private int score = 0;

    private GameObject currentASLAlphabet;
    private GameObject correctEnglishAlphabet;
    private int correctIndex;

    private bool answerSelected = false; // Prevent multiple triggers

    void Start()
    {
        UpdateScoreText();
        SpawnAlphabets();
    }

    public void SpawnAlphabets()
    {
        answerSelected = false;

        // Clean up previous objects
        if (currentASLAlphabet != null)
            Destroy(currentASLAlphabet);

        foreach (var placeholder in pokeVisualPlaceholders)
        {
            if (placeholder.childCount > 0)
                Destroy(placeholder.GetChild(0).gameObject); // Destroy any previous prefab child
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
    }

    public void CheckAnswer(GameObject selectedPokeVisual)
    {
        if (answerSelected)
        {
            Debug.Log("Answer already selected!");
            return;
        }

        answerSelected = true; // Lock input for this round

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
        }
        else
        {
            Debug.Log("Wrong!");
            score--;
        }

        UpdateScoreText();
        SpawnAlphabets();
    }

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString(); // Only display the numeric score
    }
}




*/

