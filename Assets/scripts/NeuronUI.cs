using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeuronUI : MonoBehaviour
{
    public List<GameObject> neurons = new List<GameObject>();
    public List<GameObject> weights = new List<GameObject>();

    public Gradient gradient;
    private Image neuronImage;
    private Image weightImage;

    // set color of current based on gradient 
    public void SetNeuronColor(int index, float value) {
        neuronImage = neurons[index].GetComponent<Image>();
        neuronImage.color = gradient.Evaluate(value);
    }

    // set color of weight based on gradient
    public void SetWeightsColor(int index, float value) {
        weightImage = weights[index].GetComponent<Image>();
        weightImage.color = gradient.Evaluate(value);
    }

}
