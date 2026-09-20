using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class EscolhaNomeManager : MonoBehaviour
{
    public TMP_InputField campoNome;

    public void Jogar()
    {
        string nome = campoNome.text.Trim();

        // Garante que não fique vazio se o jogador der apenas espaços
        if (string.IsNullOrEmpty(nome)) nome = "Jogador";

        PlayerPrefs.SetString("NomeJogador", nome);

        // BUSCA O HISTÓRICO: Puxa os pontos acumulados se o nome já existir
        int pontosAntigos = BuscarPontosDoJogadorNoPlacar(nome);
        GameData.PontuacaoAtual = pontosAntigos;

        // Limpa as estatísticas de chutes/defesas para começar a nova rodada do zero
        GameData.GolsAcertos = 0;
        GameData.GolsFora = 0;
        GameData.DefesasAcertas = 0;
        GameData.DefesasErradas = 0;

        PlayerPrefs.Save();
        SceneManager.LoadScene("EscolhaDePaís");
    }

    private int BuscarPontosDoJogadorNoPlacar(string nome)
    {
        if (PlayerPrefs.HasKey("FeiraCienciasLeaderboard"))
        {
            string json = PlayerPrefs.GetString("FeiraCienciasLeaderboard");
            LeaderboardSaveData data = JsonUtility.FromJson<LeaderboardSaveData>(json);

            // Localiza a ficha do jogador (ignorando maiúsculas e minúsculas)
            PlayerScoreAuxiliar jogadorExistente = data.scores.Find(p => p.playerName.Equals(nome, System.StringComparison.OrdinalIgnoreCase));

            if (jogadorExistente != null)
            {
                return jogadorExistente.score;
            }
        }
        return 0; // Se o nome for inédito, começa com 0 pontos
    }

    public void Voltar()
    {
        SceneManager.LoadScene("Menu");
    }

    [System.Serializable]
    private class LeaderboardSaveData
    {
        public List<PlayerScoreAuxiliar> scores;
    }

    // Classe espelho local para evitar que a Unity dê o erro CS0246 de falta de referência
    [System.Serializable]
    private class PlayerScoreAuxiliar
    {
        public string playerName;
        public int score;
    }
}
