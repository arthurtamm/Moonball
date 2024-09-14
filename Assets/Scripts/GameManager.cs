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

    public float initialTime = 15f;  // Tempo inicial em segundos
    public Transform player;  // Referência ao jogador
    private PlayerController playerController; // Referência ao PlayerController

    public AudioSource timerSound;  // Som do cronômetro

    // get panel
    public GameObject panel;
    public float displayDuration = 10f;

    // Cores para o tempo
    public Color greenColor = Color.green;
    public Color yellowColor = Color.yellow;
    public Color redColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        timerIsRunning = true;  // Começa a contar o tempo
        ShowInstructions();     // Exibe as instruções

        // Encontra o PlayerController
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>(); 
        }

        // Certifique-se de que o som não toque automaticamente
        timerSound.Stop();
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

        if (playerController.isGameWon || playerController.isDead)
        {
            timerSound.Stop();
        }
    }

    // Função opcional para exibir o tempo no formato MM:SS
    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);  // Converte o tempo em minutos
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);  // Converte o tempo em segundos

        float timeRemaining = initialTime - timeToDisplay;  // Calcula o tempo restante

        // show time left
        timeText.text = "Time: " + string.Format("{0:00}:{1:00}", Mathf.FloorToInt(initialTime - timeToDisplay) / 60, Mathf.FloorToInt(initialTime - timeToDisplay) % 60);

        // Mudar a cor do texto baseado no tempo restante
        if (timeRemaining <= 60f)  // Menos de 1 minuto
        {
            timeText.color = redColor;
        }
        else if (timeRemaining <= 150f && timeRemaining > 60f)  // Menos de 2.5 minutos (150 segundos), mas mais de 1 minuto
        {
            timeText.color = yellowColor;
        }
        else  // Caso contrário, verde
        {
            timeText.color = greenColor;
        }
        
        // Tocar o som do relógio apenas se o tempo restante for menor que 15 segundos
        if (timeRemaining <= 15f && !timerSound.isPlaying)
        {
            timerSound.Play();
        }

        // if time is over, player dies and stop the timer
        if (timeToDisplay >= initialTime)
        {
            timeText.text = "Time: 00:00";
            StopTimer();
            timerSound.Stop();
            playerController.Die();
        }
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
        instructionText.text = "Collect enough fuel and help the astronaut to launch the rocket. \n Press R or the Restart button to restart the game.";
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
