using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthOfDynasty : MonoBehaviour
{
    public class Ruler
    {
        public List<Ruler> children = new List<Ruler>();
    }
    public class FamilyTree
    {
        public static int CalculateFamilyTreeDepth(Ruler ruler)
        {
            // I will tackle this task using recursive, usually when i do recursive I must handle every case that could make an infinite loop, but i will assume there was no insest in the family
            // but if i had to handle those cases, I would pass a list of rulers as a second parameter that will keep track of the rulers we already evaluated and check if we have evaluated that ruler before calling the recursive function.

            bool hasChildren = ruler.children != null && ruler.children.Count > 0; // first will check if teh ruler has childrens
            int familyDepth = hasChildren ? 1 : 0; // if so, this ruler starts with 1 generation or 1 point for family depth.

            if (hasChildren) // then per each of those children, lets ask if they also had children.
            {
                int maxChildrenFamilyDepth = 0; // I want to know which children had the deepest generation tree
                foreach(Ruler child in ruler.children)
                {
                    int generationsAfterThisChild = CalculateFamilyTreeDepth(child); // recursive, ask every child of the leader how deep is their family tree
                    if (generationsAfterThisChild > maxChildrenFamilyDepth) // keep track of each result, but hold the largest one.
                    {
                        maxChildrenFamilyDepth = generationsAfterThisChild;
                    }
                }

                familyDepth += maxChildrenFamilyDepth; // add those genearions to my original count.
            }

            return familyDepth;
        }

    }
}
