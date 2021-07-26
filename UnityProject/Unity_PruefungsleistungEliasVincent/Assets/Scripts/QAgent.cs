using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using static FeatureBuilderTools;
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
            qMatrix[i]=(double[])plainRow.Clone();
        }
        previousState=new int[rewardMatrix.FeatureCount];
        for(int i=0;i<previousState.Length;i++){
            previousState[i]=0;
        } 
    }
    public QAgent(string _path){
        StreamReader file=new StreamReader(_path);
        string data="";
        string line="";
        while((line = file.ReadLine()) != null){
              data+=line;
          }
        XMLobject xo=new XMLobject();
        xo.decodeXML(data);

        discountFactor=double.Parse(xo.find("discountFactor").Payload);
        learningRate=double.Parse(xo.find("learningRate").Payload);
        explorationThreshold=double.Parse(xo.find("explorationThreshold").Payload);

        FeatureBuilder fb =new FeatureBuilder(xo.find("rewardMatrix").findOnFirstLevel("FeatureBuilder"));
        rewardMatrix = new RewardMatrix(fb);
        rewardMatrix.generateMatrix();
        
        XMLobject xo_qmatrix=xo.find("qMatrix"); 
        qMatrix=new double[xo_qmatrix.ChildCount][];
        for(int i=0;i<xo_qmatrix.ChildCount;i++){
            XMLobject xo_row=xo_qmatrix[i];
            double[] row=new double[xo_row.ChildCount];
            for(int j=0;j<xo_row.ChildCount;j++){
                row[j]=double.Parse(xo_row[j].Payload.ToString());
            }
            qMatrix[i]=row;
        }

        actionInterface=new AIActionInterface(xo.find("AIActionInterface"));

        
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
    public AIAction PredictAndTrain(AIFeatureInterface _input){//sagt AIAction basierend auf _input vorraus //TODO: prüfen ob AIFeatureInterface richtig ist //Diskusion über Random() vs System.Random()
        //Adresse (Zustand) durch _input bestimmen
        int[] stateAddress=_input.getStateAddress();
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
    public AIAction Predict(AIFeatureInterface _input){
        //Adresse (Zustand) durch _input bestimmen
        int[] stateAddress=_input.getStateAddress();
        //QMatrix befragen
        double[] state=qMatrix[rewardMatrix.addressToIndex(stateAddress)];
        int actionIndex=-1;
        double bestValue=double.MinValue;
        for(int i=0;i<state.Length;i++){//finde die Aktion mit der höchsten Belohnung
            if(state[i]>bestValue){
                bestValue=state[i];
                actionIndex=i;
            }
        }
        if(actionIndex==-1){//Fallback allerdings unnötig da bestValue mit doubloe.MinValue initialisiert wurde
               actionIndex=Mathf.RoundToInt((state.Length-1)*Random.value); 
        }
        return actionInterface[actionIndex];
    }
    public void Reward(AIFeatureInterface _input){//belohnt die KI anhand der rewardMatrix für den letzten Schritt
        //Adresse des aktuellen zusatnds bestimmen
        int[] stateAddress=_input.getStateAddress();
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
        Debug.Log("Updated "+addressToStringRev(previousState)+" to "+qMatrix[rewardMatrix.addressToIndex(previousState)][predictedAction.ActionID].ToString());
        
    }
    
    public AIFeatureInterface GetFeatureInterface(){
        AIFeatureInterface fi=new AIFeatureInterface();
        fi=rewardMatrix.FeatureBuilder.getFeatureInterface(fi);
        return fi;
    }
    
    public void saveToFile(string _path){//speichert alle Objekte als XML
        XMLobject xo=new XMLobject("QAgent");
        xo.addChild(new XMLobject("discountFactor",discountFactor.ToString()));
        xo.addChild(new XMLobject("learningRate",learningRate.ToString()));
        xo.addChild(new XMLobject("explorationThreshold",explorationThreshold.ToString()));

        xo.addChild(new XMLobject("rewardMatrix",rewardMatrix.FeatureBuilder.toXML()));

        XMLobject xo_qmatrix=new XMLobject("qMatrix");

        for(int i=0;i<qMatrix.Length;i++){
            XMLobject xo_row=new XMLobject("row");
            xo_row.addAttribute("row_index",i.ToString());
            double[] row=qMatrix[i];
            for(int j=0;j<row.Length;j++){
                xo_row.addChild(new XMLobject("cell",row[j].ToString()));
            }
            xo_qmatrix.addChild(xo_row);
        }
        xo.addChild(xo_qmatrix);
        xo.addChild(new XMLobject("AIActionInterface",actionInterface.toXML()));
        StreamWriter file=new StreamWriter(_path);
        file.WriteLine(xo.serialize());
        file.Close();
    }
    public void printQMatrix(){
        for(int i=0;i<qMatrix.Length;i++){
            string erg="";
            erg=addressToStringRev(rewardMatrix.indexToAddress(i));
            for(int j=0;j<actionInterface.ActionCount;j++){
                erg+=qMatrix[i][j].ToString()+" ";
            }
            Debug.Log(erg);
        }
    }
}