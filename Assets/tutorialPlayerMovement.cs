using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class tutorialPlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed, dashForce, jumpForce;
    [SerializeField]
    private Transform headPivotTransform, headTransform;

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


    [SerializeField]
    public int currentWeapon = 0; //0 - Fists // 1 - bow // 2 - Boomerang // 3 - motherfucker

    [Header("Weapons")]
    [SerializeField]
    private GameObject arrow, bowHands;

    public GameObject mfhitbox;

    bool IsJumping = false;


    private bool trapped;
    private ShakeSelfScript shakeSelfScript;
    private int moveCount =0;
    
    [SerializeField]
    private Animator cineAnim;

    [SerializeField]
    private ParticleSystem chainParticle;
    [SerializeField]
    private GameObject[] shackles;

    private bool bowScene = false;
    private bool hasArrow = true;

    // Start is called before the first frame update
    void Start()
    {
        moveCount = 0;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        shakeSelfScript = GetComponent<ShakeSelfScript>();
        direction = 1;
        canPunch = true;
        trapped = true;

        var emission = runParticles.emission;
        emission.enabled = false;

        currentWeapon = 0;
        rPEmitter = runParticles.emission;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;


        if (trapped)
        {
            rb.simulated = false;
            anim.SetBool("trapped", true);
            anim.Play("player_chained");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (trapped == false && bowScene == false)
        {
            handleMovement();
            handleCombat();
        }
        else if (trapped == true) handleChained();

        if (currentWeapon == 1 && bowScene == false) bowAim();
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
            if (currentWeapon == 0) anim.SetLayerWeight(1, 0.5f);
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
            else if (currentWeapon == 1 && hasArrow) StartCoroutine(bowShoot());
        }


        if ((elapsed >= comboTime) || (comboIndex >= punchCombos.Length) || isRunning)
        {
            comboIndex = 0;
        }
        if (comboIndex > 0) elapsed += Time.deltaTime;
    }
    void handleChained()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 pos = Vector2.Lerp(mousePos, headPivotTransform.position, 0.992f);
        pos.z = 0;
        headTransform.position = pos;

        Vector3 aimDirection = (mousePos - transform.position).normalized;

        float angle = (Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg);

        headTransform.eulerAngles = new Vector3(0, 0, angle);


        float x = mousePos.x - transform.position.x;
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

        if (Input.anyKeyDown && moveCount < 5)
        {
            StartCoroutine(escape());
        }
        else if (Input.anyKeyDown && moveCount >= 5)
        {
            StartCoroutine(breakChains());
        }

    }

    private IEnumerator escape()
    {
        shakeSelfScript.Begin();
        yield return new WaitForSeconds(0.23f);
        shakeSelfScript.stopShake();
        moveCount++;
    }

    private IEnumerator breakChains()
    {
        rb.simulated = true;
        trapped = false;
        anim.SetBool("trapped", false);
        chainParticle.gameObject.SetActive(true);
        chainParticle.Play();
        foreach(GameObject g in shackles)
        {
            Rigidbody2D rb = g.GetComponent<Rigidbody2D>();
            rb.simulated = true;
            float randomX = Random.Range(-5f, 5f);
            float randomY = Random.Range(0f, 10f);
            Vector2 random = new Vector2(randomX, randomY);
            rb.AddForce(random * 10);
            rb.AddTorque(Random.Range(-5f, 5f) * 15);
        }
        Time.timeScale = 0.2f;
        cineAnim.Play("cinecam_zoomin");
        yield return null;

    }

    private IEnumerator bowCutscene()
    {
        rb.velocity = Vector2.zero;
        isRunning = false;
        isDashing = false;
        isFalling = false;
        anim.SetBool("isWalking", false);
        anim.SetBool("isFalling", false);
        anim.SetBool("isJumping", false);

        canPunch = false;
        anim.SetBool("isWalking", false);
        anim.Play("player_bowShoot");
        yield return new WaitForSeconds(0.4f);
        Time.timeScale = 0.4f;
        GameObject arr = Instantiate(arrow, bowHands.transform.position, bowHands.transform.rotation);
        arr.GetComponent<Rigidbody2D>().gravityScale = 0.2f;
        arr.GetComponent<Rigidbody2D>().AddForce(arr.transform.right * 400 * direction);
        canPunch = true;
        yield return new WaitForSeconds(1.3f);
        Time.timeScale = 1;
        bowScene = false;
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
            bowHands.SetActive(false);
            leftHand.SetActive(true);
            rightHand.SetActive(true);
        }
        else if (id == 1)
        {
            bowHands.SetActive(true);
            leftHand.SetActive(false);
            rightHand.SetActive(false);
            bowScene = true;
            StartCoroutine(bowCutscene());
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

        if (collision.gameObject.layer == 3)
        {
            isGrounded = true;
        }
        if (collision.gameObject.layer == 3 && Time.timeScale < 1)
        {
            Time.timeScale = 1;
            cineAnim.Play("cinecam_zoomout");
        }
        if (collision.gameObject.CompareTag("endTutorial"))
        {
            StartCoroutine(endTutor());
        }
    }

    public IEnumerator dashCooldown()
    {
        yield return new WaitForSeconds(0.8f);
        canDash = true;
    }

    private IEnumerator bowShoot()
    {
        canPunch = false;
        anim.Play("player_bowShoot");
        yield return new WaitForSeconds(0.4f);
        hasArrow = false;
        GameObject arr = Instantiate(arrow, bowHands.transform.position, bowHands.transform.rotation);
        arr.GetComponent<Rigidbody2D>().gravityScale = 0.5f;
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

    private IEnumerator endTutor() {
        cineAnim.Play("cinecam_end");
        yield return new WaitForSeconds(3.5f);
        loadScene.SceneToLoad = 3;
        SceneManager.LoadScene(1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("weaponPickup") && currentWeapon == 0)
        {
            StartCoroutine(pickUpWeapon(collision.gameObject.GetComponent<weaponPickupScript>().weaponID));
            Destroy(collision.gameObject);
        }


        if (collision.gameObject.CompareTag("arrowPickup"))
        {
            hasArrow = true;
            Destroy(collision.gameObject.transform.parent.gameObject);
        }

        if (collision.gameObject.CompareTag("endTutorial"))
        {
            StartCoroutine(endTutor());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            isGrounded = false;
        }

    }
}
