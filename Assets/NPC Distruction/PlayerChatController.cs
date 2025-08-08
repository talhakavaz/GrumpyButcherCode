//using UnityEngine;
//using TMPro;
//using System.Collections;
//using System.Collections.Generic;

//public class PlayerChatController : MonoBehaviour
//{
//    public Transform MsgSpawnTransform; // Position above the player's head
//    public GameObject messagePrefab;    // Prefab for the message UI
//    public float typingSpeed = 0.05f;   // Speed of the typing animation

//    public bool isTalking;

//    private TextMeshProUGUI messageText; // Text component to show the message

//    private bool canRespond = false; // Flag to control player response timing
//    private Queue<string> npcMessages = new Queue<string>(); // Queue to store NPC messages
//    [SerializeField] NPCChatController npcChatController; // Reference to NPC's chat controller


//    int chatCount;

//    void Start()
//    {
//        npcMessages.Enqueue("Hey Jack!");
//        npcMessages.Enqueue("I'm fine, thank you.");
//        npcMessages.Enqueue("I Don't have any plans yet.");
//        npcMessages.Enqueue("Oh wow, thanks man!");
//        npcMessages.Enqueue("Yeah, Bye jack.");
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (canRespond) // Wait for the player to press Enter
//        {
//            string playerResponse = npcMessages.Dequeue();
//            canRespond = false;
//            // Display the player's response
//            StartCoroutine(DisplayPlayerMessage(playerResponse));

//        }
//    }

//    private IEnumerator DisplayPlayerMessage(string message)
//    {

//        // Spawn a new message object above the player's head
//        GameObject currentMessage = Instantiate(messagePrefab, MsgSpawnTransform);
//        messageText = currentMessage.GetComponentInChildren<TextMeshProUGUI>();

//        // Animate typing effect for the player's message
//        messageText.text = "";
//        foreach (char letter in message)
//        {
//            messageText.text += letter;
//            yield return new WaitForSeconds(typingSpeed);
//        }

//        // Allow the player to respond again after a short delay
//        yield return new WaitForSeconds(1f);
//        Destroy(currentMessage);
//        npcChatController.OnPlayerResponded();

//        chatCount++;

//        if (chatCount == 5)
//        {
//            isTalking = false;
//        }
//    }

//    // Method to start the player response when it's time
//    public void StartPlayerResponse()
//    {
//        isTalking = true;
//        canRespond = true;
//    }
//}
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerChatController : MonoBehaviour
{
    public Transform MsgSpawnTransform;
    public GameObject messagePrefab;
    public float typingSpeed = 0.05f;

    public bool isTalking;

    private TextMeshProUGUI messageText;
    private bool canRespond = false;
    private Queue<string> npcMessages = new Queue<string>();
    [SerializeField] NPCChatController npcChatController;
    private GameObject currentMessageObject;
    private int chatCount;

    void Start()
    {
        ResetConversation();
    }

    public void ResetConversation()
    {
        npcMessages.Clear();
        npcMessages.Enqueue("Hey Mr. Pazar!");
        npcMessages.Enqueue("I'm fine, thank you.");
        npcMessages.Enqueue("Don't have any plans yet.");
        npcMessages.Enqueue("Oh wow, thanks! I will be there!");
        npcMessages.Enqueue("I hope it will get better.");
        npcMessages.Enqueue("Take it easy..");
        npcMessages.Enqueue("Yeah, Bye Mr. Pazar!");
        chatCount = 0;
        isTalking = false;
        canRespond = false;

        if (currentMessageObject != null)
        {
            Destroy(currentMessageObject);
            currentMessageObject = null;
        }
    }

    void Update()
    {
        if (canRespond && npcChatController.enabled)
        {
            string playerResponse = npcMessages.Dequeue();
            canRespond = false;
            StartCoroutine(DisplayPlayerMessage(playerResponse));
        }
    }

    private IEnumerator DisplayPlayerMessage(string message)
    {
        currentMessageObject = Instantiate(messagePrefab, MsgSpawnTransform);
        messageText = currentMessageObject.GetComponentInChildren<TextMeshProUGUI>();
        messageText.text = "";
        foreach (char letter in message)
        {
            if (!npcChatController.enabled) yield break; // Stop if NPC chat is disabled
            messageText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(1f);
        Destroy(currentMessageObject);
        currentMessageObject = null;
        chatCount++;
        if (chatCount == 7)
        {
            isTalking = false;
            npcChatController.OnPlayerResponded();
        }
        else
        {
            npcChatController.OnPlayerResponded();
        }
    }

    public void StartPlayerResponse()
    {
        if (npcChatController.enabled)
        {
            isTalking = true;
            canRespond = true;
        }
    }
}