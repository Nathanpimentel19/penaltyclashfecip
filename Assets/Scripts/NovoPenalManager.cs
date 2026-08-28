using UnityEngine;
using System.Collections.Generic;

public class NovoPenalManager : MonoBehaviour
{
    [Header("Personagens e Objetos")]
    public Transform bola;
    public Transform goleiro;
    public Transform jogador; // Arraste o boneco do seu batedor aqui!

    int golsjogador = 0;
    int chutesRealizados = 0;
    public int totalDeChutesDaPartida = 5;

    [Header("Coordenadas do Gol")]
    public Vector2 superiorEsquerdo = new Vector2(-160f, 70f);
    public Vector2 superiorDireito = new Vector2(160f, 70f);
    public Vector2 inferiorEsquerdo = new Vector2(-160f, -40f);
    public Vector2 InferiorDireito = new Vector2(160f, -40f);
    public Vector2 centroDoGol = new Vector2(0f, 15f);

    public void RealizarChute(int cantoJogador)
    {
        if (chutesRealizados >= totalDeChutesDaPartida) return;
        chutesRealizados++;

        // Move a bola para o canto escolhido
        MoverObjetoParaCanto(bola, cantoJogador);

        // MECÂNICA DE DADOS COMBINADA COM OS AMIGOS:
        int dadoBola = Random.Range(0, 101); // Dado da bola (0 a 100)
        int dadoGoleiro = Random.Range(0, 71);  // Dado do goleiro (0 a 70)
        int cantoGoleiro;

        if (dadoBola > dadoGoleiro) // GOL!
        {
            List<int> cantosErrados = new List<int> { 0, 1, 2, 3, 4 };
            cantosErrados.Remove(cantoJogador);
            int indiceSorteado = Random.Range(0, cantosErrados.Count);
            cantoGoleiro = cantosErrados[indiceSorteado];

            golsjogador++;

            // Mensagem informativa no console
            Debug.Log($"[GOOOL] Bola: {dadoBola} vs Goleiro: {dadoGoleiro} -> A bola venceu! Goleiro pulou no canto {cantoGoleiro}.");
        }
        else // DEFESA!
        {
            cantoGoleiro = cantoJogador;
            Debug.Log($"[DEFESA] Bola: {dadoBola} vs Goleiro: {dadoGoleiro} -> O goleiro venceu e defendeu no canto {cantoGoleiro}!");
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
        Debug.Log("Fim dos pênaltis! Pronto para abrir o Quiz.");
        // A ponte com o QuizManager será recolocada assim que criarmos o script do Quiz!
    }
}
