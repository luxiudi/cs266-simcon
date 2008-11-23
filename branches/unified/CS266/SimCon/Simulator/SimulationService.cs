//-----------------------------------------------------------------------
//
//  $File: SimulationService.cs $ $Revision: 1 $
//-----------------------------------------------------------------------
//#define IMMOVABLE 100000000000

#region CODECLIP 01-1
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System.Net;
using System.IO;

using CS266.SimCon;
using CS266.SimCon.RoboticsClasses;

#region Simulation namespaces
using Microsoft.Robotics.Simulation;
using Microsoft.Robotics.Simulation.Engine;
using engineproxy = Microsoft.Robotics.Simulation.Engine.Proxy;
using Microsoft.Robotics.Simulation.Physics;
using Microsoft.Robotics.PhysicalModel;
using System.ComponentModel;
#endregion
#endregion



namespace CS266.SimCon.Simulator
{
    #region CODECLIP 02-1
    [DisplayName("Simulation Tutorial 1")]
    [Description("Simulation Tutorial 1 Service")]
    [Contract(Contract.Identifier)]

    public class SimulationTutorial1 : DsspServiceBase
    {
        [Partner("Engine",
            Contract = engineproxy.Contract.Identifier,
            CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        private engineproxy.SimulationEnginePort _engineStub =
            new engineproxy.SimulationEnginePort();

        // Main service port
        [ServicePort("/SimulationTutorial1", AllowMultipleInstances = false)]
        private SimulationTutorial1Operations _mainPort =
            new SimulationTutorial1Operations();

        public SimulationTutorial1(DsspServiceCreationPort creationPort) :
            base(creationPort)
        {

        }
    #endregion

        #region CODECLIP 03-1
        protected override void Start()
        {

            base.Start();

            WorldDimensions WD = new WorldDimensions(14, 14);

            ObjectState Barrier = new ObjectState("Obstacle1", "obstacle", new float[3] { 2f, -WD.zdim / 2f, .5f }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { .5f, -6f, .8f });
            //To make a diagonla object leave the y field as 0 otherwise use .5f as a dummy
            ObjectState Barrier2 = new ObjectState("Obstacle2", "obstacle", new float[3] { 4f, -((WD.zdim / 2f) + 4f), 0 }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 3.5f, -.5f, .8f });
            ObjectState Barrier3 = new ObjectState("Obstacle3", "obstacle", new float[3] { 11f, -((WD.zdim / 2f) - 4f), .5f }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 3f, -.5f, .8f });

            ObjectState Food1 = new ObjectState("FoodUnit1", "food", new float[3] { 10f, -10f, 0 }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 0 });
            ObjectState Food2 = new ObjectState("FoodUnit2", "food", new float[3] { 10f, -10.2f, 0 }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 0 });
            ObjectState Food3 = new ObjectState("FoodUnit3", "food", new float[3] { 10.2f, -10f, 0 }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 0 });
            ObjectState Food4 = new ObjectState("FoodUnit4", "food", new float[3] { 9.8f, -10f, 0 }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 0 });
            ObjectState Food5 = new ObjectState("FoodUnit5", "food", new float[3] { 10f, -9.8f, 0 }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 0 });
            ObjectState Food6 = new ObjectState("FoodUnit6", "food", new float[3] { 10f, -10f, .2f }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 0 });
            ObjectState Food7 = new ObjectState("FoodUnit7", "food", new float[3] { 9.8f, -9.8f, 0 }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 0 });

            ObjectState Robot1 = new ObjectState("A", "robot", new float[3] { (WD.xdim / 2f), -WD.zdim / 2f, 0 }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 0 });
            ObjectState Robot2 = new ObjectState("B", "robot", new float[3] { (WD.xdim / 2f) + 1, -WD.zdim / 2f, 0 }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 0 });
            ObjectState Robot3 = new ObjectState("C", "robot", new float[3] { (WD.xdim / 2f) + 2, -WD.zdim / 2f, 0 }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 0 });
            ObjectState Robot4 = new ObjectState("D", "robot", new float[3] { (WD.xdim / 2f) + 3, -WD.zdim / 2f, 0 }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 0 });
            ObjectState Robot5 = new ObjectState("E", "robot", new float[3] { (WD.xdim / 2f) + 4, -WD.zdim / 2f, 0 }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 0 });

            List<ObjectState> o = new List<ObjectState>();

            o.Add(Barrier);
            o.Add(Barrier2);
            o.Add(Barrier3);
            o.Add(Food1);
            o.Add(Food2);
            o.Add(Food3);
            o.Add(Food4);
            o.Add(Food5);
            o.Add(Food6);
            o.Add(Food7);
            o.Add(Robot1);
            o.Add(Robot2);
            o.Add(Robot3);
            o.Add(Robot4);
            o.Add(Robot5);

            WorldState World = new WorldState(o);
            OurSimulator os = new OurSimulator();

            int[] robots = new int[2];
            int[] barriers = new int[2];
            int[] food = new int[2];
            float[] dim = new float[2];

            //Before GenAlgorithm was part of the simulator
            //GenAlgorithm Algo;


            //May need to change file path when we enter code into repository
            TextReader tr = new StreamReader("samples/SimulationTutorials/Tutorial1/Game Parameters.txt");

            //Number of games
            string RawN = tr.ReadLine();
            int game = Int32.Parse(RawN);
            os.BatchMode = game;
            os.tr = tr;


            //Reads a line in the text file and parses the parameters
            string RawParameters = tr.ReadLine();
            string[] DelimitedParameters = RawParameters.Split(',');

            os.Algo = DetermineAlgo(DelimitedParameters);

            Console.WriteLine(game);

            WorldPair MyWorld = RandomBoard(os.Algo.robots, os.Algo.barriers, os.Algo.food, os.Algo.dim);
            Thread.Sleep(3000);
            Console.WriteLine("Not Sleeping anymore");

            os.SetWorldState(MyWorld.WS);

            os.PopulateWorld(MyWorld.WD, MyWorld.WS, os.Algo);

            ControlLoop(os);
            return;
        }
        #endregion
        #region FAKE CONTROL LOOP
        void ControlLoop(OurSimulator os)
        {
            OurSimulator a;

            while (true)
            {
                if (os.GetWorldState() == null)
                {
                    a = os.Finished();
                    if (a == null)
                        break;
                }

                os.ExecuteActions(getActions(os.GetWorldState()));
            }

            return;
        }
        #endregion

        #region DETERMINE ALGO
        GenAlgorithm DetermineAlgo(string[] DelimitedParameters)
        {
            //Place this separate if-else in another function!
            if (DelimitedParameters[0] == "GenAlgorithm")
            {
                return new GenAlgorithm(
                    new float[] { (float)System.Convert.ToSingle(DelimitedParameters[1]), (float)System.Convert.ToSingle(DelimitedParameters[2]) },
                    new int[] { Int32.Parse(DelimitedParameters[3]), Int32.Parse(DelimitedParameters[4]) },
                    new int[] { Int32.Parse(DelimitedParameters[5]), Int32.Parse(DelimitedParameters[6]) },
                    new int[] { Int32.Parse(DelimitedParameters[7]), Int32.Parse(DelimitedParameters[8]) });

            }
            else if (DelimitedParameters[0] == "DFS")
            {
                return new GenAlgorithm(
                    new float[] { (float)System.Convert.ToSingle(DelimitedParameters[1]), (float)System.Convert.ToSingle(DelimitedParameters[2]) },
                    new int[] { Int32.Parse(DelimitedParameters[3]), Int32.Parse(DelimitedParameters[4]) },
                    new int[] { Int32.Parse(DelimitedParameters[5]), Int32.Parse(DelimitedParameters[6]) },
                    new int[] { Int32.Parse(DelimitedParameters[7]), Int32.Parse(DelimitedParameters[8]) },
                    (float)System.Convert.ToSingle(DelimitedParameters[9]));
            }
            else
            {
                return new GenAlgorithm(
                    new float[] { (float)System.Convert.ToSingle(DelimitedParameters[0]), (float)System.Convert.ToSingle(DelimitedParameters[1]) },
                    new int[] { Int32.Parse(DelimitedParameters[2]), Int32.Parse(DelimitedParameters[3]) },
                    new int[] { Int32.Parse(DelimitedParameters[4]), Int32.Parse(DelimitedParameters[5]) },
                    new int[] { Int32.Parse(DelimitedParameters[6]), Int32.Parse(DelimitedParameters[7]) });
            }
        }
        #endregion
        #region DELETE ENTITIES
        //NEW!!!!!!!
        void DeleteEntities(OurSimulator os)
        {
            //for (int i = 0; i < W.objects.Count; i++)
            //Timing is off!!!!
            for (int i = 0; i < os.BoundaryList.Count; i++)
            {
                SimulationEngine.GlobalInstancePort.Delete(os.BoundaryList[i]);
            }

            for (int i = 0; i < os.RobotList.Count; i++)
            {
                SimulationEngine.GlobalInstancePort.Delete(os.RobotList[i]);
            }

            for (int i = 0; i < os.FoodList.Count; i++)
            {
                SimulationEngine.GlobalInstancePort.Delete(os.FoodList[i]);
            }

            SimulationEngine.GlobalInstancePort.Delete(os.Ground);

            return;
        }
        #endregion
        #region RANDOM BOARD
        WorldPair RandomBoard(int[] robots, int[] barriers, int[] food, float[] dim)
        {
            Random rng = new Random();
            WorldPair MyWorld;
            List<ObjectState> o = new List<ObjectState>();

            //Make return a random float
            float xdim = dim[0];
            float ydim = dim[1];


            int total_objects = (int)(xdim - 2) * (int)(ydim - 2);
            //Make random number bounded by total_objects
            int num_robots = rng.Next(robots[0], robots[1]);

            for (int i = 0; i < num_robots; i++)
            {
                int position_x = rng.Next(1, (int)(xdim - 1));
                int position_y = rng.Next(1, (int)(ydim - 1));
                string name = "robot" + i.ToString();
                //Make have random location
                o.Add(new ObjectState(name, "robot", new float[3] { (float)position_x, (float)-position_y, 0 }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 0 }));
            }

            total_objects -= num_robots;
            //Make random number bounded by total_objects
            int num_obstacles = rng.Next(barriers[0], barriers[1]);

            for (int j = 0; j < num_obstacles; j++)
            {
                string name = "Obstacle" + j.ToString();


                //Make Random or standard?
                //Both vols were 2
                float x_vol = rng.Next(1, (int)(xdim - 1));
                float y_vol = rng.Next(1, (int)(ydim - 1));
                int position_x = rng.Next(1, (int)(xdim - 1));
                int position_y = rng.Next(1, (int)(ydim - 1));

                //Consider making z-oriented obstacles
                if (rng.NextDouble() < .5) //probability .5
                {
                    //Limits the width/length of the barrier so we do not collide with the boundaries
                    while ((x_vol / 2) + (float)position_x >= xdim || (float)position_x - (x_vol / 2) <= 0)
                    {
                        x_vol = rng.Next(1, (int)(xdim - 1));
                    }
                    //objects oriented along the x axis
                    o.Add(new ObjectState(name, "obstacle", new float[3] { (float)position_x, (float)-position_y, .5f }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { x_vol, -.5f, .8f }));
                }
                else
                {
                    //Limits the width/length of the barrier so so we don't collide with the boundaries
                    while ((y_vol / 2) + (float)position_y >= ydim || (float)position_y - (y_vol / 2) <= 0)
                    {
                        y_vol = rng.Next(1, (int)(ydim - 1));
                    }
                    //objects oriented along the y axis
                    o.Add(new ObjectState(name, "obstacle", new float[3] { (float)position_x, (float)-position_y, .5f }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { .5f, -y_vol, .8f }));
                }
            }

            total_objects -= num_obstacles;
            //Make random number bounded by total objects
            int num_food = rng.Next(food[0], food[1]);
            for (int k = 0; k < num_food; k++)
            {
                string name = "FoodUnit" + k.ToString();
                int position_x = rng.Next(1, (int)(xdim - 1));
                int position_y = rng.Next(1, (int)(ydim - 1));
                //make have random location
                o.Add(new ObjectState(name, "food", new float[3] { (float)position_x, (float)-position_y, 0 }, new float[3] { 1, 0, 0 }, new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 0 }));
            }

            MyWorld = new WorldPair(new WorldDimensions(xdim, ydim), new WorldState(o));
            return MyWorld;

        }
        #endregion


        public RobotActions getActions(WorldState W)
        {
            Random rng = new Random();
            //NEW!!!!!!!
            if (rng.NextDouble() < .5)
                return null;

            RobotActions Actions = new RobotActions();
            for (int i = 0; i < W.objects.Count; i++)
            {
                if (W.objects[i].type == "robot")
                {
                    Actions.Add(new RobotAction(W.objects[i].name, (float)rng.Next(-180, 180), (float)rng.Next(0, 10)));
                }

            }
            //PHASE 1
            //RobotActions Actions = new RobotActions(new RobotAction[5] {
            //    new RobotAction("A", 90f, 9), 
            //    new RobotAction("B", 45f, 5),
            //    new RobotAction("C", 0f, 9), 
            //    new RobotAction("D", 0f, 9),
            //    new RobotAction("E", -180f, 8)});
            return Actions;
        }
    }

    public static class Contract
    {
        public const string Identifier = "http://schemas.tempuri.org/2006/06/simulationtutorial1.html";
    }

    [ServicePort]
    public class SimulationTutorial1Operations : PortSet<DsspDefaultLookup, DsspDefaultDrop>
    {
    }
}