using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour {

    //A list of all messages in the game to be accessed.
    private static string[] msgs = {
        "Hello, friend! How are you today?",
        "test"
    };

    private static string defaultmsg = "how did you even manage to do this, you're a disgrace";

    [SerializeField] private Image messageContainer;
    [SerializeField] private Image messageTail;

    // Use this for initialization
    void Start () {
        messageContainer.color = new Color(1, 1, 1, 0);
        messageTail.color = new Color(1, 1, 1, 0);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void Initiate(int message) {
        MessageBox messageBox = (MessageBox)FindObjectOfType(typeof(MessageBox));

        string msg;

        try {
            msg = msgs[message];
        }
        catch {
            msg = defaultmsg;
            Debug.LogError("Some idiot messed up and didn't give a message the right fuckin' index, lets hope he or she fixes it.");
        }

        try {
            
        }
        catch {

        }
    }

    private IEnumerator TypeOut(string message) {
        return null;
    }
}
