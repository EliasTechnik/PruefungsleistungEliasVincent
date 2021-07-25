using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QAgent{
    private RewardMatrix rewardMatrix;
    private double[][] qMatrix;
    private AIActionInterface actionInterface;
    private int[] previousState;
    private AIAction predictedAction;
    private double discountFactor=0.8;
    private double learningRate=0.5;
    private double explorationThreshold=0.8; //kleinere Werte führen zu einer "mutigeren" KI
    public QAgent(RewardMatrix _rewardMatrix, AIActionInterface _actionInterface){
        rewardMatrix=_rewardMatrix;
        actionInterface=_actionInterface;
        //qMatrix in der richtigen Größe initialisieren
        qMatrix=new double[rewardMatrix.RowCount][];
        double[] plainRow=new double[rewardMatrix.ActionCount];
        for(int i=0;i<rewardMatrix.ActionCount;i++){
            plainRow[i]=0;
        }
        for(int i=0;i<rewardMatrix.RowCount;i++){
            qMatrix[i]=plainRow;
        }
        previousState=new int[rewardMatrix.FeatureCount];
        for(int i=0;i<previousState.Length;i++){
            previousState[i]=0;
        } 
    }
    public bool setParams(double _discountFactor, double _learningRate, double _explorationThreshold){
        if(_discountFactor>=0){
            if(_learningRate>=0 && _learningRate<=1){ //Wertebereich sollte nochmal überdacht werden
                if(_explorationThreshold>=0 && _explorationThreshold<1){
                    discountFactor=_discountFactor;
                    learningRate=_learningRate;
                    explorationThreshold=_explorationThreshold;
                    return true;
                }
            }
        }
        return false;
    }
    public AIAction Predict(AIFeatureInterface _input){//sagt AIAction basierend auf _input vorraus //TODO: prüfen ob AIFeatureInterface richtig ist //Diskusion über Rnadom() vs System.Random()
        //Adresse (Zustand) durch _input bestimmen
        int[] stateAddress=new int[_input.featureCount];
        for(int i=0;i<_input.featureCount;i++){
            stateAddress[i]=_input[i].ConvertedInput;
        }
        previousState=stateAddress;
        //QMatrix befragen
        double[] state=qMatrix[rewardMatrix.addressToIndex(stateAddress)];
        int actionIndex=-1;
        //Bestimmen ob erkundet werden soll
        if(Random.value>explorationThreshold){
            //erkunden
            actionIndex=Mathf.RoundToInt((state.Length-1)*Random.value);
        }
        else{
            //auf QMatrix verlassen
            double bestValue=double.MinValue;
            for(int i=0;i<state.Length;i++){//finde die Aktion mit der höchsten Belohnung
                if(state[i]>bestValue){
                    bestValue=state[i];
                    actionIndex=i;
                }
            }
        }
        if(actionIndex==-1){//Fallback allerdings unnötig da bestValue mit doubloe.MinValue initialisiert wurde
               actionIndex=Mathf.RoundToInt((state.Length-1)*Random.value); 
        }
        predictedAction=actionInterface[actionIndex];
        return predictedAction;
    }
    public void Reward(AIFeatureInterface _input){//belohnt die KI anhand der rewardMatrix für den letzten Schritt
        //Adresse des aktuellen zusatnds bestimmen
        int[] stateAddress=new int[_input.featureCount];
        for(int i=0;i<_input.featureCount;i++){
            stateAddress[i]=_input[i].ConvertedInput;
        }
        //größtmögliche Belohnung für den jetzigen zustand ermitteln
        double maxReward=double.MinValue;
        double[] nextState=qMatrix[rewardMatrix.addressToIndex(stateAddress)];
        for(int i=0;i<nextState.Length;i++){
            if(nextState[i]>maxReward){
                maxReward=nextState[i];
            }
        }
        //QMatrix eintrag für die zuletzt ausgeführte Ation basierend auf den aktuellen Zustand updaten.
        qMatrix[rewardMatrix.addressToIndex(previousState)][predictedAction.ActionID]
            =(1-learningRate)*qMatrix[rewardMatrix.addressToIndex(previousState)][predictedAction.ActionID]
            +learningRate*(rewardMatrix.getStage(previousState)[predictedAction.ActionID]
            +discountFactor*maxReward);
        
    }
}