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

    [Header("Links de Gerenciamento")]
    public QuizManager scriptQuizManager;
    public GerenciadorDeCamisas gerenciadorDeCamisas;

    // 💾 O SEU CARTÃO DE MEMÓRIA DO SUCESSO!
    [Header("Cartão de Memória da Copa (Arraste o arquivo criado aqui)")]
    public BancoDeDadosCopa cartaoMemoria;

    [Header("Textos do Placar (UI)")]
    public TextMeshProUGUI textoGolsJogador;
    public TextMeshProUGUI textoGolsMaquina;

    [Header("Texto Exclusivo de Gol (Arraste Aqui)")]
    public GameObject textoGritoDeGol;
    public float velocidadeDoLetreiro = 1500f;

    [Header("Sistema de Confetes (Arraste Aqui)")]
    public ParticleSystem confetesGol;

    [Header("Configurações de Velocidade")]
    public float velocidadeDaBola = 35f;
    public float velocidadeDoGoleiro = 20f;

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

    private bool animacaoAtiva = false;
    private float funcaoQuedaGoleiro = 0f;

    private RectTransform rectLetreiroGol;
    private float xAlvoLetreiro = 0f;
    private bool moverLetreiroHorizontal = false;

    void Start()
    {
        if (bola != null) posicaoInicialBola = bola.position;
        if (goleiro != null) posicaoInicialGoleiro = goleiro.position;

        if (jogador == null)
        {
            GameObject findJogador = GameObject.Find("Jogador");
            if (findJogador == null) findJogador = GameObject.Find("jogador");
            if (findJogador != null) jogador = findJogador.transform;
        }

        if (jogador != null)
        {
            posicaoInicialJogador = jogador.position;

            // 👕 O PULO DO GATO: Veste o uniforme lincado no cartão de memória perfeitamente!
            if (cartaoMemoria != null && cartaoMemoria.uniformeEscolhidoPeloJogador != null)
            {
                SpriteRenderer sr = jogador.GetComponent<SpriteRenderer>();
                if (sr == null) sr = jogador.GetComponentInChildren<SpriteRenderer>();

                if (sr != null)
                {
                    sr.sprite = cartaoMemoria.uniformeEscolhidoPeloJogador;
                    sr.enabled = false;
                    sr.enabled = true;
                    Debug.Log($"[SUCESSO] Uniforme aplicado direto do cartão: {cartaoMemoria.uniformeEscolhidoPeloJogador.name}");
                }
            }
        }

        if (textoGritoDeGol != null)
        {
            rectLetreiroGol = textoGritoDeGol.GetComponent<RectTransform>();
            textoGritoDeGol.SetActive(false);
        }

        if (confetesGol != null) confetesGol.Stop();

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

        if (faseDoJogo == 0)
        {
            if (jogador != null) jogador.position = posicaoInicialBola;
            yield return new WaitForSeconds(0.05f);
        }

        bool foiGol = ChutesRealizadosMatematica(cantoClicado);

        if (foiGol)
        {
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

        yield return new WaitForSeconds(1.5f);

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

    void ChecarFimDeTurno()
    {
        if (faseDoJogo == 0 && chutesFaseChutador >= totalDeChutesPorFase)
        {
            faseDoJogo = 1;
            Debug.Log("[FASE 2] Agora você é o goleiro!");
        }
        else if (faseDoJogo == 1 && chutesFaseGoleiro >= totalDeChutesPorFase)
        {
            if (scriptQuizManager != null) scriptQuizManager.IniciarQuiz();
        }
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

            if (gerenciadorDeCamisas != null)
            {
                gerenciadorDeCamisas.MudarUniformeDaRodada(chutesFaseChutador);
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
        if (bola != null && (Vector2)bola.position != destinoBola)
        {
            bola.position = Vector3.MoveTowards(bola.position, destinoBola, velocidadeDaBola * Time.deltaTime);
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

    void ResetarPosicoesInstantaneo()
    {
        funcaoQuedaGoleiro = 0f;
        if (bola != null) { bola.position = posicaoInicialBola; destinoBola = posicaoInicialBola; }
        if (goleiro != null) { goleiro.position = posicaoInicialGoleiro; destinoGoleiro = posicaoInicialGoleiro; goleiro.rotation = Quaternion.identity; }
        if (jogador != null) { jogador.position = posicaoInicialJogador; }
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
