using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Xml;
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
    }

    private void AnalogTimeFunction()
    {
        _secondsArrow.DOLocalRotate(new Vector3(0, _secondsArrow.localEulerAngles.y + 6, 0), 0.05f);
        _minutesArrow.DOLocalRotate(new Vector3(0, _minutesArrow.localEulerAngles.y + 6, 0), 0.01f);
        _hourArrow.DOLocalRotate(new Vector3(0, _hourArrow.localEulerAngles.y + 30, 0), 0.01f);
    }


    IEnumerator DigitalTime()
    {
        while (true)
        {
            remainingSeconds++;
            //_secondsArrow.DOLocalRotate(new Vector3(0, _secondsArrow.localEulerAngles.y + 6, 0), 0.05f);
            if (remainingSeconds > 59)
            {
                remainingSeconds = 0;
                remainingMinutes++;
                //_minutesArrow.DOLocalRotate(new Vector3(0, _minutesArrow.localEulerAngles.y + 6, 0), 0.01f);
            }
            if(remainingMinutes > 59)
            {
                remainingMinutes = 0;
                remainingHours++;
                //_hourArrow.DOLocalRotate(new Vector3(0, _hourArrow.localEulerAngles.y + 30, 0), 0.01f);
            }

            string minutesText = "";
            if (remainingMinutes < 10)
            {
                minutesText = ":0" + remainingMinutes.ToString();
            }
            else
            {
                minutesText = ":" + remainingMinutes.ToString();
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

            timeText.text = "<mspace=0.5em>"+ hoursText + minutesText;


            yield return new WaitForSeconds(timing);
        }
    }
}
