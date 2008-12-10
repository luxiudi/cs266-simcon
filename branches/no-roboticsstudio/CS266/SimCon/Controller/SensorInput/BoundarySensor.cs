using System;
using System.Collections.Generic;

using System.Text;

namespace CS266.SimCon.Controller
{
    // This sensor is used to check whether an object is close to a boundary 
    public class BoundarySensor : SensorInput
    {
        public double objectSensingAngle = 15; // angle at which it can sense obstacle objects
                                                // assumed symmetric for + or - direction
        public double objectSensingDist = 80; // distance at which obstacles can be sensed
        
        public bool detectObject;
        

        public BoundarySensor() { }

        public BoundarySensor(double angle, double distance) {
            this.objectSensingAngle = angle;
            this.objectSensingDist = distance;
        }

        public BoundarySensor(ControllerWorldState worldState)
        {
            this.worldState = worldState;          
        }
  
        public void SetSensingAngle(double angle){
            objectSensingAngle = angle;
        }

        public void SetSensingDistance(double distance)
        {
            objectSensingDist = distance;
        }

        // checks if point is in the boundaries of the space
        public bool isInside(double px, double py, double minx, double miny, double maxx, double maxy)
        {
            return (px < maxx && px > minx && py < maxy && py > miny);
        }

        // Given the boundaries of the world, determine whether robot senses a boundary
        public bool senseObject(Robot robot, double minx, double miny, double maxx, double maxy){
                
                Coordinates robotLocation = robot.Location;
                    
                double angle = robot.Orientation;
                if(angle < 0){
                    // convert to [0, 360]
                    angle += 360;
                }
                // Convert to radians
                angle = angle * Math.PI / 180;

                Coordinates endLocation = new Coordinates((float)(robotLocation.X + objectSensingDist * Math.Cos(angle)), 
                                                           (float)(robotLocation.Y + objectSensingDist * Math.Sin(angle)));

                if (isInside(endLocation.X, endLocation.Y, minx, miny, maxx, maxy))
                {
                    // doesn't sense boundary; no collision
                    return false;
                }
                else
                {
                    return true;
                }
            }
            
            // Update the proximity sensor data fields
            // 1. For all objects in the world  (robot or wall or food or whatever)
            // 2. Check if robot senses any object
         
            public override void UpdateSensor()
            {
                detectObject = false;    
                bool detect = false;

                    /// TODO: get information from the world state
                    /// 
                
            }
        }
    }
