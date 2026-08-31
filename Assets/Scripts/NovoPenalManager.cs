using UnityEngine;
using System.Collections.Generic;

public class NovoPenalManager : MonoBehaviour
{
    [Header("Personagens e Objetos")]
    public Transform bola;
    public Transform goleiro;
    public Transform jogador;

    int golsjogador = 0;
    int chutesRealizados = 0;
    public int totalDeChutesDaPartida = 5;

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
        // Guarda a posição da marca do pênalti e do meio do gol assim que o jogo inicia
        if (bola != null) posicaoInicialBola = bola.position;
        if (goleiro != null) posicaoInicialGoleiro = goleiro.position;
    }

    public void RealizarChute(int cantoJogador)
    {
        if (chutesRealizados >= totalDeChutesDaPartida) return;
        chutesRealizados++;

        // RESET AUTOMÁTICO: Coloca a bola de volta na marca do pênalti e o goleiro no centro antes do chute acontecer!
        if (bola != null) bola.position = posicaoInicialBola;
        if (goleiro != null) goleiro.position = posicaoInicialGoleiro;

        // Teletransporta a bola para o canto escolhido
        MoverObjetoParaCanto(bola, cantoJogador);

        // MECÂNICA DE DADOS COMBINADA COM OS AMIGOS:
        int dadoBola = Random.Range(0, 101);
        int dadoGoleiro = Random.Range(0, 71);
        int cantoGoleiro;

        if (dadoBola > dadoGoleiro) // GOL!
        {
            List<int> cantosErrados = new List<int> { 0, 1, 2, 3, 4 };
            cantosErrados.Remove(cantoJogador);
            int indiceSorteado = Random.Range(0, cantosErrados.Count);
            cantoGoleiro = cantosErrados[indiceSorteado];

            golsjogador++;
            GameData.PontuacaoAtual += 100;

            Debug.Log($"[GOOOL] Bola: {dadoBola} vs Goleiro: {dadoGoleiro} -> Goleiro pulou no canto {cantoGoleiro}. Pontos: {GameData.PontuacaoAtual}");
        }
        else // DEFESA!
        {
            cantoGoleiro = cantoJogador;
            Debug.Log($"[DEFESA] Bola: {dadoBola} vs Goleiro: {dadoGoleiro} -> Defesa no canto {cantoGoleiro}!");
        }

        MoverObjetoParaCanto(goleiro, cantoGoleiro);

        if (chutesRealizados >= totalDeChutesDaPartida)
        {
            Invoke("ChamarOQuiz", 2f);
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

    void ChamarOQuiz()
    {
        Debug.Log("Fim dos pênaltis! Abrindo o Quiz...");
        QuizManager quiz = FindObjectOfType<QuizManager>();
        if (quiz != null)
        {
            quiz.IniciarQuiz();
        }
        else
        {
            Debug.LogError("Erro: O script QuizManager não foi encontrado na cena!");
        }
    }
}
