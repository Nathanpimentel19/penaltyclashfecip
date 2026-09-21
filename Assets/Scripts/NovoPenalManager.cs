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

    [Header("Sistema de Confetes (Arraste Aqui)")]
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
    private Animator goleiroAnimator; // NOVO: Referência para desligar/ligar o Animator do goleiro

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
        if (goleiro != null)
        {
            posicaoInicialGoleiro = goleiro.position;
            goleiroAnimator = goleiro.GetComponent<Animator>(); // Captura o animator do goleiro
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
        animacaoAtiva = true; // Bloqueia cliques repetidos imediatamente

        StartCoroutine(FluxoSincronizadoChute(cantoClicado));
    }

    System.Collections.IEnumerator FluxoSincronizadoChute(int cantoClicado)
    {
        // =========================================================================
        // FASE 1: VOCÊ É O CHUTADOR
        // =========================================================================
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
        // =========================================================================
        // FASE 2: VOCÊ É O GOLEIRO (IA CHUTANDO)
        // =========================================================================
        else if (faseDoJogo == 1)
        {
            // Força o boneco a dar um passo para trás para simular o início da corrida
            if (jogador != null) jogador.position = posicaoInicialJogador;

            // Define o alvo da corrida como a posição da bola
            destinoJogador = posicaoInicialBola;
            if (jogadorAnimator != null) jogadorAnimator.Play("Jogador_Parado");

            // Aguarda o boneco se mover fisicamente até a bola (Graças ao ajuste do Update)
            while (jogador != null && Vector2.Distance(jogador.position, (Vector3)destinoJogador) > 0.4f)
            {
                yield return null;
            }

            // Ativa a animação de chute no momento do impacto
            if (jogadorAnimator != null) jogadorAnimator.Play("Jogador_Chutando");

            if (audioSource != null)
            {
                audioSource.Play();
            }

            yield return new WaitForSeconds(0.15f);
        }

        // Executa a validação se foi gol ou se você defendeu no clique
        bool foiGol = ChutesRealizadosMatematica(cantoClicado);

        if (foiGol)
        {
            if (confetesGol != null) confetesGol.Play();

            if (rectLetreiroGol != null)
            {
                StartCoroutine(GerenciarLetreiroGol());
            }
        }

        yield return new WaitForSeconds(1.8f);

        ChecarFimDeTurno();
        ResetarPosicoesInstantaneo();
        animacaoAtiva = false;
    }



    // CORREÇÃO: Coroutine que gerencia a entrada, repouso e saída do letreiro de Gol de forma estável
    System.Collections.IEnumerator GerenciarLetreiroGol()
    {
        rectLetreiroGol.anchoredPosition = new Vector2(1200f, 0f);
        textoGritoDeGol.SetActive(true);
        xAlvoLetreiro = 0f;
        moverLetreiroHorizontal = true;

        // Espera 1.2 segundos centralizado exibindo o gol
        yield return new WaitForSeconds(1.2f);

        // Desliza para fora da tela
        xAlvoLetreiro = -1200f;

        // Espera mais 0.5 segundos até atingir a lateral externa para desligar o objeto
        yield return new WaitForSeconds(0.5f);
        moverLetreiroHorizontal = false;
        textoGritoDeGol.SetActive(false);
    }

    bool ChutesRealizadosMatematica(int cantoClicado)
    {
        bool marcouGol = false;
        int cantoGoleiroFinal = cantoClicado;

        // CORREÇÃO: Desliga o Animator do Goleiro temporariamente antes do chute,
        // permitindo que o script rotacione os braços/corpo por física manual sem bloqueios da Unity.
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
        }
        else if (faseDoJogo == 1)
        {
            if (audioSource != null)
            {
                audioSource.Play();
            }
            if (chutesFaseGoleiro >= totalDeChutesPorFase) return false;
            chutesFaseGoleiro++;

            // Inteligência Artificial escolhe o canto do chute de forma aleatória de 0 a 4
            int cantoChuteMaquina = Random.Range(0, 5);
            destinoBola = ObterCoordenadaDoCanto(cantoChuteMaquina);
            destinoGoleiro = ObterCoordenadaDoCanto(cantoClicado);

            // CORREÇÃO CRÍTICA: Aplica a rotação da queda do goleiro baseada no canto clicado pelo jogador
            cantoGoleiroFinal = cantoClicado;

            // CORREÇÃO DA MÁQUINA: Agora checa se você pulou EXATAMENTE no mesmo canto que a IA chutou!
            if (cantoChuteMaquina == cantoClicado)
            {
                // Se pulou no mesmo canto, defendeu de forma limpa!
                GameData.DefesasAcertas++;
                GameData.PontuacaoAtual += 1000;
            }
            else
            {
                // Se pulou para o canto oposto, a máquina marca gol!
                golsDaMaquina++;
                marcouGol = true;
                GameData.DefesasErradas++;
                GameData.PontuacaoAtual = Mathf.Max(0, GameData.PontuacaoAtual - 200);
            }
        }

        // Define o ângulo correto da queda lateral do goleiro baseado no canto
        if (cantoGoleiroFinal == 0 || cantoGoleiroFinal == 2) funcaoQuedaGoleiro = 65f; // Cai para a esquerda
        else if (cantoGoleiroFinal == 1 || cantoGoleiroFinal == 3) funcaoQuedaGoleiro = -65f; // Cai para a direita
        else funcaoQuedaGoleiro = 0f; // Fica no meio

        AktualizarPlacarVisual();
        return marcouGol;
    }

    void Update()
    {
        // CORREÇÃO: Removeu a trava 'faseDoJogo == 0' para permitir que o boneco corra na vez da IA
        if (jogador != null && (Vector2)jogador.position != destinoJogador)
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

        // CORREÇÃO: Religamos o Animator do Goleiro no recomeço da rodada para que ele fique em pé respirando (Idle)
        if (goleiroAnimator != null) goleiroAnimator.enabled = true;

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

    void AktualizarPlacarVisual()
    {
        AtualizarPlacarVisual();
    }

    void AtualizarPlacarVisual()
    {
        if (textoGolsJogador != null) textoGolsJogador.text = golsDoJogador.ToString();
        if (textoGolsMaquina != null) textoGolsMaquina.text = golsDaMaquina.ToString();
    }
}
