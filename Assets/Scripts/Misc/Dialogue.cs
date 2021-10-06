using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private float characterDelay = 0.08f;
    [SerializeField] private float sentenceDelay = 2;
    [SerializeField] private float textOffset = 1;
    [SerializeField] private int fontSize = 75;
    [SerializeField] private int maxCharactersPerLine = 20;
    [SerializeField] private List<String> sentences = new List<string>();

    
    private GuardController guardController;
    private GameObject textObj;
    private TextMesh tm;
    private bool triggered = false;
    private int sentenceIndex = 0;
    private int lineCharCount = 0;


    private void Start()
    {
        guardController = gameObject.transform.parent.gameObject.GetComponent<GuardController>();
        CreateTextMesh();
    }

    private void Update()
    {
        if (guardController.isDead)
        {
            Destroy(gameObject);
            return;
        }
        
        SetTextPosition();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !triggered)
            StartDialogue();
    }

    private void StartDialogue()
    {
        triggered = true;
        sentenceIndex = 0;
        tm.text = "";

        StartCoroutine(PrintCharacters());
    }

    IEnumerator PrintCharacters()
    {
        tm.text = "";
        
        foreach (char letter in sentences[sentenceIndex].ToCharArray()) {
            tm.text += letter;
            lineCharCount++;
            if (lineCharCount > maxCharactersPerLine && tm.text.ElementAt(tm.text.Length - 1) == ' ')
            {
                tm.text += "\n";
                lineCharCount = 0;
            }
            
            yield return new WaitForSeconds(characterDelay);
        }

        sentenceIndex++;
        if (sentenceIndex < sentences.Count)
            StartCoroutine(PrintNextSentence());
    }

    IEnumerator PrintNextSentence()
    {
        lineCharCount = 0;
        yield return new WaitForSeconds(sentenceDelay);
        StartCoroutine(PrintCharacters());
    }

    
    private void SetTextPosition()
    {
        tm.transform.position = transform.position + Vector3.up * textOffset;
    }

    private void CreateTextMesh()
    {
        textObj = new GameObject("Speaker_Text");
        textObj.transform.parent = transform;
        textObj.AddComponent<MeshRenderer>();
        tm = textObj.AddComponent<TextMesh>();
        tm.color = Color.white;
        tm.fontStyle = FontStyle.Italic;
        tm.alignment = TextAlignment.Center;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.characterSize = 0.06f;
        tm.fontSize = fontSize;
    }
}