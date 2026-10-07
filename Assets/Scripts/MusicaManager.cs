using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicaManager : MonoBehaviour
{
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
        GameObject[] objetos = GameObject.FindGameObjectsWithTag("MusicaManager");

        if (objetos.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        TocarMusica();

        // ADICIONADO: configura o slider de volume com o valor atual do AudioSource
        // e passa a ouvir mudanças nele pra ajustar o volume em tempo real.
        if (sliderVolumeMusica != null)
        {
            sliderVolumeMusica.value = audioSource != null ? audioSource.volume : 1f;
            sliderVolumeMusica.onValueChanged.RemoveAllListeners();
            sliderVolumeMusica.onValueChanged.AddListener(DefinirVolume);
        }

        AtualizarIconePausar();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "JogoPenaltis")
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            return;
        }

        // CORRIGIDO: quando a música está pausada (Pause()), o Unity marca
        // isPlaying = false, então sem o "!estaPausado" aqui embaixo a música
        // pausada seria trocada pra próxima sozinha, a cada frame.
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

    // ADICIONADO: pula pra próxima música da lista, com loop (depois da última volta pra primeira).
    // Chame este método no OnClick() do botão de "pular" (⏭️).
    public void ProximaMusica()
    {
        musicaAtual++;

        if (musicaAtual >= musicas.Length)
        {
            musicaAtual = 0;
        }

        TocarMusica();
    }

    // ADICIONADO: volta pra música anterior da lista, com loop
    // (se estiver na primeira, pula pra última). Chame este método
    // no OnClick() do botão de "voltar" (⏪).
    public void MusicaAnterior()
    {
        musicaAtual--;

        if (musicaAtual < 0)
        {
            musicaAtual = musicas.Length - 1;
        }

        TocarMusica();
    }

    // ADICIONADO: alterna entre pausar e retomar a música atual (sem reiniciar).
    // Chame este método no OnClick() do botão único de Play/Pause (▶️⏸️).
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

    // ADICIONADO: ajusta o volume só da música (independente do mute e dos
    // efeitos sonoros do jogo). Ligado automaticamente ao Slider em Start().
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