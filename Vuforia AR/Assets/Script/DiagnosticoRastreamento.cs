using UnityEngine;
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
/// </summary>
public class DiagnosticoRastreamento : MonoBehaviour
{
    [Header("Referências")]
    public ObserverBehaviour alvo;          // o Image Target (etiqueta)
    public TMP_Text textoDiagnostico;       // texto na tela
    public Camera cameraAR;                 // se vazio, usa Camera.main

    private TargetStatus statusAtual;
    private float fpsSuavizado;

    void Start()
    {
        if (cameraAR == null) cameraAR = Camera.main;

        if (alvo != null)
        {
            alvo.OnTargetStatusChanged += AoMudarStatus;
            statusAtual = alvo.TargetStatus;
        }
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
    }
}