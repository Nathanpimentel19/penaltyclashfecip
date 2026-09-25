using UnityEngine;
using UnityEngine.UI;

public class BotaoMutar : MonoBehaviour
{
    [Header("Ícones do Botão (opcional)")]
    [Tooltip("Arraste o sprite de 'som ligado' aqui")]
    public Sprite iconeSomLigado;
    [Tooltip("Arraste o sprite de 'som desligado' aqui")]
    public Sprite iconeSomDesligado;

    [Tooltip("A Image do próprio botão, que vai trocar de ícone. Se deixar vazio, o script pega automaticamente.")]
    public Image imagemDoBotao;

    private bool estaMutado = false;
    private const string CHAVE_SALVA = "SomMutado";

    void Awake()
    {
        if (imagemDoBotao == null)
        {
            imagemDoBotao = GetComponent<Image>();
        }
    }

    void Start()
    {
        // Lembra a preferência do jogador entre uma cena e outra / ao reabrir o jogo
        estaMutado = PlayerPrefs.GetInt(CHAVE_SALVA, 0) == 1;
        AplicarEstadoDoSom();
    }

    // Chame este método no OnClick() do botão, no Inspector
    public void AlternarSom()
    {
        estaMutado = !estaMutado;
        AplicarEstadoDoSom();

        PlayerPrefs.SetInt(CHAVE_SALVA, estaMutado ? 1 : 0);
        PlayerPrefs.Save();
    }

    void AplicarEstadoDoSom()
    {
        // Controla o volume de TODOS os sons do jogo de uma vez (chute, torcida, etc.)
        AudioListener.volume = estaMutado ? 0f : 1f;

        if (imagemDoBotao != null)
        {
            if (estaMutado && iconeSomDesligado != null)
            {
                imagemDoBotao.sprite = iconeSomDesligado;
            }
            else if (!estaMutado && iconeSomLigado != null)
            {
                imagemDoBotao.sprite = iconeSomLigado;
            }
        }
    }
}