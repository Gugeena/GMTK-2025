using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private GameObject motherfucker, bowHands, backBow;
    [SerializeField]
    private GameObject motherFuckerPrefab, arrow;
    [SerializeField]
    private GameObject boomerangPrefab;

    public GameObject mfhitbox;

    bool IsJumping = false;

    public GameObject spear;

    public GameObject boomerang;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        direction = 1;
        canPunch = true;

        hp = 150f;

        currentWeapon = 0;

        rPEmitter = runParticles.emission;

         rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {
        print("hp" + hp);
        handleMovement();
        handleCombat();
        handleHP();
    }


    void handleMovement()
    {
        
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (!isDashing) rb.velocity = new Vector2(x * speed, rb.velocity.y);

        isRunning = x != 0;
        anim.SetBool("isWalking", isRunning);
        if (isRunning)
        {
            anim.SetLayerWeight(1, 0.5f);
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
        }

        if(Input.GetMouseButtonDown(1) && canPunch)
        {
            if (currentWeapon == 3 && isGrounded) StartCoroutine(mfSpecial());
        }

        if ((elapsed >= comboTime) || (comboIndex >= punchCombos.Length) || isRunning)
        {
            comboIndex = 0;
        }
        if (comboIndex > 0) elapsed += Time.deltaTime;
    }

    public IEnumerator spearAttack()
    {
        yield break;
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
        mrb.velocity = dir * 4f;

        boomerang.SetActive(false);
        currentWeapon = 0;
        yield break;
    }


    public void handleHP()
    {
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
        print(hp);
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
        anim.Play("player_dash");
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
            motherfucker.SetActive(false);
            spear.SetActive(false);
        }
        else if (id == 1) { }
        else if (id == 2) 
        {
            print(id);
            boomerang.SetActive(true);
        }
        else if (id == 3) {
            print(id);
            motherfucker.SetActive(true);
        }
        else if (id == 4)
        {
            print(id);
            spear.SetActive(true);
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
    }

    public IEnumerator dashCooldown()
    {
        yield return new WaitForSeconds(0.8f);
        canDash = true;
    }

    public IEnumerator damage(int damage)
    {
        invincible = true;
        hp -= damage;
        yield return new WaitForSeconds(0.2f);
        invincible = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("JumpPad"))
        {
            rb.AddForce(transform.up * 425 * rb.gravityScale);
            isGrounded = false;
        }

        /*
        if (collision.gameObject.CompareTag("camTrigger"))
        {
            cameraAnim.Play("camera_rise");
        }
        else if (collision.gameObject.CompareTag("camTriggerDown"))
        {
            cameraAnim.Play("camera_fall");
        }
        */

        if (collision.gameObject.CompareTag("weaponPickup") && currentWeapon == 0)
        {
            StartCoroutine(pickUpWeapon(collision.gameObject.GetComponent<weaponPickupScript>().weaponID));
            Destroy(collision.gameObject);
        }

        if (!invincible)
        {
            if (collision.gameObject.tag == "enemyhitbox")
            {
                StartCoroutine(damage(30));
            }

            if (collision.gameObject.tag == "enemyorb")
            {
                StartCoroutine(damage(25));
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
