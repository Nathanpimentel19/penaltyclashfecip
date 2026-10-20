using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicaManager : MonoBehaviour
{
    // Endereço fixo do tocador de música (o original, que sobrevive às cenas).
    public static MusicaManager Instance;

    public AudioSource audioSource;
    public AudioClip[] musicas;
    public GameObject btnMutar;
    public GameObject btnDesmutar;

    [Header("Ícones do botão Play/Pause")]
    public Sprite iconePlay;
    public Sprite iconePause;

    [Header("Nomes dos objetos na Canvas (o script acha eles sozinho)")]
    public string nomeBotaoVoltar = "voltarMusica";
    public string nomeBotaoPausar = "PausarMusica";
    public string nomeBotaoPular = "PularMusica";
    public string nomeSliderVolume = "AjustarVolume";

    // Referências da cena atual. São refeitas toda vez que uma cena carrega.
    [HideInInspector] public Image imagemBotaoPausar;
    [HideInInspector] public Slider sliderVolumeMusica;

    private int musicaAtual = 0;
    private bool estaPausado = false;

    void Awake()
    {
        // Se já existe o MusicaManager original, este duplicado se destrói.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Avisa este script toda vez que uma cena nova termina de carregar.
        SceneManager.sceneLoaded += AoCarregarCena;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= AoCarregarCena;
            Instance = null;
        }
    }

    void Start()
    {
        // O duplicado ainda roda Start() antes de ser destruído, então ignora.
        if (Instance != this) return;

        TocarMusica();
        ReligarUIDaCena();
    }

    void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        ReligarUIDaCena();
    }

    // Procura na cena atual os botões e o slider do player e liga cada um
    // à função certa. Roda no começo do jogo e a cada troca de cena, então
    // os botões nunca ficam apontando pra objetos de uma cena que já sumiu.
    void ReligarUIDaCena()
    {
        Button voltar = EncontrarBotao(nomeBotaoVoltar);
        Button pausar = EncontrarBotao(nomeBotaoPausar);
        Button pular = EncontrarBotao(nomeBotaoPular);
        Slider slider = EncontrarSlider();

        LigarBotao(voltar, MusicaAnterior);
        LigarBotao(pausar, AlternarPausa);
        LigarBotao(pular, ProximaMusica);

        Image icone = null;
        if (pausar != null)
        {
            icone = pausar.targetGraphic as Image;
            if (icone == null) icone = pausar.GetComponent<Image>();
        }

        ConectarUI(icone, slider);

        Debug.Log("[MusicaManager] UI religada na cena '" + SceneManager.GetActiveScene().name
            + "': voltar=" + (voltar != null)
            + ", pausar=" + (pausar != null)
            + ", pular=" + (pular != null)
            + ", slider=" + (slider != null));
    }

    Button EncontrarBotao(string nome)
    {
        Button[] botoes = FindObjectsByType<Button>(FindObjectsInactive.Include);

        foreach (Button b in botoes)
        {
            if (string.Equals(b.name, nome, System.StringComparison.OrdinalIgnoreCase))
            {
                return b;
            }
        }

        return null;
    }

    Slider EncontrarSlider()
    {
        Slider[] sliders = FindObjectsByType<Slider>(FindObjectsInactive.Include);

        // 1) nome exato
        foreach (Slider s in sliders)
        {
            if (string.Equals(s.name, nomeSliderVolume, System.StringComparison.OrdinalIgnoreCase))
            {
                return s;
            }
        }

        // 2) qualquer slider com "volume" no nome
        foreach (Slider s in sliders)
        {
            if (s.name.ToLower().Contains("volume"))
            {
                return s;
            }
        }

        // 3) se só existe um slider na cena, é ele
        if (sliders.Length == 1) return sliders[0];

        return null;
    }

    void LigarBotao(Button botao, UnityAction acao)
    {
        if (botao == null) return;

        // Desliga os OnClick() que foram configurados à mão no Inspector,
        // porque eles apontam pro objeto antigo e causariam clique duplo.
        for (int i = 0; i < botao.onClick.GetPersistentEventCount(); i++)
        {
            botao.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);
        }

        botao.onClick.RemoveAllListeners();
        botao.onClick.AddListener(acao);
    }

    // Liga o ícone de play/pause e o slider de volume da cena atual.
    public void ConectarUI(Image novaImagemPausar, Slider novoSlider)
    {
        imagemBotaoPausar = novaImagemPausar;

        if (sliderVolumeMusica != null)
        {
            sliderVolumeMusica.onValueChanged.RemoveListener(DefinirVolume);
        }

        sliderVolumeMusica = novoSlider;

        if (sliderVolumeMusica != null)
        {
            sliderVolumeMusica.value = audioSource != null ? audioSource.volume : 1f;
            sliderVolumeMusica.onValueChanged.AddListener(DefinirVolume);
        }

        AtualizarIconePausar();
    }

    void Update()
    {
        if (Instance != this) return;

        if (SceneManager.GetActiveScene().name == "JogoPenaltis")
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            // O Stop() zera a música, então não faz sentido continuar "pausado".
            estaPausado = false;
            return;
        }

        // Quando a música está pausada o Unity marca isPlaying = false, então
        // sem o "!estaPausado" a música pausada seria trocada sozinha.
        if (audioSource != null && !audioSource.isPlaying && !estaPausado)
        {
            ProximaMusica();
        }
    }

    void TocarMusica()
    {
        audioSource.clip = musicas[musicaAtual];
        audioSource.Play();
        estaPausado = false;
        AtualizarIconePausar();
    }

    // Pula pra próxima música, com loop (depois da última volta pra primeira).
    public void ProximaMusica()
    {
        musicaAtual++;

        if (musicaAtual >= musicas.Length)
        {
            musicaAtual = 0;
        }

        TocarMusica();
    }

    // Volta pra música anterior, com loop (na primeira, vai pra última).
    public void MusicaAnterior()
    {
        musicaAtual--;

        if (musicaAtual < 0)
        {
            musicaAtual = musicas.Length - 1;
        }

        TocarMusica();
    }

    // Alterna entre pausar e retomar a música atual (sem reiniciar).
    public void AlternarPausa()
    {
        if (audioSource == null) return;

        if (estaPausado)
        {
            audioSource.UnPause();
            estaPausado = false;
        }
        else
        {
            audioSource.Pause();
            estaPausado = true;
        }

        AtualizarIconePausar();
    }

    void AtualizarIconePausar()
    {
        if (imagemBotaoPausar == null) return;

        if (estaPausado && iconePlay != null)
        {
            imagemBotaoPausar.sprite = iconePlay;
        }
        else if (!estaPausado && iconePause != null)
        {
            imagemBotaoPausar.sprite = iconePause;
        }
    }

    // Ajusta o volume só da música (independente dos efeitos sonoros).
    public void DefinirVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    public void Mutar()
    {
        audioSource.mute = true;

        if (btnMutar != null)
            btnMutar.SetActive(false);

        if (btnDesmutar != null)
            btnDesmutar.SetActive(true);
    }

    public void Desmutar()
    {
        audioSource.mute = false;

        if (btnMutar != null)
            btnMutar.SetActive(true);

        if (btnDesmutar != null)
            btnDesmutar.SetActive(false);
    }
}