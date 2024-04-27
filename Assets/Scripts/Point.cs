using System;
using TMPro;
using UnityEngine;

public class Point : MonoBehaviour
{
    public static event Action<int, Vector3> OnValueChanged;

    [SerializeField] private TMP_Text _index;
    [SerializeField] private TMP_InputField _x;
    [SerializeField] private TMP_InputField _y;
    [SerializeField] private TMP_InputField _z;

    private int _id;
    private Vector3 _point;

    public void Init(string index, int id, float x, float y, float z)
    {
        _index.text = index;
        _x.text = x.ToString();
        _y.text = y.ToString();
        _z.text = z.ToString();

        _id = id;
        _point = new(x, y, z);
    }

    public void ChangeValue()
    {
        _point = new(float.Parse(_x.text), float.Parse(_y.text), float.Parse(_z.text));
        OnValueChanged?.Invoke(_id, _point);
    }
}