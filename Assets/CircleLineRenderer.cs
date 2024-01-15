using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleLineRenderer : MonoBehaviour
{

    private const float DOTTED_LINE_TICKNESS_RATIO = 2.5f;
    private const int NUM_CAP_VERTICES = 6;

    [Tooltip("The radius of the circle perimeter")]
    public float radius = 2;

    [Tooltip("The width of the line for the circle perimeter")]
    public float lineWidth = 0.05f;

    [Tooltip("Check this to use the dotted line material for the circle perimeter line")]
    public bool isDotted = false;

    [Tooltip("The material for the plain line")]
    public Material plainMaterial;

    [Tooltip("The material for the dotted line")]
    public Material dottedMaterial;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.numCapVertices = NUM_CAP_VERTICES;
        SetRadius(radius);
    }

    void Update()
    {
        //While testing in-editor, refresh the circle each frame so we can test the circle by changing the fields in the inspector.
        if (Application.isEditor) SetRadius(radius);
    }

    //Call this method from other scripts to adjust the radius at runtime
    public void SetRadius(float pRadius)
    {
        radius = pRadius;

        if (radius <= 0.1f)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        float tickness = lineWidth;
        if (isDotted) tickness *= DOTTED_LINE_TICKNESS_RATIO;
        lineRenderer.startWidth = tickness;
        lineRenderer.endWidth = tickness;

        //Calculate the number of vertices needed depending on radius so it always looks round.
        //For instance, huge circles need proportionaly less vertices than smaller ones to look good.
        //Source : http://stackoverflow.com/questions/11774038/how-to-render-a-circle-with-as-few-vertices-as-possible
        float e = 0.01f; //constant ratio to adjust, reduce this value for more vertices
        float th = Mathf.Acos(2 * Mathf.Pow(1 - e / radius, 2) - 1); //th is in radian
        int numberOfVertices = Mathf.CeilToInt(2 * Mathf.PI / th);

        lineRenderer.positionCount = numberOfVertices + 1;
        for (int i = 0; i < numberOfVertices + 1; i++)
        {
            float angle = (360f / (float)numberOfVertices) * (float)i;
            lineRenderer.SetPosition(i, radius * new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0));
        }

        //Update material depending if it's a dotted line or a plain one.
        if (isDotted)
        {
            lineRenderer.material = dottedMaterial;
            lineRenderer.materials[0].mainTextureScale = new Vector3(2 * Mathf.PI * radius * (1 / tickness), 1, 1);
        }
        else
        {
            lineRenderer.material = plainMaterial;
            lineRenderer.materials[0].mainTextureScale = Vector3.one;
        }
    }

    //Call this method from other scripts to adjust the width of the line at runtime
    public void SetWidth(float pWidth)
    {
        lineWidth = pWidth;
        SetRadius(radius);
    }

    //Call this method from other scripts to switch between plain and dotted line at runtime
    public void SetIsDotted(bool pIsDotted)
    {
        if (isDotted != pIsDotted)
        {
            isDotted = pIsDotted;
            SetRadius(radius);
        }
    }
}