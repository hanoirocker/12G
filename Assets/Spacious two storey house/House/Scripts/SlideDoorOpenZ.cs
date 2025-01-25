using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hello, thank for you purchase. To use this script u need:
//Assign this script to the slide door
//Tag the door "Door"
//Assign camera to a camera value in the script
//Press "f" to open the door

public class SlideDoorOpenZ : MonoBehaviour
{
    public Camera cam;
    public float RayDistance = 1.7f;
    public float maxD = 1.7f;
    private Ray ray;
    bool open;
    public int angle = 14;
    int hide = 1;
    bool isopen = false;
    public float smooth = 0.1f;
    public float delay = 0.03f;

    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, RayDistance) && hit.transform.tag == "Door")
            {
                if (Vector3.Distance(hit.transform.position, this.transform.position) < maxD)
                {
                    if (!isopen)
                    {
                        open = !open;

                        if (open)
                        {
                            StartCoroutine("one");
                        }
                        else
                        {
                            StartCoroutine("two");
                        }
                    }
                }
            }
        }
    }


    IEnumerator one()
    {
        if (!isopen)
        {
            while (hide < angle)
            {
                transform.position += new Vector3(0,0,-smooth);
                hide = hide + 1;
                isopen = true;
                yield return new WaitForSeconds(delay);
            }
        }
        isopen = false;
        hide = 1;
        yield return null;
    }

    IEnumerator two()
    {
        if (!isopen)
        {
            while (hide < angle)
            {
                transform.position += new Vector3(0, 0, smooth);
                hide = hide + 1;
                isopen = true;
                yield return new WaitForSeconds(delay);
            }
        }
        isopen = false;
        hide = 1;
        yield return null;
    }
    //Script by Mikhail Chebotarev & Total_Marginal 
}
