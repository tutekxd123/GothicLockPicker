using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace GothicLockPicker
{
    public class Node: ICloneable,IComparable
    {

        public int GCost = 0;
        public int FCost = 0;
        public int[] Graphvalue = new int[7];
        public Tuple<int, int> MoveParent = Tuple.Create(0, 0);
        public Node? Parent = null;
        public Node(int[] graphValue)
        {
            this.Graphvalue = graphValue;
        }
        public Node(int[] graphValue, Tuple<int, int> moveParent, Node? parent)
        {
            this.Graphvalue = graphValue;
            this.MoveParent = moveParent;
            this.Parent = parent;
        }
        public Node(BindingList<LockRow> lockRows) //Conversion
        {
            //Sort by Position to ensure the order is correct
            
            for (int i = 0; i < lockRows.Count; i++) {
                this.Graphvalue[lockRows[i].Position] = lockRows[i].ValueLock;
            }



        }
        public Node(int[] graphValue, Tuple<int, int> moveParent, Node? parent, int gCost, int fCost)
        {
            this.Graphvalue = graphValue;
            this.MoveParent = moveParent;
            this.Parent = parent;
            this.GCost = gCost;
            this.FCost = fCost;
        }
        public object Clone()
        {
            return new Node((int[])this.Graphvalue.Clone(), this.MoveParent, this.Parent, this.GCost,this.FCost);
        }
        public int CompareTo(object? obj)
        {
            if (obj == null) return 0;
            if(obj is Node node)
            {
                return this.FCost.CompareTo(node.FCost);
            }
            else
            {
                throw new ArgumentException("Object is not a Node");
            }
        }
        //Overide Equals and GetHashCode to compare nodes based on their Graphvalue

    }
    internal class AstarFinder
    {
        public static int GetHeuristic(Node Current)
        {
            int result = 0;
            for(int i = 0; i < Current.Graphvalue.Length; i++)
            {
                result += Math.Abs(Current.Graphvalue[i] - 4);
            }
            return result;
        }
        static private string getRoadHumanReadable(Node CurrNode)
        {
            return "Not Implemented yet";
        }

        //we have to get new node or bool multi type something?

        static private Node? IsPossibleMoveWithUpdateGraph(Node CurrNode, int index, int move, int[,] matrixConnection)
        {
            if (move == 0) return null; //if you don't move, it is always possible
            Node CopyNode = (Node)CurrNode.Clone();
            CopyNode.Parent = CurrNode;

            int NewValue = CopyNode.Graphvalue[index] + move;

            bool OverFlow = NewValue >= 0 && NewValue <= 8;
            if (OverFlow)
            {
                return null; //it is illegal move, because value must be between 0 and 8
            }
            //Now Check in Matrix if move is possible
            for(int i = 0; i < matrixConnection.GetLength(0); i++)
            {
                if (matrixConnection[index, i] != 0) //if it isnt 0, it means that there is a connection between index and i
                {
                    int ConnectionValue = CurrNode.Graphvalue[i];
                    ConnectionValue += matrixConnection[index, i];
                    if (ConnectionValue < 0 || ConnectionValue > 8)
                    {
                        return null; //it is illegal move, because value must be between 0 and 8
                    }
                    CopyNode.Graphvalue[i] = ConnectionValue;
                }
            }
            CopyNode.MoveParent = Tuple.Create(index, move);
            return CopyNode;
        }
        static bool IsGoal(Node CurrNode)
        {
            foreach (int value in CurrNode.Graphvalue)
            {
                if (value != 4) return false; //if all Locks are in the middle position, we have reached the goal   
            }
            return true;
        }
        static private List<Node> GetNeighbours(Node CurrNode, int[,] matrix)
        {
            List<Node> neighbours = new List<Node>();
            //Mamy 3 mozliwe ruchy +1,0,-1 dla kazdego plate

            for (int i = 0; i < 7; i++)
            {
                for(int j=-1;j<=1; j++)
                {
                    if(j==0) continue; //if you don't move, it is always possible, but we don't want to add it to neighbours    
                    Node? PossibleMove = IsPossibleMoveWithUpdateGraph(CurrNode, i, j, matrix);
                    if(PossibleMove != null)
                    {
                        neighbours.Add(PossibleMove);
                    }
                }
            }
            return neighbours;
        }
        
        public static string GetResolve(int[,] matrix, BindingList<LockRow> lockRows)
        {
            //1.st convert lockRows to List<Node>Graph
            Node StartPoint = new Node(lockRows);
            List<Node> OpenSet = new List<Node>(); //Change to PriorityQueue if needed
            List<Node> ClosedSet = new List<Node>();
            OpenSet.Add(StartPoint);
            
            //TODO: Implement A* algorithm to find the optimal solution for the lock picking problem
            return "Not implemented yet!";
        }

    }
}

