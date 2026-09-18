using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EscolhaNomeManager : MonoBehaviour
{
    public TMP_InputField campoNome;

    public void Jogar()
    {
        string nome = campoNome.text;

        PlayerPrefs.SetString("NomeJogador", nome);
        PlayerPrefs.Save();

        SceneManager.LoadScene("EscolhaDePaís");
    }

    public void Voltar()
    {
        SceneManager.LoadScene("Menu");
    }
}