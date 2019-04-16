using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private Collider2D collider;
    // Start is called before the first frame update
    void Start()
    {
    }
    void Awake()
    {
        collider = GetComponent<Collider2D>();
    }
    // Update is called once per frame

    void FixedUpdate()
    {
        if (collider.isActiveAndEnabled)
        {
            Debug.Log("Arrow Hit");
            Destroy(this.gameObject);
        }
        Destroy(this.gameObject, 3f);
    }
    void OnDestroy()
    {
        
    }
}
