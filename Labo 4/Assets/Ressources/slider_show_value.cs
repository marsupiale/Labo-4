using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class slider_show_value : MonoBehaviour
{
    Text textComponent;
    void Start()
    {
        textComponent = GetComponent<Text>();
        textComponent.text = GetComponentInParent<Slider>().value.ToString();
    }

    public void setSliderValue(float sliderValue) => textComponent.text = sliderValue.ToString();

}
