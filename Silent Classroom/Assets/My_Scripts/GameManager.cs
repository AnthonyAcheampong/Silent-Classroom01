using UnityEngine;
using TMPro; // Added for TextMeshPro

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] alphabetPrefabs; // Array of 3D alphabet prefabs (A-Z)

    [SerializeField]
    private Transform spawnPoint; // Placeholder transform for position, rotation, and scale

    [SerializeField]
    private TextMeshProUGUI scoreText; // TextMeshProUGUI for displaying the score

    private GameObject currentAlphabet; // The currently spawned 3D alphabet
    private string currentAlphabetName; // Name of the current alphabet (A, B, etc.)
    private int score = 0;

    void Start()
    {
        // Spawn the first random alphabet
        SpawnRandomAlphabet();
        UpdateScoreText();
    }

    public void OnGesturePerformed(string recognizedAlphabet)
    {
        if (recognizedAlphabet == currentAlphabetName)
        {
            // Correct gesture
            score += 1;
            Debug.Log($"Correct! {recognizedAlphabet} matches {currentAlphabetName}. Score: {score}");
            DestroyCurrentAlphabet();
            SpawnRandomAlphabet();
        }
        else
        {
            // Incorrect gesture
            score -= 1;
            Debug.Log($"Incorrect! {recognizedAlphabet} does not match {currentAlphabetName}. Score: {score}");
        }

        UpdateScoreText();
    }

    private void SpawnRandomAlphabet()
    {
        int randomIndex = Random.Range(0, alphabetPrefabs.Length);
        currentAlphabetName = alphabetPrefabs[randomIndex].name; // Use prefab name directly
        Debug.Log($"Spawned Alphabet: {currentAlphabetName}");

        // Instantiate the alphabet at the spawn point's position, rotation, and scale
        currentAlphabet = Instantiate(
            alphabetPrefabs[randomIndex],
            spawnPoint.position,
            spawnPoint.rotation
        );

        // Match the scale of the spawn point
        currentAlphabet.transform.localScale = spawnPoint.localScale;
    }

    private void DestroyCurrentAlphabet()
    {
        if (currentAlphabet != null)
        {
            Destroy(currentAlphabet);
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"Score: {score}";
    }
}
