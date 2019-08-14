using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfiner : MonoBehaviour
{
    private GameObject Player;
    //private Bounds WorldBound;
    private Vector2 MaxBound;
    private Vector2 MinBound;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Bounds WorldBound = GetComponent<CompositeCollider2D>().bounds;
        if (WorldBound != null && Player !=null)
        {
            MaxBound.Set(WorldBound.center.x + WorldBound.extents.x, WorldBound.center.y + WorldBound.extents.y);
            MinBound.Set(WorldBound.center.x - WorldBound.extents.x, WorldBound.center.y - WorldBound.extents.y);
            Debug.Log("Max X: " + MaxBound.x+", Min X: "+MinBound.x);
            Debug.Log("Max X: " + MaxBound.y+", Min X: "+ MinBound.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if(Player != null && MaxBound != null && MinBound !=null)
        {
            Vector2 pos = Player.GetComponent<Rigidbody2D>().position;
            pos.x = Mathf.Clamp(pos.x, MinBound.x, MaxBound.x);
            pos.y = Mathf.Clamp(pos.y, MinBound.y, MaxBound.y);
            Player.GetComponent<Rigidbody2D>().position = pos;
        }
    }
}
