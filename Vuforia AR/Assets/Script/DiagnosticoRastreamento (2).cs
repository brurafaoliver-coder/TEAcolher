using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Vuforia;

/// <summary>
/// Tela de diagnóstico (RF-09): mostra status de rastreamento, tamanho da
/// imagem detectada e FPS. Ferramenta do grupo, NÃO faz parte da tela da criança.
///
/// Como usar:
/// 1. Crie um Canvas separado (ex.: "CanvasDiagnostico") com um TextMeshPro - Text.
/// 2. Adicione este script ao Canvas.
/// 3. Arraste o Image Target para "alvo" e o texto para "textoDiagnostico".
///
/// Captura de testes (Semana 3, opcional):
/// 4. Preencha "Condição de Luz" e "Ângulo" antes de cada rodada de testes
///    (ou deixe em branco e edite depois na planilha).
/// 5. Crie um botão (UI > Button - TextMeshPro) e arraste-o para "botaoCapturar".
/// 6. Cada toque no botão grava uma linha pronta para colar na planilha,
///    na mesma ordem de colunas da "Matriz de Testes":
///    Data | Testador | Luz | Distância | Ângulo | Status | Motivo | Tamanho | FPS | Reconheceu? | Observações
/// 7. Crie um TextMeshPro extra (opcional) e arraste para "textoCapturas"
///    para ver o histórico na tela. O botão "Copiar tudo" (opcional) copia
///    todas as linhas capturadas pra área de transferência, prontas pra colar
///    direto numa célula da planilha.
/// </summary>
public class DiagnosticoRastreamento : MonoBehaviour
{
    [Header("Referências")]
    public ObserverBehaviour alvo;          // o Image Target (etiqueta)
    public TMP_Text textoDiagnostico;       // texto na tela
    public Camera cameraAR;                 // se vazio, usa Camera.main

    [Header("Captura de testes (opcional, Semana 3)")]
    public TMP_InputField testador;         // nome de quem está testando
    public TMP_Dropdown condicaoLuz;        // Clara / Média / Escura / Colorida
    public TMP_InputField distanciaMetros;  // distância combinada do teste, em metros
    public TMP_Dropdown angulo;             // Frontal / Lateral ~30° / Lateral ~45°
    public TMP_InputField observacoes;      // observação livre da rodada atual
    public Button botaoCapturar;            // grava a leitura atual como uma linha
    public Button botaoCopiarTudo;          // copia todas as linhas capturadas
    public TMP_Text textoCapturas;          // mostra o histórico de linhas capturadas

    private readonly StringBuilder capturas = new StringBuilder();
    private int totalCapturas = 0;

    private TargetStatus statusAtual;
    private float fpsSuavizado;

    // Última leitura calculada no Update(), guardada para a captura não
    // recalcular tudo de novo e não dessincronizar do que está na tela.
    private string ultimoStatus = "-";
    private string ultimoMotivo = "-";
    private string ultimoTamanhoReal = "-";
    private float ultimoFps = 0f;

    void Start()
    {
        if (cameraAR == null) cameraAR = Camera.main;

        if (alvo != null)
        {
            alvo.OnTargetStatusChanged += AoMudarStatus;
            statusAtual = alvo.TargetStatus;
        }

        if (botaoCapturar != null) botaoCapturar.onClick.AddListener(CapturarLeitura);
        if (botaoCopiarTudo != null) botaoCopiarTudo.onClick.AddListener(CopiarTudo);
    }

    void OnDestroy()
    {
        if (alvo != null) alvo.OnTargetStatusChanged -= AoMudarStatus;
    }

    private void AoMudarStatus(ObserverBehaviour comportamento, TargetStatus status)
    {
        statusAtual = status;
        // Também aparece no Console do Unity, útil pra depurar
        Debug.Log($"[Diagnóstico] {status.Status} / {status.StatusInfo}");
    }

    void Update()
    {
        // FPS suavizado (média móvel) pra não ficar piscando
        float fpsInstantaneo = 1f / Mathf.Max(Time.unscaledDeltaTime, 0.0001f);
        fpsSuavizado = Mathf.Lerp(fpsSuavizado <= 0 ? fpsInstantaneo : fpsSuavizado,
                                  fpsInstantaneo, 0.1f);

        if (textoDiagnostico == null) return;

        string status = statusAtual.Status.ToString();       // TRACKED, EXTENDED_TRACKED, LIMITED, NO_POSE
        string motivo = statusAtual.StatusInfo.ToString();   // NORMAL, INSUFFICIENT_LIGHT, EXCESSIVE_MOTION...

        string tamanhoReal = "-";
        string tamanhoTela = "-";
        string distancia = "-";

        bool temPose = statusAtual.Status != Status.NO_POSE;
        var imageTarget = alvo as ImageTargetBehaviour;

        if (temPose && imageTarget != null && cameraAR != null)
        {
            // Tamanho configurado da etiqueta (unidades do Vuforia, normalmente metros)
            Vector2 tam = imageTarget.GetSize();
            tamanhoReal = $"{tam.x * 100f:F1} x {tam.y * 100f:F1} cm";

            // Tamanho aparente na tela (pixels): projeta os cantos da etiqueta
            Transform t = alvo.transform;
            Vector3 dx = t.right * (tam.x * 0.5f);
            Vector3 dz = t.forward * (tam.y * 0.5f);
            Vector3 c1 = cameraAR.WorldToScreenPoint(t.position - dx - dz);
            Vector3 c2 = cameraAR.WorldToScreenPoint(t.position + dx - dz);
            Vector3 c3 = cameraAR.WorldToScreenPoint(t.position - dx + dz);
            float largPx = Vector2.Distance(c1, c2);
            float altPx = Vector2.Distance(c1, c3);
            tamanhoTela = $"{largPx:F0} x {altPx:F0} px";

            // Distância câmera -> etiqueta
            distancia = $"{Vector3.Distance(cameraAR.transform.position, t.position) * 100f:F0} cm";
        }

        textoDiagnostico.text =
            $"STATUS: {status}\n" +
            $"MOTIVO: {motivo}\n" +
            $"TAMANHO REAL: {tamanhoReal}\n" +
            $"TAMANHO NA TELA: {tamanhoTela}\n" +
            $"DISTÂNCIA: {distancia}\n" +
            $"FPS: {fpsSuavizado:F0}";

        // Guarda a leitura atual para o botão "Capturar" usar
        ultimoStatus = status;
        ultimoMotivo = motivo;
        ultimoTamanhoReal = tamanhoReal;
        ultimoFps = fpsSuavizado;
    }

    /// <summary>
    /// Grava a leitura atual como uma linha pronta para colar na aba
    /// "Matriz de Testes" da planilha. Chamada pelo botaoCapturar.
    /// </summary>
    public void CapturarLeitura()
    {
        string data = System.DateTime.Now.ToString("yyyy-MM-dd");
        string nomeTestador = testador != null ? testador.text : "";
        string luz = condicaoLuz != null && condicaoLuz.options.Count > 0
            ? condicaoLuz.options[condicaoLuz.value].text : "";
        string dist = distanciaMetros != null ? distanciaMetros.text : "";
        string ang = angulo != null && angulo.options.Count > 0
            ? angulo.options[angulo.value].text : "";
        string reconheceu = (ultimoStatus == "TRACKED" || ultimoStatus == "EXTENDED_TRACKED") ? "Sim" : "Não";
        string obs = observacoes != null ? observacoes.text : "";

        // Mesma ordem de colunas da aba "Matriz de Testes":
        // Data | Testador | Luz | Distância | Ângulo | Status | Motivo | Tamanho | FPS | Reconheceu? | Observações
        string linha = string.Join("\t", new[]
        {
            data, nomeTestador, luz, dist, ang,
            ultimoStatus, ultimoMotivo, ultimoTamanhoReal,
            ultimoFps.ToString("F0"), reconheceu, obs
        });

        capturas.AppendLine(linha);
        totalCapturas++;

        if (textoCapturas != null)
        {
            textoCapturas.text = $"Capturas: {totalCapturas}\n{capturas}";
        }

        Debug.Log($"[Diagnóstico] Captura #{totalCapturas}: {linha}");
    }

    /// <summary>
    /// Copia todas as linhas capturadas (separadas por TAB e quebra de linha)
    /// para a área de transferência. Cole direto na primeira célula vazia da
    /// coluna "Data" na aba "Matriz de Testes" — o Excel/Sheets distribui
    /// cada valor na coluna certa sozinho.
    /// </summary>
    public void CopiarTudo()
    {
        GUIUtility.systemCopyBuffer = capturas.ToString();
        Debug.Log($"[Diagnóstico] {totalCapturas} linha(s) copiada(s) para a área de transferência.");
    }
}
