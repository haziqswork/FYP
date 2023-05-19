using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[System.Serializable]
public class QuizQuestion
{
    public string question;
    public string[] options;
    public string answer;
}

[System.Serializable]
public class QuizData
{
    public QuizQuestion[] questions;
}

public class PopUpTrigger : MonoBehaviour
{
    [SerializeField] private Button qButton;
    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button option1Button;
    [SerializeField] private Button option2Button;
    [SerializeField] private Button option3Button;
    [SerializeField] private Button option4Button;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private float messageDisplayTime = 2f;
    [SerializeField] private string jsonString;
    private QuizData quizData;
    private int currentQuestionIndex;
    [SerializeField] private Teleporter teleporter;

    private void Start()
    {
        qButton.onClick.AddListener(OnInteractButtonClicked);
        canvas.SetActive(false);
        teleporter = GetComponentInChildren<Teleporter>();
    }

    private void OnInteractButtonClicked()
    {
        if (canvas.activeSelf)
            return;

        jsonString = System.IO.File.ReadAllText(Application.dataPath + "/Data/questions.json");
        Debug.Log("JSON string: " + jsonString);
        quizData = JsonUtility.FromJson<QuizData>(jsonString);

        currentQuestionIndex = Random.Range(0, quizData.questions.Length);
        questionText.text = quizData.questions[currentQuestionIndex].question;
        canvas.SetActive(true);

        string[] options = quizData.questions[currentQuestionIndex].options;
        Debug.Log("Option 1: " + options[0]);
        Debug.Log("Option 2: " + options[1]);
        Debug.Log("Option 3: " + options[2]);
        Debug.Log("Option 4: " + options[3]);

        option1Button.GetComponentInChildren<TextMeshProUGUI>().text = options[0];
        option2Button.GetComponentInChildren<TextMeshProUGUI>().text = options[1];
        option3Button.GetComponentInChildren<TextMeshProUGUI>().text = options[2];
        option4Button.GetComponentInChildren<TextMeshProUGUI>().text = options[3];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AccessButton"))
        {
            OnInteractButtonClicked();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("AccessButton"))
        {
            canvas.SetActive(false);
        }
    }

    public void AnswerCheck()
    {
        string selectedAnswer = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text;

        if (selectedAnswer == quizData.questions[currentQuestionIndex].answer)
        {
            Debug.Log("Correct Answer");
            StartCoroutine(DisplayResultAndCloseCanvas("Correct Answer", true));
            teleporter.EnableTeleporter();
        }
        else
        {
            Debug.Log("Wrong Answer");
            StartCoroutine(DisplayResultAndCloseCanvas("Wrong Answer", false));
            teleporter.DisableTeleporter();
        }
    }

    private IEnumerator DisplayResultAndCloseCanvas(string message, bool isCorrect)
    {
        resultText.text = message;
        yield return new WaitForSeconds(messageDisplayTime);
        resultText.text = string.Empty;

        if (isCorrect)
        {
            questionText.text = "Correct Answer";
        }

        canvas.SetActive(false);

        if (!isCorrect)
        {
            // Handle incorrect answer logic here
        }
    }
}
