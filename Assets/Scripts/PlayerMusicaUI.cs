using UnityEngine;
using UnityEngine.UI;

// Coloque este script em um objeto que fique SEMPRE ATIVO na cena de Menu
// (por exemplo, o próprio Canvas). Toda vez que o Menu abre, ele acha o
// MusicaManager original e religa os botões, o ícone e o slider de volume.
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

        if (musica == null)
        {
            Debug.LogWarning("PlayerMusicaUI: nenhum MusicaManager encontrado na cena.");
            return;
        }

        LigarBotao(botaoVoltar, musica.MusicaAnterior);
        LigarBotao(botaoPausar, musica.AlternarPausa);
        LigarBotao(botaoPular, musica.ProximaMusica);

        musica.ConectarUI(imagemBotaoPausar, sliderVolume);
    }

    void LigarBotao(Button botao, UnityEngine.Events.UnityAction acao)
    {
        if (botao == null) return;
        botao.onClick.RemoveAllListeners();
        botao.onClick.AddListener(acao);
    }
}