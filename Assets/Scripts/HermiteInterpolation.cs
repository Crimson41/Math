using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class HermiteInterpolation : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform _pointsList;
    [SerializeField] private GameObject _pointPrefab;

    [Header("Curves")]
    [SerializeField] private List<Vector3> _points = new()
    {
        new(2, 3, -10), // A
        new(2, 7, 1), // B
        new(6, 7, -1), // C
        new(4, 9.5f, -0.1f), // D
        new(8, 7, 2), // E
        new(9, 4.5f, 0), // F
        new(9.5f, 5.5f, -1), // G
        new(6, 3, -0.5f), // H
        new(6, 2, -0.3f), // I
    };
    // Points graphiquement relevés :
    // (Abscisses, ordonnées, tangentes)

    private List<float> X_final; // Abscisses finaux
    private List<float> Y_final; // Ordonnées finaux
    private int _precisionPoints = 10;
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        Point.OnValueChanged += Point_OnValueChanged;

        _lineRenderer = GetComponent<LineRenderer>();

        char index = 'A';
        int id = 0;

        foreach (var point in _points)
        {
            Instantiate(_pointPrefab, _pointsList).GetComponent<Point>().Init(index.ToString(), id, point.x, point.y, point.z);

            index++;
            id++;
        }

        _points.Add(new());
        Draw();
    }

    private void Point_OnValueChanged(int id, Vector3 point)
    {
        _points[id] = point;
        if (id == 0) EditLastPoint();

        Draw();
    }

    private void EditLastPoint()
    {
        Vector3 lastPoint = _points.ElementAt(0);
        lastPoint.z = 0;
        _points[9] = lastPoint;
    }

    public void Draw()
    {
        Debug.Log("Drawing new curves");
        EditLastPoint();

        X_final = new(); // Abscisses finaux
        Y_final = new(); // Ordonnées finaux

        // Parcourir les points relevés et effectuer l'interpolation Hermite entre eux
        for (int i = 0; i < _points.Count - 1; i++)
        {
            Vector3 p0 = _points[i];
            Vector3 p1 = _points[i + 1];
            Hermite(p0.x, p0.y, p0.z, p1.x, p1.y, p1.z, _precisionPoints);
        }

        DrawCurve(); // Rendu final
    }

    private void Hermite(float x0, float y0, float m0, float x1, float y1, float m1, int numPoints)
    {
        for (int i = 0; i < numPoints; i++)
        {
            float t = (float)i / (numPoints - 1);
            float h00 = (1 + 2 * t) * Mathf.Pow(1 - t, 2);
            float h10 = t * Mathf.Pow(1 - t, 2);
            float h01 = Mathf.Pow(t, 2) * (3 - 2 * t);
            float h11 = Mathf.Pow(t, 2) * (t - 1);
            float X = h00 * x0 + h10 * (x0 + m0) + h01 * x1 + h11 * (x1 + m1);
            float Y = h00 * y0 + h10 * (y0 + m0) + h01 * y1 + h11 * (y1 + m1);
            X_final.Add(X);
            Y_final.Add(Y);
        }
    }
    public void DrawCurve()
    {
        //LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.positionCount = X_final.Count;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.useWorldSpace = false;

        for (int i = 0; i < X_final.Count; i++)
        {
            _lineRenderer.SetPosition(i, new Vector3(X_final[i], Y_final[i], 0));
        }
    }
}