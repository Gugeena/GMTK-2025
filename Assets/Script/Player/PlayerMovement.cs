using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private Transform headTransform, headPivotTransform;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
        handleLooking();
    }

    void handleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2 (x * speed, rb.velocity.y);

    }

    void handleLooking()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 headPosition = Vector2.Lerp(mousePos, headPivotTransform.position, 0.993f);
        headPosition.z = 0;

        headTransform.position = headPosition;
    }
}
