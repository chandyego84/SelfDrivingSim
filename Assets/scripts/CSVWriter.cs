using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// writing data to CSV
public class CSVWriter : MonoBehaviour
{
    string filename = ""; // var to write file to

    // data we want
    [System.Serializable]
    public class Data {
        public int generation;
        public float fitness; 
    }

    // list of data (fitness of each generation)
    [System.Serializable]
    public class FitnessList {
        public List<Data> genFitnessList;
    }

    // variable storing our list of data
    public FitnessList _fitnessList = new FitnessList();

    public void CreateCSV() {
            filename = "C:/Users/chand/fitnessData.csv";
            print(filename);
            // use textwriter to open stream
            TextWriter tw = new StreamWriter(filename, false); // false-->overwritew file on first time
            tw.WriteLine("Generation, Best Fitness"); // write header
            tw.Close(); // close stream
    }

    // takes everything in array list and write to csv
    public void WriteCSV() {
        if (_fitnessList.genFitnessList.Count > 0) {
            // there is data to write
            // append each line to end of the file
            TextWriter tw = new StreamWriter(filename, true); // true-->append to file

            for (int i = 0; i < _fitnessList.genFitnessList.Count; i++) {
                // write each line to csv
                tw.WriteLine(_fitnessList.genFitnessList[i].generation + "," + _fitnessList.genFitnessList[i].fitness);
            }

            tw.Close(); // close stream
        }
    }


}
