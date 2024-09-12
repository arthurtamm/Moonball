using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThwompObstacle : MonoBehaviour
{
    public Transform targetPosition;  // A posição para onde a caixa cairá (chão)
    public float fallSpeed = 5f;  // Velocidade de queda
    public float waitTime = 2f;  // Tempo que o Thwomp espera antes de subir/descansar
    private Vector3 initialPosition;  // Posição inicial (flutuando)
    private AudioSource audioSource;
    public AudioClip thwompSound;

    private bool isGameOver = false;  // Adicione uma variável para monitorar o estado do jogo

    void Start()
    {
        initialPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(AutoMove());
    }

    IEnumerator AutoMove()
    {
        while (!isGameOver)  // Enquanto não for Game Over
        {
            // Fase de queda
            yield return StartCoroutine(FallDown());
            audioSource.PlayOneShot(thwompSound);

            // Espera um tempo no chão antes de subir
            yield return new WaitForSeconds(waitTime);

            // Fase de subir
            yield return StartCoroutine(RiseUp());

            // Espera um tempo flutuando antes de cair novamente
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator FallDown()
    {
        while (transform.position != targetPosition.position && !isGameOver)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, fallSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator RiseUp()
    {
        while (transform.position != initialPosition && !isGameOver)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, fallSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // Método para ser chamado no Game Over
    public void StopThwomp()
    {
        isGameOver = true;  // Sinaliza que o jogo acabou
        audioSource.Stop();  // Para o som do Thwomp
        StopAllCoroutines();  // Para todas as corrotinas ativas
    }
}
