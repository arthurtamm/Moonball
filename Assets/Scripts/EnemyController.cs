using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;         // Referência ao jogador
    public float speed = 5f;         // Velocidade do inimigo
    public float minX, maxX, minZ, maxZ, minY;  // Limites de movimentação do inimigo (X e Z)

    private Vector3 initialPosition; // Posição inicial do inimigo, para garantir que ele não saia dos limites
    private PlayerController playerController; // Referência ao PlayerController

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;  // Armazena a posição inicial do inimigo

        // Encontra o PlayerController
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>(); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Calcula a distância entre o inimigo e o jogador
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Persegue o jogador se estiver dentro dos limites de coordenadas
        if (distanceToPlayer > 1f) // Se o inimigo está longe do jogador
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        // Calcula a direção para o jogador
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        // Limita o movimento do inimigo dentro das coordenadas definidas
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
        newPosition.y = minY; // Mantém a posição Y fixa

        // Atualiza a posição do inimigo
        transform.position = newPosition;
    }

    // Detecta colisão com o jogador
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with " + collision.gameObject.name);
        // if (collision.gameObject.CompareTag("Player"))
        if (collision.gameObject.name == "Player")
        {
            // Chama o método para matar o jogador
            Debug.Log("Player killed by enemy!");
            playerController.Die();
        }
    }

    void KillPlayer()
    {
        // Chama o método Die do PlayerController
        if (playerController != null)
        {
            playerController.Die();
        }
        else
        {
            Debug.LogError("PlayerController not found!");
        }
    }
}
