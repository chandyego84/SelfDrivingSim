using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticManager : MonoBehaviour
{
    [Header("References")]
    public CarController controller;

    [Header("Neural Network Controls")]
    public int[] netLayers = new int [] {3, 5, 2}; // size of each layer in a NN

    [Header("Controls")]
    public int initialPopulation = 85;
    [Range(0.0f, 1.0f)]
    public float mutationRate = 0.055f;

    [Header("Crossover Controls")]
    public int bestAgentSelection = 8;
    public int worstAgentSelection = 3;
    public int numberToCrossover; // e.g. if crossover is only 15, 30 children created--remaining are randomized

    [Header("Public View")]
    public int currentGeneration;
    public int currentGenome; // current car in the population

    private List<int> genePool = new List<int>(); // list of genomes in the population to select from for natural selection
    private int naturallySelected; // number of genomes selected by natural selection
    private NeuralNetwork[] population; 

    /*Generate Initial Population*/
    private void Start() {
        CreatePopulation();
    }

    /*Create initial population of cars, each having their own random NN*/
    private void CreatePopulation() {
        population = new NeuralNetwork[initialPopulation]; // create array of neural networks of size initialPopulation
        generateRandomDNA(population, 0); // generate the initial population
    }

    /*Generte random values for each neural network in the population
    starting from a specific index in population (we don't want to always randomize all in the population)
    */
    private void generateRandomDNA(NeuralNetwork[] population, int startingIndex) {
        for (int i = startingIndex; i < population.Length; i++) {
            population[i] = new NeuralNetwork();
            population[i].Initialize(netLayers); 
         }

         ResetToCurrentGenome(); // set the current car to the first one/first genome in the population
    }

    private void ResetToCurrentGenome() {
        // reset car to current genome with its own unique brain/NN
        controller.ResetNetwork(population[currentGenome]);
    }

    public void Death(float fitness, NeuralNetwork deadBrain) {
        // store car's fitness to neural network fitness
        if (currentGenome < population.Length - 1) {
            population[currentGenome].fitness = fitness;
            currentGenome++;
            ResetToCurrentGenome(); // go to next genome in population, run their brain
        }

        else {
            // repopulate
        }
    }

    /*Evaluate the population*/


    /*Natural Selection*/

    /*Crossover*/

    /*Mutation*/


}
