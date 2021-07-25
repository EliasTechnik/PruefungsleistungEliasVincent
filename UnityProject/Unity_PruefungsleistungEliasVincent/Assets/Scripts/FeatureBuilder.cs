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
    public AIFeatureInterface getFeatureInterface(AIFeatureInterface _fi){
        if(nextElement!=null){
            _fi=nextElement.getFeatureInterface(_fi);
        }
        _fi.addFeature(content);
        return _fi;
    }
}
public class InputRange{
    private double lowerLimit;
    private double upperLimit;
    private int returnValue;
    public InputRange(int _returnValue, double _lowerLimit){
        lowerLimit=_lowerLimit;
        upperLimit=double.MaxValue;
        returnValue=_returnValue;
        if(returnValue==-1){
            returnValue=0;
        }
    }
    public InputRange(int _returnValue,double _lowerLimit, double _upperLimit){
        lowerLimit=_lowerLimit;
        upperLimit=_upperLimit;
        returnValue=_returnValue;
        if(returnValue==-1){
            returnValue=0;
        }
    }
    public int IsInRange(double _value){
        if(_value>lowerLimit){
            if(_value<lowerLimit){
                return returnValue;
            }
            else{
                return -1;
            }
        }
        else{
            return -1;
        }
    }
}
public class AIFeature{ //Der name ist vlt noch ein wenig unglücklich gewählt
    private int inputResolution;
    private double[][] rewards;
    private AIActionInterface actionIF;
    private string featureName;
    private List<InputRange> steps;
    private double currentRawInput;
    public double CurrentRawInput{get{return currentRawInput;}set{currentRawInput=value;}}
    public int ConvertedInput{get{return inputToStep(currentRawInput);}}
    public string FeatureName{get{return featureName;}}
    public AIFeature(double[][] _rewards, InputRange[] _inputRange, AIActionInterface _actionIF){ 
        inputResolution=_rewards.Length; 
        rewards=_rewards;
        actionIF=_actionIF;
        featureName="Unknown Feature";
        currentRawInput=0;
        steps=new List<InputRange>();
        foreach(InputRange r in _inputRange){
            steps.Add(r);
        }
    }
    public AIFeature(double[][] _rewards, InputRange[] _inputRange, AIActionInterface _actionIF, string _name){ 
        inputResolution=_rewards.Length;   
        rewards=_rewards;
        actionIF=_actionIF;
        featureName=_name;
        currentRawInput=0;
        steps=new List<InputRange>();
        foreach(InputRange r in _inputRange){
            steps.Add(r);
        }
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
    public int inputToStep(double _input){ //vlt zu private ändern
        currentRawInput=_input;
        foreach(InputRange r in steps){
            int step=r.IsInRange(_input);
            if(step>-1){
                return step;
            }
        }
        return 0; //nicht korrekt aber vergibt Fehler
    }

}
public class AIFeatureInterface{ //Sammelt alle nutzbaren Features in einem Objekt und macht sie zugänglich
    private List<AIFeature> features;
    public AIFeatureInterface(){
        features=new List<AIFeature>();
    } 
    public void addFeature(AIFeature _feature){
        features.Add(_feature);
    }
    public int featureCount{get{return features.Count;}}
    public AIFeature getFeatureByName(string _name){
        foreach(AIFeature f in features){
            if(f.FeatureName==_name){
                return f;
            }
        }
        return null;
    }
    public AIFeature this[int index]{
        get{
            return features[index];
        }
    }
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
        rewMatrix=new double[this.RowCount][];
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
    public int ActionCount{get{return actionInterface.ActionCount;}}
    public int FeatureCount{get{return featureBuilder.getAddress().Length;}}
    public int RowCount{get{return featureBuilder.CombinationCount;}}
    public int FieldCount{get{return featureBuilder.CombinationCount*actionInterface.ActionCount;}}
    public double[] getStage(int[] _address){
        int index=indexList.getIndex(_address);
        //Debug.Log("Index: "+index.ToString());
        return rewMatrix[index];
    } 
    public int addressToIndex(int[] _address){
        return indexList.getIndex(_address);
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

