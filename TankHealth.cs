using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour {

    // index tanka; to info pošljemo PlayerManager scripti ko tank umre
    int tankIndex;
    
    // začetna vrednost zdravlja
    int tankStartHealth = 100;
    // trenutna vrednost tanka
    [HideInInspector]
    public int tankCurrentHealth;

    // slider za zdravje tanka
    Slider healthSlider;

    // nastavimo trenutno vrednost zdravja na začetno vrednost
    private void Start()
    {
        tankCurrentHealth = tankStartHealth;
    }

    // zmanjšamo trenutno vrednost zdravja za podano vrednost
    public void DecreseTankHealt(int decreseHealtValue)
    {
        // nastavimo primerno vrednost zdravja
        tankCurrentHealth -= decreseHealtValue;

        // če je trenutno zdravje manj kot 0, ga nastavimo na 0; da se izognemo morebitnim bugom
        if (tankCurrentHealth < 0)
        {
            tankCurrentHealth = 0;
        }

        // popravimo slider za zdravje
        healthSlider.value = tankCurrentHealth;

        // če smo uničili tank (če je tank health 0 ali manj)
        if (tankCurrentHealth <= 0)
        {
            OnTankDeath();
        }
    }

    // funkcija kilcana ko je tank uničen
    void OnTankDeath()
    {
        // izvede funkcijo GameFinished in poda informacijo kateri tank je bil uničen
        GameObject.Find("GameManager").GetComponent<PlayerManager>().GameFinished(tankIndex);
    }

    // nastavljanje tank indexa
    public void SetTankIndex (int _tankIndex)
    {
        // nastavimo primeren tank index
        tankIndex = _tankIndex;

        // poiščemo primeren health slider
        string sliderName = "Slider_Health_Tank_" + tankIndex.ToString();
        healthSlider = GameObject.Find(sliderName).GetComponent<Slider>();
        // nastavimo polno vrednost health sliderja
        healthSlider.value = tankStartHealth;
    }
}
