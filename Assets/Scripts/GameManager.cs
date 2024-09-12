using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private float timeSpent = 0f;  // Tempo gasto na partida
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;  // O texto de tempo que será exibido na tela
    public TextMeshProUGUI instructionText;

    // get panel
    public GameObject panel;
    public float displayDuration = 5f;

    // Start is called before the first frame update
    void Start()
    {
        timerIsRunning = true;  // Começa a contar o tempo
        ShowInstructions();     // Exibe as instruções
    }

    // Update is called once per frame
    void Update()
    {
        // Verifica se o cronômetro está rodando
        if (timerIsRunning)
        {
            // Incrementa o tempo gasto
            timeSpent += Time.deltaTime;
            DisplayTime(timeSpent);  // Atualiza o texto do cronômetro
        }
    }

    // Função opcional para exibir o tempo no formato MM:SS
    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);  // Converte o tempo em minutos
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);  // Converte o tempo em segundos

        // Atualiza o texto na UI com o tempo gasto
        timeText.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Método para retornar o tempo gasto em formato MM:SS
    public string GetTimeSpent()
    {
        float minutes = Mathf.FloorToInt(timeSpent / 60);
        float seconds = Mathf.FloorToInt(timeSpent % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Método para parar o cronômetro
    public void StopTimer()
    {
        timerIsRunning = false;
    }

    // Função para exibir o texto de instruções
    public void ShowInstructions()
    {
        instructionText.text = "Collect enough fuel and help the astronaut to launch the rocket.";
        StartCoroutine(HideInstructionsAfterDelay());
    }

    // Corrotina que esconde o texto após um tempo
    IEnumerator HideInstructionsAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);  // Espera pela duração definida
        instructionText.gameObject.SetActive(false);       // Esconde o texto
        panel.SetActive(false);                           // Esconde o painel
    }
}
