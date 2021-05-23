using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchomnBageAnimControllDemoScript : MonoBehaviour {

    [Header("Character Obj")]
    public GameObject punchignBagObj;

    [Header("Hit Particle Prefab")]
    public GameObject hitParticle;


    bool canPauseBeforeHit;
    bool isHardHit;
    bool isLighHit;
    bool startHit;

    GameObject particleSpawn;
    Animator bagAnimController;

    void Start ()
    {
       if(punchignBagObj == null)
        {
            punchignBagObj = GameObject.Find("PunchingBag_Heavy");

            if (bagAnimController == null)
            {
               bagAnimController = punchignBagObj.GetComponent<Animator>();
            }

            if (particleSpawn == null)
            {
                particleSpawn = punchignBagObj.transform.Find("ParticleSpawn_Hit").gameObject;
            }
        }


    }

    public void BagHit()
    {
        isLighHit = true;
        if (startHit)
            {
            bagAnimController.SetTrigger("Hit");
            Instantiate(hitParticle, particleSpawn.transform);
            ResetHitBool();
        }
            else StartCoroutine("PauseBeforHit");
        }

    public void BagHitHard()
    {
        isHardHit = true;
        if (startHit)
        {
            bagAnimController.SetTrigger("HitHard");
            Instantiate(hitParticle, particleSpawn.transform);
            ResetHitBool();
        }
        else StartCoroutine("PauseBeforHit");

    }

    public void BagHitSpecial()
    {
        GameObject hitPartices;

        bagAnimController.SetTrigger("HitSpecial");
       hitPartices = Instantiate(hitParticle, particleSpawn.transform);
       hitPartices.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

        hitPartices = Instantiate(hitParticle, particleSpawn.transform);
        ResetHitBool();
    }

    IEnumerator PauseBeforHit()
    {
        yield return new WaitForSeconds(.4f);

        if (isLighHit)
        {
            startHit = true;
            BagHit();
        }
        else if (isHardHit)
        {

            startHit = true;
            BagHit();
        }

        yield return null;
    }

    void ResetHitBool()
    {
        startHit = false;
        isHardHit = false;
        isLighHit = false;
    }
}
