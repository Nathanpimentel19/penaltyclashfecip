using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EscolhaDePaisManager : MonoBehaviour
{
    [Header("Time 1")]
    public Image bandeiraTime1;

    [Header("Time 2")]
    public Image bandeiraTime2;

    [Header("Bandeiras")]
    public Sprite[] bandeiras;

    private int paisTime1 = 0;
    private int paisTime2 = 0;

    public void ProximoTime1()
    {
        paisTime1++;

        if (paisTime1 >= bandeiras.Length)
            paisTime1 = 0;

        bandeiraTime1.sprite = bandeiras[paisTime1];
    }

    public void AnteriorTime1()
    {
        paisTime1--;

        if (paisTime1 < 0)
            paisTime1 = bandeiras.Length - 1;

        bandeiraTime1.sprite = bandeiras[paisTime1];
    }

    public void ProximoTime2()
    {
        paisTime2++;

        if (paisTime2 >= bandeiras.Length)
            paisTime2 = 0;

        bandeiraTime2.sprite = bandeiras[paisTime2];
    }

    public void AnteriorTime2()
    {
        paisTime2--;

        if (paisTime2 < 0)
            paisTime2 = bandeiras.Length - 1;

        bandeiraTime2.sprite = bandeiras[paisTime2];
    }

    public void Selecionar()
    {
        SceneManager.LoadScene("JogoPenaltis");
    }
}