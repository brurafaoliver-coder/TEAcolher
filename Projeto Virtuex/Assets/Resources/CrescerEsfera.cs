using UnityEngine;

public class CrescerEsfera : MonoBehaviour
{
    [Header("Configurações de Tamanho")]
    [Tooltip("Tamanho mínimo da esfera")]
    public Vector3 tamanhoMinimo = new Vector3(0.5f, 0.5f, 0.5f);

    [Tooltip("Tamanho máximo da esfera")]
    public Vector3 tamanhoMaximo = new Vector3(2.0f, 2.0f, 2.0f);

    [Header("Configurações de Velocidade")]
    [Tooltip("Velocidade da animação")]
    public float velocidade = 2.0f;

    void Update()
    {
        // Cria um valor que oscila suavemente entre 0 e 1 ao longo do tempo
        float progresso = (Mathf.Sin(Time.time * velocidade) + 1.0f) / 2.0f;

        // Interpola o tamanho da esfera entre o mínimo e o máximo
        transform.localScale = Vector3.Lerp(tamanhoMinimo, tamanhoMaximo, progresso);
    }
}