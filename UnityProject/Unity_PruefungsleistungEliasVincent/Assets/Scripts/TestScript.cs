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
            4,//FeatureResolution
            new double[4][]{//W A S D
                new double[]{-100,0,10,0},//0
                new double[]{-80,0,8,0},//1
                new double[]{-40,0,4,0},//2
                new double[]{1,1,1,1}//3
            },
            AInterface //connectedInterface
        );
        AIFeature WallBack=new AIFeature(
            4,//FeatureResolution
            new double[4][]{//W A S D
                new double[]{10,0,-100,0},//0
                new double[]{8,0,-80,0},//1
                new double[]{4,0,-40,0},//2
                new double[]{1,1,1,1}//3
            },
            AInterface //connectedInterface
        );
        AIFeature WallLeft=new AIFeature( //incorrect, just for testing!
            4,//FeatureResolution
            new double[4][]{//W A S D
                new double[]{10,0,-100,0},//0
                new double[]{8,0,-80,0},//1
                new double[]{4,0,-40,0},//2
                new double[]{1,1,1,1}//3
            },
            AInterface //connectedInterface
        );
        AIFeature WallRight=new AIFeature(//incorrect, just for testing!
            4,//FeatureResolution
            new double[4][]{//W A S D
                new double[]{10,0,-100,0},//0
                new double[]{8,0,-80,0},//1
                new double[]{4,0,-40,0},//2
                new double[]{1,1,1,1}//3
            },
            AInterface //connectedInterface
        );

        FeatureBuilder fa0=new FeatureBuilder(WallBack);
        FeatureBuilder fa1=new FeatureBuilder(WallFront,fa0);
        FeatureBuilder fa2=new FeatureBuilder(WallLeft,fa1);
        FeatureBuilder fa3=new FeatureBuilder(WallRight,fa2);

        Debug.Log("Feature Combinations: "+fa3.CombinationCount.ToString());

        fa3.Reset();
        do{
            Debug.Log("Reward for W @ "+addressToStringRev(fa3.getAddress())+" = "+fa3.getReward(fa3.getAddress(),AInterface.GetAIAction("W")).ToString());
        }while(!fa3.Next());

        RewardMatrix rewMatrix=new RewardMatrix(fa3);
        rewMatrix.generateMatrix();
        int[] ms=rewMatrix.getMatrixSize();
        Debug.Log("actions: "+ms[0].ToString()+" features: "+ms[1].ToString()+" featureCombinations: "+ms[2].ToString()+" FieldsTotal: "+ms[3].ToString());
        
        double[] stage=rewMatrix.getStage(new int[]{3,3,3,3});
        Debug.Log("reward @3333: W: "+stage[0].ToString()+" A: "+stage[1].ToString()+" S:"+stage[2].ToString()+" D:"+stage[3].ToString()+" ");


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
