﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class ConversationChangeEvent : UnityEvent<Conversation> {}

public class DecisionController : MonoBehaviour
{
    public Choice choice;
    public ConversationChangeEvent conversationChangeEvent;
    public GameObject PositiveEffect;
    public GameObject NegativeEffect;

    // BRUTE FORCE REMOVE
    public SceneChange sceneChange;

    // Uses "Template Button" to make the choice buttons
    public static DecisionController AddChoiceButton(Button templateButton, Choice choice, int index, GameObject choicePanel)
    {
        Button button = Instantiate(templateButton);

        // Trying to use Layout Groups to bypass need to manually position buttons, may need these lines later
        // int buttonSpacing = -44;
        //button.transform.SetParent(templateButton.transform.parent);
        //button.transform.local Scale = Vector3.one;
        //button.transform.localPosition = new Vector3(0, index * buttonSpacing, 0);

        // Names the button and sets it to active
        button.name = "Choice " + (index + 1);
        button.gameObject.SetActive(true);
        button.transform.SetParent(choicePanel.transform, false);

        // Gets the DecisionController off of the button and sets the proper data in the Choice, then returns
        DecisionController decisionController = button.GetComponent<DecisionController>();
        // Get DialogueManager in order to send message asking to change the active
        decisionController.choice = choice;
        // Add choice inspection and effect propagation
        button.onClick.AddListener(() => SendImpactEffect(choice));
        // Add Distortion Level propagation, character attached to choice
        button.onClick.AddListener(() => SendDistortionLevel(choice.distortionEffect));
        // Add conversation segue
        button.onClick.AddListener(() => SetChoice(choice.nextConversation));

        // TODO For History Tracker - add listener, send message with choice text? -just an idea
        button.onClick.AddListener(() => LoadScene(choice.nextScene));

        // Positive or negative effect particle listener here?

        return decisionController;
    }

    private static void SendImpactEffect(Choice selection){
        if (selection.hasEffect){
          if (selection.impact == CharacterEffects.Meet){
            selection.character.hasMetBefore = true;
          }
        }
    }

    private static void SetChoice(Conversation nextConversation)
    {
        GameObject dm = GameObject.Find("DialogueManager");
        dm.SendMessage("ChangeConversation", nextConversation);
    }

    void Start()
    {
        // If there's no Conversation Change Event, make one
        if (conversationChangeEvent == null)
            conversationChangeEvent = new ConversationChangeEvent();

        // Get the text component in the button and set the text to match the choice text
        GetComponent<Button>().GetComponentInChildren<Text>().text = choice.text;
    }

    // Invokes the event and broadcasts what conversation to load based on choice
    public void MakeDecision()
    {
        if(choice.distortionEffect <= 0)
        {
            Debug.Log("PlayPositiveeffect");

        }
        else
        {
            Debug.Log("PlayNegativeEffect");
        }
        conversationChangeEvent.Invoke(choice.nextConversation);
    }
    
    //public static void PlayEffect(GameObject particle, Button BTN)
    //{
       //Move to UI Manager
        //GameObject effect = Instantiate(particle, BTN.gameObject.transform);
        //effect.transform.parent = null;
        //delete after 2-3 seconds





    //}

private static void SendDistortionLevel(int distortionChange)
    {
        GameObject distortionManager = GameObject.Find("DistortionManager");
        distortionManager.SendMessage("UpdateDistortionLevel", distortionChange);
    }

    // BRUTE FORCE, REMOVE THESE TOO
    public static void LoadScene(string sceneName)
    {
        GameObject ui = GameObject.Find("UIController");
        ui.SendMessage("LoadScene", sceneName);
    }
}
