using UnityEngine;
using UnityEngine.UI;

public class BarraGasolina : MonoBehaviour
{
    public Image barraGasolina;
    public PlayerData playerData; // Referencia al jugador

    void Update()
    {
        if (playerData == null) return;
        barraGasolina.fillAmount = playerData.GetGasolina() / playerData.gasolinaMaxima;
        if (barraGasolina.fillAmount <= 0.1f)
            barraGasolina.color = Color.red;
        else if (barraGasolina.fillAmount <= 0.5f)
            barraGasolina.color = Color.yellow;
        else
            barraGasolina.color = Color.green;
    }
}