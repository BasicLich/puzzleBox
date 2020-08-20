using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickerLogic : MonoBehaviour
{
    public Camera Cam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                TileSetter tile = hit.collider.GetComponent<TileSetter>();
                if(tile != null)
                {
                    tile.Press();
                }
            }
        }

    }
}
