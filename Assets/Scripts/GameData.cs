using UnityEngine;

public static class GameData
{
    public static int PontuacaoAtual = 0;
    public static string PaisSelecionado = "";

    // Dados do Time 1 (Esquerda)
    public static Sprite BandeiraTime1;
    public static string SiglaTime1;

    // Dados do Time 2 (Direita)
    public static Sprite BandeiraTime2;
    public static string SiglaTime2;

    // Estatísticas dos Pênaltis para o Quiz ler depois
    public static int GolsAcertos = 0;
    public static int GolsFora = 0;
    public static int DefesasAcertas = 0;
    public static int DefesasErradas = 0;
}
