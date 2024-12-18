using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AlphabetManager : MonoBehaviour
{
    public GameObject[] aslPrefabs;
    public GameObject[] englishPrefabs;
    public Transform aslPlaceholder;
    public Transform[] pokeVisualPlaceholders;
    public GameObject[] timerCubes;
    public GameObject correctPrefab;
    public GameObject wrongPrefab;
    public Transform[] feedbackPlaceholders;
    public Transform treasurePlaceholder;
    public Transform[] lifePlaceholders;
    public GameObject[] treasurePrefabs; // Assign Treasure1 to Treasure5 in the Inspector
    public GameObject lifeSpherePrefab;
    public Transform pauseAnimationPlaceholder; // Placeholder for pause animation
    public GameObject pauseAnimationPrefab; // FBX animation prefab for pause
    public Transform gameOverPlaceholder; // Placeholder for game over animation
    public GameObject gameOverPrefab; // FBX animation prefab for game over

    public GameObject[] countdownPrefabs; // Prefabs for countdown (1, 2, 3)
    public Transform countdownPlaceholder; // Placeholder for countdown prefabs

    public float initialTimeInterval = 1.5f;
    public float minTimeInterval = 0.5f;
    public float speedIncrement = 0.1f;
    private float timeInterval;
    public float treasureDisplayTime = 3f;
    public TextMeshProUGUI scoreText;

    private int score = 0;
    private int wrongAnswerCount = 0;
    private int deductionsCount = 0; // Tracks total deductions for timer and wrong answers
    private int livesRemaining = 3;
    private HashSet<int> milestonesAchieved = new HashSet<int>(); // Track milestones achieved

    private GameObject currentASLAlphabet;
    private GameObject correctEnglishAlphabet;
    private int correctIndex;

    private bool answerSelected = false;
    private Coroutine timerCoroutine;
    private bool isPaused = false;
    private bool isGameStopped = false;
    private GameObject spawnedPausePrefab; // Track the spawned pause prefab
    private List<GameObject> lifeSpheres = new List<GameObject>();
    private List<GameObject> activeFeedbackPrefabs = new List<GameObject>();

    void Start()
    {
        timeInterval = initialTimeInterval;
        SpawnLifeSpheres();
        UpdateScoreText();
        StartCoroutine(StartCountdown());
    }

    private void SpawnLifeSpheres()
    {
        foreach (var placeholder in lifePlaceholders)
        {
            GameObject lifeSphere = Instantiate(lifeSpherePrefab, placeholder.position, placeholder.rotation);
            lifeSpheres.Add(lifeSphere);
        }
    }

    private IEnumerator StartCountdown()
    {
        foreach (GameObject countdownPrefab in countdownPrefabs)
        {
            GameObject countdownInstance = Instantiate(countdownPrefab, countdownPlaceholder.position, countdownPlaceholder.rotation, countdownPlaceholder);
            yield return new WaitForSeconds(2f); // Wait for 1 second between countdown numbers
            Destroy(countdownInstance);
        }

        SpawnAlphabets(); // Start the game after the countdown
    }

    public void SpawnAlphabets()
    {
        if (isPaused || isGameStopped) return;

        answerSelected = false;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        if (currentASLAlphabet != null)
            Destroy(currentASLAlphabet);

        foreach (var placeholder in pokeVisualPlaceholders)
        {
            if (placeholder.childCount > 0)
                Destroy(placeholder.GetChild(0).gameObject);
        }

        foreach (var feedback in activeFeedbackPrefabs)
        {
            Destroy(feedback);
        }
        activeFeedbackPrefabs.Clear();

        foreach (var cube in timerCubes)
        {
            cube.SetActive(false);
        }

        int aslIndex = Random.Range(0, aslPrefabs.Length);
        currentASLAlphabet = Instantiate(aslPrefabs[aslIndex], aslPlaceholder.position, aslPlaceholder.rotation, aslPlaceholder);

        correctEnglishAlphabet = englishPrefabs[aslIndex];
        correctIndex = Random.Range(0, pokeVisualPlaceholders.Length);

        List<int> usedIndices = new List<int> { aslIndex };

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

            Instantiate(alphabetToSpawn, pokeVisualPlaceholders[i].position, pokeVisualPlaceholders[i].rotation, pokeVisualPlaceholders[i]);
        }

        timerCoroutine = StartCoroutine(StartTimer());
    }

    public void CheckAnswer(GameObject selectedPokeVisual)
    {
        if (answerSelected || isPaused || isGameStopped) return;

        answerSelected = true;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        int selectedIndex = System.Array.IndexOf(pokeVisualPlaceholders, selectedPokeVisual.transform);

        if (selectedIndex < 0 || selectedIndex >= pokeVisualPlaceholders.Length)
        {
            Debug.LogError("Invalid placeholder index for the selected poke visual.");
            return;
        }

        Transform prefabChild = selectedPokeVisual.transform.GetChild(0);
        if (prefabChild == null)
        {
            Debug.LogError("No prefab child found under the selected Poke Visual!");
            return;
        }

        string selectedName = prefabChild.name.Replace("(Clone)", "").Trim();
        string correctName = correctEnglishAlphabet.name.Replace("(Clone)", "").Trim();

        Debug.Log($"Selected Alphabet: {selectedName}");
        Debug.Log($"Correct Alphabet: {correctName}");

        GameObject feedbackPrefab;
        Transform feedbackPlaceholder;

        if (selectedName == correctName)
        {
            Debug.Log("Correct!");
            score++;
            wrongAnswerCount = 0;
            deductionsCount = 0; // Reset deductions if a correct answer is selected

            feedbackPrefab = correctPrefab;
            feedbackPlaceholder = feedbackPlaceholders[selectedIndex];

            if (IsMilestone(score))
            {
                StartCoroutine(DisplayTreasure());
            }
        }
        else
        {
            Debug.Log("Wrong!");

            if (score > 0)
            {
                score--;
            }
            else
            {
                LoseLife();
            }

            deductionsCount++;

            feedbackPrefab = wrongPrefab;
            feedbackPlaceholder = feedbackPlaceholders[selectedIndex];

            if (deductionsCount >= 3)
            {
                LoseLife();
                deductionsCount = 0; // Reset deductions after losing a life
            }
        }

        GameObject spawnedFeedback = Instantiate(feedbackPrefab, feedbackPlaceholder.position, feedbackPlaceholder.rotation, feedbackPlaceholder);
        activeFeedbackPrefabs.Add(spawnedFeedback);

        UpdateScoreText();

        StartCoroutine(DelayNextQuestion());
    }

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
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

        if (score > 0)
        {
            score--;
        }
        else
        {
            LoseLife();
        }

        deductionsCount++;

        if (deductionsCount >= 3)
        {
            LoseLife();
            deductionsCount = 0; // Reset deductions after losing a life
        }

        UpdateScoreText();
        SpawnAlphabets();

        timeInterval = Mathf.Max(minTimeInterval, timeInterval - speedIncrement);
    }

    public void ToggleGamePause()
    {
        isGameStopped = !isGameStopped;

        if (isGameStopped)
        {
            Debug.Log("Game Paused!");

            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }

            if (currentASLAlphabet != null)
                currentASLAlphabet.SetActive(false);

            foreach (var placeholder in pokeVisualPlaceholders)
            {
                if (placeholder.childCount > 0)
                {
                    placeholder.GetChild(0).gameObject.SetActive(false);
                }
            }

            if (spawnedPausePrefab == null && pauseAnimationPrefab != null && pauseAnimationPlaceholder != null)
            {
                spawnedPausePrefab = Instantiate(pauseAnimationPrefab, pauseAnimationPlaceholder.position, pauseAnimationPlaceholder.rotation, pauseAnimationPlaceholder);
            }
        }
        else
        {
            Debug.Log("Game Resumed!");

            if (spawnedPausePrefab != null)
            {
                Destroy(spawnedPausePrefab);
                spawnedPausePrefab = null;
            }

            SpawnAlphabets();
        }
    }

    private void LoseLife()
    {
        if (livesRemaining > 0)
        {
            livesRemaining--;

            lifeSpheres[livesRemaining].SetActive(false);
            Debug.Log($"Life {livesRemaining + 1} lost!");

            if (livesRemaining == 0)
            {
                Debug.Log("Game Over!");
                isGameStopped = true;

                // Stop all activity
                if (timerCoroutine != null)
                {
                    StopCoroutine(timerCoroutine);
                }

                if (currentASLAlphabet != null)
                {
                    Destroy(currentASLAlphabet);
                }

                foreach (var placeholder in pokeVisualPlaceholders)
                {
                    if (placeholder.childCount > 0)
                    {
                        Destroy(placeholder.GetChild(0).gameObject);
                    }
                }

                foreach (var feedback in activeFeedbackPrefabs)
                {
                    Destroy(feedback);
                }
                activeFeedbackPrefabs.Clear();

                // Spawn the Game Over prefab
                if (gameOverPrefab != null && gameOverPlaceholder != null)
                {
                    Instantiate(gameOverPrefab, gameOverPlaceholder.position, gameOverPlaceholder.rotation, gameOverPlaceholder);
                }
            }
        }
    }

    private bool IsMilestone(int score)
    {
        if (milestonesAchieved.Contains(score)) return false;

        bool isMilestone = score % 10 == 0 && score / 10 <= treasurePrefabs.Length;

        if (isMilestone)
        {
            milestonesAchieved.Add(score);
        }

        return isMilestone;
    }



    private IEnumerator DisplayTreasure()
    {
        isPaused = true;

        if (currentASLAlphabet != null) Destroy(currentASLAlphabet);
        foreach (var placeholder in pokeVisualPlaceholders)
        {
            if (placeholder.childCount > 0)
                Destroy(placeholder.GetChild(0).gameObject);
        }

        int treasureIndex = Mathf.Min(score / 10 - 1, treasurePrefabs.Length - 1);
        GameObject treasure = Instantiate(treasurePrefabs[treasureIndex], treasurePlaceholder.position, treasurePlaceholder.rotation);
        Debug.Log("Treasure spawned!");

        yield return new WaitForSeconds(treasureDisplayTime);

        Destroy(treasure);
        isPaused = false;
        SpawnAlphabets();
    }





    private IEnumerator DelayNextQuestion()
    {
        yield return new WaitForSeconds(0.3f);

        foreach (var feedback in activeFeedbackPrefabs)
        {
            Destroy(feedback);
        }
        activeFeedbackPrefabs.Clear();

        SpawnAlphabets();
    }
}
