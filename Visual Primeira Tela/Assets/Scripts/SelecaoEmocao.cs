
using UnityEngine;
using UnityEngine.UI;

public class SelecaoEmocao : MonoBehaviour
{
    public Button[] botoes;
    public Color corNormal = Color.white;
    public Color corSelecionada = new Color(0.75f, 0.9f, 1f);

    private Button botaoAtual;

    void Start()
    {
        foreach (Button botao in botoes)
        {
            if (botao == null) continue;

            botao.onClick.AddListener(() => Selecionar(botao));
        }
    }

    void Selecionar(Button botao)
    {
        if (botaoAtual != null)
        {
            botaoAtual.GetComponent<Image>().color = corNormal;
        }

        botaoAtual = botao;
        botaoAtual.GetComponent<Image>().color = corSelecionada;

        Debug.Log("Emoção selecionada: " + botao.name);
    }
}
