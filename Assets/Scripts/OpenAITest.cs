using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using System.Threading.Tasks;

public class OpenAITest : MonoBehaviour
{
    public void PressButton(){
        SetOpenAI();
    }

    private async void SetOpenAI(){
        //var api = new OpenAIClient(OpenAIAuthentication.LoadFromDirectory("your/path/to/.openai"));;

        var api = new OpenAIClient();
        var model = await api.ModelsEndpoint.GetModelDetailsAsync("text-davinci-003");
        Debug.Log(model.ToString());

        var result = await api.CompletionsEndpoint.CreateCompletionAsync("Create an outline for an essay about jelly beans:", 
            maxTokens: 150, temperature: 0.3, frequencyPenalty: 0.0, model: model);
        Debug.Log(result);
    }
}