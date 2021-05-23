using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.UI;
using UnityEngine;

public class RyunosukeAnimControllDemoScript : MonoBehaviour {

    [Header("Character Obj")]
    public GameObject character;

    [Header ("Game Objects")]
    public GameObject punchingBagObj;
    public GameObject energyBallObj;
    public GameObject fireballObj;
    public GameObject spawnPointEnBall;

    [Header("Numeric Values")]
    public float attackTimeLimit;
    public float specialChargeTime;
    public float fireballSpeed;

    [Header("UI Elements")]
    public Button butWalking;
    public Button butAttacking;
    public Button butBlock;

    // Action booleans
    bool blocking;
    bool walking;
    bool attacking;
    bool hit;

    float speed;
    int attackNumber;
    float attackTime;
    bool startAttackTimer;
    bool canLaunchFIreBall;

    Color ButOrgiginalColor;
    Animator animController;
    PunchomnBageAnimControllDemoScript bagScript;
    GameObject energyBall;
    GameObject fireball;
    Vector3 spawnEnBallLocation;
    Quaternion spawnEnBallRotation;
    GameObject bagHitLocationObj;
    Vector3 bagHitLocation;

    void Start()
    {
        if (punchingBagObj == null)
        {
            punchingBagObj = GameObject.Find("PunchingBag_Heavy");
        }

        bagScript = this.GetComponent<PunchomnBageAnimControllDemoScript>();
        bagHitLocationObj = punchingBagObj.transform.Find("HitLocation").gameObject;

        animController = character.GetComponent<Animator>();
        ButOrgiginalColor = butWalking.colors.normalColor;


    }

    void Update()
    {
        if (startAttackTimer)
        {
            attackTime += Time.deltaTime;

            if (attackTime >= attackTimeLimit || attackNumber > 6)
            {
                attackTime = 0;
                attackNumber = 0;
                attacking = false;
                startAttackTimer = false;
                animController.SetBool("Attacking", attacking);
                animController.SetInteger("AttackInt", attackNumber);
            }
        }

        if(canLaunchFIreBall)
        {
             speed += Time.deltaTime * fireballSpeed;
             Vector3 fireballLocation;

            bagHitLocation = bagHitLocationObj.transform.position;

            fireballLocation = fireball.transform.position;
            fireballLocation = Vector3.MoveTowards(fireballLocation, bagHitLocation, speed);
            fireball.transform.position = fireballLocation;

            if(fireballLocation == bagHitLocation)
            {
                
                bagScript.BagHitSpecial();
                Destroy(fireball);
                canLaunchFIreBall = false;
                fireballLocation = new Vector3(0, 0, 0);
                speed = 0;
            }
        }
    }

    [ContextMenu("Start Attack Time")]
    void startClock()
    {
        if (!startAttackTimer)
        {
            startAttackTimer = true;
        }
        else if (startAttackTimer)
        {
            startAttackTimer = false;
        }
        attackTime = 0;
    }

    public void BagHit()
    {
        bagScript.BagHit();
    }

    public void BagHitHard()
    {
        bagScript.BagHitHard();
    }

    public void AttackButton()
    {
        if (!hit)
        {
            if(attackNumber == 0)
            {
                attacking = true;
                attackNumber = 1;
                animController.SetBool("Attacking", attacking);
                animController.SetInteger("AttackInt", attackNumber);
                attackTime = 0;
                startAttackTimer = true;
            }

            else if(attackNumber > 0)
            {
                attackTime = 0;
                attackNumber ++;
                animController.SetBool("Attacking", attacking);
                animController.SetInteger("AttackInt", attackNumber);
            }

            if (attackNumber >= 4)
            {
                BagHit();
            }
            else BagHitHard();

        }
    }

    void ResetButtBool()
    {
        walking = false;
        attacking = false;
        hit = false;
        blocking = false;

        animController.SetBool("Walking", walking);
        animController.SetBool("Attacking", attacking);
        animController.SetBool("Hit", hit);
        animController.SetBool("Blocking", blocking);

        attackTime = 0;
        attackNumber =0;
        animController.SetInteger("AttackInt", attackNumber);

        var butcolor = butWalking.colors;
        butcolor.normalColor = ButOrgiginalColor;
        butcolor.highlightedColor = ButOrgiginalColor;
        butWalking.colors = butcolor;
        butAttacking.colors = butcolor;
        butBlock.colors = butcolor;
    }

    public void Jump()
    {
        ResetButtBool();
        animController.SetTrigger("Jump");
    }

    public void Walk()
    {
        var butcolor = butWalking.colors;

        if (!walking)
        {
            walking = true;
            butcolor.normalColor = Color.red;
            butcolor.highlightedColor = Color.red;
            butWalking.colors = butcolor;
        }
        else if (walking)
        {
            walking = false;
            butcolor.normalColor = ButOrgiginalColor;
            butcolor.highlightedColor = ButOrgiginalColor;
            butWalking.colors = butcolor;
        }
        animController.SetBool("Walking", walking);

        attacking = false;
        hit = false;
        blocking = false;

        animController.SetBool("Attacking", attacking);
        animController.SetBool("Hit", hit);
        animController.SetBool("Blocking", blocking);

        butcolor.normalColor = ButOrgiginalColor;
        butcolor.highlightedColor = ButOrgiginalColor;
        butAttacking.colors = butcolor;
        butBlock.colors = butcolor;
    }

    public void Blocking()
    {
        var butcolor = butWalking.colors;

        if (!blocking)
        {
            blocking = true;
            butcolor.normalColor = Color.red;
            butcolor.highlightedColor = Color.red;
            butBlock.colors = butcolor;
        }
        else if (blocking)
        {
            blocking = false;
            butcolor.normalColor = ButOrgiginalColor;
            butcolor.highlightedColor = ButOrgiginalColor;
            butBlock.colors = butcolor;
        }
        animController.SetBool("Blocking", blocking);

        walking = false;
        attacking = false;
        hit = false;

        animController.SetBool("Walking", walking);
        animController.SetBool("Attacking", attacking);
        animController.SetBool("Hit", hit);

        butcolor.normalColor = ButOrgiginalColor;
        butcolor.highlightedColor = ButOrgiginalColor;
        butWalking.colors = butcolor;
        butAttacking.colors = butcolor;
    }

    public void Special01()
    {
        ResetButtBool();
        animController.SetTrigger("Special01");
        spawnEnBallLocation = spawnPointEnBall.transform.position;
        spawnEnBallRotation = spawnPointEnBall.transform.localRotation;

        energyBall = Instantiate(energyBallObj, spawnEnBallLocation, spawnEnBallRotation);
        energyBall.transform.parent = spawnPointEnBall.transform;
        StartCoroutine("FireBall");
        //BagHitHard();
    }
    IEnumerator FireBall()
    {
        yield return new WaitForSeconds(specialChargeTime);
        Destroy(energyBall);

        spawnEnBallLocation = spawnPointEnBall.transform.position;
        fireball = Instantiate(fireballObj, spawnEnBallLocation, new Quaternion(0,0.5f,0,1));

        canLaunchFIreBall = true;
        yield return null;
    }

    public void Attack01()
    {
        ResetButtBool();
        attackNumber = 1;
        attacking = true;
        animController.SetBool("Attacking", attacking);
        animController.SetInteger("AttackInt", attackNumber);
        StartCoroutine("ResetAtacking");
        BagHit();
        // animController.Play("Attack01");

    }
    public void Attack02()
    {
        ResetButtBool();
        attackNumber = 2;
        attacking = true;
        animController.SetBool("Attacking", attacking);
        animController.SetInteger("AttackInt", attackNumber);
        StartCoroutine("ResetAtacking");
        BagHit();
        //animController.Play("Attack02");
    }
    public void Attack03()
    {
        ResetButtBool();
        attackNumber = 3;
        attacking = true;
        animController.SetBool("Attacking", attacking);
        animController.SetInteger("AttackInt", attackNumber);
        StartCoroutine("ResetAtacking");
        BagHit();
        // animController.Play("Attack03");
    }
    public void Attack04()
    {
        ResetButtBool();
        attackNumber = 4;
        attacking = true;
        animController.SetBool("Attacking", attacking);
        animController.SetInteger("AttackInt", attackNumber);
        StartCoroutine("ResetAtacking");
        BagHit();
        // animController.Play("Attack04");
    }
    public void Attack05()
    {
        ResetButtBool();
        attackNumber = 5;
        attacking = true;
        animController.SetBool("Attacking", attacking);
        animController.SetInteger("AttackInt", attackNumber);
        StartCoroutine("ResetAtacking");
        BagHitHard();
        //animController.Play("Attack05");
    }
    public void Attack06()
    {
        ResetButtBool();
        attackNumber = 6;
        attacking = true;
        animController.SetBool("Attacking", attacking);
        animController.SetInteger("AttackInt", attackNumber);
        StartCoroutine("ResetAtacking");
        BagHitHard();
        //animController.Play("Attack06");
    }

    public void Hit01()
    {
        ResetButtBool();
        animController.Play("Hit_Mid_Light");
    }
    public void Hit02()
    {
        ResetButtBool();
        animController.Play("Hit_Mid_Medium");
    }
    public void Hit03()
    {
        ResetButtBool();
        animController.Play("Hit_Mid_Hard");
    }

    public void Hit04()
    {
        ResetButtBool();
        animController.Play("Hit_High_Light");
    }
    public void Hit05()
    {
        ResetButtBool();
        animController.Play("Hit_High_Medium");
    }
    public void Hit06()
    {
        ResetButtBool();
        animController.Play("Hit_High_Hard");
    }

    IEnumerator ResetAtacking()
    {
        yield return new WaitForSeconds(attackTimeLimit);

        ResetButtBool();

        yield return null;


    }
}
