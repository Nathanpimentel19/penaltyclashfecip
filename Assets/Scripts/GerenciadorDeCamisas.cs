using UnityEngine;
using System.Collections.Generic;

public class GerenciadorDeCamisas : MonoBehaviour
{
    [System.Serializable]
    public class CadastroSelecao
    {
        public string nomeSelecao;
        public Sprite desenhoDesseUniforme;
    }

    [Header("Configuração do Atleta (Arraste o objeto real do jogador aqui)")]
    public SpriteRenderer jogadorSpriteRenderer;

    [Header("Armário de Seleções (Clique no '+' para criar as caixinhas!)")]
    public List<CadastroSelecao> armarioDeCamisas = new List<CadastroSelecao>();

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
                jogadorSpriteRenderer = jogadorObj.GetComponentInChildren<SpriteRenderer>();
                jogadorAnimator = jogadorObj.GetComponentInChildren<Animator>();
            }
        }
        else if (jogadorAnimator == null)
        {
            jogadorAnimator = jogadorSpriteRenderer.GetComponent<Animator>();
        }
    }

    public void MudarUniformeDaRodada(int rodada)
    {
        BuscarComponentes();

        if (jogadorSpriteRenderer == null || armarioDeCamisas.Count == 0) return;

        string paisSelecionado = GameData.PaisSelecionado;

        Sprite uniformeEscolhido = null;

        foreach (var camisa in armarioDeCamisas)
        {
            if (camisa.nomeSelecao.Trim().ToLower() == paisSelecionado.Trim().ToLower())
            {
                uniformeEscolhido = camisa.desenhoDesseUniforme;
                break;
            }
        }

        if (uniformeEscolhido == null && armarioDeCamisas.Count > 0)
        {
            uniformeEscolhido = armarioDeCamisas[0].desenhoDesseUniforme;
        }

        if (uniformeEscolhido != null)
        {
            if (jogadorAnimator != null) jogadorAnimator.enabled = false;

            jogadorSpriteRenderer.sprite = uniformeEscolhido;

            // 🚨 O ADICIONAL DO GIGANTE: Força o boneco e o esqueleto dele a ficarem na escala 4.5x na mesma hora!
            jogadorSpriteRenderer.transform.localScale = new Vector3(4.5f, 4.5f, 1f);
            if (jogadorSpriteRenderer.transform.parent != null)
            {
                jogadorSpriteRenderer.transform.parent.localScale = new Vector3(4.5f, 4.5f, 1f);
            }

            Debug.Log($"[SISTEMA INTERLIGADO] Uniforme trocado e tamanho 4.5x travado de fábrica para: {paisSelecionado}");

            Invoke(nameof(ReativarAnimator), 0.05f);
        }
    }

    void ReativarAnimator()
    {
        if (jogadorAnimator != null) jogadorAnimator.enabled = true;
    }
}
