using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteInfo : MonoBehaviour
{
    
    SpriteRenderer spriteRenderer;

    public bool colliding;

    Color currentColor;

    public Bounds spriteBounds;

    // Start is called before the first frame update
    void Start()
    {
        currentColor = Color.white;

        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteBounds = spriteRenderer.bounds;

        colliding = false;
    }

    // Update is called once per frame
    void Update()
    {
        spriteBounds = spriteRenderer.bounds;

        if (colliding)
        {
            // If true, this object is colliding with something else
            currentColor = Color.red;
        }
        else
        {
            currentColor = Color.white;
        }

        spriteRenderer.color = currentColor;
    }
}
