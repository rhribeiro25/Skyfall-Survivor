using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class UnityAdControle : MonoBehaviour {

    public static bool showAds = true;

    public static void ShowAd()
    {
#if UNITY_ADS
        ShowOptions options = new ShowOptions();
        options.resultCallback = Unpause;
        if (Advertisement.IsReady())
        {
            Advertisement.Show(options);
        }
#endif

        PauseMenu.pause = true;
    }

    #if UNITY_ADS
    public static void Unpause()
    {
        PauseMenu.pause = false;
    }
    #endif

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
