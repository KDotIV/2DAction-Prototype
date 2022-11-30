using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.DiagSystem;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConvo playerConvo;
        [SerializeField] TextMeshProUGUI speakerName;
        [SerializeField] TextMeshProUGUI speakerText;
        [SerializeField] Button nextButton;
        [SerializeField] GameObject AIResponse;
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject choicePrefab;
        [SerializeField] Button quitButton;
        void Start()
        {
            playerConvo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConvo>();
            playerConvo.onConversationUpdated += UpdateUI;
            nextButton.onClick.AddListener(() => playerConvo.Next());
            quitButton.onClick.AddListener(() => playerConvo.Quit());
            UpdateUI();
        }

        void Next()
        {
            playerConvo.Next();
        }

        public void UpdateUI()
        {
            gameObject.SetActive(playerConvo.IsActive());
            if(!playerConvo.IsActive())
            {
                return;
            }
            AIResponse.SetActive(!playerConvo.isChoosing());
            choiceRoot.gameObject.SetActive(playerConvo.isChoosing());
            if(playerConvo.isChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                speakerName.text = playerConvo.GetSpeaker();
                speakerText.text = playerConvo.GetText();
                nextButton.gameObject.SetActive(playerConvo.HasNext());
            }
        }

        private void BuildChoiceList()
        {
            foreach (Transform item in choiceRoot)
            {
                Destroy(item.gameObject);
            }
            foreach (DialogueNode choiceText in playerConvo.GetChoices())
            {
                GameObject _choiceInstance = Instantiate(choicePrefab, choiceRoot);
                var _textComp = _choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                _textComp.text = choiceText.GetText();
                Button _button = _choiceInstance.GetComponentInChildren<Button>();
                _button.onClick.AddListener(() => 
                {
                    playerConvo.SelectChoice(choiceText);
                });
            }
        }
    }
}
