using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHoverDescription : MonoBehaviour
{
    [SerializeField] private string TitleText;
    [SerializeField] private int id;

    [SerializeField] [TextArea(5, 10)] private string DescriptionText;
    private DescriptionManager Manager;
    private bool hover;

    private void Start()
    {
        Manager = GameObject.Find("DescriptionPanel").GetComponent<DescriptionManager>();
    }
    public void DisplayDescription()
    {
        Manager.DisplayText(DescriptionText, TitleText, id);
    }
}
