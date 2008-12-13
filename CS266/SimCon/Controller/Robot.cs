﻿using System;
using System.Collections.Generic;

using System.Text;

namespace CS266.SimCon.Controller
{
    public class Robot : PhysObject
    {
//<<<<<<< .mine
//        public Robot(int id, ObjectType type, Coordinates location, float orientation, float width, float height) :
//            base(id, type, location, orientation, width, height)
//=======
        public Robot(int id, Coordinates location, float orientation, float width, float height) :
            base(id, ObjectType.Robot, location, orientation, width, height)

        {
            Stop();
        }

        bool CanMoveForward = true;
        bool CanMoveBackward = true;
      
      
        public PhysicalRobotAction CurrentAction;
       
        public Dictionary<String, SensorInput> Sensors;

        // Needed
        // A way to determine vision cone, which objects are in it

        public Algorithm CurrentAlgorithm;

        // TODO Figure out units

      

        public void MoveForward(float distance)
        {
            if (CanMoveForward)
            {
                ControlLoop.ActionQueue.Enqueue(new PhysicalRobotAction(this.Id, PhysicalActionType.MoveForward, distance));
            }
        }

        public void MoveBackward(float distance)
        {
            if (CanMoveBackward)
            {
                ControlLoop.ActionQueue.Enqueue(new PhysicalRobotAction(this.Id, PhysicalActionType.MoveBackward, distance));
            }
        }

        public void Stop()
        {
            ControlLoop.ActionQueue.Enqueue(new PhysicalRobotAction(this.Id, PhysicalActionType.Stop));
        }

        //public void ChangeSpeed(float speed)
        //{
        //    ControlLoop.ActionQueue.Enqueue(new PhysicalRobotAction(this.Id, PhysicalActionType.SetSpeed, speed));
        //}

        public void Turn(float degrees)
        {
            ControlLoop.ActionQueue.Enqueue(new PhysicalRobotAction(this.Id, PhysicalActionType.Turn, degrees));
        }


         public void createNewRobot(int IdOfNewRobot)
        {
            //what is the value we should pass here? Maybe the door position?
            float value = 90;
            ControlLoop.ActionQueue.Enqueue(new PhysicalRobotAction(IdOfNewRobot, PhysicalActionType.CreateRobot, value ));

        }

  


    }
}
