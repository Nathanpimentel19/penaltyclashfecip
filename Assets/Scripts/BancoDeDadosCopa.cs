using UnityEngine;

[CreateAssetMenu(fileName = "CartaoMemoriaCopa", menuName = "CartaoMemoriaCopa")]
public class BancoDeDadosCopa : ScriptableObject
{
    [Header("Memória do Uniforme Escolhido")]
    public Sprite uniformeEscolhidoPeloJogador;
}