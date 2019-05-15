using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour {

    private static string defaultmsg = "how did you even manage to do this, you're a disgrace";

    [SerializeField] private Image messageContainer;
    [SerializeField] private Image messageTail;

    public Status status { get; private set; }
    public bool display {
        set {
            if (value) {
                messageContainer.color = new Color(1, 1, 1, 1);
                messageTail.color = new Color(1, 1, 1, 1);
            }
            else {
                messageContainer.color = new Color(1, 1, 1, 0);
                messageTail.color = new Color(1, 1, 1, 0);
            }
        }
    }

    public enum Status {
        //The status of the message box at the moment.
        Typing,
        Waiting,
        Standby
    }

    // Use this for initialization
    void Start () {
        display = false;
        status = Status.Standby;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initalize() {

    }

    private IEnumerator TypeOut(string message) {
        return null;
    }
}




