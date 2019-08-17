using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyConfiner : MonoBehaviour
{
    [SerializeField] private bool IsABossArea = false;
    [SerializeField] private GameObject BossCamera;
    private List<Transform> Enemies;
    private Transform player;
    private Vector2 MaxBound;
    private Vector2 MinBound;
    private bool PlayerIsEngaged = false;
    private bool Cleared = false;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Enemies = new List<Transform>();
        Bounds WorldBound = GetComponent<Collider2D>().bounds;
        if (WorldBound != null && Enemies !=null)
        {
            MaxBound.Set(WorldBound.center.x + WorldBound.extents.x, WorldBound.center.y + WorldBound.extents.y);
            MinBound.Set(WorldBound.center.x - WorldBound.extents.x, WorldBound.center.y - WorldBound.extents.y);
        }
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Enemy"))
            {
                Enemies.Add(child);
            }
        }
    }
    private void FixedUpdate()
    {
        if (player)
        {
            if (!PlayerIsEngaged && !Cleared && IsABossArea && BossCamera &&
                player.transform.position.x < MaxBound.x && player.transform.position.x > MinBound.x &&
                player.transform.position.y < MaxBound.y && player.transform.position.y > MinBound.y)
            {
                PlayerIsEngaged = true;
                GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
                BossCamera.SetActive(true);
            }
            else if (PlayerIsEngaged && CheckIfVictorious())
            {
                PlayerIsEngaged = false;
                GameObject.FindGameObjectWithTag("MainCamera").SetActive(true);
                BossCamera.SetActive(false);
            }

            if(PlayerIsEngaged && !Cleared)
            {
                if (MaxBound != null && MinBound != null)
                {
                    Vector2 pos = player.GetComponent<Rigidbody2D>().position;
                    pos.x = Mathf.Clamp(pos.x, MinBound.x, MaxBound.x);
                    pos.y = Mathf.Clamp(pos.y, MinBound.y, MaxBound.y);
                    player.GetComponent<Rigidbody2D>().position = pos;
                }
            }

            if (Enemies != null && MaxBound != null && MinBound != null)
            {
                foreach (Transform EnemyPos in Enemies)
                {
                    if (EnemyPos)
                    {
                        Vector2 pos = EnemyPos.position;
                        pos.x = Mathf.Clamp(pos.x, MinBound.x, MaxBound.x);
                        pos.y = Mathf.Clamp(pos.y, MinBound.y, MaxBound.y);
                        EnemyPos.GetComponent<Rigidbody2D>().position = pos;
                    }
                }
            }
        }
    }
    private bool CheckIfVictorious()
    {
        bool Victory = true;
        foreach (Transform EnemyPos in Enemies)
        {
            if (EnemyPos)
            {
                Victory = false;
            }
        }
        if (Victory)
        {
            Cleared = true;
        }
        return Victory;
    }
}
