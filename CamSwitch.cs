using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cam1;
    public GameObject cam2;
    public GameObject cam3;
    public GameObject cam4;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isActiveAndEnabled)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                cam1.SetActive(false);
                cam2.SetActive(false);
                cam3.SetActive(false);
                cam4.SetActive(true);
            }
             else if (Input.GetKeyDown(KeyCode.V))
            {
                cam1.SetActive(false);
                cam2.SetActive(false);
                cam3.SetActive(true);
                cam4.SetActive(false);
            }
             else if (Input.GetKeyDown(KeyCode.B))
            {
                cam1.SetActive(false);
                cam2.SetActive(true);
                cam3.SetActive(false);
                cam4.SetActive(false);
            }

            else if (Input.GetKeyDown(KeyCode.N))
            {
                cam1.SetActive(true);
                cam2.SetActive(false);
                cam3.SetActive(false);
                cam4.SetActive(false);
            }
           
            
        }
    }
}
