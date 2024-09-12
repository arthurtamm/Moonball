using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement; // Para carregar cenas

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float speed = 0;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject winPopup; // Pop-up para mostrar vitória
    public GameObject astronaut;
    public GameObject rocket;
    public Camera mainCamera; // Referência à câmera principal
    public GameObject finalCamera;  // Referência à câmera final do foguete

    private int count;
    public AudioClip itemPickupSound;
    public AudioClip winningSound;

    public AudioClip gameOverSound;
    public AudioSource backgroundMusic;
    private AudioSource audioSource;
    public int requiredFuel = 15; // Número de combustíveis necessários para vencer
    public float rocketSpeed = 10f; // Velocidade de subida do foguete
    public bool isGameWon = false;

    // Referência ao GameManager para pegar o tempo
    public GameManager gameManager;

    // Referência à tela de Game Over
    public GameObject gameOverScreen;

    // Posição limite para "matar" o jogador
    public float deathYThreshold = -10.0f;

    // Array para armazenar os Thwomps
    private ThwompObstacle[] thwomps;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        count = 0; 
        SetCountText();
        
        // Certifique-se de que o pop-up de vitória e a tela de game over estão ocultos ao iniciar o jogo
        if (winTextObject != null)
        {
            winTextObject.SetActive(false);
        }

        if (winPopup != null)
        {
            winPopup.SetActive(false); // Oculta o pop-up de vitória inicialmente
        }

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);  // Oculta a tela de Game Over inicialmente
        }
        
        // Busca todos os Thwomps na cena
        thwomps = FindObjectsOfType<ThwompObstacle>(); // Encontra todos os Thwomps na cena

        // Certifique-se de que a câmera do foguete está desativada ou não usada até a vitória
        finalCamera.gameObject.SetActive(false);

        winPopup.SetActive(false);

    }

    void OnMove(InputValue movementValue)
    {
        if (!isGameWon) // Bloqueia movimento se o jogo estiver ganho
        {
            Vector2 movementVector = movementValue.Get<Vector2>();

            movementX = movementVector.x;
            movementY = movementVector.y;
        }
    }

    void SetCountText() 
    {
       countText.text = "Fuel: " + count.ToString();
    }

    void FixedUpdate() 
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    void Update() 
    {
        // Verificar se o jogador caiu do mapa
        if (transform.position.y < deathYThreshold)
        {
            Die();  // Chama o método para lidar com a "morte"
        }

        // Verifica se o jogador está próximo do astronauta e tem combustível suficiente para vencer
        if (Vector3.Distance(transform.position, astronaut.transform.position) < 3.0f && count >= requiredFuel && !isGameWon)
        {
            isGameWon = true;
            StartWinSequence(); // Inicia a sequência de vitória
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("PickUp")) 
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();

            audioSource.PlayOneShot(itemPickupSound);
        }
    }

    public void Die()
    {
        // Toca o som de Game Over
        audioSource.PlayOneShot(gameOverSound);

        // Para a música de fundo
        backgroundMusic.Stop();

        // Exibe a tela de Game Over
        gameOverScreen.SetActive(true);
        
        // Para o movimento do jogador
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Para todos os sons atuais
        // audioSource.Stop();

        // Opcional: Desabilitar controles ou reiniciar o jogo
        this.enabled = false; // Desativa o script para impedir mais movimentação

        // Para o movimento de todos os Thwomps
        foreach (ThwompObstacle thwomp in thwomps)
        {
            thwomp.StopThwomp();  // Chama o método para parar os Thwomps
        }

        // Para o cronômetro
        gameManager.StopTimer();
    }

    // Método para reiniciar o jogo
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Reinicia a cena atual
    }

    // Método para voltar ao menu principal
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");  // Carrega a cena do menu principal (ajuste o nome conforme sua cena)
    }

    // Método que inicia a sequência de vitória
    void StartWinSequence()
    {
        // Desativa o movimento do jogador
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        this.enabled = false; // Impede que o jogador se mova

        // Toca o som de vitória
        audioSource.PlayOneShot(winningSound);

        // Muda a câmera para focar no foguete
        StartCoroutine(TransitionCameraToRocket());

        // Para o cronômetro
        gameManager.StopTimer();

        // Inicia a decolagem do foguete em uma coroutine
        StartCoroutine(RocketTakeOffCoroutine());
    }

    // Corrotina para fazer a transição da câmera até o foguete
// Corrotina para fazer a transição da câmera até o foguete
    IEnumerator TransitionCameraToRocket()
    {
        float duration = 2.0f; // Duração da transição da câmera
        Vector3 initialPosition = mainCamera.transform.position;
        Quaternion initialRotation = mainCamera.transform.rotation;

        // Pegue a posição e rotação da finalCamera
        Vector3 finalPosition = finalCamera.transform.position;
        Quaternion finalRotation = finalCamera.transform.rotation;

        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(initialPosition, finalPosition, elapsedTime / duration);
            mainCamera.transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = finalPosition;
        mainCamera.transform.rotation = finalRotation;

        // Após a transição da câmera, o foguete começa a decolar
        yield return new WaitForSeconds(1.0f); // Pausa breve para dar tempo ao jogador perceber

        // RocketTakeOff(); 

        // Mostra o pop-up de vitória
        ShowWinPopup();
    }

    IEnumerator RocketTakeOffCoroutine()
    {
        while (true) // Continua a mover o foguete indefinidamente até ser parado manualmente
        {
            rocket.transform.Translate(Vector3.up * rocketSpeed * Time.deltaTime);
            yield return null; // Espera até o próximo frame
        }
    }


    // Exibe o pop-up de vitória
    void ShowWinPopup()
    {
        winPopup.SetActive(true); // Exibe o pop-up
        TextMeshProUGUI winMessage = winPopup.GetComponentInChildren<TextMeshProUGUI>();
        winMessage.text = "You Win!\nTime: " + gameManager.GetTimeSpent() + "\nFuel: " + count.ToString();
    }
}
