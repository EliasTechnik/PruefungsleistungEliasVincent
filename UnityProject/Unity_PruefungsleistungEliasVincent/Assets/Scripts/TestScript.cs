using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static FeatureBuilderTools;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Action Interface erstellen
        AIActionInterface AInterface=new AIActionInterface();
        AInterface.addAction("W");
        AInterface.addAction("A");
        AInterface.addAction("S");
        AInterface.addAction("D");

        //W
        AIFeature WallFront=new AIFeature(
            new double[4][]{//W A S D
                new double[]{-100,0,10,0},//0
                new double[]{-80,0,8,0},//1
                new double[]{-40,0,4,0},//2
                new double[]{1,1,1,1}//3
            },
            new InputRange[]{
                new InputRange(0,50),
                new InputRange(1,40,50),
                new InputRange(2,30,40),
                new InputRange(3,0,30),
            },
            AInterface, //connectedInterface
            "WallFront"
        );
        AIFeature WallBack=new AIFeature(
            new double[4][]{//W A S D
                new double[]{10,0,-100,0},//0
                new double[]{8,0,-80,0},//1
                new double[]{4,0,-40,0},//2
                new double[]{1,1,1,1}//3
            },
            new InputRange[]{
                new InputRange(0,50),
                new InputRange(1,40,50),
                new InputRange(2,30,40),
                new InputRange(3,0,30),
            },
            AInterface, //connectedInterface
            "WallBack"
        );
        AIFeature WallLeft=new AIFeature( //incorrect, just for testing!
            new double[4][]{//W A S D
                new double[]{10,0,-100,0},//0
                new double[]{8,0,-80,0},//1
                new double[]{4,0,-40,0},//2
                new double[]{1,1,1,1}//3
            },
            new InputRange[]{
                new InputRange(0,50),
                new InputRange(1,40,50),
                new InputRange(2,30,40),
                new InputRange(3,0,30),
            },
            AInterface, //connectedInterface
            "WallLeft"
        );
        AIFeature WallRight=new AIFeature(//incorrect, just for testing!
            new double[4][]{//W A S D
                new double[]{10,0,-100,0},//0
                new double[]{8,0,-80,0},//1
                new double[]{4,0,-40,0},//2
                new double[]{1,1,1,1}//3
            },
            new InputRange[]{
                new InputRange(0,50),
                new InputRange(1,40,50),
                new InputRange(2,30,40),
                new InputRange(3,0,30),
            },
            AInterface, //connectedInterface
            "WallRight"
        );

        FeatureBuilder fa0=new FeatureBuilder(WallFront);
        FeatureBuilder fa1=new FeatureBuilder(WallBack,fa0);
        FeatureBuilder fa2=new FeatureBuilder(WallLeft,fa1);
        //FeatureBuilder fa3=new FeatureBuilder(WallRight,fa2);

        FeatureBuilder fa3=new FeatureBuilder(WallRight,new FeatureBuilder(WallLeft,new FeatureBuilder(WallBack,new FeatureBuilder(WallFront))));

        Debug.Log("Feature Combinations: "+fa3.CombinationCount.ToString());

        fa3.Reset();
        do{
            Debug.Log("Reward for W @ "+addressToStringRev(fa3.getAddress())+" = "+fa3.getReward(fa3.getAddress(),AInterface.GetAIAction("W")).ToString());
        }while(!fa3.Next());

        RewardMatrix rewMatrix=new RewardMatrix(fa3);
        rewMatrix.generateMatrix();
        Debug.Log("actions: "+rewMatrix.ActionCount.ToString()+" features: "+rewMatrix.FeatureCount.ToString()+" featureCombinations: "+rewMatrix.RowCount.ToString()+" FieldsTotal: "+rewMatrix.FieldCount.ToString());
        
        double[] stage=rewMatrix.getStage(new int[]{1,0,2,3});
        Debug.Log("reward @1023: W: "+stage[0].ToString()+" A: "+stage[1].ToString()+" S:"+stage[2].ToString()+" D:"+stage[3].ToString()+" ");

        AIFeatureInterface fi=new AIFeatureInterface();
        fi=fa3.getFeatureInterface(fi);

        for(int i=0;i<fi.featureCount;i++){
            Debug.Log("Feature "+i.ToString()+": "+fi[i].FeatureName);
        }
        for(int i=0;i<AInterface.ActionCount;i++){
            Debug.Log("Index: "+i.ToString()+" Action: "+AInterface[i].ActionID.ToString()+":"+AInterface[i].ActionName);
        }

        QAgent ai=new QAgent(rewMatrix,AInterface);

        fi[0].CurrentRawInput=35;
        fi[1].CurrentRawInput=30;
        fi[2].CurrentRawInput=1000;
        fi[3].CurrentRawInput=80;

        AIAction prediction=ai.Predict(fi);

        Debug.Log("Predicted: "+prediction.ActionName);

        ai.Reward(fi);

        ai.setParams(0.5,0.5,1);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
