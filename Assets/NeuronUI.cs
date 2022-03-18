using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeuronUI : MonoBehaviour
{
    public List<GameObject> neurons = new List<GameObject>();
    

    public Gradient gradient;
    private Image neuronImage;

    // set color of current based on gradient 
    public void SetNeuronColor(int index, float value) {
        neuronImage = neurons[index].GetComponent<Image>();
        neuronImage.color = gradient.Evaluate(value);
    }

}
