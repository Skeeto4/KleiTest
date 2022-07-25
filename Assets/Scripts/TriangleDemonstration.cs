using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static WhatsThePoint;

public class TriangleDemonstration : MonoBehaviour
{
    [System.Serializable]
    public class CountryOBJ
    {
        public string name;
        public TriangleOBJ[] trianglesOBJs;
        public Country country { private set; get; }

        public void Setup()
        {
            country = new Country();
            country.name = this.name;
            country.triangles = new List<Triangle>();
            for (int i = 0; i < trianglesOBJs.Length; i++)
            {
                trianglesOBJs[i].Setup();
                country.triangles.Add(trianglesOBJs[i].triangle);
            }
        }
    }
    [System.Serializable]
    public class TriangleOBJ
    {
        public RectTransform[] triangleEdgesObjects;
        public Triangle triangle { private set; get; }

        public void Setup()
        {
            Point p1 = new Point() { x = triangleEdgesObjects[0].anchoredPosition.x, y = triangleEdgesObjects[0].anchoredPosition.y };
            Point p2 = new Point() { x = triangleEdgesObjects[1].anchoredPosition.x, y = triangleEdgesObjects[1].anchoredPosition.y };
            Point p3 = new Point() { x = triangleEdgesObjects[2].anchoredPosition.x, y = triangleEdgesObjects[2].anchoredPosition.y };
            Debug.Log($"Triangle: ({p1.x}, {p1.y} | {p2.x}, {p2.y} | {p3.x}, {p3.y})");
            triangle = new Triangle(p1, p2, p3);
        }
    }

    public Text textLabel;
    public CountryOBJ[] countries;
    private Map map;

    void Start()
    {
        List<Country> countriesData = new List<Country>();
        for (int i =0; i < countries.Length; i++)
        {
            countries[i].Setup();
            countriesData.Add(countries[i].country);
        }

        map = new Map() { countries = countriesData };
    }

    // Update is called once per frame
    void Update()
    {
        Country country = map.Intersects(new Point() { x = Input.mousePosition.x, y = Input.mousePosition.y });
        textLabel.text = country == null ? "" : country.name;
       // Debug.Log("[Cursor] " + Input.mousePosition.ToString());
    }
}
