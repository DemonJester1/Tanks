using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    // 2 skripti
    TankFire fireScript;
    TankMoveAndAim moveAndAimScript;

    // Slike z gumbi za premike, merjenje, moč, ...
    GameObject[] img_SelectionImageGameObjects;
    // slika ki se pokaže ko je igra končana
    public GameObject endGameImageGameObject;
    // napis na začetku runde; kateri igralec je navrsti
    GameObject playersTurnGameObject;

    // index igralca ki je navrsti
    int playerOnHisTurn;
    // array vseh tankov
    public GameObject[] tanks;

    // bool ki pove ali čakamo preden začne rundo naslednji igralec
    bool waitingForNextPlayer;

    /// timer properties
    /// 
    // koliko časa ima igralec vsako rundo
    public float startRoundTime = 30f;
    // koliko časa ima igralec še to rundo
    float roundTimeLeft;
    // text za izpis preostalega časa
    Text timerText;
    // ali se čas odšteva ali ne
    bool timerIsCounting;

    private void Start()
    {
        // poiščemo slike
        img_SelectionImageGameObjects = new GameObject[4];
        img_SelectionImageGameObjects[0] = GameObject.Find("Img_00_Blank");
        img_SelectionImageGameObjects[1] = GameObject.Find("Img_01_BasicSelection");
        img_SelectionImageGameObjects[2] = GameObject.Find("Img_02_MoveSelection");
        img_SelectionImageGameObjects[3] = GameObject.Find("Img_03_AimSelection");

        // poiščemo text za timer
        timerText = GameObject.Find("Txt_Timer").GetComponent<Text>();
        // izpišemo timer z začetno vrednostjo; F1 pomeni z eno decimalko;
        timerText.text = startRoundTime.ToString("F1");

        // poišče napis za naslednjega igralca
        playersTurnGameObject = GameObject.Find("Txt_PlayersTurn");
        // deaktivira to sliko
        playersTurnGameObject.SetActive(false);
    }

    // za odštevanje časa
    private void Update()
    {
        // če odštevamo (timerIsCounting == true);
        if (timerIsCounting)
        {
            // odštejemo za deta time
            roundTimeLeft -= Time.unscaledDeltaTime;
            // izpišemo text trenutnega časa; F1 pomeni z eno decimalko;
            timerText.text = roundTimeLeft.ToString("F1");
        }

        // če smo prišli do 0 sec
        if (roundTimeLeft < 0)
        {
            // ne bomo več odštevali
            timerIsCounting = false;
            // nastavi čas na 0; da ne bo bugov
            roundTimeLeft = 0f;
            // izpiši text; F1 pomeni z eno decimalko;
            timerText.text = roundTimeLeft.ToString("F1");
            // začni korutino za naslednjega igralca
            StartCoroutine(WaitThenStartNextPlayerTrun());
        }
    }

    /// <summary>
    /// vedno ustvari prevelik array
    /// mislim da je sama skripta prehitra in se sami tanki ne uničijo dovolj hitro
    /// ker je skripta prej tukaj kot se tanki uničijo ustvari array z dodatnimi tanki ki se potem uničijo
    /// tako ostane array z praznimi zapisi v njem...
    /// tukaj dejansko želim samo aktivne tanke tako da sem zaenkrat naredil manjši bypass
    /// </summary>
    public IEnumerator NewRoundStarted()
    {
        Debug.LogWarning("Attention needed in this script!\n Double Click to open.");
        // počaka 0,2 sekuni; zakaj? glej summary zgoraj;
        yield return new WaitForSeconds(0.2f);

        // da ne odštvamo časa že takoj od začetka
        timerIsCounting = false;

        // nastavi potrebno za začetek nove runde
        SetUpNewRound();

        // nehamo čakat 
        waitingForNextPlayer = false;
        
        // če je endgame image active, ga daj na inactive
        if (endGameImageGameObject.activeInHierarchy)
        {
            endGameImageGameObject.SetActive(false);
        }
        yield return null;
    }

    // samo na začtku igre, ko je ustvarjena nova mapa
    void SetUpNewRound()
    {
        // index igralca navrsti damo na -1; tako ni nihče na vrsti
        playerOnHisTurn = -1;

        // poiščemo vse tanke v igri
        tanks = GameObject.FindGameObjectsWithTag("Player");
        // vsakemu povemo kdo je navrsti
        foreach (GameObject tank in tanks)
        {
            if (tank != null)
                tank.GetComponent<TankMoveAndAim>().SetSelectedTank(playerOnHisTurn);
        }

        // začnemo korutino za začetek nasljednjega igralca
        StartCoroutine(WaitThenStartNextPlayerTrun());
    }

    // korutina za začetek nasljednjega igralca
    IEnumerator WaitThenStartNextPlayerTrun()
    {

        /// Waiting ...
        /// 
        // timerja še ne odšteva
        timerIsCounting = false;

        // please wait sliko nastavi na active
        if (!img_SelectionImageGameObjects[0].activeInHierarchy)
        {
            img_SelectionImageGameObjects[0].SetActive(true);
        }

        // čakamo dokler je true ... ; na false se postavi ko izteče timer, ali ko se uniči iztrelek
        while (waitingForNextPlayer)
        {
            yield return null;
        }
        
        /// start next tanks turn
        /// 
        // določimo index naslednjega igralca
        int nextPlayerIndex = playerOnHisTurn + 1;
        // če je index večji ali enak številu vseh tankov, je na vrsti spet tank z indexom 0;
        if (nextPlayerIndex >= tanks.Length)
        {
            nextPlayerIndex = 0;
        }

        // nastavimo index igralca na vrsti
        playerOnHisTurn = nextPlayerIndex;
        // pošljemo ta index vsem tankom
        foreach (GameObject tank in tanks)
        {
            if (tank != null)
                tank.GetComponent<TankMoveAndAim>().SetSelectedTank(playerOnHisTurn);
        }

        // aktiviramo vse slike z gumbi
        foreach (GameObject imageGO in img_SelectionImageGameObjects)
        {
            if (!imageGO.activeInHierarchy)
            {
                imageGO.SetActive(true);
            }
        }

        // poiščemo primerne skritpe na tankih
        fireScript = tanks[playerOnHisTurn].GetComponent<TankFire>();
        moveAndAimScript = tanks[playerOnHisTurn].GetComponent<TankMoveAndAim>();

        // deaktiviramo merjeneje z dotikom če je slučajno še aktivirano; to bi se zgodilo če se odšteje čas ko še merimo;
        TouchAimDeactivate();

        // nastavi sliderje in input fielde; just back check
        fireScript.FirePowerChangeBySliderOrInputField(false, false);
        moveAndAimScript.AimValueChangeBySliderOrInputField(false, false);

        // izpiše kateri igralec je na vrsti
        playersTurnGameObject.GetComponent<Text>().text = "Player " + playerOnHisTurn + "!";
        // aktivira sliko
        playersTurnGameObject.SetActive(true);
        // počaka #.#f sekund; ta čas je napis aktiven
        while (true)
        {
            yield return new WaitForSeconds(1.5f);
            break;
        }
        // ne rabimo slike ki pravi Please Wait;
        img_SelectionImageGameObjects[0].SetActive(false);

        // deaktiviramo napis;
        playersTurnGameObject.SetActive(false);

        // nastavimo preostali čas na začetni čas
        roundTimeLeft = startRoundTime;
        // začnemo šteti
        timerIsCounting = true;
        yield return null;
    }

    // funkcija da prenehamo čakati za začetek runde naslednjega igralca; kilcana ko je iztrelek uničen ali ko odleti iz mape;
    public void StopWaitingForNextPlayer ()
    {
            waitingForNextPlayer = false;
    }

    // konec igre; ko je uničen en od tankov;
    public void GameFinished (int tankDefetedIndex)
    {
        // aktiviraj sliko
        endGameImageGameObject.SetActive(true);
        // prenehaj šteti čas
        timerIsCounting = false;
        // ustavi vse koorutine v tej skripti
        StopAllCoroutines();
        // določi zmagovalca
        int winner = tankDefetedIndex + 1;
        // če je winner večji od števila vseh tankov, potem je zmagovalec igalec 0;
        if (winner >= tanks.Length)
            winner = 0;
        // poišči text in izpiši zmagovalca
        GameObject.Find("Txt_Winner").GetComponent<Text>().text = "Player " + winner + " has won!";
    }


    #region Button Slider & InputField Functions
    /// funkcije za gumbe
    /// 
    // gumb: končaj premike tanka; on pointer up;
    public void MoveStop()
    {
        moveAndAimScript.MoveTank(0);
    }

    // gumb: premik tanka v desno; on pointer down;
    public void MoveRight()
    {
        moveAndAimScript.MoveTank(1);
    }

    // gumb: premik tanka v levo; on pointer down;
    public void MoveLeft()
    {
        moveAndAimScript.MoveTank(-1);
    }

    // gumb: povečanje kota rotacije cevi; on click;
    public void IncreseAimAngel()
    {
        moveAndAimScript.ChangeCevRotationValue(1);
    }

    // gumb: zmanjšanje kota rotacije cevi; on click;
    public void DecreseAimAnge()
    {
        moveAndAimScript.ChangeCevRotationValue(-1);
    }

    // gumb: za aktivacijo spremembe rotacije cevi s klikom; aktivira ko gremo na aim menu; on click;
    public void TouchAimActivate()
    {
        moveAndAimScript.touchAimActive = true;
    }

    // gumb: za deaktivacijo spremembe rotacije cevi s klikom; aktivira ko gremo iz aim menuja; on click;
    public void TouchAimDeactivate()
    {
        moveAndAimScript.touchAimActive = false;
    }

    // gumb: povečanje moči iztrelka; on click;
    public void IncreseFirePower()
    {
        fireScript.ChangeFirePower(1);
    }

    // gumb: zmanjšanje moči iztrelka; on click;
    public void DecreseFirePover()
    {
        fireScript.ChangeFirePower(-1);
    }

    // gumb: iztreli missile; on click;
    public void FireMissle()
    {
        fireScript.FireMissile();

        // začnemo čakat na naslednjega igralca
        waitingForNextPlayer = true;
        StartCoroutine(WaitThenStartNextPlayerTrun());
    }

    /// sliders and inputField functions
    /// 
    // fire power slider; on value change;
    public void FirePowerSliderChanged ()
    {
        fireScript.FirePowerChangeBySliderOrInputField(true, false);
    }

    // fire power input field; on value change;
    public void FirePowerInputFieldChanged ()
    {
        fireScript.FirePowerChangeBySliderOrInputField(false, true);
    }

    // aim rotation slider; on value change;
    public void AimRotationSliderChanged()
    {
        moveAndAimScript.AimValueChangeBySliderOrInputField(true, false);
    }

    // aim rotation slider; on value change;
    public void AimRotationInputFieldChanged()
    {
        moveAndAimScript.AimValueChangeBySliderOrInputField(false, true);
    }

    #endregion
}
