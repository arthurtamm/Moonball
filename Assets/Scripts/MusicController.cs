using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController instance; // Singleton para garantir que haja apenas um MusicController

    private AudioSource audioSource;

    void Awake()
    {
        // Verifica se já existe um controlador de música
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Não destrua ao carregar uma nova cena
        }
        else
        {
            Destroy(gameObject); // Se já existe um controlador, destrua este
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void StopMusic()
    {
        audioSource.Stop(); // Para a música
    }
}
