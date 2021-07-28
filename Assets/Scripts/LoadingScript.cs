using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{
    public TextMeshProUGUI loading;
    public int num;
    // Start is called before the first frame update
    void Start()
    {
        loading = this.GetComponent<TextMeshProUGUI>();
        num = 1;
        StartCoroutine(loadingPoints());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator loadingPoints()
	{
        switch (num)
        {
            case 3:
                loading.text = "Loading...";
                num = 1;
                break;
            case 2:
                loading.text = "Loading..";
                num = 3;
                break;
            case 1:
                loading.text = "Loading.";
                num = 2;
                break;
        }

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(loadingPoints());
    }
}
