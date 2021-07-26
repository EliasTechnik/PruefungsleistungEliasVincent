using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance {get; private set;}

    private PlayerObject Player;
    private bool inputai=false;
    [SerializeField] private TargetObject Target;
    [SerializeField] private GameObject Arrow;
    [SerializeField] private ObstacleGenerator obstacleGenerator;

    public bool respawnbool;

    //KI Start
    AIActionInterface actionInterface;
    AIFeature Wall_0;
    AIFeature Wall_45;
    AIFeature Wall_90;
    AIFeature Wall_135;
    AIFeature Wall_180;
    AIFeature Wall_225;
    AIFeature Wall_270;
    AIFeature Wall_315;
    AIFeature TargetDistance;
    FeatureBuilder fb;
    RewardMatrix rewMatrix;
    QAgent agent;
    AIFeatureInterface ai_input;

    //KI End

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        //KI instanziieren
        actionInterface=new AIActionInterface();
        actionInterface.addAction("W"); //ID=0
        actionInterface.addAction("A"); //ID=1
        actionInterface.addAction("S"); //ID=2
        actionInterface.addAction("D"); //ID=3

        InputRange[] IRdistance=new InputRange[]{
                new InputRange(0,3,true), //Weit weg
                new InputRange(1,2,3),    //näher dran  
                new InputRange(2,0,1)     //nah dran  
            };

        Wall_0=new AIFeature(//frontal
            new double[3][]{//W A S D
                new double[]{1,1,1,1},
                new double[]{-2,1,1,1},
                new double[]{-10,1,1,1},
            },
            IRdistance,
            actionInterface,
            "Wall_0"
        );

        Wall_45=new AIFeature(//frontal links
            new double[3][]{//W A S D
                new double[]{1,1,1,1},
                new double[]{-1,-1,1,1},
                new double[]{-5,-5,1,1},
            },
            IRdistance,
            actionInterface,
            "Wall_45"
        );

        Wall_90=new AIFeature(//links
            new double[3][]{//W A S D
                new double[]{1,1,1,1},
                new double[]{1,-2,1,1},
                new double[]{1,-10,1,1},
            },
            IRdistance,
            actionInterface,
            "Wall_90"
        );

        Wall_135=new AIFeature(//hinten links
            new double[3][]{//W A S D
                new double[]{1,1,1,1},
                new double[]{1,-1,-1,1},
                new double[]{1,-5,-5,1},
            },
            IRdistance,
            actionInterface,
            "Wall_135"
        );

        Wall_180=new AIFeature(//hinten
            new double[3][]{//W A S D
                new double[]{1,1,1,1},
                new double[]{1,1,-2,1},
                new double[]{1,1,-10,1},
            },
            IRdistance,
            actionInterface,
            "Wall_180"
        );
        Wall_225=new AIFeature(//hinten rechts
            new double[3][]{//W A S D
                new double[]{1,1,1,1},
                new double[]{1,1,-1,-1},
                new double[]{1,1,-5,-5},
            },
            IRdistance,
            actionInterface,
            "Wall_225"
        );
        Wall_270=new AIFeature(//rechts
            new double[3][]{//W A S D
                new double[]{1,1,1,1},
                new double[]{1,1,1,-2},
                new double[]{1,1,1,-10},
            },
            IRdistance,
            actionInterface,
            "Wall_270"
        );
        Wall_315=new AIFeature(//frontal rechts
            new double[3][]{//W A S D
                new double[]{1,1,1,1},
                new double[]{-1,1,1,-1},
                new double[]{-5,1,1,-5},
            },
            IRdistance,
            actionInterface,
            "Wall_315"
        );

        fb=new FeatureBuilder(
            Wall_315,
            new FeatureBuilder(
                Wall_270,
                new FeatureBuilder(
                    Wall_225,
                    new FeatureBuilder(
                        Wall_180,
                        new FeatureBuilder(
                            Wall_135,
                            new FeatureBuilder(
                                Wall_90,
                                new FeatureBuilder(
                                    Wall_45,
                                    new FeatureBuilder(
                                        Wall_0
                                    )
                                )
                            )
                        )
                    )
                )
            )
        );
        Debug.Log("Feature Combinations: "+fb.CombinationCount.ToString());

        rewMatrix=new RewardMatrix(fb);
        rewMatrix.generateMatrix();

        ai_input=new AIFeatureInterface();
        ai_input=fb.getFeatureInterface(ai_input);

        agent=new QAgent(rewMatrix, actionInterface);
        
        //KI Ende

        Player = new PlayerObject(GameObject.Find("player_obj"));
        Player.RespawnPlayer();
        obstacleGenerator.HandleObstacles();
    }

    private void Update() {

        if (inputai) {
            //switch(id) {}
        } else {
            if (Input.GetKey(KeyCode.W)) {
            Player.HandleMovement(KeyCode.W);
            }
            if (Input.GetKey(KeyCode.S)) {
            Player.HandleMovement(KeyCode.S);
            }
            if (Input.GetKey(KeyCode.A)) {
            Player.HandleMovement(KeyCode.A);
            }
            if (Input.GetKey(KeyCode.D)) {
            Player.HandleMovement(KeyCode.D);
            }
        }
        

        if (respawnbool) {
            //obstacleGenerator.HandleObstacles();
            //Player.RespawnPlayer();
            Player.shouldRespawn=false;
            respawnbool = false;
        }
        //Player.UpdateMove();
        UpdateArrows();
    }

        private void UpdateArrows(){
        Vector3 tp=Target.transform.position;
        tp.y+=0.6f;
        Vector3 direction=(tp-Arrow.transform.position).normalized;
        Quaternion lookRotation=Quaternion.LookRotation(-direction);
        Arrow.transform.rotation=Quaternion.Slerp(Arrow.transform.rotation,lookRotation,1);
    }

    public void toggleAI() {
        switch(inputai) {
            case true:
            inputai = false;
            break;
            case false:
            inputai = true;
            break;
        }
    }
}
