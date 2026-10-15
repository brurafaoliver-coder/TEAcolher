using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Estados emocionais do termômetro (RF-02). Os valores numéricos ficam fixos
/// para não quebrar dados salvos mais tarde (guia de novembro).
/// Ajuste os nomes/quantidade conforme as 3 a 5 opções que o grupo escolher.
/// </summary>
public enum Emocao
{
    Nenhuma = -1,
    Feliz = 0,
    Calmo = 1,
    Triste = 2,
    Bravo = 3,
    Medo = 4
}

/// <summary>
/// Guarda o estado do fluxo da criança entre as telas
/// (termômetro -> respiração -> contagem regressiva).
/// As próximas telas leem daqui; assim a seleção já fica conectada
/// ao restante do fluxo mesmo que ele ainda não exista.
/// </summary>
public static class FluxoEstado
{
    public static Emocao EmocaoSelecionada { get; set; } = Emocao.Nenhuma;
}

/// <summary>
/// Tela do termômetro de emoções (RF-02), na tela da CRIANÇA.
/// NÃO é a tela de diagnóstico; mantenha em um Canvas separado.
///
/// Como usar:
/// 1. Crie um Canvas "CanvasCrianca" e, dentro, um painel "PainelTermometro".
/// 2. Crie um botão (UI > Button) por emoção, grande, só com imagem (sem texto).
/// 3. Adicione este script ao PainelTermometro.
/// 4. Em "opcoes", adicione um item por emoção: escolha a emoção e arraste o botão.
/// 5. (Opcional) Arraste o painel da próxima tela para "proximaTela".
/// </summary>
public class TermometroEmocional : MonoBehaviour
{
    [Serializable]
    public class OpcaoEmocao
    {
        public Emocao emocao;
        public Button botao;
    }

    [Header("Opções (uma por emoção)")]
    public OpcaoEmocao[] opcoes;

    [Header("Fluxo")]
    [Tooltip("Painel que aparece depois da escolha (ex.: respiração). Pode ficar vazio por enquanto.")]
    public GameObject proximaTela;

    [Tooltip("Tempo de espera depois do toque, para a criança ver o feedback, em segundos.")]
    public float atrasoAposEscolha = 0.8f;

    [Header("Feedback visual do toque")]
    public float escalaSelecionada = 1.15f;
    public float duracaoAnimacao = 0.25f;

    /// <summary>Disparado quando a criança escolhe uma emoção.</summary>
    public event Action<Emocao> AoEscolherEmocao;

    private bool escolhaFeita;

    void Awake()
    {
        foreach (var opcao in opcoes)
        {
            if (opcao.botao == null) continue;
            var emocaoLocal = opcao.emocao; // cópia, para o clique capturar o valor certo
            var botaoLocal = opcao.botao;
            botaoLocal.onClick.AddListener(() => Escolher(emocaoLocal, botaoLocal));
        }
    }

    void OnEnable()
    {
        // Sempre que a tela aparece, recomeça limpa
        escolhaFeita = false;
        FluxoEstado.EmocaoSelecionada = Emocao.Nenhuma;
        foreach (var opcao in opcoes)
        {
            if (opcao.botao == null) continue;
            opcao.botao.interactable = true;
            opcao.botao.transform.localScale = Vector3.one;
        }
    }

    private void Escolher(Emocao emocao, Button botao)
    {
        if (escolhaFeita) return; // evita toque duplo
        escolhaFeita = true;

        FluxoEstado.EmocaoSelecionada = emocao;
        AoEscolherEmocao?.Invoke(emocao);
        Debug.Log($"[Termômetro] Emoção escolhida: {emocao}");

        // Os outros botões ficam desativados, sem sumir de repente
        foreach (var opcao in opcoes)
        {
            if (opcao.botao != null && opcao.botao != botao)
                opcao.botao.interactable = false;
        }

        StartCoroutine(AnimarEAvancar(botao.transform));
    }

    private IEnumerator AnimarEAvancar(Transform alvo)
    {
        // "Pulinho" suave de escala no botão escolhido
        Vector3 inicio = Vector3.one;
        Vector3 fim = Vector3.one * escalaSelecionada;
        float t = 0f;
        while (t < duracaoAnimacao)
        {
            t += Time.unscaledDeltaTime;
            alvo.localScale = Vector3.Lerp(inicio, fim, Mathf.SmoothStep(0f, 1f, t / duracaoAnimacao));
            yield return null;
        }

        yield return new WaitForSecondsRealtime(atrasoAposEscolha);

        if (proximaTela != null)
        {
            proximaTela.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
