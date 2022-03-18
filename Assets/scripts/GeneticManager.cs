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
    public int initialPopulation = 50;
    [Range(0.0f, 1.0f)]
    public float mutationRate = 0.055f;

    [Header("Crossover Controls")]
    public int bestAgentSelection = 10;
    public int worstAgentSelection = 3;
    public int numberToCrossover = 20; // HARDCODED: actually is population - bestAgentSelection. then, remainder = 2(#crossover), so #crossover = remainder / 2

    [Header("Public View")]
    public int currentGeneration = 0;
    public int currentGenome = 0; // current car in the population

    private List<int> genePool = new List<int>(); // list of genomes in the population to select from for natural selection
    private int naturallySelected = 0; // number of genomes selected by natural selection
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
    // NOTE: WE MAY NOT NEED THE STARTING INDEX AT ALL, SINCE WE WILL BE BREEDING REMAINING POPULATION FROM GENE POOL ALL THE TIME.
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
            // start a new generation; REPOPULATE
            currentGeneration++;
            currentGenome = 0;
            naturallySelected = 0;
            genePool.Clear();

            // sort the population by fitness (descending)
            SortPopulation();
            // upload current gen's fittest genome to a text file 
            
            // pick best population, x amount of best agents, remaining population will be breeded
            NeuralNetwork[] nextPopulation = PickFittestPopulation();
            // breeding
            Breed(nextPopulation);
            // mutating

            // reset to current genome (first car in population)
            ResetToCurrentGenome();
        }
    }

    /*Natural Selection*/
    private void SortPopulation() {
        // sort the population by fitness (descending order) -- bubble sort. yeah, it's slow regardless of sort algo i pick
        for (int i = 0; i < population.Length; i++) {
            for (int j = i + 1; j < population.Length; j++) {
                if (population[i].fitness < population[j].fitness) {
                    NeuralNetwork temp = population[j];
                    population[j] = population[i];
                    population[i] = temp;
                }
            }
        }
    }

    private NeuralNetwork[] PickFittestPopulation() {
        // pick best x agents, remaining are to be breeded from gene pool containing the x best agents and y worst agents
        
        NeuralNetwork[] nextPopulation = new NeuralNetwork[initialPopulation];

        // the best x agents are the first x agents in the next population
        for (int i = 0; i < bestAgentSelection; i++) {
            nextPopulation[i] = population[i]; // copy the best agents to the next population; MAY NEED TO DO A DEEP COPY??? god, i hope not
            naturallySelected++;

            // add the best agents to the gene pool
            int timesToAddGood = (int)(population[i].fitness * 10); // fitter = more occurences in gene pool = more likely to be selected

            for (int g = 0; g < timesToAddGood; g++) {
                genePool.Add(i); // add best agents' index to gene pool
            }
        }

        // add the worst agents to the gene pool
        for (int j = 0; j < worstAgentSelection; j++) {
            int worstAgentIndex = population.Length - 1 - j; // index of the bad agents in the population
            int timesToAddBad = (int)(population[worstAgentIndex].fitness * 10);

            for (int b = 0; b < timesToAddBad; b++) {
                genePool.Add(worstAgentIndex); // add bad agents' index to gene pool
            }
        }

        return nextPopulation; // return the next population containing the x best agents and remaining unitialized genomes
    }

    /*Crossover: Will need to maintain a steady population, for this, make it 50.
    Process for each new generation:
    - Pick top 10 from previous generation
    - Breed the remaining 40 others from the gene pool containing the 10 best agents and 3 worst agents. 
    - Better the agents, the more likely to be chosen for breeding.
    - 40 children = 2 * # of crossovers, since each crossover produces two children. 
    - Thus, need to have 20 crossovers per generation.
    */
    private void Breed(NeuralNetwork[] nextPopulation) {
        // indices of parents in the current population
        int AParent = 0;
        int BParent = 0;

        for (int crosses = 0; crosses < numberToCrossover; crosses++) {
            // keep breeding until population is full
            while (AParent == BParent) {
                // pick random parents from the gene pool (same parent can't breed with itself)
                AParent = genePool[Random.Range(0, genePool.Count)]; 
                BParent = genePool[Random.Range(0, genePool.Count)];
            }

            // initialize the children
            NeuralNetwork child1 = new NeuralNetwork();
            NeuralNetwork child2 = new NeuralNetwork();
            child1.Initialize(netLayers);
            child2.Initialize(netLayers);
            child1.fitness = 0;
            child2.fitness = 0;

            // crossover of parents; genes are the WEIGHTS of the children
            // Child1: first half of weights from parent A, second half from parent B
            for (int i = 1; i < child1.layers.Length; i++) {
                int neuronsInPrev = child1.layers[i - 1]; // number of neurons in previous layer
                for (int j = 0; j < child1.neurons[i].Length; j++) {
                    // go through the neurons in current layer
                    for (int k = 0; k < neuronsInPrev; k++) {
                        if (j < child1.neurons[i].Length / 2) {
                            // first half of the layer
                            // go through the weights of neuron and make it of parent A's weights
                            child1.weights[i-1][j][k] = population[AParent].weights[i-1][j][k];
                        }
                        else {
                            // second half of the layer
                            // go through the weights of neuron and make it of parent B's weights
                            child1.weights[i-1][j][k] = population[BParent].weights[i-1][j][k];
                        }
                    }                
                }
            }
            naturallySelected++;
            print("nat selected: " + naturallySelected);
            nextPopulation[naturallySelected - 1] = child1; // add child1 to next population


            // Child2: first half of weights from parent B, second half from parent A
            for (int i = 1; i < child2.layers.Length; i++) {
                int neuronsInPrev = child2.layers[i - 1]; // number of neurons in previous layer
                for (int j = 0; j < child2.neurons[i].Length; j++) {
                    // go through the neurons in current layer
                    for (int k = 0; k < neuronsInPrev; k++) {
                        if (j < child2.neurons[i].Length / 2) {
                            // first half of the layer
                            // go through the weights of neuron and make it of parent B's weights
                            child2.weights[i-1][j][k] = population[BParent].weights[i-1][j][k];
                        }
                        else {
                            // second half of the layer
                            // go through the weights of neuron and make it of parent A's weights
                            child2.weights[i-1][j][k] = population[AParent].weights[i-1][j][k];
                        }
                    }
                }
            }
            naturallySelected++;
            print("nat selected: " + naturallySelected);
            nextPopulation[naturallySelected - 1] = child2; // add child1 to next population
        }
    }


    /*Crossover*/

    /*Mutation*/


}