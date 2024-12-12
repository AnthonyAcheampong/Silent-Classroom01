/*
using UnityEngine;

public class AnswerTrigger : MonoBehaviour
{
    public string CorrectAnswer { get; private set; }
    private VRPictureManager quizManager;

    /// <summary>
    /// Initializes the AnswerTrigger with the correct answer and the game manager reference.
    /// </summary>
    /// <param name="correctAnswer">The correct answer string.</param>
    /// <param name="manager">The VRPictureManager managing the game.</param>
    public void Initialize(string correctAnswer, VRPictureManager manager)
    {
        CorrectAnswer = correctAnswer;
        quizManager = manager;
    }

    /// <summary>
    /// Called when this object is interacted with.
    /// </summary>
    private void OnMouseDown()
    {
        if (quizManager != null)
        {
            quizManager.OnAnswerSelected(gameObject);
        }
        else
        {
            Debug.LogError("[ERROR] VRPictureManager reference is missing!");
        }
    }
}


*/