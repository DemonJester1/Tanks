using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileCollision : MonoBehaviour {

    // 2 skripit ki jih poiščemo v Start funkciji
    MapManager mapManagerScript;
    PlayerManager playerManagerScript;

    // informacija o mapi
    int[] heightArray;

    private void Start()
    {
        // poiščemo skripte
        mapManagerScript = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        playerManagerScript = GameObject.Find("GameManager").GetComponent<PlayerManager>();
        
        // poiščemo lastnost mape
        heightArray = mapManagerScript.heightArray;
        //InvokeRepeating("CheckForGroundCollision", 0f, 0.005f);
    }
    
    private void Update()
    {
        // uniči iztrelek če je pod mapo - če odleti levo ali desno iz mape
        if (transform.position.x < -5 || transform.position.x > heightArray.Length + 5)
        {
            // nehamo čakat na nasljednega igralca; more biti pred Destroy, ker drugače to nebi izvedlo (ničena bi bila tudi skripta);
            playerManagerScript.StopWaitingForNextPlayer();
            // uničimo iztrelek
            Destroy(gameObject);
        }

        /// by-passing ground collider; samo back check
        ///
        // če smo v mapi
        if (transform.position.x > 0 && transform.position.x < heightArray.Length - 1)
        {
            // če je pozicija iztrelka nižja od višine grounda na tem mestu
            if (transform.position.y <= heightArray[((int)transform.position.x)])
            {
                HitDetected();
            }
        }
    }

    // če zadanemo collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // če smo zadeli tank
        if (collision.tag == "Player")
        {
            // zmanjšaj health tanka
            collision.GetComponent<TankHealth>().DecreseTankHealt(15);

        }
        HitDetected();
    }

    // sem dal kar v svojo funkcijo da je manj kode
    void HitDetected ()
    {
        // pridobimo info o lokaciji kjer smo zadeli collider oz. ground; enaka trenutni poziciji iztrelka
        Vector2 collisionPointMap = new Vector2(transform.position.x, transform.position.y);
        // uniči morebitno mapo okoli te pozicije
        mapManagerScript.StartDestroyAroundPointOnMap((int)collisionPointMap.x, (int)collisionPointMap.y, 15);

        /// nehamo čakat na nasljednega igralca; more biti pred Destroy, ker drugače to nebi izvedlo (ničena bi bila tudi skripta);
        ///playerManagerScript.StopWaitingForNextPlayer();   // če iščes tole se je premaknilo v MapManagment skripto na konec "SendMapInfoToTanks()" funkcije
        
        // uničimo iztrelek
        Destroy(gameObject);
    }
}
