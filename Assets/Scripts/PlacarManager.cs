using UnityEngine;
using UnityEngine.UI;
using TMPro; // Para mexer nos textos

public class placarmanager : MonoBehaviour
{
    [Header("Elementos do Time da Esquerda")]
    public Image bandeiraEsquerda;
    public TextMeshProUGUI siglaEsquerda;

    [Header("Elementos do Time da Direita")]
    public Image bandeiraDireita;
    public TextMeshProUGUI siglaDireita;

    [Header("Textos Fixos dos Gols")]
    public TextMeshProUGUI golEsquerdaText;
    public TextMeshProUGUI golDireitaText;

    void Start()
    {
        // 1. Zera os gols
        if (golEsquerdaText != null) golEsquerdaText.text = "0";
        if (golDireitaText != null) golDireitaText.text = "0";

        // 2. Puxa as bandeiras e siglas salvas na tela de seleção (GameData)
        if (bandeiraEsquerda != null) bandeiraEsquerda.sprite = GameData.BandeiraTime1;
        if (siglaEsquerda != null) siglaEsquerda.text = GameData.SiglaTime1;

        if (bandeiraDireita != null) bandeiraDireita.sprite = GameData.BandeiraTime2;
        if (siglaDireita != null) siglaDireita.text = GameData.SiglaTime2;
    }

    // Esta é a função que você já tinha para mudar no meio do jogo, se precisar
    public void SelecionarTimes(string nomeTime1, Sprite fotoBandeira1, string nomeTime2, Sprite fotoBandeira2)
    {
        siglaEsquerda.text = nomeTime1;
        bandeiraEsquerda.sprite = fotoBandeira1;

        siglaDireita.text = nomeTime2;
        bandeiraDireita.sprite = fotoBandeira2;
    }
}