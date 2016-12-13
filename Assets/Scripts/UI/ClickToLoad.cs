using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Scripts.UI
{
    public class ClickToLoad : MonoBehaviour {

        // Use this for initialization
        void Start ()
        {
	
        }
	
        // Update is called once per frame
        void Update ()
        {
            if (CrossPlatformInputManager.GetButtonDown("Fire1"))
            {
                SceneManager.LoadScene("limbo");
            }
        }
    }
}
