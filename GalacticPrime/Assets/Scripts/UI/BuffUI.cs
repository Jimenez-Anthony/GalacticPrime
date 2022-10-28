using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffUI : MonoBehaviour
{

    public TMP_Text description;
    public Image image;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void SetBuff(Sprite img, string des) {
        description.text = des;
        image.sprite = img;
    }
}
