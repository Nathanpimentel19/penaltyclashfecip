using UnityEngine;

public class GerenciadorDeCamisas : MonoBehaviour
{
    [Header("Configuração do Atleta (Arraste o Jogador aqui)")]
    public SpriteRenderer jogadorSpriteRenderer;

    [Header("Armário de Seleções — 48 gavetas na MESMA ORDEM do array de bandeiras/países do EscolhaDePaisManager")]
    public Sprite[] bonecos = new Sprite[48];

    private Animator jogadorAnimator;

    void Awake()
    {
        BuscarComponentes();
    }

    void BuscarComponentes()
    {
        if (jogadorSpriteRenderer == null)
        {
            GameObject jogadorObj = GameObject.Find("Jogador");
            if (jogadorObj == null) jogadorObj = GameObject.Find("jogador");

            if (jogadorObj != null)
            {
                jogadorSpriteRenderer = jogadorObj.GetComponent<SpriteRenderer>();
                if (jogadorSpriteRenderer == null) jogadorSpriteRenderer = jogadorObj.GetComponentInChildren<SpriteRenderer>();

                jogadorAnimator = jogadorObj.GetComponent<Animator>();
                if (jogadorAnimator == null) jogadorAnimator = jogadorObj.GetComponentInChildren<Animator>();
            }
        }
        else if (jogadorAnimator == null)
        {
            jogadorAnimator = jogadorSpriteRenderer.GetComponent<Animator>();
            if (jogadorAnimator == null && jogadorSpriteRenderer.transform.parent != null)
            {
                jogadorAnimator = jogadorSpriteRenderer.transform.parent.GetComponent<Animator>();
            }
        }
    }

    // Troca o boneco do jogador pelo índice do país (mesmo índice usado na tela EscolhaDePaís).
    // eInimigo == false -> veste o Time 1 (GameData.IndexTime1)
    // eInimigo == true  -> veste o Time 2 / adversário (GameData.IndexTime2)
    public void MudarUniformeDaRounda(int rodada, bool eInimigo)
    {
        BuscarComponentes();

        if (jogadorSpriteRenderer == null || bonecos == null || bonecos.Length == 0) return;

        int indexAlvo = eInimigo ? GameData.IndexTime2 : GameData.IndexTime1;

        if (indexAlvo < 0 || indexAlvo >= bonecos.Length) return;

        Sprite uniformeEscolhido = bonecos[indexAlvo];

        if (uniformeEscolhido != null)
        {
            if (jogadorAnimator != null) jogadorAnimator.enabled = false;

            jogadorSpriteRenderer.sprite = uniformeEscolhido;

            Invoke(nameof(ReativarAnimator), 0.05f);
        }
    }

    void ReativarAnimator()
    {
        if (jogadorAnimator != null) jogadorAnimator.enabled = true;
    }
}