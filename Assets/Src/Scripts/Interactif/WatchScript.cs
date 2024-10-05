using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class WatchScript : MonoBehaviour
{
    //Digital Time
    public float timing = 0.1f;
    private int hours = 0;
    private int minutes = 0;
    //Digital Time
    [Header("Digital Watch")]
    public TextMeshProUGUI timeText;
    private int remainingHours, remainingMinutes, remainingSeconds;


    //Analog Time
    [Header("Analog Watch")]
    private List<Material> _normalMaterialList = new List<Material>();
    [SerializeField] private Transform _hourArrow, _minutesArrow, _secondsArrow;

    void Start()
    {
        StartTime();
    }

    public void StartTime()
    {
        StartCoroutine(DigitalTime());
        StartCoroutine(AnalogTime());
    }


    IEnumerator DigitalTime()
    {
        while (true)
        {
            remainingSeconds++;
            if (remainingSeconds > 59)
            {
                remainingSeconds = 0;
                remainingMinutes++;
            }
            if(remainingMinutes > 59)
            {
                remainingMinutes = 0;
                remainingHours++;
            }

            string minutesText = "";
            if (remainingMinutes < 10)
            {
                minutesText = " :0" + remainingMinutes.ToString();
            }
            else
            {
                minutesText = " :" + remainingMinutes.ToString();
            }
            string hoursText = "";
            if (remainingHours < 10)
            {
                hoursText = "0" + remainingHours.ToString();
            }
            else
            {
                hoursText = remainingHours.ToString();
            }

            timeText.text = hoursText + minutesText;


            yield return new WaitForSeconds(timing);
        }
    }
    IEnumerator AnalogTime()
    {
        while (true)
        {       
            yield return new WaitForSeconds(timing);
            _secondsArrow.DOLocalRotate(new Vector3(0, _secondsArrow.localEulerAngles.y + 6, 0), 0.05f);
            if (remainingSeconds == 0)
            {
                _minutesArrow.DOLocalRotate(new Vector3(0, _minutesArrow.localEulerAngles.y + 6, 0), 0.1f);
                _secondsArrow.localEulerAngles = Vector3.zero;
            }
            if (remainingMinutes == 0)
            {
                _hourArrow.DOLocalRotate(new Vector3(0, _hourArrow.localEulerAngles.y + 6, 0), 0.1f);
                _hourArrow.localEulerAngles = Vector3.zero;
            }
        }
    }
}
