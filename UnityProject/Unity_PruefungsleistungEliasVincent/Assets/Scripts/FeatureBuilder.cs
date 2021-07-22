using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static FeatureBuilderTools; //nicht erforderlich
public static class FeatureBuilderTools{
    public static string addressToString(int[] address){
        string erg="";
        foreach(int i in address){
            erg+=i.ToString();
        }
        return erg;
    }
    public static string addressToStringRev(int[] address){
        string erg="";
        for(int i=address.Length-1;i>=0;i--){
            erg+=address[i].ToString();
        }
        return erg;
    }
}

public class AddressLookupTable{
    private List<List<int>> addressList;

    public AddressLookupTable(){
        addressList=new List<List<int>>();
    }
    public int getIndex(int[] _address){
        for(int i=0;i<addressList.Count;i++){
            List<int> l=addressList[i];
            int[] a2=l.ToArray();
            int match=0;
            for(int j=0;j<a2.Length;j++){
                if(_address[j]==a2[j]){
                    match++;
                }
            }
            if(match==a2.Length){
                return i;
            }
        }
        return -1;
    }
    public int[] getAddress(int index){
        if(index<addressList.Count){
            return addressList[index].ToArray();
        }
        else{
            return null;
        }   
    }
    public void add(int[] _address){
        List<int> l =new List<int>();
        foreach(int i in _address){
            l.Add(i);
        }
        addressList.Add(l);
    }
}
public class FeatureBuilder{
    private FeatureBuilder nextElement; //null wenn es das letzte Element ist
    private AIFeature content;
    private int exponent;
    private int pointer;
    public int Pointer{get{return pointer;}}
    public int Exponent{get{return exponent;}}
    public int CombinationCount{get{
        if(nextElement==null){
            return exponent+1;
        }
        else{
            return (exponent+1)*nextElement.CombinationCount;
        }
    }}
    public AIActionInterface ActionInterface{get{return content.ActionIF;}}
    public FeatureBuilder(AIFeature _content){
        content=_content;
        exponent=content.InputResolution-1;
        nextElement=null;
    }
    public FeatureBuilder(AIFeature _content, FeatureBuilder _nextObject){
        content=_content;
        exponent=content.InputResolution-1;
        nextElement=_nextObject;
    }
    public double getReward(int[] _address, AIAction _action){//Rekursiv
        if(_address.Length==1){
            return content.Rewards(_action)[_address[0]];
        }
        else{
            int[] newAddress=new int[_address.Length-1];
            for(int i=0;i<_address.Length-1;i++){
                newAddress[i]=_address[i+1];
            }
            return content.Rewards(_action)[_address[0]]+nextElement.getReward(newAddress,_action);
        }
    }
    public AIFeature getFeature(int[] _address){
        if(_address.Length==1){
            return content;
        }
        else{
            int[] newAddress=new int[_address.Length-1];
            for(int i=0;i<_address.Length-1;i++){
                newAddress[i]=_address[i+1];
            }
            return nextElement.getFeature(newAddress);
        }
    }
    public void Reset(){
       pointer=0; 
       if(nextElement!=null){
           nextElement.Reset();
       }
    }
    public bool Next(){
        if(nextElement!=null){
            if(nextElement.Next()){ //true wenn der pointer auf 0 gesprungen ist
                if(pointer<exponent){
                    pointer++;
                    return false;
                }
                else{
                    pointer=0;
                    return true;
                }
            }
            else{
                return false;
            }
        }
        else{
            if(pointer<exponent){
                pointer++;
                return false;
            }
            else{
                pointer=0;
                return true;
            }
        } 
    }
    public int[] getAddress(){
        if(nextElement==null){//Letztes Element der Permutation
            //Debug.Log("LastAddressBit: "+pointer.ToString());
            return new int[]{pointer};
        }
        else{
            int[] result=nextElement.getAddress();
            int[] result2=new int[result.Length+1];
            for(int i=0;i<result.Length;i++){
                result2[i]=result[i];
            }
            result2[result.Length]=pointer;
            return result2;
        }
    }
}
public class AIFeature{ //Der name ist vlt noch ein wenig unglücklich gewählt
    private int inputResolution;
    private double[][] rewards;
    private AIActionInterface actionIF;
    public AIFeature(int _inputResolution, double[][] _rewards, AIActionInterface _actionIF){ 
        inputResolution=_inputResolution;
        rewards=_rewards;
        actionIF=_actionIF;
    }
    public int InputResolution{get{return inputResolution;}}
    public double[] Rewards(AIAction _action){
        List<double> result=new List<double>();
        for(int i=0;i<inputResolution;i++){
            result.Add(rewards[i][_action.ActionID]);
        }
        return result.ToArray();
    }
    public AIActionInterface ActionIF{get{return actionIF;}}

}
public class QLearning{

}
public class QModell{

}
public class RewardMatrix{
    private AIActionInterface actionInterface;
    private double[][] rewMatrix;
    private AddressLookupTable indexList;
    private FeatureBuilder featureBuilder;
    public RewardMatrix(FeatureBuilder _featureBuilder){
        featureBuilder=_featureBuilder;
        actionInterface=featureBuilder.ActionInterface;
        indexList=new AddressLookupTable();
    }
    public void generateMatrix(){//method: sum
        rewMatrix=new double[getMatrixSize()[2]][];
        featureBuilder.Reset();
        int index=0;
        do{
            double[] row=new double[actionInterface.ActionCount];
            for(int actionCounter=0;actionCounter<actionInterface.ActionCount;actionCounter++){
                row[actionCounter]=featureBuilder.getReward(featureBuilder.getAddress(),actionInterface.GetAIAction(actionCounter));
            }
            rewMatrix[index]=row;
            index++;
            indexList.add(featureBuilder.getAddress());
        }while(!featureBuilder.Next());
    }
    public int[] getMatrixSize(){//actions, features,featureCombinations,Fields
        int[] size=new int[4];
        size[0]=actionInterface.ActionCount;
        size[1]=featureBuilder.getAddress().Length;
        size[2]=featureBuilder.CombinationCount;
        size[3]=size[2]*size[0];
        return size;
    }
    public double[] getStage(int[] _address){
        int index=indexList.getIndex(_address);
        Debug.Log("Index: "+index.ToString());
        return rewMatrix[index];
    } 
}
public class AIAction{
    private string actionName;
    private int actionID;
    public string ActionName{get{return actionName;}}
    public int ActionID{get{return actionID;}}
    public AIAction(int _actionID, string _actionName){
        actionName=_actionName;
        actionID=_actionID;
    }
}
public class AIActionInterface{
    private List<AIAction> actions;
    private int actionCount;
    public int ActionCount{get{return actionCount;}}
    public AIAction this[int index]{
        get{
            return actions[index];
        }
    }
    public AIActionInterface(){
        actions=new List<AIAction>();
        actionCount=0;
    }
    public int addAction(string _name){
        actions.Add(new AIAction(actionCount,_name));
        actionCount++;
        return actionCount-1;
    }
    public AIAction GetAIAction(int actionIndex){
        foreach(AIAction a in actions){
            if(a.ActionID==actionIndex){
                return a;
            }
        }
        return null;
    }
    public AIAction GetAIAction(string _name){
        foreach(AIAction a in actions){
            if(a.ActionName==_name){
                return a;
            }
        }
        return null;
    }
}

