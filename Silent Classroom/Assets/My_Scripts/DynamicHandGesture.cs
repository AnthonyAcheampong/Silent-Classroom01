using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Hands;

namespace UnityEngine.XR.Hands.Samples.GestureSample
{
    /// <summary>
    /// A gesture that detects when a hand is held in a static shape and orientation for a minimum amount of time.
    /// It also tracks dynamic gestures like J and Z.
    /// </summary>
    public class StaticHandGesture : MonoBehaviour
    {
        [SerializeField]
        XRHandTrackingEvents m_HandTrackingEvents;

        [SerializeField]
        ScriptableObject m_HandShapeOrPose;

        [SerializeField]
        Transform m_TargetTransform;

        [SerializeField]
        Image m_Background;

        [SerializeField]
        UnityEvent m_GesturePerformed;

        [SerializeField]
        UnityEvent m_GestureEnded;

        [SerializeField]
        float m_MinimumHoldTime = 0.2f;

        [SerializeField]
        float m_GestureDetectionInterval = 0.1f;

        [SerializeField]
        StaticHandGesture[] m_StaticGestures;

        [SerializeField]
        Image m_Highlight;

        [Header("Dynamic Gesture Assets")]
        [SerializeField]
        GameObject startingGestureAsset; // Assign starting gesture asset here

        [SerializeField]
        GameObject endingGestureAsset; // Assign ending gesture asset here

        XRHandShape m_HandShape;
        XRHandPose m_HandPose;
        bool m_WasDetected;
        float m_TimeOfLastConditionCheck;
        Color m_BackgroundDefaultColor;
        Color m_BackgroundHighlightColor = new Color(0f, 0.627451f, 1f);

        private bool isGestureActive = false; // Flag to track if a gesture is active
        private bool isStartingGestureRecognized = false; // Track if starting gesture is recognized
        private bool isEndingGestureRecognized = false; // Track if ending gesture is recognized

        void Awake()
        {
            m_BackgroundDefaultColor = m_Background.color;

            if (m_Highlight)
            {
                m_Highlight.enabled = false;
                m_Highlight.gameObject.SetActive(true);
            }
        }

        void OnEnable()
        {
            m_HandTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
            m_HandShape = m_HandShapeOrPose as XRHandShape;
            m_HandPose = m_HandShapeOrPose as XRHandPose;
        }

        void OnDisable() => m_HandTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);

        public void StartGesture()
        {
            isGestureActive = true; // Mark gesture as active
            isStartingGestureRecognized = false; // Reset recognition flags
            isEndingGestureRecognized = false;
        }

        public void EndGesture()
        {
            isGestureActive = false; // Mark gesture as inactive
            OnGestureEnded(); // Call to recognize the gesture
        }

        public void OnGestureEnded()
        {
            // Make the prediction only if both gestures were recognized
            if (isStartingGestureRecognized && isEndingGestureRecognized)
            {
                Debug.Log("Detected Gesture: J"); // or other relevant message
                m_GesturePerformed?.Invoke(); // Fire event for gesture performed
            }
            else
            {
                Debug.Log("No matching gesture detected.");
            }
        }

        private bool RecognizeStartingGesture(XRHandJointsUpdatedEventArgs eventArgs)
        {
            // Implement your recognition logic here
            Debug.Log("Checking for starting gesture...");
            // Example condition (replace with actual condition)
            if (true) // Placeholder, replace with actual logic
            {
                isStartingGestureRecognized = true;
                Debug.Log("Starting gesture recognized.");
                return true;
            }
            Debug.Log("Starting gesture not recognized.");
            return false;
        }

        private bool RecognizeEndingGesture(XRHandJointsUpdatedEventArgs eventArgs)
        {
            // Implement your recognition logic here
            Debug.Log("Checking for ending gesture...");
            // Example condition (replace with actual condition)
            if (true) // Placeholder, replace with actual logic
            {
                isEndingGestureRecognized = true;
                Debug.Log("Ending gesture recognized.");
                return true;
            }
            Debug.Log("Ending gesture not recognized.");
            return false;
        }

        void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
        {
            if (!isActiveAndEnabled || Time.timeSinceLevelLoad < m_TimeOfLastConditionCheck + m_GestureDetectionInterval)
                return;

            var detected = m_HandTrackingEvents.handIsTracked &&
                m_HandShape != null && m_HandShape.CheckConditions(eventArgs) ||
                m_HandPose != null && m_HandPose.CheckConditions(eventArgs);

            if (!m_WasDetected && detected)
            {
                StartGesture(); // Start gesture tracking
                RecognizeStartingGesture(eventArgs); // Check for starting gesture
            }
            else if (m_WasDetected && !detected)
            {
                EndGesture(); // End gesture tracking if no longer detected
                m_Background.color = m_BackgroundDefaultColor;
            }

            // Check for ending gesture when gesture is active
            if (isGestureActive)
            {
                RecognizeEndingGesture(eventArgs); // Check for ending gesture
            }

            m_WasDetected = detected;
            m_TimeOfLastConditionCheck = Time.timeSinceLevelLoad;

            // Implementing minimum hold time check
            if (isGestureActive && (Time.timeSinceLevelLoad - m_TimeOfLastConditionCheck) >= m_MinimumHoldTime)
            {
                Debug.Log("Gesture held long enough.");
                // Additional logic can be added here if needed
            }
        }

        public bool highlightVisible
        {
            set
            {
                if (m_Highlight)
                    m_Highlight.enabled = value;
            }
        }
    }
}
