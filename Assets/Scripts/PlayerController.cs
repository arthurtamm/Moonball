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
    public float speed = 10;
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

    public AudioSource clockSound;
    private AudioSource audioSource;
    public int requiredFuel = 0; // Número de combustíveis necessários para vencer
    public float rocketSpeed = 10f; // Velocidade de subida do foguete
    public bool isGameWon = false;

    // Nova variável para evitar que o som de Game Over toque várias vezes
    public bool isDead = false;

    // Referência ao GameManager para pegar o tempo
    public GameManager gameManager;

    // Referência à tela de Game Over
    public GameObject gameOverScreen;

    // Posição limite para "matar" o jogador
    public float deathYThreshold = -10.0f;

    // Array para armazenar os Thwomps
    private ThwompObstacle[] thwomps;

    public float minX_drop = -40f;
    public float maxX_drop = -10f;
    public float minZ_drop = 40f;
    public float maxZ_drop = 60f;

    public bool hasMoved = false; // Verifica se o jogador já se moveu

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        count = 0; 
        SetCountText();
        
        // Se o controlador de música do menu existir, pare a música do menu
        if (MusicController.instance != null)
        {
            MusicController.instance.StopMusic();
        }

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
        if (!isGameWon && !isDead) // Bloqueia movimento se o jogo estiver ganho ou o jogador estiver morto
        {
            Vector2 movementVector = movementValue.Get<Vector2>();

            movementX = movementVector.x;
            movementY = movementVector.y;
        }

        // Verifica se o jogador se moveu pela primeira vez
        if (!hasMoved && (movementX != 0 || movementY != 0))
        {
            hasMoved = true;
            gameManager.HideInstructions(); // Chama o método para ocultar as instruções
        }
    }

    void SetCountText() 
    {
       countText.text = count.ToString();
    }

    void FixedUpdate() 
    {
        if (!isDead) // Impede movimento se o jogador estiver morto
        {
            Vector3 movement = new Vector3(movementX, 0.0f, movementY);
            rb.AddForce(movement * speed);
        }
    }

    void Update() 
    {
        // Verificar se o jogador caiu do mapa
        if (transform.position.y < deathYThreshold && !isDead)
        {
            Die();  // Chama o método para lidar com a "morte"
        }

        // Verifica se o jogador está próximo do astronauta e tem combustível suficiente para vencer
        if (Vector3.Distance(transform.position, astronaut.transform.position) < 3.0f && count >= requiredFuel && !isGameWon)
        {
            isGameWon = true;
            StartWinSequence(); // Inicia a sequência de vitória
        }

        Debug.Log("X: " + transform.position.x + " Z: " + transform.position.z);
        Debug.Log("minX: " + minX_drop + " maxX: " + maxX_drop + " minZ: " + minZ_drop + " maxZ: " + maxZ_drop);
        if (transform.position.x > minX_drop && transform.position.x < maxX_drop && transform.position.z > minZ_drop && transform.position.z < maxZ_drop)
        {
            speed = 15;

            if (maxX_drop >= -15)
            {
                speed = 22;
            }
        }
        else
        {
            speed = 10;
        }

        // Verifica se o jogador pressionou a tecla "R" ou o botão "Reiniciar" no controle
        if (Keyboard.current.rKey.wasPressedThisFrame)// || Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            RestartGame(); // Chama o método para reiniciar o jogo
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

    void StopThwomp()
    {
        foreach (ThwompObstacle thwomp in thwomps)
        {
            thwomp.StopThwomp();
        }
    }

    public void Die()
    {
        if (isDead) return; // Garante que o método seja chamado apenas uma vez

        isDead = true; // Marca o jogador como morto

        clockSound.Stop(); // Para o som do cronômetro
        
        // Toca o som de Game Over apenas uma vez
        audioSource.PlayOneShot(gameOverSound);

        // Para a música de fundo
        backgroundMusic.Stop();

        // Exibe a tela de Game Over
        gameOverScreen.SetActive(true);
        
        // Para o movimento do jogador
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        StopThwomp();

        // Para o cronômetro
        gameManager.StopTimer();

        // Impede mais movimentação do jogador
        this.enabled = false;
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

        StopThwomp(); // Para o movimento de todos os Thwomps

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

    private void OnDrawGizmos()
    {
        // Desenha um cubo para representar a área de vigilância
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(new Vector3((minX_drop + maxX_drop) / 2, 0, (minZ_drop + maxZ_drop) / 2),
                            new Vector3(maxX_drop - minX_drop, 1, maxZ_drop - minZ_drop));
    }

}
