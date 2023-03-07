using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using System.Net.Http;

public class GPT3 : MonoBehaviour
{
    public GameObject LoadingText;
    public GameObject StartUI;
    public GameObject MindMapUI;

    public TMP_InputField TextEntry;

    public GameObject ParentNode;
    public GameObject[] ChildNodes;

    public GameObject GenDisplay;
    public TextMeshProUGUI GenText;
        
    private List<string> childText = new List<string>();
    private List<string>[] grandchildText = new List<string>[4];

    private string returnedData;
    private int sectionCounter = -1;
    private int listCounter = 0;

    private bool firstRun = true;

    private void Start(){
        Reset();
    }

    public void PressButton(){
        StartUI.SetActive(false);
        LoadingText.SetActive(true);

        SetOpenAI();
    }

    public async void SetOpenAI(){
        const string key = "YOUR API KEY";
        const string url = "https://api.openai.com/v1/chat/completions";

        var request = new
        {
            model = "gpt-3.5-turbo",
            prompt = "Create an outline for an essay about " + TextEntry.text + " without an introduction or conclusion:",
            max_tokens = 130,
            temperature = 0.3,
            n = 1,

        };
    
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");
        var requestJson = JsonConvert.SerializeObject(request);
        var requestContent = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
        var httpResponseMessage = await httpClient.PostAsync(url, requestContent);
        var jsonString = await httpResponseMessage.Content.ReadAsStringAsync();

        returnedData = result.ToString();

        Debug.Log(returnedData);

        FormatData();
    }

    private void FormatData(){
        List <string> lines = new List<string>(returnedData.Split('\n'));

        for (int i=0; i<lines.Count; i++){
            Debug.Log(lines[i]);

            if (lines[i] != "" && lines[i].Length > 1){
                if (lines[i][0] == 'I'){    //Child
                    childText.Add(lines[i].Substring(lines[i].IndexOf('.') + 1));

                    sectionCounter++;
                    listCounter = 0;
                } else if (lines[i][0] != 'V' && lines[i][0] != 'E' && lines[i][0] != 'F' && lines[i][0] != 'G' && lines[i][0] != 'H') {    //Grandchild, ignore 5th bullet point due to 2d space limitation
                    grandchildText[listCounter][sectionCounter] = lines[i].Substring(2);
                    listCounter++;
                } else if (lines[i][0] == 'V'){ //Immediately end if beyond 4 Child Nodes
                    i = lines.Count;
                }
            }
        }

        firstRun = false;
        ShowData();
    }

    public void ShowData(){
        LoadingText.SetActive(false);
        GenDisplay.SetActive(true);

        GenText.text = returnedData;
    }

    public void CreateMindMap(){
        LoadingText.SetActive(true);
        GenDisplay.SetActive(false);

        ParentNode.transform.Find("Element Text").GetComponent<TextMeshProUGUI>().text = TextEntry.text;

        List<GameObject> GrandchildNodes = new List<GameObject>();
        for (int x = 0; x < ChildNodes.Length; x++){
            if (childText.Count > x){
                ChildNodes[x].SetActive(true);
                ChildNodes[x].transform.Find("Element Text").GetComponent<TextMeshProUGUI>().text = childText[x];
            } else {
                ChildNodes[x].SetActive(false);
            }

            GrandchildNodes.Clear();
            foreach(Transform grandchild in ChildNodes[x].transform){
                if (grandchild.tag == "Grandchild"){
                    GrandchildNodes.Add(grandchild.gameObject);
                }
            }

            //x & y text being added is not on correct nodes?
            for (int y = 0; y < GrandchildNodes.Count; y++){  //GrandchildText 1st = List position, 2nd = Child
                Debug.Log("Child: " + x + ", Grandchild: " + y + ", gchild text: " + grandchildText[y][x]);
                if (grandchildText[y][x] != "" && grandchildText[y][x] != null){
                    GrandchildNodes[y].SetActive(true);
                    GrandchildNodes[y].transform.Find("Element Text").GetComponent<TextMeshProUGUI>().text = grandchildText[y][x];
                } else {
                    GrandchildNodes[y].SetActive(false);
                }
            }
        }

        LoadingText.SetActive(false);
        MindMapUI.SetActive(true);
    }

    public void Reset(){
        StartUI.SetActive(true);
        LoadingText.SetActive(false);
        MindMapUI.SetActive(false);
        GenDisplay.SetActive(false);
        sectionCounter = -1;

        childText.Clear();
        for (int i=0; i<4; i++){
            if (!firstRun)
                grandchildText[i].Clear();
            grandchildText[i] = new List<string> {"", "", "", "", ""};
        }
    }

    /*private void SetChildren(GameObject[] objects, List<string> values){
        for (int i = 0; i < objects.Length; i++){
            if (values[i] != null || values[i] != ""){

            } else {
                objects[i].SetActive(false);
            }

            //GameObject[] grandChildren = objects[i].FindGameObjectsWithTag("Node");
            //get string values
            //SetGrandchildren(grandChildren, values);
        }

    }

    private void SetGrandchildren(GameObject[] objects, List<string> values){
        for (int i = 0; i < objects.Length; i++){
            if (values[i] != null || values[i] != ""){
                objects[i].GetComponent<TextMeshProUGUI>().text = values[i];
            } else {
                objects[i].SetActive(false);
            }
        }
    }*/
}