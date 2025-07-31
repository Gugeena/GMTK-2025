using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public Sprite boomerangSprite;
    public Sprite spearSprite;
    public Sprite mfSprite;
    public Sprite bowSprite;
    public Sprite mjolnirSprite;

    public bool Boomerang, Spear, MF, Bow, Mjolnir;

    private SpriteRenderer spriteRenderer;

    Vector3 mousePos;
    Vector3 direction;
    float angle;

    Rigidbody2D rb;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (Boomerang) spriteRenderer.sprite = boomerangSprite;
        else if (Spear) spriteRenderer.sprite = spearSprite;
        else if (MF) spriteRenderer.sprite = mfSprite;
        else if (Bow) spriteRenderer.sprite = bowSprite;
        else if (Mjolnir) spriteRenderer.sprite = mjolnirSprite;

        float width = spriteRenderer.sprite.bounds.size.x;
        float height = spriteRenderer.sprite.bounds.size.y;

        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        collider.size= new Vector2(width, height);

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        mousePos = Camera.main.WorldToScreenPoint(Input.mousePosition);
        direction = mousePos - this.transform.position;
        angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

        }
    }
    public void attack()
    {
        rb.velocity = transform.right * 25f;

    }
}
