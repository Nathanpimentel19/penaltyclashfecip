using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class NovoPenalManager : MonoBehaviour
{
    [Header("Personagens e Objetos")]
    public Transform bola;
    public Transform goleiro;
    public Transform jogador;

    [Header("Link Direto com o Quiz")]
    public QuizManager scriptQuizManager;

    [Header("Textos do Placar (UI)")]
    public TextMeshProUGUI textoGolsJogador;
    public TextMeshProUGUI textoGolsMaquina;

    [Header("Configurações das Rodadas")]
    public int totalDeChutesPorFase = 5;
    private int chutesFaseChutador = 0;
    private int chutesFaseGoleiro = 0;

    // Contadores reais de gols
    private int golsDoJogador = 0;
    private int golsDaMaquina = 0;

    // 0 = Jogador Chutando | 1 = Jogador no Gol (Goleiro)
    private int faseDoJogo = 0;

    [Header("Coordenadas do Gol")]
    public Vector2 superiorEsquerdo = new Vector2(-5.71f, 1.78f);
    public Vector2 superiorDireito = new Vector2(5.54f, 1.45f);
    public Vector2 inferiorEsquerdo = new Vector2(-5.21f, -2.73f);
    public Vector2 InferiorDireito = new Vector2(5.71f, -2.51f);
    public Vector2 centroDoGol = new Vector2(0.08f, -0.84f);

    private Vector2 posicaoInicialBola;
    private Vector2 posicaoInicialGoleiro;

    void Start()
    {
        if (bola != null) posicaoInicialBola = bola.position;
        if (goleiro != null) posicaoInicialGoleiro = goleiro.position;

        AtualizarPlacarVisual();
        Debug.Log("[FASE 1 INICIADA] Você é o batedor! Chute no gol.");
    }

    public void RealizarChute(int cantoClicado)
    {
        if (bola != null) bola.position = posicaoInicialBola;
        if (goleiro != null) goleiro.position = posicaoInicialGoleiro;

        // ==========================================
        // MODO 1: JOGADOR CHUTANDO (Fase 0)
        // ==========================================
        if (faseDoJogo == 0)
        {
            if (chutesFaseChutador >= totalDeChutesPorFase) return;
            chutesFaseChutador++;

            MoverObjetoParaCanto(bola, cantoClicado);

            int dadoBola = Random.Range(0, 101);
            int dadoGoleiro = Random.Range(0, 51);
            int cantoGoleiro;

            if (dadoBola > dadoGoleiro)
            {
                List<int> cantosErrados = new List<int> { 0, 1, 2, 3, 4 };
                cantosErrados.Remove(cantoClicado);
                int indiceSorteado = Random.Range(0, cantosErrados.Count);
                cantoGoleiro = cantosErrados[indiceSorteado];

                golsDoJogador++; // SÓ SOMA GOL AQUI!
                GameData.PontuacaoAtual += 100;
            }
            else
            {
                cantoGoleiro = cantoClicado;
                // CORRIGIDO: Se a máquina defendeu, o placar dela NÃO sobe mais! Continua igual.
            }

            MoverObjetoParaCanto(goleiro, cantoGoleiro);
            AtualizarPlacarVisual();

            if (chutesFaseChutador >= totalDeChutesPorFase)
            {
                Invoke("TrocarParaModoLoverGoleiro", 2f);
            }
        }
        // ==========================================
        // MODO 2: JOGADOR NO GOL / GOLEIRO (Fase 1)
        // ==========================================
        else if (faseDoJogo == 1)
        {
            if (chutesFaseGoleiro >= totalDeChutesPorFase) return;
            chutesFaseGoleiro++;

            int cantoChuteMaquina = Random.Range(0, 5);
            MoverObjetoParaCanto(bola, cantoChuteMaquina);
            MoverObjetoParaCanto(goleiro, cantoClicado);

            int dadoMaquina = Random.Range(0, 101);
            int dadoGoleiroJogador = Random.Range(0, 51);

            if (cantoChuteMaquina == cantoClicado && dadoGoleiroJogador >= 25)
            {
                // CORRIGIDO: Se você defendeu, ganhou +100 pontos globais, mas o placar de Gols não muda!
                GameData.PontuacaoAtual += 100;
            }
            else
            {
                golsDaMaquina++; // GOL DA MÁQUINA! Placar da França sobe.
            }

            AtualizarPlacarVisual();

            if (chutesFaseGoleiro >= totalDeChutesPorFase)
            {
                Invoke("ChamarOQuizDefinitivo", 2f);
            }
        }
    }

    void TrocarParaModoLoverGoleiro()
    {
        faseDoJogo = 1;
        if (bola != null) bola.position = posicaoInicialBola;
        if (goleiro != null) goleiro.position = posicaoInicialGoleiro;
        Debug.Log("[FASE 2 INICIADA] Mudou de turno! Agora você é o goleiro.");
    }

    void Update()
    {
        // Garante que o placar continue atualizado na tela
        System.Action placarAction = AtualizarPlacarVisual;
        placarAction.Invoke();
    }

    void AtualizarPlacarVisual()
    {
        if (textoGolsJogador != null)
        {
            textoGolsJogador.text = golsDoJogador.ToString();
        }

        if (textoGolsMaquina != null)
        {
            textoGolsMaquina.text = golsDaMaquina.ToString();
        }
    }

    void MoverObjetoParaCanto(Transform objeto, int canto)
    {
        if (objeto == null) return;
        if (canto == 0) objeto.position = superiorEsquerdo;
        else if (canto == 1) objeto.position = superiorDireito;
        else if (canto == 2) objeto.position = inferiorEsquerdo;
        else if (canto == 3) objeto.position = InferiorDireito;
        else if (canto == 4) objeto.position = centroDoGol;
    }

    void ChamarOQuizDefinitivo()
    {
        if (scriptQuizManager != null)
        {
            scriptQuizManager.IniciarQuiz();
        }
    }
}
