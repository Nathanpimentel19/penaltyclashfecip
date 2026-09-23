using UnityEngine;

public static class GameData
{
    public static int IndexTime1;
    public static int IndexTime2;
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

    // =========================================================================
    // NOVO: Histórico Acumulado Local (Persistência para o Ranking Local)
    // =========================================================================
    public static int TotalHistoricoPontos = 0;
    public static int TotalHistoricoGolsAcertos = 0;
    public static int TotalHistoricoGolsFora = 0;
    public static int TotalHistoricoDefesasAcertas = 0;
    public static int TotalHistoricoDefesasErradas = 0;

    /// <summary>
    /// Reseta apenas os dados voláteis da partida atual para começar uma nova rodada do zero.
    /// </summary>
    /// <summary>
    /// Reseta apenas os dados de pontuação e eventos da rodada atual, 
    /// preservando a escolha de países, bandeiras e siglas dos times!
    /// </summary>
    public static void ResetarPartidaAtual()
    {
        // Reseta apenas o placar e pontos voláteis
        PontuacaoAtual = 0;
        GolsAcertos = 0;
        GolsFora = 0;
        DefesasAcertas = 0;
        DefesasErradas = 0;

        // ATENÇÃO: As variáveis abaixo NÃO foram adicionadas aqui para serem zeradas!
        // PaisSelecionado, BandeiraTime1, SiglaTime1, BandeiraTime2, SiglaTime2 
        // continuam salvas perfeitamente para a próxima partida!
    }



    /// <summary>
    /// Carrega do computador o histórico local salvo para o jogador atual.
    /// </summary>
    public static void CarregarHistoricoLocal(string nomeJogador)
    {
        string prefixo = nomeJogador.Trim().ToLower();

        TotalHistoricoGolsAcertos = PlayerPrefs.GetInt(prefixo + "_HistGolsAcertos", 0);
        TotalHistoricoGolsFora = PlayerPrefs.GetInt(prefixo + "_HistGolsFora", 0);
        TotalHistoricoDefesasAcertas = PlayerPrefs.GetInt(prefixo + "_HistDefesasAcertas", 0);
        TotalHistoricoDefesasErradas = PlayerPrefs.GetInt(prefixo + "_HistDefesasErradas", 0);
    }

    /// <summary>
    /// Grava permanentemente no registro do computador o acumulado do histórico local.
    /// </summary>
    public static void SalvarHistoricoLocal(string nomeJogador)
    {
        string prefixo = nomeJogador.Trim().ToLower();

        PlayerPrefs.SetInt(prefixo + "_HistGolsAcertos", TotalHistoricoGolsAcertos);
        PlayerPrefs.SetInt(prefixo + "_HistGolsFora", TotalHistoricoGolsFora);
        PlayerPrefs.SetInt(prefixo + "_HistDefesasAcertas", TotalHistoricoDefesasAcertas);
        PlayerPrefs.SetInt(prefixo + "_HistDefesasErradas", TotalHistoricoDefesasErradas);
        PlayerPrefs.Save();
    }
}
