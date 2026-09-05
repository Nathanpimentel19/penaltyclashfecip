using UnityEngine;
using System.Collections.Generic;

public class NovoPenalManager : MonoBehaviour
{
    [Header("Personagens e Objetos")]
    public Transform bola;
    public Transform goleiro;
    public Transform jogador;

    [Header("Link Direto com o Quiz (Arraste Aqui)")]
    public QuizManager scriptQuizManager; // NOVO: Link físico direto para evitar falhas!

    [Header("Configurações das Rodadas")]
    public int totalDeChutesPorFase = 5;
    private int chutesFaseChutador = 0;
    private int chutesFaseGoleiro = 0;

    int golsDoJogador = 0;
    int defesasDoJogador = 0;
    int golsDaMaquina = 0;

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

        Debug.Log("[FASE 1 INICIADA] Você é o batedor! Chute no gol.");
    }

    public void RealizarChute(int cantoClicado)
    {
        if (bola != null) bola.position = posicaoInicialBola;
        if (goleiro != null) goleiro.position = posicaoInicialGoleiro;

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

                golsDoJogador++;
                GameData.PontuacaoAtual += 100;
            }
            else
            {
                cantoGoleiro = cantoClicado;
            }

            MoverObjetoParaCanto(goleiro, cantoGoleiro);

            if (chutesFaseChutador >= totalDeChutesPorFase)
            {
                Invoke("TrocarParaModoLoverGoleiro", 2f);
            }
        }
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
                defesasDoJogador++;
                GameData.PontuacaoAtual += 100;
            }
            else
            {
                golsDaMaquina++;
            }

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
        Debug.Log("Fim absoluto das rodadas. Abrindo painel do Quiz...");

        // CORRIGIDO: Agora ele usa o link direto arrastado, sem chance de errar!
        if (scriptQuizManager != null)
        {
            scriptQuizManager.IniciarQuiz();
        }
        else
        {
            Debug.LogError("Erro Grave: Você esqueceu de arrastar o QuizManager para o script do GerenciadorDoJogo no Inspector!");
        }
    }
}
