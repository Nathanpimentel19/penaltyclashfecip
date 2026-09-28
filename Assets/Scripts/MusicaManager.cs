using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicaManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] musicas;
    public GameObject btnMutar;
    public GameObject btnDesmutar;

    private int musicaAtual = 0;

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

        if (audioSource != null && !audioSource.isPlaying)
        {
            ProximaMusica();
        }
    }

    void TocarMusica()
    {
        audioSource.clip = musicas[musicaAtual];
        audioSource.Play();
    }

    void ProximaMusica()
    {
        musicaAtual++;

        if (musicaAtual >= musicas.Length)
        {
            musicaAtual = 0;
        }

        TocarMusica();
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