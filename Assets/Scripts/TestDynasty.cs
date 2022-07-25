using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DepthOfDynasty;

public class TestDynasty : MonoBehaviour
{
    public Ruler familyRootRuler;

    // Start is called before the first frame update
    void Start()
    {
        familyRootRuler = new Ruler();
        familyRootRuler.children = new List<Ruler>();

        Ruler gen2_child_1 = new Ruler() { children = new List<Ruler>() };
        Ruler gen2_child_2 = new Ruler(){ children = new List<Ruler>() };
        Ruler gen2_child_3 = new Ruler() { children = new List<Ruler>() };

        familyRootRuler.children.Add(gen2_child_1);
        familyRootRuler.children.Add(gen2_child_2);
        familyRootRuler.children.Add(gen2_child_3);

        // so far: root -> c1, c2, c3

        Ruler gen3_child_1 = new Ruler(){ children = new List<Ruler>() };
        Ruler gen3_child_2 = new Ruler(){ children = new List<Ruler>() };

        gen2_child_1.children.Add(gen3_child_1);
        gen2_child_3.children.Add(gen3_child_2);

        // so far: root -> c1->c1, c2, c3->c1


        Ruler gen4_child_1 = new Ruler() { children = new List<Ruler>() };
        gen3_child_2.children.Add(gen4_child_1);

        // so far: root -> c1->c1, c2, c3->c1->c1

        Debug.Log("root family depth: " + DepthOfDynasty.FamilyTree.CalculateFamilyTreeDepth(familyRootRuler));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
