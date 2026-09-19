using UnityEngine;
using UnityEngine.SceneManagement;

public class FimPartidaManager : MonoBehaviour
{
    public void JogarNovamente()
    {
        SceneManager.LoadScene("EscolhaDePaís");
    }

    public void Voltar()
    {
        SceneManager.LoadScene("Menu");
    }
}