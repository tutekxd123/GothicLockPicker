using System;
using System.Collections.Generic;
using System.Text;

namespace GothicLockPicker
{
    public class Node : Lock
    {
        int gCost = 0;
        public Node(int value) : base(value)
        {
        }
    }
    internal class AstarFinder
    {
        public static int GetHeuristic(List<Node> nodes)
        {
            int result = 0;
            foreach (Node node in nodes)
            {
                result += Math.Abs(node.Value-4);
            }
            return result;
        }
    }
}

