using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    [Header("Componentes de Interface (UI)")]
    public GameObject painelDoQuiz;
    public TextMeshProUGUI textoPergunta;
    public TextMeshProUGUI[] textoAlternativas;
    public TextMeshProUGUI textoPontuacaoVisual;
    public GameObject botaoJogarDeNovo;
    public GameObject botaoVoltar;

    [Header("Telas de Encerramento e Classificação")]
    public Button botaoVerTopGlobal;

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
    private bool respondeuCorreto = false;

    void Start()
    {
        if (painelDoQuiz != null) painelDoQuiz.SetActive(false);
        if (botaoJogarDeNovo != null) botaoJogarDeNovo.SetActive(false);
        if (botaoVerTopGlobal != null) botaoVerTopGlobal.gameObject.SetActive(false);
        AtualizarTextoDePontos();
    }

    public void IniciarQuiz()
    {
        if (painelDoQuiz != null) painelDoQuiz.SetActive(true);
        if (botaoJogarDeNovo != null) botaoJogarDeNovo.SetActive(false);
        if (botaoVerTopGlobal != null) botaoVerTopGlobal.gameObject.SetActive(false);

        AtualizarTextoDePontos();
        GerarPerguntaUnica();
    }

    void GerarPerguntaUnica()
    {
        if (textoPergunta == null || textoAlternativas == null || textoAlternativas.Length < 4)
        {
            Debug.LogError("Configure os componentes visuais no Inspector!");
            return;
        }

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
            if (textoAlternativas[i] != null)
            {
                textoAlternativas[i].text = letras[i] + alternativasFinais[i];

                Button botaoPai = textoAlternativas[i].GetComponentInParent<Button>();
                if (botaoPai != null)
                {
                    int indiceResposta = i;
                    botaoPai.onClick.RemoveAllListeners();
                    botaoPai.onClick.AddListener(() => Responder(indiceResposta));
                    botaoPai.gameObject.SetActive(true);
                    botaoPai.interactable = true;
                }
            }
        }
    }

    public void Responder(int alternativaEscolhida)
    {
        foreach (var texto in textoAlternativas)
        {
            if (texto != null)
            {
                Button botaoPai = texto.GetComponentInParent<Button>();
                if (botaoPai != null) botaoPai.interactable = false;
            }
        }

        if (alternativaEscolhida == alternativaCorretaIndice)
        {
            GameData.PontuacaoAtual += 500;
            respondeuCorreto = true;
            Debug.Log("Resposta Correta! +500 pontos.");
        }
        else
        {
            respondeuCorreto = false;
            Debug.Log($"Resposta Errada! O vencedor foi {campeaoCorreto}.");
        }

        AtualizarTextoDePontos();
        TerminarQuiz();
    }

    void TerminarQuiz()
    {
        int pontosGolsGanhos = GameData.GolsAcertos * 1000;
        int pontosGolsPerdidos = GameData.GolsFora * 200;
        int pontosDefesasGanhas = GameData.DefesasAcertas * 1000;
        int pontosDefesasPerdidas = GameData.DefesasErradas * 200;

        string feedbackQuiz = respondeuCorreto ?
            "<color=green>Acertou! (+500 pontos)</color>" :
            $"<color=red>Errou! A resposta correta era: {campeaoCorreto}</color>";

        if (textoPergunta != null)
        {
            textoPergunta.text = $"{feedbackQuiz}\n\n" +
                                 $"<b>Fim do Jogo!</b>\n\n" +
                                 $"Gols acertos: {GameData.GolsAcertos} (+{pontosGolsGanhos} pts)\n" +
                                 $"<line-height=130%>Gols fora: {GameData.GolsFora} (-{pontosGolsPerdidos} pts)</line-height>\n" +
                                 $"Defesas acertas: {GameData.DefesasAcertas} (+{pontosDefesasGanhas} pts)\n" +
                                 $"<line-height=130%>Defesas erradas: {GameData.DefesasErradas} (-{pontosDefesasPerdidas} pts)</line-height>\n\n" +
                                 $"<b>Pontuação Final Total: {GameData.PontuacaoAtual} pontos.</b>";
        }

        foreach (var texto in textoAlternativas)
        {
            if (texto != null)
            {
                Button botaoPai = texto.GetComponentInParent<Button>();
                if (botaoPai != null) botaoPai.gameObject.SetActive(false);
            }
        }

        if (botaoJogarDeNovo != null)
        {
            botaoJogarDeNovo.SetActive(true);
            Button btn = botaoJogarDeNovo.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(AbrirJanelaConfirmacaoRestart);
            }
        }

        if (botaoVoltar != null)
        {
            botaoVoltar.SetActive(true);
            Button btn = botaoVoltar.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(VoltarParaOMenuPrincipal);
            }
        }

        if (botaoVerTopGlobal != null)
        {
            botaoVerTopGlobal.gameObject.SetActive(true);
            botaoVerTopGlobal.onClick.RemoveAllListeners();
            botaoVerTopGlobal.onClick.AddListener(IrParaTelaDeClassificacao);
        }
    }

    public void IrParaTelaDeClassificacao()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene("CenaPlacar");
    }

    public void AbrirJanelaConfirmacaoRestart()
    {
        string nomeSalvo = PlayerPrefs.GetString("NomeJogador", "Jogador");

        if (textoPergunta != null)
        {
            textoPergunta.text = $"Deseja continuar como <b>{nomeSalvo}</b> ou trocar o jogador?\n\n Jogar Novamente = Continuar / Voltar = Trocar Jogador.";
        }

        if (botaoJogarDeNovo != null)
        {
            TMP_Text txtBotao = botaoJogarDeNovo.GetComponentInChildren<TMP_Text>();
            if (txtBotao != null) txtBotao.text = "CONTINUAR";

            Button btn = botaoJogarDeNovo.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(ReiniciarMantendoMesmoNome);
            }
        }

        if (botaoVoltar != null)
        {
            TMP_Text txtBotao = botaoVoltar.GetComponentInChildren<TMP_Text>();
            if (txtBotao != null) txtBotao.text = "TROCAR JOGADOR";

            Button btn = botaoVoltar.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(VoltarParaTelaDeNome);
            }
        }
    }

    private void ReiniciarMantendoMesmoNome()
    {
        ZerarEstatisticasDaPartida();
        // Altera o carregamento para ir direto para a tela onde o jogador escolhe o país
        SceneManager.LoadScene("EscolhaDePaís");
    }

    private void VoltarParaTelaDeNome()
    {
        ZerarEstatisticasDaPartida();
        PlayerPrefs.DeleteKey("NomeJogador");
        PlayerPrefs.Save();
        SceneManager.LoadScene("EscolhaNome");
    }

    public void VoltarParaOMenuPrincipal()
    {
        SceneManager.LoadScene("Menu");
    }

    private void ZerarEstatisticasDaPartida()
    {
        GameData.PontuacaoAtual = 0;
        GameData.GolsAcertos = 0;
        GameData.GolsFora = 0;
        GameData.DefesasAcertas = 0;
        GameData.DefesasErradas = 0;
    }

    void Update()
    {
        AtualizarTextoDePontos();
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
