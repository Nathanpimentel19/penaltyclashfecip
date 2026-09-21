using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Top10Manager : MonoBehaviour
{
    [Header("Configurações de UI")]
    [SerializeField] private Transform containerLinhasPlacar;
    [SerializeField] private GameObject prefabLinhaPlacar;

    private List<Top10PlayerScoreAuxiliar> localLeaderboard = new List<Top10PlayerScoreAuxiliar>();

    private void Start()
    {
        CarregarEExibirPlacar();
    }

    public void CarregarEExibirPlacar()
    {
        localLeaderboard.Clear();

        if (PlayerPrefs.HasKey("FeiraCienciasLeaderboard"))
        {
            string json = PlayerPrefs.GetString("FeiraCienciasLeaderboard");
            LeaderboardSaveDataAuxiliar data = JsonUtility.FromJson<LeaderboardSaveDataAuxiliar>(json);

            if (data != null && data.scores != null)
            {
                localLeaderboard = data.scores;
            }
        }

        // Limpa as linhas visuais do painel
        foreach (Transform child in containerLinhasPlacar)
        {
            Destroy(child.gameObject);
        }

        // Ordena do maior para o menor placar
        localLeaderboard.Sort((x, y) => y.score.CompareTo(x.score));

        // Renderiza as linhas na tela
        for (int i = 0; i < localLeaderboard.Count; i++)
        {
            if (i >= 10) break;

            GameObject item = Instantiate(prefabLinhaPlacar, containerLinhasPlacar);
            TMP_Text itemText = item.GetComponentInChildren<TMP_Text>();

            if (itemText != null)
            {
                itemText.text = $"#{i + 1}  |  {localLeaderboard[i].playerName.ToUpper()}  |  {localLeaderboard[i].score} pts";
            }
        }
    }

    public void VoltarAoMenuPrincipal()
    {
        SceneManager.LoadScene("Menu");
    }

    [Serializable]
    private class LeaderboardSaveDataAuxiliar
    {
        public List<Top10PlayerScoreAuxiliar> scores; // Sincronizado perfeitamente com o salvamento do Quiz
    }

    [Serializable]
    private class Top10PlayerScoreAuxiliar
    {
        public string playerName;
        public int score;
    }
}
