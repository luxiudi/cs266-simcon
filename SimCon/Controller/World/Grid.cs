﻿using System;
using System.Collections.Generic;
using System.Text;
using CS266.SimCon.Controller;

namespace CS266.SimCon.Controller
{
    public class Grid
    {
        // World state to figure out locations of current robots
        public ControllerWorldState ws;

        // Actual grid
        public GridData[,] gridData;

        // Dimensions of the world
        public double WorldWidth;
        public double WorldHeight;

        // public double minX;
        // public double minY;

        // Dimensions of the grid
        public int NumSquaresX;
        public int NumSquaresY;

        // Previous location of robot. Only exists if robot has continuous markings on
        public Dictionary<Robot, Coordinates> prevLocations;
        public Dictionary<Robot, Coordinates> prevLocationsForMark;

        // constructor gets a reference to the world state
        public Grid(ControllerWorldState ws, double WorldWidth, double WorldHeight, int numX, int numY)
        {
            this.prevLocations = new Dictionary<Robot, Coordinates>();
            this.prevLocationsForMark = new Dictionary<Robot, Coordinates>();
            this.ws = ws;
            this.WorldWidth = WorldWidth;
            this.WorldHeight = WorldHeight;
            this.NumSquaresX = numX;
            this.NumSquaresY = numY;

            gridData = new GridData[NumSquaresX, NumSquaresY];
            for (int i = 0; i < NumSquaresX; i++)
            {
                for (int j = 0; j < NumSquaresY; j++)
                {
                    gridData[i, j] = new GridData(i,j);
                }
            }
        }

        public void Mark(Robot robot, bool continuous){

            
            if(!continuous || !prevLocationsForMark.ContainsKey(robot)){
                  getGridLoc(robot.Location).pheromoneLevel++;
                  return;
            }
            Coordinates newLoc = new Coordinates(robot.Location.X, robot.Location.Y);
            Coordinates prevLoc = new Coordinates(prevLocationsForMark[robot].X, prevLocations[robot].Y);
            Coordinates curLoc = new Coordinates(prevLocationsForMark[robot].X, prevLocations[robot].Y);

            // Mark continuously with interpolation
            // Assume previous location is already marked
            //Console.WriteLine("In Mark:: new location is for " + newLoc.X + ", " + newLoc.Y);
            GridData finalSpot = getGridLoc(newLoc);
            //Console.WriteLine("In Mark:: destination grid coordinates is " + finalSpot.row + ", " + finalSpot.col);

            //Console.WriteLine("In Mark: Getting grid loc current spot for " + curLoc.X + "," + curLoc.Y);
            GridData curSpot = getGridLoc(curLoc);
            //Console.WriteLine("In Mark:: old grid coordinates is " + finalSpot.row + ", " + finalSpot.col);

            if (finalSpot.row == curSpot.row && finalSpot.col == curSpot.col)
            {
                // don't mark anything
                return;
            }
            double incr = (double)Math.Min((WorldHeight / NumSquaresY), (WorldWidth / NumSquaresX)) / 2;
            double dX = robot.Location.X - prevLocationsForMark[robot].X;
            double dY = robot.Location.Y - prevLocationsForMark[robot].Y;

            double incrX;
            double incrY;

            if (dX != 0 || dY != 0)
            {
                incrX = incr * dX / (double)Math.Sqrt(dX * dX + dY * dY);
                incrY = incr * dY / (double)Math.Sqrt(dX * dX + dY * dY);
            }
            else
            {
                // 0 distance
                incrX = 0;
                incrY = 0;
            }
            Console.WriteLine("Incrx is " + incrX + " and incrY is " + incrY);
                do
                {
                    GridData nextSpot;

                    //Console.WriteLine("In Mark: Before incrementing, we were at location " + curLoc.X + ", " + curLoc.Y + " trying to reach " + newLoc.X + ", " + newLoc.Y);
                    //Console.WriteLine("In Mark: We are at grid location " + curSpot.row + ", " + curSpot.col + " trying to reach " + finalSpot.row + ", " + finalSpot.col);
                    // Increment X and Y
                    curLoc.X += incrX;
                    curLoc.Y += incrY;

                    //Console.WriteLine("In Mark: After incrementing, we are at location " + curLoc.X + ", " + curLoc.Y + " trying to reach " + newLoc.X + ", " + newLoc.Y);

                    // Get next grid spot
                    
                    nextSpot = getGridLoc(curLoc);
                    //Console.WriteLine("In Mark: current grid loc is now " + nextSpot.row + "," + nextSpot.col);
                    // If different mark it
                    if (nextSpot != curSpot)
                    {
                        nextSpot.pheromoneLevel++;
                        curSpot = nextSpot;
                    }
                } while (curSpot.row != finalSpot.row || curSpot.col != finalSpot.col);

                
                return;
        }

        public void PrintGrid()
        {
            for (int i = 0; i < NumSquaresY; i++)
            {
                for (int j = 0; j < NumSquaresX; j++)
                {
                    Console.Write(gridData[i, j].numTimesVisited +" ");
                }
                Console.WriteLine();
            }


        }
        public void GridUpdate(Robot robot)
        {

            Console.WriteLine(robot.Id);
            

            // Take out robot from locations of where robot is in the grid
            GridData location = findObj(robot);
            if (location != null)
            {
                location.objectsInSquare.Remove(robot);
            }

            // Add robot
            getGridLoc(robot.Location).objectsInSquare.Add(robot);

            Coordinates newLoc = new Coordinates(robot.Location.X, robot.Location.Y);
            
            GridData finalSpot = getGridLoc(newLoc);
            Console.WriteLine("Robot location -- " + newLoc.X + "," + newLoc.Y);
            Console.WriteLine("Grid location -- " + finalSpot.row + ", " + finalSpot.col);

            // Update prevLocationsForMark so that they're synchronized. This assume that Mark is called AFTER GridUpdate
            foreach (Robot r in prevLocations.Keys)
            {
                prevLocationsForMark[r] = new Coordinates(prevLocations[r].X, prevLocations[r].Y);
            }

            if(!prevLocations.ContainsKey(robot)){    
                prevLocations[robot] = newLoc;
                finalSpot.numTimesVisited++;    
                 return;
            }



            Coordinates prevLoc = new Coordinates(prevLocations[robot].X, prevLocations[robot].Y);
            Coordinates curLoc = new Coordinates(prevLocations[robot].X, prevLocations[robot].Y);
            //Console.WriteLine("Getting grid loc for " + curLoc.X + "," + curLoc.Y);
            GridData curSpot = getGridLoc(curLoc);

            if(finalSpot.row == curSpot.row && finalSpot.col == curSpot.col){
                // don't mark anything
                return;
            }

             double incr = (double)Math.Min((WorldHeight / NumSquaresY), (WorldWidth / NumSquaresX)) / 2;
                double dX = robot.Location.X - prevLocations[robot].X;
                double dY = robot.Location.Y - prevLocations[robot].Y;
                double incrX = incr * dX / (double)Math.Sqrt(dX * dX + dY * dY);
                double incrY = incr * dY / (double)Math.Sqrt(dX * dX + dY * dY);


                //Console.WriteLine("Prev location is " + curLoc.X + ", " + curLoc.Y + " trying to reach " + newLoc.X + ", " + newLoc.Y);
                do
                {
                    GridData nextSpot;
                    // Increment X and Y
                    curLoc.X += incrX;
                    curLoc.Y += incrY;

                    //Console.WriteLine("After incrementing, getting grid loc for " + curLoc.X + "," + curLoc.Y);
                    // Get next grid spot
                    nextSpot = getGridLoc(curLoc);

                    // If different mark it
                    if (nextSpot != curSpot)
                    {
                        nextSpot.numTimesVisited++;
                        curSpot = nextSpot;
                    }
                    //} while (curSpot != finalSpot);
                } while (curSpot.row != finalSpot.row || curSpot.col != finalSpot.col);
                prevLocations[robot] = curLoc;
                return;
        }
        
        // Turn on continuous marking
        public void TurnOnContinuousMarking(Robot robot)
        {
            // Make a copy of the location
            Coordinates locCopy = new Coordinates(robot.Location.X, robot.Location.Y);

            // Get location of robot and store in prevLocations
            prevLocations.Add(robot, locCopy);
        }

        // Turn off continuous marking, take robot off of prevLocations list
        public void TurnOffContinuousMarking(Robot robot)
        {
            prevLocations.Remove(robot);
        }


        /* Get the location on the grid (actually the GridData object at that
         * location, which contains all the information)
         */ 
        public GridData getGridLoc(Coordinates location)
        {

            int gridX;
            int gridY;
            // Get grid coordinates. Round down
            if (location.X >= WorldWidth)
                gridX = NumSquaresX - 1;
            //else if (location.X < 0)
            //    gridX = 0;
            else if (location.X < 0 && location.X > -50)
                gridX = 0;
            else
                gridX = (int)Math.Floor(NumSquaresX * location.X / WorldWidth);
   
            if (location.Y >= WorldHeight)
                gridY = NumSquaresY - 1;
            //else if (location.Y < 0)
             //   gridY = 0;
            else if (location.Y < 0 && location.Y > -50)
                gridY = 0;
            else
                gridY = (int) Math.Floor(NumSquaresY * location.Y / WorldHeight);
            
            return gridData[gridX, gridY];
        }

        public GridData getGridLoc(int gridX, int gridY)
        {
            return gridData[gridX, gridY];
        }

        // Find an object in the grid (right now, only robots)
        public GridData findObj(PhysObject obj)
        {
            for (int i = 0; i < NumSquaresX; i++)
            {
                for (int j = 0; j < NumSquaresY; j++)
                {
                    if (gridData[i, j].objectsInSquare.Contains(obj))
                        return gridData[i, j];
                }
            }
            return null;
        }

        public int[] getLocObj(PhysObject obj)
        {
            for (int i = 0; i < NumSquaresX; i++)
            {
                for (int j = 0; j < NumSquaresY; j++)
                {
                    if (gridData[i, j].objectsInSquare.Contains(obj))
                    {
                        int[] loc = { i, j };
                        return loc;
                    }
                }
            }
            return null;
        }

        // returns the coordinates to the center of a grid cell
        public Coordinates getCenterOfCell(int gridX, int gridY)
        {
            double x = ((double)gridX + (double).5) * getLengthGridX();
            double y = ((double)gridY + (double).5) * getLengthGridY();
            return new Coordinates(x, y);
        }

        public double getLengthGridX()
        {
            return (double)WorldWidth / (double)NumSquaresX;
        }

        public double getLengthGridY()
        {
            return (double)WorldHeight / (double)NumSquaresY;
        }

        public List<PhysObject> getObjectsInCell(int x, int y)
        {
            return gridData[x, y].objectsInSquare;
        }

        public bool gridVisited()
        {
            for (int i = 0; i < NumSquaresX; i++)
            {
                for (int j = 0; j < NumSquaresY; j++)
                {
                    if (gridData[i, j].numTimesVisited == 0)
                        return false;
                }
            }
            return true;
        }
    }
}
