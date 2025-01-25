using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hello, thank for you purchase. To use this script u need:
//Assign this script to the door
//Tag the door "Door"
//Assign camera to a camera value in the script
//Press "f" to open the door

public class GarageDoorOpen : MonoBehaviour
{
    public float smooth = 1;
    int hide = 1;
    public int angle = 90;
    public Camera cam;
    public float RayDistance = 1.7f;
    public float maxD = 1.7f;
    private Ray ray;
    bool open = false;
    public bool isopen;

    void FixedUpdate()
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
                transform.rotation *= Quaternion.Euler(-smooth, 0 , 0);
                hide = hide + 1;
                isopen = true;
                yield return null;
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
                transform.rotation *= Quaternion.Euler(smooth, 0, 0);
                hide = hide + 1;
                isopen = true;
                yield return null;
            }
        }
        isopen = false;
        hide = 1;
        yield return null;
    }
    //Script by Mikhail Chebotarev & Total_Marginal 
}
