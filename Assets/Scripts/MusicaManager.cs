using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicaManager : MonoBehaviour
{
    // ADICIONADO: endereço fixo do tocador de música. Qualquer script (ou a UI
    // de uma cena nova) consegue achar o MusicaManager original por aqui,
    // mesmo depois de trocar de cena.
    public static MusicaManager Instance;

    public AudioSource audioSource;
    public AudioClip[] musicas;
    public GameObject btnMutar;
    public GameObject btnDesmutar;

    [Header("Player de Música (Pausar / Pular / Voltar)")]
    [Tooltip("Imagem do botão único de Play/Pause, que troca de ícone sozinho.")]
    public Image imagemBotaoPausar;
    public Sprite iconePlay;
    public Sprite iconePause;

    [Header("Volume da Música (separado dos efeitos sonoros)")]
    public Slider sliderVolumeMusica;

    private int musicaAtual = 0;
    private bool estaPausado = false;

    void Awake()
    {
        // CORRIGIDO: se já existe um MusicaManager (o original, que sobreviveu
        // à troca de cena), este novo se destrói e o original continua valendo.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // O duplicado ainda roda Start() antes de ser destruído, então ignora.
        if (Instance != this) return;

        TocarMusica();
        ConectarUI(imagemBotaoPausar, sliderVolumeMusica);
    }

    // ADICIONADO: religa o ícone de play/pause e o slider de volume da cena atual.
    // O PlayerMusicaUI chama isso toda vez que o Menu abre, porque o Canvas
    // antigo foi destruído na troca de cena e o novo precisa ser conectado.
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