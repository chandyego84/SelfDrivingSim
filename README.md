### Introduction
The goal of this project was to simulate a "self-driving" car in the Unity engine using neural networks and the genetic algorithm. The neural network was used to navigate through the track and was trained or optimized through an evolutionary process. Given a predefined path, the car's ability to drive itself was primarily based on how far it was able to move along the track without hitting another object or steering off to the side of the track. To add some more complexity, I ensured that the car had to take slight turns. 

### Running It
Honestly, I did this project during spring break of my sophomore year in college. I didn't think about documenting how to run it. Um, will need to do at some point...

### Neural Networks
Neural networks are the core of deep learning algorithms and both their name and structure are inspired by the human brain. The neural network is a collection of neurons that are connected to each other and can be trained to learn patterns in data. They rely on data to learn and improve their accuracy over time. The network consists of an input layer where the value from each input node travels through the arbitrary amount of hidden layers, and then finally through the output nodes to produce some results. Each node produces an output based on the connections from the previous layer (weights) and some bias, and can be described shortly by an activation function which sums the product of the weights and the corresponding node values attached to the weights, plus a bias value. If you want to learn more, this [project on recognizing handwritten digits](http://neuralnetworksanddeeplearning.com/chap1.html) is a good place to start.

![CAR!](car_gif.gif)

### Genetic Algorithm
As stated, the neural network needs to be trained in order to produce desired results which should allow the car to navigate through the track. The genetic algorithm is a heuristic evolutionary process inspired by the process of natural selection. Genetic algorithms are typically used to solve optimization or search problems, where in this case the neural network is being optimized to produce the best outputs for the car to travel through the track. Again, for more learning, this [article](https://medium.com/@AnasBrital98/genetic-algorithm-explained-76dfbc5de85d) is a good place to start.

There are variations to each step, but the main algorithm is as follows:
1. Create a population of random individuals (set of cars with random neural networks and random weights)
2. Evaluate the fitness of each individual
3. Select the best performing or fittest individuals for the next generation
4. Create a new generation of individuals using crossover and mutation
5. Repeat steps 2-4 until the desired number of generations is reached

### Implementation
A car's neural network is structured with three nodes in the input layer which are lasers pointing directly in front of the car and 45 degrees to each side, five nodes in the hidden layer, and two output nodes which were the acceleration and turning angle of the car. The lasers or input nodes returned the distances between the car and objects in front of the car. These values fed in through the hidden layers using the feed forward algorithm at a rate of 50 Hz and produced an acceleration value between 0 and 1 and a turning angle between -1 and 1. Each neural network had different weight values, but I kept the biases the same for simplicity.

The parameters for the genetic algorithm implementation were mostly arbitrary and certain methods were chosen based on simplicity, as this is my first time working with NNs or training algorithms. The fitness of a car was determined by the distance it covered on the track and its average speed with a stronger importance to the distance it covered. Each generation had 80 cars where 10 of the fittest cars automatically moved on to the next generation and their neural networks were added to a gene pool with a likelihood of being selected for crossover (breeding) proportional to the fitness value resulting from the neural network. Three of the worst performing cars and their neural networks were also added into this gene pool to reduce chances of converging to a [local optima](http://www2.denizyuret.com/pub/aitr1569/node6.html). The gene pool was then used to create 60 new cars (implying 30 crossovers, as each crossover produced two children) with neural networks breeded from two parents in the gene pool. One-point [crossover method](https://www.sciencedirect.com/topics/computer-science/point-crossover) was used for the creation of the two new children, where each child had one half of its network from parent A and the other half from parent B. The remaining 7 children were simply random neural networks for diversity and preventing local optima. I also added a mutation rate of 0.05% for each new car to, again, prevent converging to a local optima. The mutation consists of randomly changing some of the weights of the neural network. 

### Conclusion
After multiple sessions fixing and debugging local optima errors and incorrect crossovers, it took about a dozen generations to produce a neural network that could move through the course at a sufficient rate. 

![Fitness Data](fitnessGraph.jpg)

[(Shoutout to this post. Helped me with creating my first NN).](https://towardsdatascience.com/building-a-neural-network-framework-in-c-16ef56ce1fef)
