using System;
using System.Collections.Generic;
using UnityEngine;

public class HistoricoEmojis : MonoBehaviour
{
    public GameObject itemHistorico;
    public Transform content;

    private List<string> historico = new List<string>();

    public void AdicionarEmoji(string emoji)
    {
        string dataHora = DateTime.Now.ToString("dd/MM/yyyy - HH:mm");

        string registro = emoji + "  " + dataHora;

        historico.Add(registro);

        CriarItem(registro);
    }

    void CriarItem(string registro)
    {
        GameObject novoItem = Instantiate(itemHistorico, content);

        ItemHistorico item = novoItem.GetComponent<ItemHistorico>();

        item.Configurar(registro);
    }
}