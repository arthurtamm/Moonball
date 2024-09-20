using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;         // Referência ao jogador
    public float speed = 7f;         // Velocidade do inimigo
    public float patrolSpeed = 2f;   // Velocidade quando está patrulhando
    public float minX, maxX, minZ, maxZ, minY;  // Limites de movimentação do inimigo (X e Z)

    private Vector3 initialPosition; // Posição inicial do inimigo, para garantir que ele não saia dos limites
    private PlayerController playerController; // Referência ao PlayerController
    private Vector3 patrolTarget;    // Ponto de destino durante a patrulha

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;  // Armazena a posição inicial do inimigo
        SetNewPatrolTarget();  // Define um novo ponto de patrulha inicialmente

        // Encontra o PlayerController
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>(); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Verifica se o jogador está dentro da área de vigilância
        if (IsPlayerInWatchArea())
        {
            FollowPlayer();
        }
        else
        {
            // Retorna ao comportamento de patrulha se o jogador estiver fora da área de vigilância
            PatrolArea();
        }
    }

    // Método para verificar se o jogador está dentro da área de vigilância
    bool IsPlayerInWatchArea()
    {
        return player.position.x >= minX && player.position.x <= maxX &&
               player.position.z >= minZ && player.position.z <= maxZ;
    }

    // Persegue o jogador
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

    // Patrulha a área (movimenta-se de forma aleatória)
    void PatrolArea()
    {
        // Move o inimigo para o ponto de patrulha
        Vector3 direction = (patrolTarget - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * patrolSpeed * Time.deltaTime;

        // Limita o movimento do inimigo dentro das coordenadas definidas
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
        newPosition.y = minY; // Mantém a posição Y fixa

        // Atualiza a posição do inimigo
        transform.position = newPosition;

        // Verifica se chegou ao ponto de patrulha
        if (Vector3.Distance(transform.position, patrolTarget) < 1f)
        {
            SetNewPatrolTarget();  // Define um novo ponto de patrulha quando chegar ao destino
        }
    }

    // Define um novo ponto de patrulha dentro dos limites
    void SetNewPatrolTarget()
    {
        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);
        patrolTarget = new Vector3(randomX, minY, randomZ);  // Define um novo alvo de patrulha
    }

    // Detecta colisão com o jogador
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with " + collision.gameObject.name);

        if (collision.gameObject.name == "Player")
        {
            // Chama o método para matar o jogador
            Debug.Log("Player killed by enemy!");
            playerController.Die();
        }
    }

    private void OnDrawGizmos()
    {
        // Desenha um cubo para representar a área de vigilância
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3((minX + maxX) / 2, minY, (minZ + maxZ) / 2),
                            new Vector3(maxX - minX, 1, maxZ - minZ));
    }
}
