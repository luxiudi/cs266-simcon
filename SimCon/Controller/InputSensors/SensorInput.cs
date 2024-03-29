﻿using System;
using System.Collections.Generic;

using System.Text;

namespace CS266.SimCon.Controller.InputSensors
{
    //InputSensor is the robot's sensor
    //InputSensor is the base class for the various types of Sensors. 
    //ie LightSensor is a child of InputSensor, that will contain a function that returns the amount of light a 
    //
    //
    //

    public abstract class InputSensor
    {
        public Robot robot;
        public ControllerWorldState worldState;
        //Replace the worldState with the new worldstate passed in from the parameter;
        public void UpdateWorldState(ControllerWorldState ws){
            worldState = ws;
        }
   
        public abstract void UpdateSensor();
    }
}
