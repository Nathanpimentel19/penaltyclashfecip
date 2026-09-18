using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void Jogar()
    {
        SceneManager.LoadScene("EscolhaNome");
    }

    public void Sair()
    {
        Application.Quit();
    }

    public void Creditos()
    {
        SceneManager.LoadScene("Creditos");
    }
    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
}