using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject phonePanel;
    public GameObject resultPanel;
    public GameObject gameStartPanel;

    [Header("UI Elements")]
    public Text messageText;
    public Text resultText;
    public Text senderText;
    public Button spamButton;
    public Button notSpamButton;
    public Button retryButton;
    public Button closeButton;
    public Button startButton;
    public Button nextLevelButton;
    public Button mesgOneBtn;
    public Button mesgTwoBtn;
    public Button mesgThreeBtn;
    public Text[] inboxMesg;
    public Text titleText;
    public Text scoreText;
    public Text welcomeText;
    public GameObject stageText;

    [Header("Game Settings")]
    public float beginerTime = 30f;
    public float interTime = 15f;
    public float expertTime = 5f;

    public AudioSource alertSound;

    private float setTimer = 10f;
    private Coroutine timerCoroutine;
    private int currentScore = 0, totalScore = 0;
    private int currentQuestionIndex = 0;
    private int currentLevel = 1;
    private Button currentMessageButton;

    private List<(string sender, string message, bool isSpam)> questions = new List<(string, string, bool)>
    {
        ("CommBank ", "🚨 Urgent! Your bank account is locked. Click here immediately: http://bank-login-fix.com", true),
        ("FREE GIFT ", "Congratulations! You’ve won a free iPhone 🎁. Claim now: http://freeprize.com", true),
         ("AUSPOST ", "Unusual login detected on your account. Verify now:https://auspost.com.au/mypost/track/search", false),
        ("FACEBOOK ", "Unusual login activity detected. Secure account now: http://bit.ly/verifyface-bookaccount", true),
        ("Alinta Energy ", "Reminder: Your bill payment is overdue. Pay immediately to avoid suspension. Click here: https://bit.ly/3XyZAbC", true),
        ("Coles Online ", "Your grocery order #23908 has been shipped. Login here to track: https://www.coles.com.au/order-tracker ", false),
        ("ANZ ", "Your 2FA code is 448291. If you didn’t request this, secure your account here: http://anz-secure.net", true),
        ("Outlook ", "Your inbox storage is 90% full. Please archive old emails. https://outlook.live.com/", false),
        ("IT Support ", "Hi, this is IT Support. We’ve detected malware on your computer. Please download the security update here: http://it-support-update.com", true),
    };

    void Start()
    {
        totalScore = questions.Count;
        resultPanel.SetActive(false);
        phonePanel.SetActive(false);
        gameStartPanel.SetActive(true);

        stageText.SetActive(false);

        welcomeText.gameObject.SetActive(true);

        inboxMesg[0].gameObject.SetActive(false);
        inboxMesg[1].gameObject.SetActive(false);
        inboxMesg[2].gameObject.SetActive(false);
        inboxMesg[3].gameObject.SetActive(false);

        scoreText.gameObject.SetActive(false);
        titleText.gameObject.SetActive(false);

        startButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
        mesgOneBtn.gameObject.SetActive(false);
        mesgTwoBtn.gameObject.SetActive(false);
        mesgThreeBtn.gameObject.SetActive(false);

        spamButton.onClick.AddListener(() => HandleAnswer(true));
        notSpamButton.onClick.AddListener(() => HandleAnswer(false));
        retryButton.onClick.AddListener(RestartGame);
        closeButton.onClick.AddListener(QuitGame);
        nextLevelButton.onClick.AddListener(StartNextLevel);

        startButton.onClick.AddListener(() =>
        {
            welcomeText.gameObject.SetActive(false);
            startButton.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(true);
            titleText.gameObject.SetActive(true);
            titleText.text = "Inbox , Beginner Level ";

            LoadLevel(1);
        });

        mesgOneBtn.onClick.AddListener(() => OpenMessage(currentLevelStartIndex() + 0, mesgOneBtn));
        mesgTwoBtn.onClick.AddListener(() => OpenMessage(currentLevelStartIndex() + 1, mesgTwoBtn));
        mesgThreeBtn.onClick.AddListener(() => OpenMessage(currentLevelStartIndex() + 2, mesgThreeBtn));
    }

    int currentLevelStartIndex() => (currentLevel - 1) * 3;

    void LoadLevel(int level)
    {
        

        if (level == 1)
        {
        titleText.text = $" Inbox, Beginner Level";
            setTimer = beginerTime;

        }
        else if(level == 2)
        {

        titleText.text = $" Inbox, Intermediate Level";
            setTimer = interTime;
        }
        else if(level == 3)
        {

        titleText.text = $" Inbox, Expert Level";
            setTimer = expertTime;
        }
        currentLevel = level;
        int startIndex = currentLevelStartIndex();

        TriggerNewMessage(mesgOneBtn, inboxMesg[0], $"From: {questions[startIndex].sender}");
        TriggerNewMessage(mesgTwoBtn, inboxMesg[1], $"From: {questions[startIndex + 1].sender}");
        inboxMesg[2].gameObject.SetActive(false); 
        mesgThreeBtn.gameObject.SetActive(false);
    }

    void TriggerNewMessage(Button msgBtn, Text msgText, string label)
    {
        if (alertSound != null) alertSound.Play();
        msgBtn.gameObject.SetActive(true);
        msgText.gameObject.SetActive(true);
        msgText.text = label;
    }

    void OpenMessage(int questionIndex, Button btn)
    {
        timerCoroutine = StartCoroutine(MessageTimer());

        currentQuestionIndex = questionIndex;
        currentMessageButton = btn;

        var question = questions[questionIndex];
        senderText.text = $"From: {question.sender}";
        messageText.text = question.message;

        phonePanel.SetActive(true);
        resultPanel.SetActive(false);
        gameStartPanel.SetActive(false);
    }

    void HandleAnswer(bool? selectedSpam)
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        phonePanel.SetActive(false);
        resultPanel.SetActive(true);
        stageText.SetActive(false);

        bool correctAnswer = questions[currentQuestionIndex].isSpam;

        int localIndex = currentQuestionIndex % 3;

        if (selectedSpam == null)
        {
            resultText.text = "Time's up! No answer selected.";
            inboxMesg[localIndex].color = Color.red;
        }
        else if (selectedSpam == correctAnswer)
        {
            resultText.text = "✅ Correct!";
            inboxMesg[localIndex].color = Color.green;
            currentScore++;
        }
        else
        {
            resultText.text = "❌ Wrong!";          
            inboxMesg[localIndex].color = Color.red;
        }

        if ((currentQuestionIndex % 3) == 1)
        {
            int startIndex = currentLevelStartIndex();
            TriggerNewMessage(mesgThreeBtn, inboxMesg[2], $"From: {questions[startIndex + 2].sender}");
            StartCoroutine(NextQuestionAfterDelay(2f));
        }
        else if ((currentQuestionIndex % 3) == 2)
        {
            ShowLevelResult();
        }
        else
        {
            StartCoroutine(NextQuestionAfterDelay(2f));
        }
    }

    void ShowLevelResult()
    {
        phonePanel.SetActive(false);
        resultPanel.SetActive(true);
        gameStartPanel.SetActive(true);

        stageText.SetActive(true);

        resultText.text = "";

        if (currentLevel < 3)
        {
            nextLevelButton.gameObject.SetActive(true);
            retryButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(true);
        }
        else
        {
            if (currentScore <= 8)
            {
                nextLevelButton.gameObject.SetActive(false);
                retryButton.gameObject.SetActive(true);
                closeButton.gameObject.SetActive(true);
                StartCoroutine(LastInstructionDisplay(0f));
            }
            else
            {
                titleText.text = "Congratulations you passed";
                nextLevelButton.gameObject.SetActive(false);
                retryButton.gameObject.SetActive(true);
                closeButton.gameObject.SetActive(true);
            }
        }
    }

    IEnumerator NextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        phonePanel.SetActive(false);
        resultPanel.SetActive(false);
        gameStartPanel.SetActive(true);

    }
    
    IEnumerator LastInstructionDisplay(float delay)
    {
        yield return new WaitForSeconds(delay);
        phonePanel.SetActive(false);
        resultPanel.SetActive(false);
        gameStartPanel.SetActive(true);
        inboxMesg[0].gameObject.SetActive(false);
        inboxMesg[1].gameObject.SetActive(false);
        inboxMesg[2].gameObject.SetActive(false);
        mesgOneBtn.gameObject.SetActive(false);
        mesgTwoBtn.gameObject.SetActive(false);
        mesgThreeBtn.gameObject.SetActive(false);
        welcomeText.gameObject.SetActive(true);
        //scoreText.gameObject.SetActive(false);

        titleText.text = "Feedback !";
        welcomeText.text = "You are required to complete this smishing training as a part of company cyber awereness compaign.  Please click start to begin !";
        welcomeText.text = "1. Read messages thoroughly\n\n" +
            "2. Look for https in the link which makes them safe\n\n" +
            "3. Never trust shortened URLs\n\n" +
            "4. If there is any text from employer, double check with the team   before proceeding";

        retryButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);

    }

    IEnumerator MessageTimer()
    {
        yield return new WaitForSeconds(setTimer);
        HandleAnswer(null);
    }

    private void Update()
    {
        scoreText.text = $"[ {currentScore} / {totalScore} ]";

    }

    void StartNextLevel()
    {

        nextLevelButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        resultPanel.SetActive(false);
        gameStartPanel.SetActive(true);

        inboxMesg[0].color = Color.white;
        inboxMesg[1].color = Color.white;
        inboxMesg[2].color = Color.white;

        inboxMesg[0].gameObject.SetActive(false);
        inboxMesg[1].gameObject.SetActive(false);
        inboxMesg[2].gameObject.SetActive(false);

        LoadLevel(currentLevel + 1);
    }

    void RestartGame()
    {
        welcomeText.text = "You are required to complete this smishing training as a part of company cyber awereness compaign.  Please click start to begin !";
        currentScore = 0;
        currentQuestionIndex = 0;
        currentLevel = 1;

        resultPanel.SetActive(false);
        phonePanel.SetActive(false);
        gameStartPanel.SetActive(true);

        welcomeText.gameObject.SetActive(true);

        inboxMesg[0].gameObject.SetActive(false);
        inboxMesg[1].gameObject.SetActive(false);
        inboxMesg[2].gameObject.SetActive(false);
        inboxMesg[3].gameObject.SetActive(false);
        
        mesgOneBtn.gameObject.SetActive(false);
        mesgTwoBtn.gameObject.SetActive(false);
        mesgThreeBtn.gameObject.SetActive(false);

        scoreText.gameObject.SetActive(false);
        titleText.gameObject.SetActive(false);

        inboxMesg[0].color = Color.white;
        inboxMesg[1].color = Color.white;
        inboxMesg[2].color = Color.white;

        resultText.text = "";

        startButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
    }

    void QuitGame()
    {
        Debug.Log("Game Closed.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
