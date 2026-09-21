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

        // Impede que o campo fique em branco se o jogador apenas apertar espaço
        if (string.IsNullOrEmpty(nome)) nome = "Jogador";

        PlayerPrefs.SetString("NomeJogador", nome);

        // BUSCA O HISTÓRICO: Puxa os pontos acumulados se o nome já existir no placar
        int pontosAntigos = BuscarPontosDoJogadorNoPlacar(nome);
        GameData.PontuacaoAtual = pontosAntigos;

        // Limpa as estatísticas de chutes/defesas para a nova rodada começar zerada
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

            // Localiza a ficha do jogador ignorando maiúsculas e minúsculas
            PlayerScoreAuxiliar jogadorExistente = data.scores.Find(p => p.playerName.Equals(nome, System.StringComparison.OrdinalIgnoreCase));

            if (jogadorExistente != null)
            {
                return jogadorExistente.score;
            }
        }
        return 0; // Se for um jogador inédito, começa com 0 pontos
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

    [System.Serializable]
    private class PlayerScoreAuxiliar
    {
        public string playerName;
        public int score;
    }
}
