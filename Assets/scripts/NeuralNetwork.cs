using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Random = UnityEngine.Random;

public class NeuralNetwork : MonoBehaviour
{
    private int[] layers; // number of neurons in each layer
    private float[][] neurons;
    private float[][][] weights;

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
            // go through neurons in current layer
            // give random weights to neurons
            for (int j = 0; j < neurons[i].Length; j++) { 
                float[] neuronWeights = new float[neuronsInPreviousLayer]; // create new array of weights for each neuron
                for (int k = 0; k < neuronsInPreviousLayer; k++) {
                    // give random weight to each neuron
                    neuronWeights[k] = Random.Range(-1f, 1f); // weight between -1 and 1
                }

                layerWeightsList.Add(neuronWeights);
            } 

            weightsList.Add(layerWeightsList.ToArray()); // convert list to jagged array
        }

        weights = weightsList.ToArray(); // convert list to jagged array

    }

    /*Feed forward algorithm*/
    public float[] FeedForward(float[] inputs) {
        // set input neurons
        for (int i = 0; i < inputs.Length; i++) {
            neurons[0][i] = inputs[i]; 
        }

        // go through layers with neurons with weights (hidden, output)
        for (int i = 1; i < layers.Length; i++) {
            // iterate through each layer
            for (int j = 0; j < neurons[i].Length; j++) {
                // iterate through neurons of each layer
                float value = 0.25f; // start w/ constant bias value
                for (int k = 0; k < neurons[i - 1].Length; k++) {
                    // iterate through neurons in previous layer
                    value += weights[i-1][j][k] * neurons[i-1][k]; // sum of (weights * neurons)
                }
                // set neuron value
                neurons[i][j] = Sigmoid(value);
            }
        }

        return neurons[neurons.Length - 1]; // return output neurons
    }

    public float Sigmoid(float value) {
        return (float)(1/(1+Mathf.Exp(-value)));
    }
} 
