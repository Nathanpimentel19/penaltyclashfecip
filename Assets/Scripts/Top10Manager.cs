using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Top10Manager : MonoBehaviour
{
    [Header("Configurações de UI")]
    [SerializeField] private Transform containerLinhasPlacar; // Objeto pai (ex: Content do ScrollView)
    [SerializeField] private GameObject prefabLinhaPlacar;     // Prefab do texto com TextMeshPro

    private List<Top10PlayerScore> localLeaderboard = new List<Top10PlayerScore>();

    private void Start()
    {
        // Apenas carrega e exibe o histórico existente de quem já jogou
        CarregarEExibirPlacar();
    }

    public void CarregarEExibirPlacar()
    {
        if (PlayerPrefs.HasKey("FeiraCienciasLeaderboard"))
        {
            string json = PlayerPrefs.GetString("FeiraCienciasLeaderboard");
            LeaderboardSaveData data = JsonUtility.FromJson<LeaderboardSaveData>(json);
            localLeaderboard = data.scores;
        }

        // Limpa a tela antes de desenhar para não duplicar nada por segurança
        foreach (Transform child in containerLinhasPlacar)
        {
            Destroy(child.gameObject);
        }

        // Organiza a lista do maior para o menor
        localLeaderboard.Sort((x, y) => y.score.CompareTo(x.score));

        // Desenha apenas os 10 melhores no painel visual
        for (int i = 0; i < localLeaderboard.Count; i++)
        {
            if (i >= 10) break; // Trava para exibir estritamente até o Top 10

            GameObject item = Instantiate(prefabLinhaPlacar, containerLinhasPlacar);
            TMP_Text itemText = item.GetComponentInChildren<TMP_Text>();

            if (itemText != null)
            {
                // Formatação visual padronizada do ranking: #1  |  NOME SOBRENOME  |  7500 pts
                itemText.text = $"#{i + 1}  |  {localLeaderboard[i].playerName.ToUpper()}  |  {localLeaderboard[i].score} pts";
            }
        }
    }

    // Função para você vincular ao botão de "VOLTAR" na sua cena de Placar
    public void VoltarAoMenuPrincipal()
    {
        SceneManager.LoadScene("Menu");
    }

    [Serializable]
    private class LeaderboardSaveData
    {
        public List<Top10PlayerScore> scores;
    }

    // Estrutura renomeada exclusivamente para este script para evitar o erro CS0101
    [Serializable]
    private class Top10PlayerScore
    {
        public string playerName;
        public int score;
    }
}