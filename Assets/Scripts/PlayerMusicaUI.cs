using UnityEngine;
using UnityEngine.UI;

// Coloque este script em um objeto que fique SEMPRE ATIVO na cena de Menu
// (por exemplo, o próprio Canvas). Toda vez que o Menu abre, ele acha o
// MusicaManager original e religa os botões, o ícone e o slider de volume.
//
// VERSÃO DE DIAGNÓSTICO: tem Debug.Log em cada etapa pra descobrir onde quebra.
public class PlayerMusicaUI : MonoBehaviour
{
    [Header("Botões do player (arraste da Canvas)")]
    public Button botaoVoltar;
    public Button botaoPausar;
    public Button botaoPular;

    [Header("Ícone do Play/Pause e Slider de volume")]
    public Image imagemBotaoPausar;
    public Slider sliderVolume;

    void Start()
    {
        MusicaManager musica = MusicaManager.Instance;

        Debug.Log("[PlayerMusicaUI] Start rodou. MusicaManager.Instance existe? " + (musica != null));

        if (musica == null)
        {
            Debug.LogWarning("[PlayerMusicaUI] nenhum MusicaManager encontrado na cena.");
            return;
        }

        Debug.Log("[PlayerMusicaUI] AudioSource existe? " + (musica.audioSource != null)
            + " | tocando? " + (musica.audioSource != null && musica.audioSource.isPlaying));

        LigarBotao("Voltar", botaoVoltar, musica.MusicaAnterior);
        LigarBotao("Pausar", botaoPausar, musica.AlternarPausa);
        LigarBotao("Pular", botaoPular, musica.ProximaMusica);

        musica.ConectarUI(imagemBotaoPausar, sliderVolume);
        Debug.Log("[PlayerMusicaUI] Tudo ligado.");
    }

    void LigarBotao(string nome, Button botao, UnityEngine.Events.UnityAction acao)
    {
        if (botao == null)
        {
            Debug.LogWarning("[PlayerMusicaUI] Botão " + nome + " está VAZIO no Inspector.");
            return;
        }

        botao.onClick.RemoveAllListeners();
        botao.onClick.AddListener(() =>
        {
            Debug.Log("[PlayerMusicaUI] Clique recebido no botão " + nome);
            acao();
        });
    }
}