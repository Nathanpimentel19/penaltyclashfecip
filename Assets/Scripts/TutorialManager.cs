using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public Image imagemTutorial;
    public Sprite[] imagens;

    private int imagemAtual = 0;

    public void Proximo()
    {
        imagemAtual++;

        if (imagemAtual >= imagens.Length)
        {
            SceneManager.LoadScene("Menu");
            return;
        }

        imagemTutorial.sprite = imagens[imagemAtual];
    }
}