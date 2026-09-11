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

    [Header("Configurações de Velocidade")]
    public float velocidadeDaBola = 35f;
    public float velocidadeDoGoleiro = 20f;
    public float velocidadeDoJogador = 14f;

    [Header("Configurações das Rodadas")]
    public int totalDeChutesPorFase = 5;
    private int chutesFaseChutador = 0;
    private int chutesFaseGoleiro = 0;

    private int golsDoJogador = 0;
    private int golsDaMaquina = 0;
    private int faseDoJogo = 0;

    [Header("Coordenadas do Gol")]
    public Vector2 superiorEsquerdo = new Vector2(-5.71f, 1.78f);
    public Vector2 superiorDireito = new Vector2(5.54f, 1.45f);
    public Vector2 inferiorEsquerdo = new Vector2(-5.21f, -2.73f);
    public Vector2 InferiorDireito = new Vector2(5.71f, -2.51f);
    public Vector2 centroDoGol = new Vector2(0.08f, -0.84f);

    private Vector2 posicaoInicialBola;
    private Vector2 posicaoInicialGoleiro;
    private Vector2 posicaoInicialJogador;

    private Vector2 destinoBola;
    private Vector2 destinoGoleiro;
    private Vector2 destinoJogador;

    private bool animacaoAtiva = false;
    private float rotationalAlvoJogador = 0f; // Nome unificado e corrigido

    void Start()
    {
        if (bola != null) posicaoInicialBola = bola.position;
        if (goleiro != null) posicaoInicialGoleiro = goleiro.position;
        if (jogador != null) posicaoInicialJogador = jogador.position;

        ResetarPosicoesInstantaneo();
        AtualizarPlacarVisual();
    }

    public void RealizarChute(int cantoClicado)
    {
        if (animacaoAtiva) return;
        StartCoroutine(FluxoSincronizadoChute(cantoClicado));
    }

    System.Collections.IEnumerator FluxoSincronizadoChute(int cantoClicado)
    {
        animacaoAtiva = true;

        // ----------------------------------------------------
        // PASSO 1: O JOGADOR CORRE ATÉ A BOLA
        // ----------------------------------------------------
        if (faseDoJogo == 0)
        {
            destinoJogador = posicaoInicialBola;

            while (jogador != null && Vector2.Distance(jogador.position, destinoJogador) > 0.4f)
            {
                yield return null;
            }

            // ANIMAÇÃO DA PERNA: Ajusta a rotação para chicotear o corpo para frente no impacto!
            rotationalAlvoJogador = -35f;
            yield return new WaitForSeconds(0.1f);
        }

        // ----------------------------------------------------
        // PASSO 2: IMPACTO IMEDIATO (BOLA E GOLEIRO REAGEM)
        // ----------------------------------------------------
        chutesRealizadosMatematica(cantoClicado);

        // Espera o tempo do chute terminar
        yield return new WaitForSeconds(1.5f);

        // ----------------------------------------------------
        // PASSO 3: RESET VISUAL DA PARTIDA E FIM DE TURNO
        // ----------------------------------------------------
        ChecarFimDeTurno();
        ResetarPosicoesInstantaneo();
        animacaoAtiva = false;
    }

    void chutesRealizadosMatematica(int cantoClicado)
    {
        if (faseDoJogo == 0)
        {
            if (chutesFaseChutador >= totalDeChutesPorFase) return;
            chutesFaseChutador++;

            destinoBola = ObterCoordenadaDoCanto(cantoClicado);

            int dadoBola = Random.Range(0, 101);
            int dadoGoleiro = Random.Range(0, 51);
            int cantoGoleiro;

            if (dadoBola > dadoGoleiro)
            {
                List<int> cantosErrados = new List<int> { 0, 1, 2, 3, 4 };
                cantosErrados.Remove(cantoClicado);
                cantoGoleiro = cantosErrados[Random.Range(0, cantosErrados.Count)];
                golsDoJogador++;
                GameData.PontuacaoAtual += 100;
            }
            else
            {
                cantoGoleiro = cantoClicado;
            }

            destinoGoleiro = ObterCoordenadaDoCanto(cantoGoleiro);
        }
        else if (faseDoJogo == 1)
        {
            if (chutesFaseGoleiro >= totalDeChutesPorFase) return;
            chutesFaseGoleiro++;

            int cantoChuteMaquina = Random.Range(0, 5);
            destinoBola = ObterCoordenadaDoCanto(cantoChuteMaquina);
            destinoGoleiro = ObterCoordenadaDoCanto(cantoClicado);

            int dadoMaquina = Random.Range(0, 101);
            int dadoGoleiroJogador = Random.Range(0, 51);

            if (cantoChuteMaquina == cantoClicado && dadoGoleiroJogador >= 25)
            {
                GameData.PontuacaoAtual += 100;
            }
            else
            {
                golsDaMaquina++;
            }
        }

        AtualizarPlacarVisual();
    }

    void Update()
    {
        if (faseDoJogo == 0 && jogador != null)
        {
            if ((Vector2)jogador.position != destinoJogador)
            {
                jogador.position = Vector3.MoveTowards(jogador.position, destinoJogador, velocidadeDoJogador * Time.deltaTime);
            }

            // CORRIGIDO: Rotação unificada e sem erros de C#
            Quaternion rotationAtual = jogador.rotation;
            Quaternion rotationDestino = Quaternion.Euler(0, 0, rotationalAlvoJogador);
            jogador.rotation = Quaternion.RotateTowards(rotationAtual, rotationDestino, 300f * Time.deltaTime);
        }

        if (bola != null && (Vector2)bola.position != destinoBola)
        {
            bola.position = Vector3.MoveTowards(bola.position, destinoBola, velocidadeDaBola * Time.deltaTime);
            bola.Rotate(Vector3.forward * -600f * Time.deltaTime);
        }

        if (goleiro != null && (Vector2)goleiro.position != destinoGoleiro)
        {
            goleiro.position = Vector3.MoveTowards(goleiro.position, destinoGoleiro, velocidadeDoGoleiro * Time.deltaTime);
        }
    }

    void ChecarFimDeTurno()
    {
        if (faseDoJogo == 0 && chutesFaseChutador >= totalDeChutesPorFase)
        {
            faseDoJogo = 1;
            Debug.Log("[FASE 2 INICIADA] Agora você é o goleiro!");
        }
        else if (faseDoJogo == 1 && chutesFaseGoleiro >= totalDeChutesPorFase)
        {
            if (scriptQuizManager != null) scriptQuizManager.IniciarQuiz();
        }
    }

    void ResetarPosicoesInstantaneo()
    {
        rotationalAlvoJogador = 0f;
        if (bola != null) { bola.position = posicaoInicialBola; destinoBola = posicaoInicialBola; bola.rotation = Quaternion.identity; }
        if (goleiro != null) { goleiro.position = posicaoInicialGoleiro; destinoGoleiro = posicaoInicialGoleiro; }
        if (jogador != null) { jogador.position = posicaoInicialJogador; destinoJogador = posicaoInicialJogador; jogador.rotation = Quaternion.identity; }
    }

    Vector2 ObterCoordenadaDoCanto(int canto)
    {
        if (canto == 0) return superiorEsquerdo;
        if (canto == 1) return superiorDireito;
        if (canto == 2) return inferiorEsquerdo;
        if (canto == 3) return InferiorDireito;
        return centroDoGol;
    }

    void UpdateTextoDePontos() { AtualizarPlacarVisual(); }
    void SystemAction() { AtualizarPlacarVisual(); }
    void AtualizarPlacarVisual()
    {
        if (textoGolsJogador != null) textoGolsJogador.text = golsDoJogador.ToString();
        if (textoGolsMaquina != null) textoGolsMaquina.text = golsDaMaquina.ToString();
    }
}
