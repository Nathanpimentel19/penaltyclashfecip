using UnityEngine;

[CreateAssetMenu(fileName = "NovoBancoCopa", menuName = "Copa/BancoDeDados")]
public class BancoDeDadosCopa : ScriptableObject
{
    // A caixinha física que vai segurar a foto do uniforme entre as telas!
    public Sprite uniformeEscolhidoPeloJogador;
    public string nomeDoPaisEscolhido;
}
