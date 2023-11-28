using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Monster;
    bool timer;
    Commander commander;
    // Update is called once per frame

    private void Start()
    {
        commander = GameObject.Find("Commander").GetComponent<Commander>();
        timer = true;
    }
    void Update()
    {
        if (timer)
        {
            GameObject newAlly = Instantiate(Monster, this.transform.position + new Vector3(1, 0), Quaternion.identity);
            commander.addAlly(newAlly);
            timer = false;
            Invoke("ResetTimer",4);
        }

    }

    void ResetTimer()
    {
        timer = true;
    }
}
