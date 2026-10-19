using UnityEngine;

public class EmojiPainel : MonoBehaviour
{
    [SerializeField] private GameObject painel;

    void Start()
    {
        painel.SetActive(false); // começa escondido
    }

    public void MostrarPainel() => painel.SetActive(true);
    public void EsconderPainel() => painel.SetActive(false);

    // Chamado pelos botões, com o número do emoji (0 a 4)
    public void EscolherEmoji(int indice)
    {
        Debug.Log("Emoji escolhido: " + indice);
        // aqui você salva a resposta
    }
}