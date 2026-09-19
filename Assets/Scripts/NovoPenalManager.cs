using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

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

    [Header("Texto Exclusivo de Gol (Arraste Aqui)")]
    public GameObject textoGritoDeGol;
    public float velocidadeDoLetreiro = 1500f;

    [Header("NOVO: Sistema de Confetes (Arraste Aqui)")]
    public ParticleSystem confetesGol;

    [Header("Áudios")]
    public AudioSource audioSource;
    public AudioSource audioTorcida;

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
    private float funcaoQuedaGoleiro = 0f;

    private Animator jogadorAnimator;

    private RectTransform rectLetreiroGol;
    private float xAlvoLetreiro = 0f;
    private bool moverLetreiroHorizontal = false;

    void Start()
    {
        if (audioTorcida != null)
        {
            audioTorcida.Play();
        }
        if (bola != null) posicaoInicialBola = bola.position;
        if (goleiro != null) posicaoInicialGoleiro = goleiro.position;
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

        // Garante que os confetes comecem parados
        if (confetesGol != null) confetesGol.Stop();

        ResetarPosicoesInstantaneo();
        AtualizarPlacarVisual();
    }

    Animator MathAnimator(Transform target)
    {
        return target.GetComponent<Animator>();
    }

    public void RealizarChute(int cantoClicado)
    {
        if (animacaoAtiva) return;
        StartCoroutine(FluxoSincronizadoChute(cantoClicado));
    }

    System.Collections.IEnumerator FluxoSincronizadoChute(int cantoClicado)
    {
        animacaoAtiva = true;

        if (faseDoJogo == 0)
        {
            destinoJogador = posicaoInicialBola;
            if (jogadorAnimator != null) jogadorAnimator.Play("Jogador_Parado");

            while (jogador != null && Vector2.Distance(jogador.position, destinoJogador) > 0.4f)
            {
                yield return null;
            }

            if (jogadorAnimator != null) jogadorAnimator.Play("Jogador_Chutando");

            if (audioSource != null)
            {
                audioSource.Play();
            }

            yield return new WaitForSeconds(0.15f);
        }

        bool foiGol = ChutesRealizadosMatematica(cantoClicado);

        if (foiGol)
        {
            // DISPARA OS CONFETES VISUAIS!
            if (confetesGol != null) confetesGol.Play();

            if (rectLetreiroGol != null)
            {
                rectLetreiroGol.anchoredPosition = new Vector2(1200f, 0f);
                textoGritoDeGol.SetActive(true);
                xAlvoLetreiro = 0f;
                moverLetreiroHorizontal = true;
                Invoke(nameof(DeslizarParaForaDaTela), 1.2f);
            }
        }

        yield return new WaitForSeconds(1.8f);

        ChecarFimDeTurno();
        ResetarPosicoesInstantaneo();
        animacaoAtiva = false;
    }

    void DeslizarParaForaDaTela()
    {
        xAlvoLetreiro = -1200f;
        Invoke(nameof(EsconderTextoDeGol), 0.4f);
    }

    void EsconderTextoDeGol()
    {
        moverLetreiroHorizontal = false;
        if (textoGritoDeGol != null) textoGritoDeGol.SetActive(false);
    }

    bool ChutesRealizadosMatematica(int cantoClicado)
    {
        bool marcouGol = false;
        int cantoGoleiroFinal = cantoClicado;

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
                GameData.PontuacaoAtual += 100;
                marcouGol = true;
            }
            else
            {
                cantoGoleiroFinal = cantoClicado;
            }

            destinoGoleiro = ObterCoordenadaDoCanto(cantoGoleiroFinal);
        }
        else if (faseDoJogo == 1)
        {
            if (audioSource != null)
            {
                audioSource.Play();
            }
            if (chutesFaseGoleiro >= totalDeChutesPorFase) return false;
            chutesFaseGoleiro++;

            int cantoChuteMaquina = Random.Range(0, 5);
            destinoBola = ObterCoordenadaDoCanto(cantoChuteMaquina);
            destinoGoleiro = ObterCoordenadaDoCanto(cantoClicado);
            cantoGoleiroFinal = cantoClicado;

            int dadoGoleiroJogador = Random.Range(0, 51);

            if (cantoChuteMaquina == cantoClicado && dadoGoleiroJogador >= 25)
            {
                GameData.PontuacaoAtual += 100;
            }
            else
            {
                golsDaMaquina++;
                marcouGol = true;
            }
        }

        if (cantoGoleiroFinal == 0 || cantoGoleiroFinal == 2) funcaoQuedaGoleiro = 65f;
        else if (cantoGoleiroFinal == 1 || cantoGoleiroFinal == 3) funcaoQuedaGoleiro = -65f;
        else funcaoQuedaGoleiro = 0f;

        AtualizarPlacarVisual();
        return marcouGol;
    }

    void Update()
    {
        if (faseDoJogo == 0 && jogador != null && (Vector2)jogador.position != destinoJogador)
        {
            jogador.position = Vector3.MoveTowards(jogador.position, destinoJogador, velocidadeDoJogador * Time.deltaTime);
        }

        if (bola != null && (Vector2)bola.position != destinoBola)
        {
            bola.position = Vector3.MoveTowards(bola.position, destinoBola, velocidadeDaBola * Time.deltaTime);
            bola.Rotate(Vector3.forward * -600f * Time.deltaTime);
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

        if (moverLetreiroHorizontal && rectLetreiroGol != null)
        {
            Vector2 pos = rectLetreiroGol.anchoredPosition;
            pos.x = Mathf.MoveTowards(pos.x, xAlvoLetreiro, velocidadeDoLetreiro * Time.deltaTime);
            rectLetreiroGol.anchoredPosition = pos;
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
            if (audioTorcida != null)
            {
                audioTorcida.Stop();
            }

            if (scriptQuizManager != null) scriptQuizManager.IniciarQuiz();
        }
    }

    void ResetarPosicoesInstantaneo()
    {
        funcaoQuedaGoleiro = 0f;
        if (jogadorAnimator != null) jogadorAnimator.Play("Jogador_Parado");
        if (bola != null) { bola.position = posicaoInicialBola; destinoBola = posicaoInicialBola; bola.rotation = Quaternion.identity; }
        if (goleiro != null) { goleiro.position = posicaoInicialGoleiro; destinoGoleiro = posicaoInicialGoleiro; goleiro.rotation = Quaternion.identity; }
        if (jogador != null) { jogador.position = posicaoInicialJogador; destinoJogador = posicaoInicialJogador; }
    }

    Vector2 ObterCoordenadaDoCanto(int canto)
    {
        if (canto == 0) return superiorEsquerdo;
        if (canto == 1) return superiorDireito;
        if (canto == 2) return inferiorEsquerdo;
        if (canto == 3) return InferiorDireito;
        return centroDoGol;
    }

    void AtualizarPlacarVisual()
    {
        if (textoGolsJogador != null) textoGolsJogador.text = golsDoJogador.ToString();
        if (textoGolsMaquina != null) textoGolsMaquina.text = golsDaMaquina.ToString();
    }
}
