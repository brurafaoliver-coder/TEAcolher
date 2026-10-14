using System;
using System.Collections.Generic;

[Serializable]
public class RegistroEmocao
{
    public string dataHora; // ex: "08/10/2026 14:30"
    public int indice;      // 0 a 4
}

[Serializable]
public class ListaRegistros
{
    public List<RegistroEmocao> registros = new List<RegistroEmocao>();
}