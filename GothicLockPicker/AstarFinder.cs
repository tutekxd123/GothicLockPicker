using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace GothicLockPicker
{
    public class Node: ICloneable,IComparable
    {

        public int GCost = 0;
        public int FCost = 0;
        public List<int> Graphvalue = new List<int>();

        public Tuple<int, int> MoveParent = Tuple.Create(0, 0);
        public Node? Parent = null;
        public Node(List<int> graphValue)
        {
            this.Graphvalue = graphValue;
        }
        public Node(List<int> graphValue, Tuple<int, int> moveParent, Node? parent)
        {
            this.Graphvalue = graphValue;
            this.MoveParent = moveParent;
            this.Parent = parent;
        }
        public Node(BindingList<LockRow> lockRows) //Conversion
        {
            //Sort by Position to ensure the order is correct
            this.Graphvalue.EnsureCapacity(lockRows.Count()+1);
            int MaxPosition = lockRows.Max(row => row.Position);
            // Initialize all elements to zero
            for (int i = 0; i <= MaxPosition; i++)
            {
                this.Graphvalue.Add(0);
            }

            for (int i = 0; i < lockRows.Count; i++) {
                this.Graphvalue[lockRows[i].Position] = lockRows[i].ValueLock;
            }



        }
        public Node(List<int> graphValue, Tuple<int, int> moveParent, Node? parent, int gCost, int fCost)
        {
            this.Graphvalue = graphValue;
            this.MoveParent = moveParent;
            this.Parent = parent;
            this.GCost = gCost;
            this.FCost = fCost;
        }
        public object Clone()
        {
            return new Node(new List<int>(this.Graphvalue), this.MoveParent, this.Parent, this.GCost,this.FCost);
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

    }
    internal class AstarFinder
    {
        public static int GetHeuristic(Node Current)
        {
            int result = 0;
            for(int i = 0; i < Current.Graphvalue.Count(); i++)
            {
                result += Math.Abs(Current.Graphvalue[i] - 3); // Distance to middle Position
            }
            return result;
        }
        static private string getRoadHumanReadable(Node? CurrNode)
        {
            List<string> result = new List<string>();
            while (CurrNode != null && CurrNode?.Parent!=null) {
                string WayToMove = CurrNode?.MoveParent.Item2 == 1 ? "Right(A)" : "Left(D)";
                string StatusAfter = "[" + string.Join(",", CurrNode.Graphvalue.Select(x => x + 1)) + "]";
                result.Add($"Move Lock {CurrNode?.MoveParent.Item1+1} by {WayToMove} {StatusAfter}");
                CurrNode = CurrNode?.Parent;

            }
            //Reverse Stringh
            result.Reverse();
            return string.Join("\r\n", result);
        }

        //we have to get new node or bool multi type something?

        static private Node? IsPossibleMoveWithUpdateGraph(Node CurrNode, int index, int move, int[,] matrixConnection)
        {
            if (move == 0) return null; //if you don't move, it is always possible
            Node CopyNode = (Node)CurrNode.Clone();
            CopyNode.Parent = CurrNode;

            int NewValue = CopyNode.Graphvalue[index] + move;

            bool OverFlow = NewValue < 0 || NewValue >= 7;
            if (OverFlow)
            {
                return null; //it is illegal move, because value must be between 0 and 7
            }
            //Now Check in MatrixConnections if move is possible
            for(int i = 0; i < CurrNode.Graphvalue.Count(); i++)
            {
                if (matrixConnection[index, i] != 0) //if it isnt 0, it means that there is a connection between index and i
                {
                    if(i == index) continue; //we don't want to check the same lock

                    int ConnectionValue = CurrNode.Graphvalue[i];
                    ConnectionValue += (move*matrixConnection[index, i]);
                    if (ConnectionValue < 0 || ConnectionValue >= 7)
                    {
                        return null; //it is illegal move, because value must be between 0 and 7
                    }
                    CopyNode.Graphvalue[i] = ConnectionValue;
                }

            }
            CopyNode.Graphvalue[index] = NewValue;
            CopyNode.MoveParent = Tuple.Create(index, move);
            return CopyNode;
        }
        static bool IsGoal(Node CurrNode)
        {
            foreach (int value in CurrNode.Graphvalue)
            {
                if (value != 3) return false; //if all Locks are in the middle position, we have reached the goal   
            }
            return true;
        }
        static private List<Node> GetNeighbours(Node CurrNode, int[,] matrix)
        {
            List<Node> neighbours = new List<Node>();
            //2 moves every plate L or R

            for (int i = 0; i < CurrNode.Graphvalue.Count(); i++)
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
        
        public static string GetResolve(int[,] matrix, BindingList<LockRow> lockRows, int limitCost)
        {
            //1.st convert lockRows to List<Node>Graph
            Node StartPoint = new Node(lockRows);
            PriorityQueue<Node, int> OpenSet = new PriorityQueue<Node, int>();
            HashSet<string> visited = new HashSet<string>();
            Dictionary<string, int> bestG = new();


            OpenSet.Enqueue(StartPoint, GetHeuristic(StartPoint));
            bestG[string.Join(",", StartPoint.Graphvalue)] = 0;
            while (OpenSet.Count > 0)
            {
                Node CurrentNode = OpenSet.Dequeue();
                if (CurrentNode.GCost >= limitCost)
                {
                    return "FAILED TO FIND SOLUTION, COST LIMIT REACHED";
                }

                if (IsGoal(CurrentNode))
                {
                    //We have reached the goal, now we have to get the path from the start to the goal
                    return getRoadHumanReadable(CurrentNode);
                }
                visited.Add(string.Join(",", CurrentNode.Graphvalue));
                List<Node> Neighbours = GetNeighbours(CurrentNode, matrix);
                foreach (Node Neighbour in Neighbours)
                {
                    string keyNeigh = string.Join(",", Neighbour.Graphvalue);
                    int newG = CurrentNode.GCost + 1;

                    if (bestG.TryGetValue(keyNeigh, out int oldG) && oldG <= newG)
                    {
                        continue;
                    }

                    Neighbour.GCost = newG;
                    Neighbour.FCost = newG + GetHeuristic(Neighbour);
                    Neighbour.Parent = CurrentNode;

                    bestG[keyNeigh] = newG;
                    OpenSet.Enqueue(Neighbour, Neighbour.FCost);
                }

            }
            return "FAILED TO FIND SOLUTION";
        }

    }
}

