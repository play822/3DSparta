using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;

    public GameObject curInteractGameObject;
    public TextMeshProUGUI promptText;
    private Camera camera;
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    SetPromptText();
                }
            }
            else
            {
                curInteractGameObject = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

     private void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = GetInteractPrompt();
    }
    private string GetInteractPrompt()
    {
         var item = curInteractGameObject.gameObject.GetComponent<JumpPower>();
        if (item != null)
        {
            string str = item.data.displayName + "\n" + item.data.description;
            return str;    
        }
        else
        {
            ItemData data = curInteractGameObject.gameObject.GetComponent<Item>().data;
            string str = data.displayName + "\n" + data.description;
            return str;
        }
    }


}
