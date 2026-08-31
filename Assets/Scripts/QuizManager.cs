using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("Componentes de Interface (UI)")]
    public GameObject painelDoQuiz;
    public TextMeshProUGUI textoPergunta;
    public TextMeshProUGUI[] textoAlternativas;
    public TextMeshProUGUI textoPontuacaoVisual;

    private Dictionary<int, string> historicoCopas = new Dictionary<int, string>()
    {
        {1930, "Uruguai"}, {1934, "Itália"}, {1938, "Itália"}, {1950, "Uruguai"},
        {1954, "Alemanha"}, {1958, "Brasil"}, {1962, "Brasil"}, {1966, "Inglaterra"},
        {1970, "Brasil"}, {1974, "Alemanha"}, {1978, "Argentina"}, {1982, "Itália"},
        {1986, "Argentina"}, {1990, "Alemanha"}, {1994, "Brasil"}, {1998, "França"},
        {2002, "Brasil"}, {2006, "Itália"}, {2010, "Espanha"}, {2014, "Alemanha"},
        {2018, "França"}, {2022, "Argentina"}, {2026, "Espanha"}
    };

    private List<string> todosCampeoes = new List<string>()
    {
        "Brasil", "Alemanha", "Itália", "Argentina", "França", "Uruguai", "Espanha", "Inglaterra"
    };

    private string campeaoCorreto;
    private int alternativaCorretaIndice;

    void Start()
    {
        if (painelDoQuiz != null) painelDoQuiz.SetActive(false);
        AtualizarTextoDePontos();
    }

    public void IniciarQuiz()
    {
        if (painelDoQuiz != null) painelDoQuiz.SetActive(true);
        AtualizarTextoDePontos();
        GerarPerguntaUnica();
    }

    void GerarPerguntaUnica()
    {
        List<int> anos = new List<int>(historicoCopas.Keys);
        int anoSorteado = anos[Random.Range(0, anos.Count)];
        campeaoCorreto = historicoCopas[anoSorteado];

        textoPergunta.text = $"Qual país foi o ganhador da Copa do Mundo de {anoSorteado}?";

        List<string> opcoesIncorretas = new List<string>(todosCampeoes);
        opcoesIncorretas.Remove(campeaoCorreto);
        EmbaralharLista(opcoesIncorretas);

        List<string> alternativasFinais = new List<string>();
        alternativasFinais.Add(campeaoCorreto);
        alternativasFinais.Add(opcoesIncorretas[0]);
        alternativasFinais.Add(opcoesIncorretas[1]);
        alternativasFinais.Add(opcoesIncorretas[2]);

        EmbaralharLista(alternativasFinais);
        alternativaCorretaIndice = alternativasFinais.IndexOf(campeaoCorreto);

        string[] letras = { "A) ", "B) ", "C) ", "D) " };
        for (int i = 0; i < 4; i++)
        {
            textoAlternativas[i].text = letras[i] + alternativasFinais[i];
        }
    }

    public void Responder(int alternativaEscolhida)
    {
        if (alternativaEscolhida == alternativaCorretaIndice)
        {
            GameData.PontuacaoAtual += 100; // Soma +100 pontos globais se acertar!
            Debug.Log("Resposta Correta!");
        }
        else
        {
            Debug.Log($"Resposta Errada! O vencedor foi {campeaoCorreto}.");
        }

        AtualizarTextoDePontos();
        TerminarQuiz();
    }

    void TerminarQuiz()
    {
        textoPergunta.text = $"Fim do Jogo!\nPontuação Final Total: {GameData.PontuacaoAtual} pontos.";

        foreach (var texto in textoAlternativas)
        {
            if (texto != null && texto.gameObject.transform.parent != null)
            {
                texto.gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    void AtualizarTextoDePontos()
    {
        if (textoPontuacaoVisual != null)
        {
            textoPontuacaoVisual.text = "Pontos: " + GameData.PontuacaoAtual.ToString();
        }
    }

    void EmbaralharLista<T>(List<T> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = lista[i];
            lista[i] = lista[j];
            lista[j] = temp;
        }
    }
}
