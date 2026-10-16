using UnityEngine;
using UnityEngine.UI;

public class ItemHistorico : MonoBehaviour
{
    public Text texto;

    public void Configurar(string registro)
    {
        texto.text = registro;
    }
}