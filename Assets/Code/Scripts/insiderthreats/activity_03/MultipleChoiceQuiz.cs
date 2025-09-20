using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultipleChoiceQuiz : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] answers;
        public int correctAnswerIndex;
    }

    [Header("Quiz Settings")]
    public List<Question> questions = new List<Question>();
    public bool randomizeQuestions = false;

    [Header("UI References")]
    public TMP_Text questionTextTMP;
    public Button[] answerButtons;          // assign button GameObjects in inspector
    public TMP_Text[] answerTextsTMP;       // optional: will auto-fill if left empty

    [Header("Feedback")]
    public float feedbackDelay = 1.2f;      // seconds to wait before moving on
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    private int currentQuestionIndex = 0;
    private ColorBlock[] originalColorBlocks;
    private Image[] buttonImages;

    void Awake()
    {
        if (answerButtons == null || answerButtons.Length == 0)
        {
            Debug.LogError("Please assign answerButtons in the inspector.");
            return;
        }

        // auto-fill answerTextsTMP from button children if not provided / wrong size
        if (answerTextsTMP == null || answerTextsTMP.Length != answerButtons.Length)
        {
            answerTextsTMP = new TMP_Text[answerButtons.Length];
            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerTextsTMP[i] = answerButtons[i].GetComponentInChildren<TMP_Text>();
            }
        }

        // cache original ColorBlocks and Image components
        originalColorBlocks = new ColorBlock[answerButtons.Length];
        buttonImages = new Image[answerButtons.Length];
        for (int i = 0; i < answerButtons.Length; i++)
        {
            originalColorBlocks[i] = answerButtons[i].colors;
            buttonImages[i] = answerButtons[i].GetComponent<Image>();
        }

        if (randomizeQuestions) ShuffleQuestions();
    }

    void Start()
    {
        DisplayQuestion();
    }

    void DisplayQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            questionTextTMP.text = "Quiz Completed!";
            foreach (var b in answerButtons) b.gameObject.SetActive(false);
            return;
        }

        var q = questions[currentQuestionIndex];
        questionTextTMP.text = q.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].interactable = true;

            // reset visual color to original
            if (buttonImages[i] != null)
                buttonImages[i].color = originalColorBlocks[i].normalColor;

            if (i < q.answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerTextsTMP[i].text = q.answers[i];
                int idx = i; // capture
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(idx));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnAnswerSelected(int index)
    {
        // start coroutine to show feedback then advance
        StartCoroutine(HandleAnswerRoutine(index));
    }

    IEnumerator HandleAnswerRoutine(int selectedIndex)
    {
        // disable all buttons to prevent further clicks
        for (int i = 0; i < answerButtons.Length; i++)
            answerButtons[i].interactable = false;

        var q = questions[currentQuestionIndex];
        bool isCorrect = (selectedIndex == q.correctAnswerIndex);

        // highlight the selected button
        if (selectedIndex >= 0 && selectedIndex < buttonImages.Length && buttonImages[selectedIndex] != null)
            buttonImages[selectedIndex].color = isCorrect ? correctColor : wrongColor;

        // if wrong, also highlight the correct answer
        if (!isCorrect && q.correctAnswerIndex >= 0 && q.correctAnswerIndex < buttonImages.Length && buttonImages[q.correctAnswerIndex] != null)
            buttonImages[q.correctAnswerIndex].color = correctColor;

        // optional: change question text briefly (uncomment if wanted)
        // questionTextTMP.text = isCorrect ? "Correct!" : "Wrong!";

        yield return new WaitForSeconds(feedbackDelay);

        // reset colors back to original
        for (int i = 0; i < buttonImages.Length; i++)
        {
            if (buttonImages[i] != null)
                buttonImages[i].color = originalColorBlocks[i].normalColor;
        }

        currentQuestionIndex++;
        DisplayQuestion();
    }

    void ShuffleQuestions()
    {
        for (int i = 0; i < questions.Count; i++)
        {
            Question tmp = questions[i];
            int r = Random.Range(i, questions.Count);
            questions[i] = questions[r];
            questions[r] = tmp;
        }
    }
}
