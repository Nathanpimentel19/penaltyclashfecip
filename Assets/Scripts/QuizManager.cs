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
    private bool _jaProcessouFim = false;

    void Start()
    {
        if (painelDoQuiz != null) painelDoQuiz.SetActive(false);
        if (botaoJogarDeNovo != null) botaoJogarDeNovo.SetActive(false);
        AtualizarTextoDePontos();
    }

    void Update()
    {
        AtualizarTextoDePontos();
    }

    public void IniciarQuiz()
    {
        _jaProcessouFim = false;
        if (painelDoQuiz != null) painelDoQuiz.SetActive(true);
        if (botaoJogarDeNovo != null) botaoJogarDeNovo.SetActive(false);

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
        if (_jaProcessouFim) return;
        _jaProcessouFim = true;

        List<Top10PlayerScoreAuxiliar> listaTemporaria = new List<Top10PlayerScoreAuxiliar>();

        if (PlayerPrefs.HasKey("FeiraCienciasLeaderboard"))
        {
            string jsonAntigo = PlayerPrefs.GetString("FeiraCienciasLeaderboard");
            LeaderboardSaveDataAuxiliar dadosCarregados = JsonUtility.FromJson<LeaderboardSaveDataAuxiliar>(jsonAntigo);

            if (dadosCarregados != null && dadosCarregados.scores != null)
            {
                listaTemporaria.AddRange(dadosCarregados.scores);
            }
        }

        string nomeJogador = PlayerPrefs.GetString("NomeJogador", "Jogador").Trim();

        // 1. CARREGA O HISTÓRICO REAL DO DISCO ANTES DE QUALQUER ALTERAÇÃO
        string prefixo = nomeJogador.ToLower();
        int histGolsAcertos = PlayerPrefs.GetInt(prefixo + "_HistGolsAcertos", 0);
        int histGolsFora = PlayerPrefs.GetInt(prefixo + "_HistGolsFora", 0);
        int histDefesasAcertas = PlayerPrefs.GetInt(prefixo + "_HistDefesasAcertas", 0);
        int histDefesasErradas = PlayerPrefs.GetInt(prefixo + "_HistDefesasErradas", 0);

        Top10PlayerScoreAuxiliar jogadorExistente = listaTemporaria.Find(p => p.playerName.Trim().ToUpper().Equals(nomeJogador.ToUpper()));
        int pontosAnterioresDoHistorico = (jogadorExistente != null) ? jogadorExistente.score : 0;

        // 2. CONGELA OS VALORES ATUAIS EM VARIÁVEIS LOCAIS (Evita bug de reset precoce)
        int golsAcertosNestaPartida = GameData.GolsAcertos;
        int golsForaNestaPartida = GameData.GolsFora;
        int defesasAcertasNestaPartida = GameData.DefesasAcertas;
        int defesasErradasNestaPartida = GameData.DefesasErradas;
        int pontosBonusQuiz = respondeuCorreto ? 500 : 0;

        // 3. CALCULA OS PONTOS DA RODADA COM BASE NAS VARIÁVEIS CONGELADAS
        int pontosGolsGanhos = golsAcertosNestaPartida * 1000;
        int pontosGolsPerdidos = golsForaNestaPartida * 200;
        int pontosDefesasGanhas = defesasAcertasNestaPartida * 1000;
        int pontosDefesasPerdidas = defesasErradasNestaPartida * 200;

        int pontuacaoCalculadaDestaPartida = (pontosGolsGanhos + pontosDefesasGanhas) - (pontosGolsPerdidos + pontosDefesasPerdidas) + pontosBonusQuiz;

        // 4. COMPUTA O ACÚMULO HISTÓRICO REAL SEM DUPLICIDADE
        int totalExibicaoGolsAcertos = histGolsAcertos + golsAcertosNestaPartida;
        int totalExibicaoGolsFora = histGolsFora + golsForaNestaPartida;
        int totalExibicaoDefesasAcertas = histDefesasAcertas + defesasAcertasNestaPartida;
        int totalExibicaoDefesasErradasFix = histDefesasErradas + defesasErradasNestaPartida;
        int totalExibicaoPontos = Mathf.Max(0, pontosAnterioresDoHistorico + pontuacaoCalculadaDestaPartida);

        // 5. ATUALIZA AS VARIÁVEIS ESTÁTICAS DO GAMEDATA COM OS VALORES CONSOLIDADOS
        GameData.PontuacaoAtual = pontuacaoCalculadaDestaPartida;
        GameData.TotalHistoricoGolsAcertos = totalExibicaoGolsAcertos;
        GameData.TotalHistoricoGolsFora = totalExibicaoGolsFora;
        GameData.TotalHistoricoDefesasAcertas = totalExibicaoDefesasAcertas;
        GameData.TotalHistoricoDefesasErradas = totalExibicaoDefesasErradasFix;
        GameData.TotalHistoricoPontos = totalExibicaoPontos;

        // 6. SALVA OS DADOS FÍSICOS NO COMPUTADOR IMEDIATAMENTE
        GameData.SalvarHistoricoLocal(nomeJogador);
        SalvarPointsNoComputador();

        string feedbackQuiz = respondeuCorreto ?
            "<color=green>Acertou o Quiz! (+500 pontos)</color>" :
            $"<color=red>Errou o Quiz! Resposta correta: {campeaoCorreto}</color>";

        // 7. EXIBE AS INFORMAÇÕES USANDO AS VARIÁVEIS LOCAIS SEGURAS
        if (textoPergunta != null)
        {
            textoPergunta.text = $"<size=80%><margin-left=5px><margin-right=5px>{feedbackQuiz}\n\n" +
                                 $"<b>Fim do Jogo!</b>\n\n" +
                                 $"Gols acertos: {golsAcertosNestaPartida} (+{pontosGolsGanhos} pts) | Total: {totalExibicaoGolsAcertos}\n" +
                                 $"Gols fora: {golsForaNestaPartida} (-{pontosGolsPerdidos} pts) | Total: {totalExibicaoGolsFora}\n" +
                                 $"Defesas acertas: {defesasAcertasNestaPartida} (+{pontosDefesasGanhas} pts) | Total: {totalExibicaoDefesasAcertas}\n" +
                                 $"Defesas erradas: {defesasErradasNestaPartida} (-{pontosDefesasPerdidas} pts) | Total: {totalExibicaoDefesasErradasFix}\n\n" +
                                 $"<b>Pontuação Total da Partida: {pontuacaoCalculadaDestaPartida} pts.</b>\n" +
                                 $"<b>Pontuação Geral Acumulada: {totalExibicaoPontos} pts</b></margin></margin></size>";
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
                btn.onClick.AddListener(SalvarEVoltarParaOMenu);
            }
        }
    } // Fim do método TerminarQuiz

    public void AbrirJanelaConfirmacaoRestart()
    {
        string nomeSalvo = PlayerPrefs.GetString("NomeJogador", "Jogador");

        // ETAPA 1: Pergunta sobre manter o mesmo Nome de Jogador
        if (textoPergunta != null)
        {
            textoPergunta.text = $"Deseja continuar como <b>{nomeSalvo}</b> ou trocar o jogador?\n\n Voltar = trocar jogador\n\n Jogar Novamente = continuar como <b>{nomeSalvo}</b>";
        }

        if (botaoJogarDeNovo != null)
        {
            Button btn = botaoJogarDeNovo.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                // Avança para a Etapa 2 (Pergunta das Seleções)
                btn.onClick.AddListener(PerguntarConfirmacaoTimes);
            }
        }

        if (botaoVoltar != null)
        {
            Button btn = botaoVoltar.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(SalvarEVoltarParaOMenu);
            }
        }
    }

    // ETAPA 2: Pergunta sobre manter as mesmas Seleções/Times
    private void PerguntarConfirmacaoTimes()
    {
        if (textoPergunta != null)
        {
            textoPergunta.text = "Deseja manter as mesmas <b>Seleções/Times</b> da partida anterior?\n\n Voltar = Escolher novos países\n\n Jogar Novamente = Manter mesmos times";
        }

        if (botaoJogarDeNovo != null)
        {
            Button btn = botaoJogarDeNovo.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    // Mantém as seleções em GameData, limpa o placar e joga de novo
                    GameData.ResetarPartidaAtual();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                });
            }
        }

        if (botaoVoltar != null)
        {
            Button btn = botaoVoltar.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    // Limpa os dados voláteis da partida
                    GameData.ResetarPartidaAtual();

                    // CORREÇÃO: Carrega a sua cena pelo nome exato do arquivo
                    SceneManager.LoadScene("EscolhaDePaís");
                });
            }
        }
    }


    public void SalvarEVoltarParaOMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void SalvarPointsNoComputador()
    {
        List<Top10PlayerScoreAuxiliar> listaTemporaria = new List<Top10PlayerScoreAuxiliar>();
        if (PlayerPrefs.HasKey("FeiraCienciasLeaderboard"))
        {
            string jsonAntigo = PlayerPrefs.GetString("FeiraCienciasLeaderboard");
            LeaderboardSaveDataAuxiliar dadosCarregados = JsonUtility.FromJson<LeaderboardSaveDataAuxiliar>(jsonAntigo);
            if (dadosCarregados != null && dadosCarregados.scores != null)
            {
                listaTemporaria.AddRange(dadosCarregados.scores);
            }
        }
        string nomeJogador = PlayerPrefs.GetString("NomeJogador", "Jogador").Trim().ToUpper();
        int pontuacaoFinalParaORanking = GameData.TotalHistoricoPontos;
        Top10PlayerScoreAuxiliar jogadorExistente = listaTemporaria.Find(p =>
            p.playerName.Trim().ToUpper().Equals(nomeJogador));
        if (jogadorExistente != null)
        {
            jogadorExistente.score = pontuacaoFinalParaORanking;
        }
        else
        {
            listaTemporaria.Add(new Top10PlayerScoreAuxiliar(nomeJogador, pontuacaoFinalParaORanking));
        }
        listaTemporaria.Sort((x, y) => y.score.CompareTo(x.score));
        LeaderboardSaveDataAuxiliar jsonNovoData = new LeaderboardSaveDataAuxiliar();
        jsonNovoData.scores = listaTemporaria;
        string jsonNovo = JsonUtility.ToJson(jsonNovoData);
        PlayerPrefs.SetString("FeiraCienciasLeaderboard", jsonNovo);
        PlayerPrefs.Save();
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

    [System.Serializable]
    private class LeaderboardSaveDataAuxiliar
    {
        public List<Top10PlayerScoreAuxiliar> scores;
    }

    [System.Serializable]
    private class Top10PlayerScoreAuxiliar
    {
        public string playerName;
        public int score;
        public Top10PlayerScoreAuxiliar(string name, int points)
        {
            playerName = name;
            score = points;
        }
    }
}
