using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditosManager : MonoBehaviour
{
    public void Voltar()
    {
        SceneManager.LoadScene("Menu");
    }
}