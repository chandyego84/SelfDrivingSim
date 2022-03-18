using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Random = UnityEngine.Random;

[Serializable]
public class NeuralNetwork : MonoBehaviour
{
    public int[] layers; // number of neurons in each layer

    public float[][] neurons; // contains neurons in each layer
    public float[][][] weights; // contains weights between neurons

    public float fitness;

    // neuron and weight counts for UI display tracking
    private int neuronCount = 0;
    private int weightCount = 0;

    /*constructing the matrix*/
    public void Initialize(int[] inputLayers) {
        // initialize layers w/ size of each layer (# of neurons)
        this.layers = new int[inputLayers.Length]; // init # of layers
        for (int i = 0; i < inputLayers.Length; i++) { // go through each layer
            this.layers[i] = inputLayers[i]; // init size of ith layer (# of neurons)
        }

        InitNeurons();
        InitWeights();
    }   

    /*Initialize the neurons*/
    private void InitNeurons() {
        List<float[]> neuronsList = new List<float[]>();

        for (int i = 0; i < layers.Length; i ++) { // go through all layers
            // creating new float array of size based on # of neurons in ith layer
            neuronsList.Add(new float[layers[i]]);
        }
        
        neurons = neuronsList.ToArray(); // convert list to jagged array
    }

    /*Initialize the weights*/
    private void InitWeights() {
        List<float[][]> weightsList = new List<float[][]>();

        // go through all layers with weight connection (starts at hidden layer)
        for (int i = 1; i < layers.Length; i++) {
            List<float[]> layerWeightsList = new List<float[]>(); // create new list of weights for each layer

            int neuronsInPreviousLayer = layers[i - 1]; // # of neurons in previous layer
            for (int j = 0; j < neurons[i].Length; j++) { 
                // go through neurons in current layer
                float[] neuronWeights = new float[neuronsInPreviousLayer]; // create new array of weights for each neuron
                for (int k = 0; k < neuronsInPreviousLayer; k++) {
                    // give random weight to each neuron
                    neuronWeights[k] = Random.Range(-1f, 1f); // weight between -1 and 1
                }

                layerWeightsList.Add(neuronWeights); // add neuron's weights to layer's weights list
            } 

            weightsList.Add(layerWeightsList.ToArray()); // add the layer's weights list to weights list and convert list to jagged array
        }

        weights = weightsList.ToArray(); // convert list to jagged array

    }

    /*Feed forward algorithm*/
    public (float, float) FeedForward(float[] inputs, NeuronUI neuronUI) {
        // reset neuron count and weight count (reset so NN visual is updated)
        neuronCount = 0;
        weightCount = 0;

        // set input neurons
        for (int i = 0; i < inputs.Length; i++) {
            neurons[0][i] = inputs[i];
            neuronUI.SetNeuronColor(i, inputs[i]);
            neuronCount++;
        }

        // go through layers with neurons with weights (hidden, output)
        for (int i = 1; i < layers.Length; i++) {
            // iterate through each layer
            for (int j = 0; j < neurons[i].Length; j++) {
                // iterate through neurons of each layer
                float value = 0.25f; // start w/ constant bias value
                for (int k = 0; k < neurons[i - 1].Length; k++) {
                    // iterate through neurons in previous layer
                    value += weights[i-1][j][k] * neurons[i-1][k]; // sum of all (weight * neuron)
                    neuronUI.SetWeightsColor(weightCount, weights[i-1][j][k]);
                    weightCount++;
                }
                // set neuron value
                // use tanh activation fnuction
                neurons[i][j] = (float)Math.Tanh(value);
                neuronUI.SetNeuronColor(neuronCount, neurons[i][j]); // set color of neuron based on value
                neuronCount++;
            }
        }

        // first output: acceleration, second output: steering
        return (Sigmoid(neurons[neurons.Length-1][0]), (float)Math.Tanh((neurons[neurons.Length-1][1])));
        //return neurons[neurons.Length - 1]; // return output neurons
    }

    // activation fxn returnign 0-1, used for steering value
    public float Sigmoid(float value) {
        return (float)(1/(1+Mathf.Exp(-value)));
    }
} 