using UnityEngine;

public class ControleTelas : MonoBehaviour
{
    public GameObject telaPrincipal;
    public GameObject telaHistorico;

    public void AbrirHistorico()
    {
        telaPrincipal.SetActive(false);
        telaHistorico.SetActive(true);
    }

    public void Voltar()
    {
        telaHistorico.SetActive(false);
        telaPrincipal.SetActive(true);
    }
}