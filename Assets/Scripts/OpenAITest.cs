using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using System.Threading.Tasks;

public class OpenAITest : MonoBehaviour
{
    public string userInput;

    public void PressButton(){
        SetOpenAI();
    }

    public async void SetOpenAI(){
        var api = new OpenAIClient();
        var model = await api.ModelsEndpoint.GetModelDetailsAsync("text-davinci-003");
        Debug.Log(model.ToString());

        //Start "Loading" Icon

        var result = await api.CompletionsEndpoint.CreateCompletionAsync("Create an outline for an essay about " + userInput + " without an introduction or conclusion:", 
            maxTokens: 150, temperature: 0.3, frequencyPenalty: 0.0, model: model);
        Debug.Log(result);

        //End "Loading"
    }

    private void FormatData(){
        //Set Title Node as Center

        //Remove Introduction Section

        //Breakup Headers & Subsections into proper Nodes

        //Remove Conclusion

    }
}