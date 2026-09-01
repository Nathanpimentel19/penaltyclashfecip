using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EscolhaDePaisManager : MonoBehaviour
{
    [Header("Time 1")]
    public Image bandeiraTime1;

    [Header("Time 2")]
    public Image bandeiraTime2;

    [Header("Bandeiras (Arraste os Sprites no Inspector)")]
    public Sprite[] bandeiras;

    [Header("Nomes das 48 Seleções")]
    public string[] nomesPaises = {
        "África do Sul", "Alemanha", "Arábia Saudita", "Argélia", "Argentina",
        "Austrália", "Áustria", "Bélgica", "Bolívia", "Brasil",
        "Camarões", "Canadá", "Chile", "Colômbia", "Coreia do Sul",
        "Costa Rica", "Croácia", "Dinamarca", "Equador", "Escócia",
        "Espanha", "Estados Unidos", "Egito", "França", "Gana",
        "Holanda", "Hungria", "Inglaterra", "Irã", "Irlanda",
        "Itália", "Japão", "Marrocos", "México", "Nigéria",
        "Noruega", "Nova Zelândia", "País de Gales", "Paraguai", "Peru",
        "Polônia", "Portugal", "Rússia", "Sérvia", "Suécia",
        "Suiça", "Uruguai", "Venezuela"
    };

    [Header("Siglas das 48 Seleções")]
    public string[] siglasPaises = {
        "RSA", "GER", "KSA", "ALG", "ARG",
        "AUS", "AUT", "BEL", "BOL", "BRA",
        "CMR", "CAN", "CHI", "COL", "KOR",
        "CRC", "CRO", "DEN", "ECU", "SCO",
        "ESP", "USA", "EGY", "FRA", "GHA",
        "NED", "HUN", "ENG", "IRN", "IRL",
        "ITA", "JPN", "MAR", "MEX", "NGA",
        "NOR", "NZL", "WAL", "PAR", "PER",
        "POL", "POR", "RUS", "SRB", "SWE",
        "SUI", "URU", "VEN"
    };

    private int paisTime1 = 0;
    private int paisTime2 = 0;

    public void ProximoTime1()
    {
        paisTime1++;
        if (paisTime1 >= bandeiras.Length) paisTime1 = 0;
        bandeiraTime1.sprite = bandeiras[paisTime1];
    }

    public void AnteriorTime1()
    {
        paisTime1--;
        if (paisTime1 < 0) paisTime1 = bandeiras.Length - 1;
        bandeiraTime1.sprite = bandeiras[paisTime1];
    }

    public void ProximoTime2()
    {
        paisTime2++;
        if (paisTime2 >= bandeiras.Length) paisTime2 = 0;
        bandeiraTime2.sprite = bandeiras[paisTime2];
    }

    public void AnteriorTime2()
    {
        paisTime2--;
        if (paisTime2 < 0) paisTime2 = bandeiras.Length - 1;
        bandeiraTime2.sprite = bandeiras[paisTime2];
    }

    public void Selecionar()
    {
        // Salva os dados selecionados no GameData antes de mudar de cena
        if (nomesPaises.Length > paisTime1) GameData.PaisSelecionado = nomesPaises[paisTime1];
        if (siglasPaises.Length > paisTime1) GameData.SiglaTime1 = siglasPaises[paisTime1];
        if (bandeiras.Length > paisTime1) GameData.BandeiraTime1 = bandeiras[paisTime1];

        if (siglasPaises.Length > paisTime2) GameData.SiglaTime2 = siglasPaises[paisTime2];
        if (bandeiras.Length > paisTime2) GameData.BandeiraTime2 = bandeiras[paisTime2];

        // Linha de teste para ver a bandeira salva no Console da Unity
        Debug.Log("Bandeira 1 salva: " + GameData.BandeiraTime1 + " | Sigla 1: " + GameData.SiglaTime1);

        // Carrega a sua cena do pênalti
        SceneManager.LoadScene("JogoPenaltis");
    }
}