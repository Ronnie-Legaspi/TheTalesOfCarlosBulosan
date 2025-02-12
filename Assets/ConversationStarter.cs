using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] private NPCConversation myConversation;

    private void  OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if(Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E))
            {
                ConversationManager.Instance.StartConversation(myConversation);
            }
           
        }
        
    }
}
