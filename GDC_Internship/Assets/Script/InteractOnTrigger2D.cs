using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InteractOnTrigger2D : MonoBehaviour
{
    public LayerMask layers;                                        //biar gampang bedain layer?
    public UnityEvent OnEnter, OnExit;                              //bisa MANGGIL SESUATU dengan gampang!

    protected Collider2D m_Collider;

    void Reset()                                                    //gak ngerti kenapa reset, tapi tahu fungsinya
    {
        layers = LayerMask.NameToLayer("Everything");
        m_Collider = GetComponent<Collider2D>();
        m_Collider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled)                                                //coba jangan dipake dulu, kayak, bukannya udah pasti terjadi?
            return;

        if (layers.Contains(other.gameObject))                       //Contains di sini dicover di LayerMaskExtension
        {
            ExecuteOnEnter(other);                                   //other kayaknya input kita sendiri dari menu di unity
        }
    }

    void OnTriggerExit2D(Collider2D other)                          //sama dengan diatas
    {
        if (!enabled)
            return;

        if (layers.Contains(other.gameObject))
        {
            ExecuteOnExit(other);
        }
    }

    protected virtual void ExecuteOnEnter(Collider2D other)         //virtual itu buat anak class, tapi emang punya anak?
    {
        OnEnter.Invoke();
    }

    protected virtual void ExecuteOnExit(Collider2D other)
    {
        OnExit.Invoke();
    }

    //////// layer dititip
    //////// ini hanya bisa bekerja kalau di defin di static class, gak bisa yang generic (tanpa static/dinamic)
    /*
        public static bool Contains(this LayerMask layers, GameObject gameObject)
        {
            return 0 != (layers.value & 1 << gameObject.layer);
        }
    */
}

