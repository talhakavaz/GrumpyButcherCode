//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public class NPC_Charchter : MonoBehaviour
//{
//    public Transform target;
//    public Transform home;
//    public bool IsChatComplete;
//    [SerializeField] NPCChatController npcChat;
//    private NavMeshAgent navMeshAgent;

//    bool isReached;

//    void Start()
//    {
//        navMeshAgent = GetComponent<NavMeshAgent>();

//    }

//    void Update()
//    {
//        if(!isReached)
//        {
//            navMeshAgent.SetDestination(target.position);
//            if(Vector3.Distance(transform.position, target.position) <= 1)
//            {
//                npcChat.enabled = true;
//                isReached = true;
//            }
//        }

//        if(IsChatComplete)
//        {
//            navMeshAgent.SetDestination(home.position);
//        }

//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Charchter : MonoBehaviour
{
    public Transform target; // NPC TargetPos
    public Transform home;  // NPC Home
    public bool IsChatComplete;
    [SerializeField] NPCChatController npcChat;
    [SerializeField] private float conversationRadius = 3f; // Radius of the detection sphere
    [SerializeField] private float checkInterval = 0.5f; // How often to check for player
    private NavMeshAgent navMeshAgent;
    private bool isReached = false;
    [SerializeField] bool isPlayerNearby = false;
    [SerializeField] Animator NpcCharacterAnimator;


    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        npcChat.enabled = false; // Disable chat at start
        StartCoroutine(CheckPlayerProximity()); // Start proximity check
    }

    void Update()
    {
        // Move to target if not reached
        if (!isReached)
        {
            NpcCharacterAnimator.SetBool("IsIdle", false);
            NpcCharacterAnimator.SetBool("IsWalk", true);
            navMeshAgent.SetDestination(target.position);
            if (Vector3.Distance(transform.position, target.position) <= 1)
            {
                isReached = true;
                navMeshAgent.isStopped = true; // Stop moving
            }
        }
        else
        {
            NpcCharacterAnimator.SetBool("IsIdle", true);
            NpcCharacterAnimator.SetBool("IsWalk", false);
        }

        // Move to home if chat is complete
        if (IsChatComplete)
        {
            NpcCharacterAnimator.SetBool("IsIdle", false);
            NpcCharacterAnimator.SetBool("IsWalk", true);
            Debug.Log("ChatComplete........");
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(home.position);
        }
    }

    private IEnumerator CheckPlayerProximity()
    {
        while (true)
        {
            if (!isReached || IsChatComplete)
            {
                Debug.Log("!isReached || IsChatComplete");
                yield return new WaitForSeconds(checkInterval);
                continue;
            }

            // Check for player in sphere
            Collider[] hits = Physics.OverlapSphere(transform.position, conversationRadius);
            bool playerFound = false;
            foreach (Collider hit in hits)
            {
                if (hit.gameObject.CompareTag("Player")) // Check for Player tag
                {
                    playerFound = true;
                    if (!isPlayerNearby)
                    {
                        StartConversation();
                    }
                    break;
                }
            }

            if (!playerFound && isPlayerNearby)
            {
                StopConversation();
            }

            yield return new WaitForSeconds(checkInterval); // Wait before next check
        }
    }

    public void StartConversation()
    {
        isPlayerNearby = true;
        npcChat.enabled = true; // Enable chat
    }

    public void StopConversation()
    {
        isPlayerNearby = false;
        npcChat.enabled = false; // Disable chat
        npcChat.ResetConversation(); // Reset conversation
    }

    // Visualize the sphere in the editor for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, conversationRadius);
    }
}