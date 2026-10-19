using UnityEngine;

public class ControleTelas : MonoBehaviour
{
    [Header("Telas")]
    public GameObject telaPrincipal;
    public GameObject telaHistorico;

    [Header("Botões específicos")]
    public GameObject botaoHistorico;

    [Header("Realidade Aumentada")]
    public GameObject menuCamera;     // câmera comum do menu
    public GameObject arCamera;       // AR Camera do Vuforia
    public GameObject imageTarget;    // Image Target (com seu projeto como filho)
    public GameObject botaoVoltarAR;  // botão Voltar da tela de AR
    public GameObject botaoAbrirAR;   // botão que abre a AR (se estiver fora da telaPrincipal)

    void Start()
    {
        telaPrincipal.SetActive(true);
        telaHistorico.SetActive(false);
        botaoHistorico.SetActive(true);

        // AR começa desligada
        arCamera.SetActive(false);
        imageTarget.SetActive(false);
        botaoVoltarAR.SetActive(false);
        menuCamera.SetActive(true);
        if (botaoAbrirAR != null) botaoAbrirAR.SetActive(true);
    }

    // ---------- HISTÓRICO (igual ao seu) ----------
    public void AbrirHistorico()
    {
        telaPrincipal.SetActive(false);
        telaHistorico.SetActive(true);
        botaoHistorico.SetActive(false);
    }

    public void Voltar()
    {
        telaHistorico.SetActive(false);
        telaPrincipal.SetActive(true);
        botaoHistorico.SetActive(true);
    }

    // ---------- REALIDADE AUMENTADA ----------
    public void AbrirAR()
    {
        telaPrincipal.SetActive(false);
        botaoHistorico.SetActive(false);
        if (botaoAbrirAR != null) botaoAbrirAR.SetActive(false);
        menuCamera.SetActive(false);

        arCamera.SetActive(true);
        imageTarget.SetActive(true);
        botaoVoltarAR.SetActive(true);
    }

    public void VoltarDaAR()
    {
        botaoVoltarAR.SetActive(false);
        imageTarget.SetActive(false);
        arCamera.SetActive(false);

        menuCamera.SetActive(true);
        telaPrincipal.SetActive(true);
        botaoHistorico.SetActive(true);
        if (botaoAbrirAR != null) botaoAbrirAR.SetActive(true);
    }
}