using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed, dashForce, jumpForce;
    [SerializeField]
    private Transform camFollowTransform, headPivotTransform;

    [SerializeField]
    private AnimationClip[] punchCombos;
    [SerializeField]
    private GameObject leftHand, rightHand;

    [SerializeField]
    private Animator cameraAnim;

    private int comboIndex = 0;
    [SerializeField]
    private float comboTime;
    private float elapsed;

    [SerializeField]
    private ParticleSystem runParticles;

    private ParticleSystem.EmissionModule rPEmitter;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isRunning, isDashing, isGrounded, isFalling;
    private bool canPunch;

    private int direction;

    public Transform RLocation;
    public Transform LLocation;

    public bool canDash = true;

    public GameObject meleehitbox;
    public static float hp = 0;
    public bool invincible = false;

    bool canLose = true;

    public UnityEngine.UI.Slider hpslider;
    float hpCurVel = 0f;

    [SerializeField]
    public int currentWeapon = 0; //0 - Fists // 1 - bow // 2 - Boomerang // 3 - motherfucker // 4 - spear

    [Header("Weapons")]
    [SerializeField]
    private GameObject motherfucker;
    [SerializeField]
    private GameObject motherFuckerPrefab, arrow, bowHands;
    [SerializeField]
    private GameObject boomerangPrefab;

    public GameObject mfhitbox;

    private bool hasArrow;

    bool IsJumping = false;

    public GameObject spear;

    public GameObject boomerang;

    public GameObject spearhitbox;

    public GameObject spearPrefab;

    public bool ukvegadavaida = false;

    public GameObject fadeOut;

    public GameObject hurtparticle;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        direction = 1;
        canPunch = true;

        hp = 150f;

        StartCoroutine(pickUpWeapon(0));

        rPEmitter = runParticles.emission;

        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseScript.Paused) return;
        handleMovement();
        handleCombat();
        handleHP();
        if (currentWeapon == 1) bowAim();
    }


    void handleMovement()
    {
        if(Input.GetKeyDown(KeyCode.Q) && currentWeapon != 0)
        {
            print("shevedi axla iaragebi unda gavtisho");
            StartCoroutine(pickUpWeapon(0));
        }

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (!isDashing) rb.velocity = new Vector2(x * speed, rb.velocity.y);

        isRunning = x != 0;

        anim.SetBool("isWalking", isRunning);
        if (isRunning)
        {
            if (currentWeapon == 0 || currentWeapon == 3)
            {
                anim.SetLayerWeight(1, 0.5f);
            }
         
            if (!runParticles.isPlaying) rPEmitter.enabled = true;

            if (isGrounded)
            {
                var emission = runParticles.emission;
                emission.enabled = true;
            }
            else
            {
                var emission = runParticles.emission;
                emission.enabled = false;
            }
        }
        else 
        { 
            anim.SetLayerWeight(1, 1f);
            var emission = runParticles.emission;
            emission.enabled = false;
        }
        if (isFalling)
        {
            rb.gravityScale = 3f;
        }
        else
        {
            rb.gravityScale = 1f;
        }

        if (!isDashing)
        {
            if (x < 0 && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                direction = -1;
            }
            else if (x > 0 && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                direction = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && canDash)
        {
            canDash = false;
            StartCoroutine(dash());
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !IsJumping)
        {
            StartCoroutine(jump());
        }

        isFalling = rb.velocity.y < -0.1f;
        anim.SetBool("isFalling", isFalling);
        anim.SetBool("isJumping", !isGrounded);
    }

    void handleCombat()
    {
        if (Input.GetMouseButtonDown(0) && canPunch)
        {
            if (currentWeapon == 0) StartCoroutine(punch());
            else if (currentWeapon == 3) StartCoroutine(mfAttack());
            else if (currentWeapon == 4) StartCoroutine(spearAttack());
            else if (currentWeapon == 2) StartCoroutine(boomerangAttack());
            else if (currentWeapon == 1 && hasArrow) StartCoroutine(bowShoot());
        }

        if(Input.GetMouseButtonDown(1) && canPunch)
        {
            if (currentWeapon == 3 && isGrounded) StartCoroutine(mfSpecial());
            if (currentWeapon == 4) StartCoroutine(spearspecialAttack());
        }

        if ((elapsed >= comboTime) || (comboIndex >= punchCombos.Length) || isRunning)
        {
            comboIndex = 0;
        }
        if (comboIndex > 0) elapsed += Time.deltaTime;
    }

    public IEnumerator spearAttack()
    {
        canPunch = false;
        anim.Play("player_spearattack");
        yield return new WaitForSeconds(0.4f);
        spearhitbox.SetActive(true);
        yield return new WaitForSeconds(0.12f);
        spearhitbox.SetActive(false);
        yield return new WaitForSeconds(0.7f);
        canPunch = true;
    }

    public IEnumerator spearspecialAttack()
    {
        canPunch = false;
        anim.Play("player_spearspecialattack");
        yield return new WaitForSeconds(1.2f);
        Transform temp = transform.GetChild(3).GetChild(2);
        Vector2 mfPos = temp.position;
        Quaternion mfRot = temp.rotation;
        GameObject m = Instantiate(spearPrefab, mfPos, mfRot);
        spear.SetActive(false);
        currentWeapon = 0;
        canPunch = true;
        anim.SetBool("shouldChargeIn", false);
    }

    public IEnumerator boomerangAttack()
    {
        //anim.Play("player_mf_special");
        //yield return new WaitForSeconds(1.2f);
        Transform temp = transform.GetChild(3).GetChild(3);
        Vector2 mfPos = temp.position;
        Quaternion mfRot = temp.rotation;
        GameObject m = Instantiate(boomerangPrefab, mfPos, mfRot);
        Rigidbody2D mrb = m.GetComponent<Rigidbody2D>();


        Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // gamoitvlis in world space sad aris mouse
        Vector2 dir = mousepos - mfPos; // gvadzlevs directions -1;0 for left da egeti shit boomerangidan mausamde
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // radianebidan gadaaq degreeshi'
        mrb.velocity = dir.normalized * 15f;

        boomerang.SetActive(false);
        currentWeapon = 0;
        yield break;
    }

    public IEnumerator gadasvla()
    {
        ukvegadavaida = true;
        fadeOut.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene(3);
    }

    public void handleHP()
    {
        if(hp <= 0 && !ukvegadavaida)
        {
            StartCoroutine(gadasvla());
        }

        if (hpslider.value > 150) hpslider.value = 150;
        hp = Mathf.Clamp(hp, 0, 150);
        float currHP = Mathf.SmoothDamp(hpslider.value, hp, ref hpCurVel, 0.2f);
        hpslider.value = currHP;

        if (hp < 150 && canLose)
        {
            canLose = false;
            StartCoroutine(losehp());
        }
    }


    public IEnumerator losehp()
    {
        hp++;
        yield return new WaitForSeconds(0.2f);
        canLose = true;
    }

    private IEnumerator punch()
    {
        canPunch = false;
        anim.Play(punchCombos[comboIndex].name);
        meleehitbox.SetActive(true);
        yield return new WaitForSeconds(0.12f);
        meleehitbox.SetActive(false);
        comboIndex++;
        elapsed = 0;
        yield return new WaitForSeconds(0.4f);
        canPunch = true;
    }

    private IEnumerator mfAttack()
    {
        canPunch = false;
        anim.Play("player_mf_attack");
        yield return new WaitForSeconds(0.6f);
        mfhitbox.SetActive(true);
        yield return new WaitForSeconds(0.12f);
        mfhitbox.SetActive(false);
        yield return new WaitForSeconds(0.7f);
        canPunch = true;
    }


    private IEnumerator mfSpecial()
    {
        canPunch = false;
        anim.Play("player_mf_special");
        yield return new WaitForSeconds(1.2f);
        Transform temp = transform.GetChild(3).GetChild(1);
        Vector2 mfPos = temp.position;
        Quaternion mfRot = temp.rotation;
        GameObject m = Instantiate(motherFuckerPrefab, mfPos, mfRot);
        m.GetComponent<mfScript>().direction = direction;
        motherfucker.SetActive(false);
        currentWeapon = 0;
        canPunch = true;
    }


    private IEnumerator bowShoot()
    {
        canPunch = false;
        //anim.Play("player_bowShoot");
        yield return new WaitForSeconds(0.4f);
        hasArrow = false;
        GameObject arr = Instantiate(arrow, bowHands.transform.position, bowHands.transform.rotation);
        arr.GetComponent<Rigidbody2D>().AddForce(arr.transform.right * 200 * direction);
        canPunch = true;
    }

    private void bowAim()
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 aimDirection = (mousePos - transform.position).normalized * direction;

        float angle = (Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg);

        bowHands.transform.eulerAngles = new Vector3(0, 0, angle);


    }

    private IEnumerator dash()
    {
        print("dashing");
        isDashing = true;
        rb.gravityScale = 0f;
        if (!isGrounded)
        {
            rb.AddForce(transform.right * direction * (dashForce - 200));
        }
        else
        {
            rb.AddForce(transform.right * direction * dashForce);
        }
        if(currentWeapon != 4) anim.Play("player_dash");
        else anim.Play("player_speardash");
        yield return new WaitForSeconds(0.3f);
        isDashing = false;
        rb.gravityScale = 1f;
        StartCoroutine(dashCooldown());
        yield break;
    }
    private IEnumerator jump()
    {
        IsJumping = true;
        rb.AddForce(transform.up * jumpForce);
        isGrounded = false;
        yield return null;
        IsJumping = false;
    }

    private IEnumerator pickUpWeapon(int id)
    {
        currentWeapon = id;
        
        if (id == 0)
        {
            print("Settings =weapons==off=");
            motherfucker.SetActive(false);
            spear.SetActive(false);
            boomerang.SetActive(false);

            bowHands.SetActive(false);
            leftHand.SetActive(true);
            rightHand.SetActive(true);
            anim.SetBool("shouldChargeIn", false);

            currentWeapon = 0;
        }
        else if (id == 1)
        {
            hasArrow = true;
            bowHands.SetActive(true);
            leftHand.SetActive(false);
            rightHand.SetActive(false);
        }
        else if (id == 2) 
        {
            boomerang.SetActive(true);

            motherfucker.SetActive(false);
            spear.SetActive(false);

            bowHands.SetActive(false);
            leftHand.SetActive(true);
            rightHand.SetActive(true);
            anim.SetBool("shouldChargeIn", false);

        }
        else if (id == 3) {
            motherfucker.SetActive(true);

            spear.SetActive(false);
            boomerang.SetActive(false);
        }
        else if (id == 4)
        {
            spear.SetActive(true);
            motherfucker.SetActive(false);
            spear.SetActive(false);

            anim.SetBool("shouldChargeIn", true);

            bowHands.SetActive(false);
            leftHand.SetActive(true);
            rightHand.SetActive(true);
        }
        yield return null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "LLocation")
        {
            this.transform.position = new Vector3(RLocation.position.x, this.transform.position.y, 0);
            if (isDashing)
            {
                //direction = -1;
                StartCoroutine(dash());
            }
        }
        else if (collision.gameObject.name == "RLocation")
        {
            this.transform.position = new Vector3(LLocation.position.x, this.transform.position.y, 0);
            if (isDashing)
            {
                //direction = -1;
                StartCoroutine(dash());
            }
        }

        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 8)
        {
            isGrounded = true;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            print("hit by enemy itself");
            /*
            hp -= 5;
            float direction = Mathf.Sign(-transform.localScale.x);
            float knockback = 4f;
            Vector2 force = new Vector2(-direction, 0);
            rb.AddForce(force * (knockback * 100f), ForceMode2D.Impulse);
            */
        }
    }

    public IEnumerator dashCooldown()
    {
        yield return new WaitForSeconds(0.8f);
        canDash = true;
    }

    public IEnumerator damage(int damage)
    {
        Instantiate(hurtparticle, new Vector2(transform.position.x, transform.position.y - 0.75f), Quaternion.identity);
        PauseScript.dmg += damage;
        invincible = true;
        hp -= damage;
        yield return new WaitForSeconds(0.2f);
        invincible = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("weaponPickup") && currentWeapon == 0)
        {
            print("iaragis ageba: " + collision.gameObject.GetComponent<weaponPickupScript>().weaponID);
            StartCoroutine(pickUpWeapon(collision.gameObject.GetComponent<weaponPickupScript>().weaponID));
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("arrowPickup"))
        {
            hasArrow = true;
            Destroy(collision.gameObject.transform.parent.gameObject);
        }

        if (!invincible)
        {
            if (collision.gameObject.tag == "enemyhitbox")
            {
                /*
                float direction = Mathf.Sign(-transform.localScale.x);
                float knockback = 4f;
                Vector2 force = new Vector2(-direction, 0);
                rb.AddForce(force * (knockback * 100f), ForceMode2D.Impulse);
                */
                StartCoroutine(damage(30));
                print("hit by enemy hitbox");
            }

            if (collision.gameObject.tag == "enemyorb")
            {
                 /*
                float direction = Mathf.Sign(-transform.localScale.x);
                float knockback = 4f;
                Vector2 force = new Vector2(-direction, 0);
                rb.AddForce(force * (knockback * 100f), ForceMode2D.Impulse);
                */
                StartCoroutine(damage(25));
            }

            if (collision.gameObject.tag == "Enemy")
            {
                /*
                hp -= 5;
                float direction = Mathf.Sign(transform.localScale.x);
                float knockback = 4f;
                Vector2 force = new Vector2(-direction, 0);
                 */
                //rb.AddForce(force * (knockback * 10f), ForceMode2D.Impulse);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 8)
        {
            isGrounded = false;
        }
    }
}
