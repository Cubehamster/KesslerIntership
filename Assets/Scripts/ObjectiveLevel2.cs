﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveLevel2 : MonoBehaviour
{
    LevelManager level;
    public Transform Spawnpoint;
    public Transform WayPointTarget_1;
    public Transform WayPointTarget_2;
    public ShipController SC;

    bool WayPoint_1 = false;
    bool WayPoint_2 = false;
    bool safetyCheck = false;
    bool checking = false;
    bool levelCompleted = false;
    bool playOnce = false;
    bool playOnce2 = false;
    bool playOnce3 = false;
    public TextMeshPro missionAccomplished;
    public TextMeshPro tutorialText;

    [SerializeField] private float alpha;
    [SerializeField] private float alpha2;
    [SerializeField] private Material missionAccomplishTextMat;
    [SerializeField] private Material tutorialTextMat;
    bool textFade = false;
    bool zoomCheck = false;
    bool laserCheck = false;

    float rocketObjectiveAngle;

    bool textFade2 = false;
    bool startMission = false;

    float selfdestructTImer = 3.49f;

    // Start is called before the first frame update
    void Awake()
    {
        startMission = false;
        playOnce = false;
        playOnce2 = false;
        playOnce3 = false;
        alpha = 0;
        alpha2 = 0;
        level = Object.FindObjectOfType<LevelManager>();
        WayPointTarget_2.gameObject.SetActive(false);


        missionAccomplishTextMat = missionAccomplished.GetComponent<Renderer>().material;
        missionAccomplishTextMat.EnableKeyword("_FaceColor");
        tutorialTextMat = tutorialText.GetComponent<Renderer>().material;
        tutorialTextMat.EnableKeyword("_FaceColor");

        StartCoroutine(LevelText(missionAccomplished));
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<ShipController>().rocketExists)
        {
            Vector2 rocketVector2 = new Vector2(SC.rocketModel.transform.position.x - SC.earth.transform.position.x, SC.rocketModel.transform.position.y - SC.earth.transform.position.y);
            Vector2 spawnVector2 = new Vector2(Spawnpoint.position.x - -SC.earth.transform.position.x, Spawnpoint.transform.position.y - -SC.earth.transform.position.y);
            rocketObjectiveAngle = Vector2.Angle(rocketVector2, spawnVector2);
        }
    
        

        if (textFade)
        {
            alpha = Mathf.Lerp(alpha, 1, 0.035f);
            missionAccomplishTextMat.SetColor("_FaceColor", new Color(1f, 1f, 1f, alpha));
        }
        else
        {
            alpha = Mathf.Lerp(alpha, 0, 0.035f);
            missionAccomplishTextMat.SetColor("_FaceColor", new Color(1f, 1f, 1f, alpha));
        }
        if (textFade2)
        {
            alpha2 = Mathf.Lerp(alpha2, 1, 0.025f);
            tutorialTextMat.SetColor("_FaceColor", new Color(1f, 1f, 1f, alpha2));
        }
        else
        {
            alpha2 = Mathf.Lerp(alpha2, 0, 0.035f);
            tutorialTextMat.SetColor("_FaceColor", new Color(1f, 1f, 1f, alpha2));
        }

        if (!playOnce2 && !SC.refueling && !SC.hasCrashed && startMission)
        {
            playOnce2 = true;
            StartCoroutine(FadeToolTip());
        }

        if (SC.rocketExists && startMission)
        {
            if (WayPointTarget_1 != null)
            {
                if (!WayPoint_1)
                {
                    //scoreText.text = "Fly to Waypoint";         
                }

                if (rocketObjectiveAngle > 170 && !playOnce)
                {
                    WayPoint_1 = true;
                    playOnce = true;
                    StartCoroutine(DisableDelayed(WayPointTarget_1.gameObject));
                    WayPointTarget_2.gameObject.SetActive(true);
                    StartCoroutine(ChangeText(missionAccomplished, "Land Safely"));
                }
            }

            if (WayPointTarget_2 != null && startMission && WayPoint_1)
            {
                if ((WayPointTarget_2.position - SC.rocketModel.transform.position).magnitude < 0.7f && !WayPoint_2)
                {
                    if (SC.FuelPercentage == 1 && !checking && SC.hasLanded && WayPoint_1)
                    {
                        StartCoroutine(SafetyCheck());
                    }
                    if (safetyCheck)
                    {
                        if (!playOnce3)
                        {
                            WayPoint_2 = true;
                            playOnce3 = true;
                            StartCoroutine(DisableDelayed(WayPointTarget_2.gameObject));
                    
                        }
                        if (WayPoint_1 && WayPoint_2 && !levelCompleted)
                        {
                            StartCoroutine(ChangeText(missionAccomplished, "Mission Accomplished"));
                            levelCompleted = true;
                            SC.zoomLevel = 5;
                            SC.boosterEnabled = false;
                            SC.missionCompleted = true;
             
                        }


                    }

                }
            }
        }
        else
        {
            if (!levelCompleted)
            {
                WayPointTarget_1.gameObject.SetActive(true);
                WayPoint_2 = false;
                WayPointTarget_2.gameObject.SetActive(false);
                WayPoint_1 = false;
                checking = false;
                safetyCheck = false;
                playOnce = false;
                playOnce3 = false;
            }
        }

        if (Input.GetKey(KeyCode.Mouse1) && zoomCheck)
            StartCoroutine(LaserCheck());

        if (gameObject.GetComponent<ShipController>().FuelPercentage == 0 && !gameObject.GetComponent<ShipController>().hasCrashed && !gameObject.GetComponent<ShipController>().selfDestruct)
        {
            if (Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.Mouse1))
            {
                selfdestructTImer -= Time.deltaTime;
                tutorialText.text = Mathf.RoundToInt(selfdestructTImer).ToString();
                if (selfdestructTImer < 0.5f)
                {
                    textFade2 = false;
                    gameObject.GetComponent<ShipController>().selfDestruct = true;
                }
            }
            else if (!gameObject.GetComponent<ShipController>().selfDestruct)
            {
                selfdestructTImer = 3.45f;
                tutorialText.text = "(Hold Right + Left Mouse) = Selfdestruct";
                textFade2 = true;
            }
        }

        if (gameObject.GetComponent<ShipController>().hasCrashed && !Input.GetKey(KeyCode.Mouse0))
        {
            tutorialText.text = "(Hold Right Mouse) = Respawn";
            textFade2 = true;
            textFade = false;
            WayPoint_1 = false;
        }
        else if (gameObject.GetComponent<ShipController>().respawning && laserCheck)
        {
            textFade2 = false;
            textFade = true;
            missionAccomplished.text = "Orbit the Planet";
        }
    }

    IEnumerator LaserCheck()
    {
        yield return new WaitForSeconds(1f);
        laserCheck = true;
        textFade2 = false;
    }

    IEnumerator SafetyCheck()
    {
        checking = true;
        if (gameObject.GetComponent<ShipController>().refueling && gameObject.GetComponent<ShipController>().hasLanded)
        {
            yield return new WaitForSeconds(0.2f);
            safetyCheck = true;
            checking = false;
        }
        else
        {
            checking = false;
            safetyCheck = false;
        }
    }

    IEnumerator DisableDelayed(GameObject target)
    {
        target.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1f);
        target.SetActive(false);
        playOnce = false;
    }

    IEnumerator LevelText(TextMeshPro titleText)
    {
        titleText.text = "Level 2: Orbiting";
        textFade = true;
        yield return new WaitForSeconds(2);
        textFade = false;
        startMission = true;
        StartCoroutine(ScrollCheck(missionAccomplished));
    }

    IEnumerator ScrollCheck(TextMeshPro titleText)
    {
        yield return new WaitForSeconds(1.8f);
        gameObject.GetComponent<ShipController>().missionStart = true;
        yield return new WaitForSeconds(1.8f);
        textFade = true;
        textFade2 = true;
        gameObject.GetComponent<ShipController>().boosterEnabled = true;
        titleText.text = "Orbit the Planet";
        tutorialText.text = "(Right Mouse) = Laser";
        zoomCheck = true;
    }

    IEnumerator ChangeText(TextMeshPro titleText, string newText)
    {
        textFade = false;
        yield return new WaitForSeconds(1.5f);
        textFade = true;
        titleText.text = newText;

    }

    IEnumerator FadeToolTip()
    {
        yield return new WaitForSeconds(1f);
        textFade2 = false;
    }
}
