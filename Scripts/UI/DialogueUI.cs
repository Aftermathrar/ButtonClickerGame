using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ButtonGame.Dialogue;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

namespace ButtonGame.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI conversantName;
        [SerializeField] TextMeshProUGUI extraHeaderText;
        [SerializeField] GameObject textGroupGO;
        [SerializeField] TextMeshProUGUI DialogueText;
        [SerializeField] Button nextButton;
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject choicePrefab;
        [SerializeField] Button quitButton;
        
        List<string> hotkeys = new List<string>();
        List<Button> choiceButtons = new List<Button>();
        int choiceCount;

        private void Awake() 
        {
            foreach (Transform item in choiceRoot)
            {
                choiceButtons.Add(item.GetComponent<Button>());
                item.gameObject.SetActive(false);
            }
            hotkeys = new List<string> {"Button0", "Button1", "Button2", "Button3", 
                "Button4", "Button5", "Button6","Button7" ,"Button8" , "Button9"};

            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            nextButton.onClick.AddListener(() => playerConversant.Next());
            quitButton.onClick.AddListener(() => playerConversant.Quit());
            playerConversant.onConversationUpdated += UpdateUI;
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateUI();
        }

        private void Update() 
        {
            if(Input.GetButtonDown("Submit"))
            {
                if(nextButton.IsActive()) nextButton.onClick.Invoke();
                else if(quitButton.IsActive()) quitButton.onClick.Invoke();
            }
            if(choiceRoot.gameObject.activeSelf)
            {
                for (int i = 0; i < choiceButtons.Count; i++)
                {
                    if(choiceButtons[i].IsActive() && Input.GetButtonDown(hotkeys[i]))
                    {
                        choiceButtons[i].onClick.Invoke();
                    }
                }
            }
        }

        void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if(!playerConversant.IsActive())
            {
                return;
            }
            conversantName.text = playerConversant.GetCurrentConversantName();
            textGroupGO.SetActive(true);
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());
            if(playerConversant.IsChoosing())
            {
                DialogueNode[] AINodes = playerConversant.GetResponses().ToArray();
                if(AINodes.Length > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, AINodes.Length);
                    conversantName.text = playerConversant.GetCurrentConversantName(AINodes[randomIndex]);
                    DialogueText.text = playerConversant.GetText(AINodes[randomIndex]);
                    nextButton.gameObject.SetActive(false);
                    quitButton.gameObject.SetActive(false);
                }
                else
                {
                    textGroupGO.SetActive(false);
                }
                ReturnChoicesToPool(choiceCount);
                BuildChoiceList();
            }
            else
            {
                DialogueText.text = playerConversant.GetText();
                bool hasNext = playerConversant.HasNext();
                nextButton.gameObject.SetActive(hasNext);
                quitButton.gameObject.SetActive(!hasNext);
            }
        }

        private void BuildChoiceList()
        {
            choiceCount = 0;
            foreach(DialogueNode choiceNode in playerConversant.GetChoices())
            {
                Button button;
                if (choiceButtons.Count > choiceCount)
                {
                    button = choiceButtons[choiceCount];
                    button.gameObject.SetActive(true);
                }
                else
                {
                    GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                    button = choiceInstance.GetComponent<Button>();
                    choiceButtons.Add(button);
                }
                button.onClick.AddListener(() =>
                {
                    playerConversant.SelectChoice(choiceNode);
                });
                TextMeshProUGUI textComp = button.GetComponentInChildren<TextMeshProUGUI>();
                textComp.text = choiceNode.GetText();
                choiceCount++;
            }
        }

        private void ReturnChoicesToPool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
