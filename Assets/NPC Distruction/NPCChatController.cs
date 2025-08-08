//using UnityEngine;
//using TMPro;
//using System.Collections;
//using System.Collections.Generic;

//public class NPCChatController : MonoBehaviour
//{
//    public Transform MsgTransform; // Position above the NPC's head
//    public GameObject messagePrefab;    // Prefab for the message UI
//    public float typingSpeed = 0.05f;   // Speed of the typing animation
//    [SerializeField] PlayerChatController playerChat;
//    [SerializeField] NPC_Charchter npc_Char;

//    private Queue<string> npcMessages = new Queue<string>(); // Queue to store NPC messages
//    private TextMeshProUGUI messageText; // Text component to show the message


//    // Start is called before the first frame update
//    void Start()
//    {
//        // Sample messages for the NPC
//        npcMessages.Enqueue("Hey Joseph!");
//        npcMessages.Enqueue("How are you today?");
//        npcMessages.Enqueue("What's your plan for weekends?");
//        npcMessages.Enqueue("Great, I have some plan for you.");
//        npcMessages.Enqueue("Okay then will meet you soon...");

//        // Start the conversation
//        StartCoroutine(DisplayNextMessage());
//    }

//    private IEnumerator DisplayNextMessage()
//    {
//        if (npcMessages.Count > 0)
//        {
//            // Spawn a new message object above the NPC's head
//          GameObject  currentMessage = Instantiate(messagePrefab, MsgTransform);
//            messageText = currentMessage.GetComponentInChildren<TextMeshProUGUI>();

//            string message = npcMessages.Dequeue();
//            yield return StartCoroutine(TypeMessage(message, currentMessage));
//        }
//        else
//        {
//            yield return new WaitForSeconds(1f);
//            npc_Char.IsChatComplete = true;
//        }
//    }

//    private IEnumerator TypeMessage(string message, GameObject obj)
//    {
//        // Clear any existing text
//        messageText.text = "";

//        // Animate typing effect
//        foreach (char letter in message)
//        {
//            messageText.text += letter;
//            yield return new WaitForSeconds(typingSpeed);
//        }
//        yield return new WaitForSeconds(1f);
//        Destroy(obj);
//        playerChat.StartPlayerResponse();

//    }

//    // Call this method when the player responds (PlayerChatController will call this)
//    public void OnPlayerResponded()
//    {
//        // Move to the next NPC message in the queue
//        StartCoroutine(DisplayNextMessage());
//    }
//}
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NPCChatController : MonoBehaviour
{
    public Transform MsgTransform;
    public GameObject messagePrefab;
    public float typingSpeed = 0.05f;
    [SerializeField] PlayerChatController playerChat;
    [SerializeField] NPC_Charchter npc_Char;

    private Queue<string> npcMessages = new Queue<string>();
    private TextMeshProUGUI messageText;
    [SerializeField] GameObject currentMessageObject;

    void Start()
    {
        ResetConversation();
        Debug.Log("++++++++++++++++");
    }

    public void ResetConversation()
    {
        npcMessages.Clear();
        npcMessages.Enqueue("Hey Yusuf!");
        npcMessages.Enqueue("How are you today?");
        npcMessages.Enqueue("What's your plan for the weekend?");
        npcMessages.Enqueue("Great, I have some plan for you. We will go to River Buffet!");
        npcMessages.Enqueue("Doordash is not giving me good orders nowadays");
        npcMessages.Enqueue("I don't know brother, these economy and debt is killing me");
        npcMessages.Enqueue("Okay then I will see you soon...");


        //if (currentMessageObject != null)
        //{
        //    Destroy(currentMessageObject);
        //    currentMessageObject = null;
        //}
        StartCoroutine(DisplayNextMessage());
    }

    private IEnumerator DisplayNextMessage()
    {
        if (npcMessages.Count > 0 && enabled)
        {
            currentMessageObject = Instantiate(messagePrefab, MsgTransform);
            messageText = currentMessageObject.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log("Message instantiated: " + currentMessageObject.name);
            string message = npcMessages.Dequeue();
            yield return StartCoroutine(TypeMessage(message, currentMessageObject));
            Debug.Log("\\\\\\\\\\\\" + npcMessages.Count);

        }
        else if (npcMessages.Count == 0)
        {
            yield return new WaitForSeconds(1f);
            npc_Char.IsChatComplete = true;
            Debug.Log("\\\\\\\\\\\\");
        }
    }

    private IEnumerator TypeMessage(string message, GameObject obj)
    {
        messageText.text = "";
        foreach (char letter in message)
        {
            if (!enabled) yield break; // Stop if disabled (player left sphere)
            messageText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(1f);
        Destroy(obj);
        currentMessageObject = null;
        playerChat.StartPlayerResponse();


    }

    public void OnPlayerResponded()
    {
        if (enabled)
        {
            StartCoroutine(DisplayNextMessage());
        }
    }
}