using UnityEngine;

public static class GameData
{
    public static int PontuacaoAtual = 0;
    public static string PaisSelecionado = "";

    // 👕 A VAGA NOVA DA CAMISA: Guarda o sprite do batedor fatiado escolhido no menu!
    public static Sprite UniformeSelecionado;

    // Dados do Time 1 (Esquerda)
    public static Sprite BandeiraTime1;
    public static string SiglaTime1;

    // Dados do Time 2 (Direita)
    public static Sprite BandeiraTime2;
    public static string SiglaTime2;
}
