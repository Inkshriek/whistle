using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ShaderColorAnimator : MonoBehaviour {

	public string _shaderVariable;
	public Color _value;

	private Material _material;
	private int _shaderVariableId;

	// Use this for initialization
	void Start () {
		Renderer renderer = GetComponent<Renderer>();
		_material = renderer.material;
		_shaderVariableId = Shader.PropertyToID(_shaderVariable);
	}
	
	// Update is called once per frame
	void Update () {
		_material.SetColor(_shaderVariableId, _value);
	}
}
