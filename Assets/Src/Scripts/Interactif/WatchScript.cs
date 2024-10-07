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
    public float timing;
    public int hours = 0;
    public int minutes = 0;
    //Digital Time
    [Header("Digital Watch")]
    public TextMeshProUGUI timeText;
    public int remainingHours, remainingMinutes, remainingSeconds;


    //Analog Time
    [Header("Analog Watch")]
    private List<Material> _normalMaterialList = new List<Material>();
    [SerializeField] private Transform _hourArrow, _minutesArrow, _secondsArrow;

    void Start()
    {
    }

    public void StartTime()
    {
        //StartCoroutine(DigitalTime());
        AnalogTimeFunction();
        AnalogSecond();
    }

    private void AnalogTimeFunction()
    {        
        _minutesArrow.DOLocalRotate(new Vector3(0, 360, 0), timing*60, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        _hourArrow.DOLocalRotate(new Vector3(0, 360, 0), timing*720, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1,LoopType.Restart);
    }
    private void AnalogSecond()
    {
        _secondsArrow.DOLocalRotate(new Vector3(0, 360, 0), timing, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).OnComplete(() =>
        {
            AnalogSecond();
            SetDigitalTime();
        });
    }



    IEnumerator DigitalTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(timing);
            remainingMinutes++;
            if (remainingMinutes > 59)
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

            if (remainingHours >= 7)
            {
                Debug.Log("7h passé !");
                GameManager.instance.LoseCondition();
            }
          
        }
    }
    private void SetDigitalTime()
    {
        remainingMinutes++;
        if (remainingMinutes > 59)
        {
            remainingMinutes = 0;
            remainingHours++;
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

        timeText.text = "<mspace=0.5em>" + hoursText + minutesText;

        if (remainingHours >= 7)
        {
            GameManager.instance.LoseCondition();
        }
    }
}
