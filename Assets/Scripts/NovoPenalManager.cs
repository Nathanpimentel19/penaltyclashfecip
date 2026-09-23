using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NovoPenalManager : MonoBehaviour
{
    [Header("Personagens e Objetos")]
    public Transform bola;
    public Transform goleiro;
    public Transform jogador;

    [Header("Link Direto com o Quiz")]
    public QuizManager scriptQuizManager;

    [Header("Gerenciador de Camisas")]
    public GerenciadorDeCamisas gerenciadorDeCamisas;

    [Header("Textos do Placar (UI)")]
    public TextMeshProUGUI textoGolsJogador;
    public TextMeshProUGUI textoGolsMaquina;

    [Header("Texto Exclusivo de Gol (Arraste Aqui)")]
    public GameObject textoGritoDeGol;
    public float velocidadDoLetreiro = 1500f;

    [Header("Avisos de Fase")]
    public GameObject avisoJogador;
    public GameObject avisoGoleiro;

    [Header("Sistema de Confetes (Arraste Aqui)")]
    public ParticleSystem confetesGol;

    [Header("Texto Exclusivo de Defesa (Arraste Aqui)")]
    public GameObject textoVoceDefendeu;
    public float duracaoTextoDefendeu = 1.5f;

    [Header("Áudios")]
    public AudioSource audioSource;
    public AudioSource audioTorcida;

    [Header("Configurações de Velocidade")]
    public float velocidadDaBola = 35f;
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
    private float funcaoQuedaGoleiro = 0f;

    private Animator jogadorAnimator;
    private Animator goleiroAnimator;

    private RectTransform rectLetreiroGol;
    private float xAlvoLetreiro = 0f;
    private bool moverLetreiroHorizontal = false;

    private readonly int hashParado = Animator.StringToHash("Jogador_Parado");
    private readonly int hashChutando = Animator.StringToHash("Jogador_Chutando");
    private readonly WaitForSeconds esperaCurta = new WaitForSeconds(0.05f);
    private readonly WaitForSeconds esperaChute = new WaitForSeconds(0.15f);
    private readonly WaitForSeconds esperaReset = new WaitForSeconds(1.8f);

    void Start()
    {
        if (avisoJogador != null)
        {
            avisoJogador.SetActive(true);
            Invoke(nameof(EsconderAvisoJogador), 2f);
        }
        if (avisoGoleiro != null)
        {
            avisoGoleiro.SetActive(true);
        }
            if (audioTorcida != null) audioTorcida.Play();
        if (bola != null) posicaoInicialBola = bola.position;
        if (goleiro != null)
        {
            posicaoInicialGoleiro = goleiro.position;
            goleiroAnimator = goleiro.GetComponent<Animator>();
        }
        if (jogador != null)
        {
            posicaoInicialJogador = jogador.position;
            jogadorAnimator = MathAnimator(jogador);
        }

        if (textoGritoDeGol != null)
        {
            rectLetreiroGol = textoGritoDeGol.GetComponent<RectTransform>();
            textoGritoDeGol.SetActive(false);
        }

        if (confetesGol != null) confetesGol.Stop();

        if (textoVoceDefendeu != null) textoVoceDefendeu.SetActive(false);

        ResetarPosicoesInstantaneo();
        AtualizarPlacarVisual();

        // Veste o jogador com o uniforme do Time 1 (GameData.SiglaTime1) assim que a cena abre.
        if (gerenciadorDeCamisas != null) gerenciadorDeCamisas.MudarUniformeDaRounda(0, false);
    }

    Animator MathAnimator(Transform target)
    {
        return target.GetComponent<Animator>();
    }

    public void RealizarChute(int cantoClicado)
    {
        if (animacaoAtiva) return;
        animacaoAtiva = true;
        StartCoroutine(FluxoSincronizadoChute(cantoClicado));
    }

    System.Collections.IEnumerator FluxoSincronizadoChute(int cantoClicado)
    {
        if (faseDoJogo == 0)
        {
            destinoJogador = posicaoInicialBola;
            if (jogadorAnimator != null) jogadorAnimator.Play(hashParado);

            while (jogador != null && Vector2.Distance(jogador.position, destinoJogador) > 0.4f)
            {
                yield return null;
            }

            if (jogadorAnimator != null) jogadorAnimator.Play(hashChutando);
            if (audioSource != null) audioSource.Play();
            yield return esperaChute;
        }
        else if (faseDoJogo == 1)
        {
            if (jogador != null) jogador.position = posicaoInicialJogador;
            destinoJogador = posicaoInicialBola;
            if (jogadorAnimator != null) jogadorAnimator.Play(hashParado);

            while (jogador != null && Vector2.Distance(jogador.position, (Vector3)destinoJogador) > 0.4f)
            {
                yield return null;
            }

            if (jogadorAnimator != null) jogadorAnimator.Play(hashChutando);
            if (audioSource != null) audioSource.Play();
            yield return esperaChute;
        }

        bool foiGol = ChutesRealizadosMatematica(cantoClicado);

        if (foiGol)
        {
            if (confetesGol != null) confetesGol.Play();
            if (rectLetreiroGol != null) StartCoroutine(GerenciarLetreiroGol());
        }
        else if (faseDoJogo == 1 && textoVoceDefendeu != null)
        {
            // ADICIONADO: mostra "Você defendeu" quando o jogador está na fase de goleiro
            // e o chute da máquina não virou gol.
            StartCoroutine(GerenciarTextoDefendeu());
        }

        yield return esperaReset;

        ChecarFimDeTurno();
        ResetarPosicoesInstantaneo();
        animacaoAtiva = false;
    }

    System.Collections.IEnumerator GerenciarLetreiroGol()
    {
        rectLetreiroGol.anchoredPosition = new Vector2(1200f, 0f);
        textoGritoDeGol.SetActive(true);
        xAlvoLetreiro = 0f;
        moverLetreiroHorizontal = true;
        yield return new WaitForSeconds(1.2f);
        xAlvoLetreiro = -1200f;
        yield return new WaitForSeconds(0.5f);
        moverLetreiroHorizontal = false;
        textoGritoDeGol.SetActive(false);
    }

    // ADICIONADO: exibe o texto "Você defendeu" por um tempo e depois esconde.
    System.Collections.IEnumerator GerenciarTextoDefendeu()
    {
        textoVoceDefendeu.SetActive(true);
        yield return new WaitForSeconds(duracaoTextoDefendeu);
        textoVoceDefendeu.SetActive(false);
    }

    bool ChutesRealizadosMatematica(int cantoClicado)
    {
        bool marcouGol = false;
        int cantoGoleiroFinal = cantoClicado;

        if (goleiroAnimator != null) goleiroAnimator.enabled = false;

        if (faseDoJogo == 0)
        {
            if (chutesFaseChutador >= totalDeChutesPorFase) return false;
            chutesFaseChutador++;

            destinoBola = ObterCoordenadaDoCanto(cantoClicado);

            int dadoBola = Random.Range(0, 101);
            int dadoGoleiro = Random.Range(0, 51);

            if (dadoBola > dadoGoleiro)
            {
                List<int> cantosErrados = new List<int> { 0, 1, 2, 3, 4 };
                cantosErrados.Remove(cantoClicado);
                cantoGoleiroFinal = cantosErrados[Random.Range(0, cantosErrados.Count)];
                golsDoJogador++;

                GameData.GolsAcertos++;
                GameData.PontuacaoAtual += 1000;
                marcouGol = true;
            }
            else
            {
                cantoGoleiroFinal = cantoClicado;
                GameData.GolsFora++;
                GameData.PontuacaoAtual = Mathf.Max(0, GameData.PontuacaoAtual - 200);
            }

            destinoGoleiro = ObterCoordenadaDoCanto(cantoGoleiroFinal);

            // CORRIGIDO 100%: Alinhado com a fiação 'DaRounda' do script do Santiago!
            if (gerenciadorDeCamisas != null)
            {
                gerenciadorDeCamisas.MudarUniformeDaRounda(chutesFaseChutador, false);
            }
        }
        else if (faseDoJogo == 1)
        {
            if (chutesFaseGoleiro >= totalDeChutesPorFase) return false;
            chutesFaseGoleiro++;

            int cantoChuteMaquina = Random.Range(0, 5);
            destinoBola = ObterCoordenadaDoCanto(cantoChuteMaquina);
            destinoGoleiro = ObterCoordenadaDoCanto(cantoClicado);
            cantoGoleiroFinal = cantoClicado;

            if (cantoChuteMaquina == cantoClicado)
            {
                marcouGol = false;

                // CORRIGIDO: faltava contabilizar a defesa certa aqui, por isso
                // o quiz final mostrava "Defesas acertas" sempre zerado.
                GameData.DefesasAcertas++;
                GameData.PontuacaoAtual += 1000;
            }
            else
            {
                golsDaMaquina++;
                marcouGol = true;

                // CORRIGIDO: faltava contabilizar a defesa errada (gol sofrido) e
                // descontar pontos, igual já era feito na fase de batedor.
                GameData.DefesasErradas++;
                GameData.PontuacaoAtual = Mathf.Max(0, GameData.PontuacaoAtual - 200);
            }

            // CORRIGIDO 100%: Alinhado com a fiação 'DaRounda' do script do Santiago!
            if (gerenciadorDeCamisas != null)
            {
                gerenciadorDeCamisas.MudarUniformeDaRounda(chutesFaseGoleiro, true);
            }
        }

        if (cantoGoleiroFinal == 0 || cantoGoleiroFinal == 2) funcaoQuedaGoleiro = 65f;
        else if (cantoGoleiroFinal == 1 || cantoGoleiroFinal == 3) funcaoQuedaGoleiro = -65f;
        else funcaoQuedaGoleiro = 0f;

        AtualizarPlacarVisual();
        return marcouGol;
    }
    void EsconderAvisoJogador()
    {
        if (avisoJogador != null)
        {
            avisoJogador.SetActive(false);
        }
    }
    void EsconderAvisoGoleiro()
    {
        if (avisoGoleiro != null)
        {
            avisoGoleiro.SetActive(false);
        }
    }
    void Update()
    {
        if (bola != null && (Vector2)bola.position != destinoBola)
        {
            bola.position = Vector3.MoveTowards(bola.position, destinoBola, velocidadDaBola * Time.deltaTime);
        }

        if (goleiro != null)
        {
            if ((Vector2)goleiro.position != destinoGoleiro)
            {
                goleiro.position = Vector3.MoveTowards(goleiro.position, destinoGoleiro, velocidadeDoGoleiro * Time.deltaTime);
            }
            Quaternion rotAtual = goleiro.rotation;
            Quaternion rotDestino = Quaternion.Euler(0, 0, funcaoQuedaGoleiro);
            goleiro.rotation = Quaternion.RotateTowards(rotAtual, rotDestino, 350f * Time.deltaTime);
        }

        if (jogador != null && (Vector2)jogador.position != destinoJogador && animacaoAtiva)
        {
            jogador.position = Vector3.MoveTowards(jogador.position, destinoJogador, velocidadeDoJogador * Time.deltaTime);
        }

        if (moverLetreiroHorizontal && rectLetreiroGol != null)
        {
            Vector2 pos = rectLetreiroGol.anchoredPosition;
            pos.x = Mathf.MoveTowards(pos.x, xAlvoLetreiro, velocidadDoLetreiro * Time.deltaTime);
            rectLetreiroGol.anchoredPosition = pos;
        }
    }

    void ChecarFimDeTurno()
    {
        if (faseDoJogo == 0 && chutesFaseChutador >= totalDeChutesPorFase)
        {
            faseDoJogo = 1;
            Debug.Log("[FASE 2] Agora você é o goleiro!");

            // ADICIONADO: troca o uniforme IMEDIATAMENTE ao entrar na fase de goleiro,
            // sem esperar a primeira defesa terminar de ser processada.
            if (gerenciadorDeCamisas != null)
            {
                gerenciadorDeCamisas.MudarUniformeDaRounda(0, true);
            }
        }
        else if (faseDoJogo == 1 && chutesFaseGoleiro >= totalDeChutesPorFase)
            if (avisoGoleiro != null)
            {
                avisoGoleiro.SetActive(true);
                Invoke(nameof(EsconderAvisoGoleiro), 2f);
            }
        {
            if (scriptQuizManager != null) scriptQuizManager.IniciarQuiz();
        }
    }
    void ResetarPosicoesInstantaneo() { funcaoQuedaGoleiro = 0f; if (goleiroAnimator != null) goleiroAnimator.enabled = true; if (bola != null) { bola.position = posicaoInicialBola; destinoBola = posicaoInicialBola; } if (goleiro != null) { goleiro.position = posicaoInicialGoleiro; destinoGoleiro = posicaoInicialGoleiro; goleiro.rotation = Quaternion.identity; } if (jogador != null) { jogador.position = posicaoInicialJogador; destinoJogador = posicaoInicialJogador; } if (jogadorAnimator != null) jogadorAnimator.Play(hashParado); }
    Vector2 ObterCoordenadaDoCanto(int canto) { if (canto == 0) return superiorEsquerdo; if (canto == 1) return superiorDireito; if (canto == 2) return inferiorEsquerdo; if (canto == 3) return InferiorDireito; return centroDoGol; }
    void AtualizarPlacarVisual() { if (textoGolsJogador != null) textoGolsJogador.text = golsDoJogador.ToString(); if (textoGolsMaquina != null) textoGolsMaquina.text = golsDaMaquina.ToString(); }
}