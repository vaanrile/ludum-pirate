using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WatchScript : MonoBehaviour
{
    public bool isDigital;
    //Digital Time
    [Header("Digital Watch")]
    public Text timeText;
    private int remainingMinutes, remainingSeconds;


    //Analog Time
    [Header("Analog Watch")]
    private List<Material> _normalMaterialList = new List<Material>();
    [SerializeField] private Transform _hourArrow, _minutesArrow, _secondsArrow;

    void Start()
    {
        remainingMinutes = 15;
        //StartTime();
    }

    public void StartTime()
    {
        if (isDigital)
        {
            StartCoroutine(DigitalTime());
        }
        else
        {
            StartCoroutine(AnalogTime());
        }
    }


    IEnumerator DigitalTime()
    {
        while (true)
        {
            remainingSeconds--;
            if (remainingSeconds < 0)
            {
                remainingSeconds = 59;
                remainingMinutes--;
            }
            if (remainingSeconds < 10)
            {
                timeText.text = remainingMinutes.ToString() + " :0" + remainingSeconds.ToString();
            }
            else
            {
                timeText.text = remainingMinutes.ToString() + " :" + remainingSeconds.ToString();
            }
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator AnalogTime()
    {
        remainingSeconds = 60;
        while (true)
        {       
            yield return new WaitForSeconds(1);
            _secondsArrow.DOLocalRotate(new Vector3(0, _secondsArrow.localEulerAngles.y + 6, 0), 0.05f);
            remainingSeconds--;
            if (remainingSeconds == 0)
            {
                _minutesArrow.DOLocalRotate(new Vector3(0, _minutesArrow.localEulerAngles.y + 6, 0), 0.1f);
                _secondsArrow.localEulerAngles = Vector3.zero;
                remainingSeconds = 60;
            }
        }
    }
}
