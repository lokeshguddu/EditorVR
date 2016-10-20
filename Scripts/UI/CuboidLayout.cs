﻿using UnityEngine;
using UnityEngine.EventSystems;

public class CuboidLayout : UIBehaviour
{
	static readonly Vector2 kCuboidPivot = new Vector2(0.5f, 0.5f);
	const float kLayerHeight = 0.004f;
	const float kExtraSpace = 0.00055f; // To avoid Z-fighting

	[SerializeField]
	RectTransform[] m_TargetTransforms;

	[SerializeField]
	RectTransform[] m_TargetHighlightTransforms;

	[Header("Prefab Templates")]
	[SerializeField]
	GameObject m_CubePrefab;

	[SerializeField]
	GameObject m_HighlightCubePrefab;

	Transform[] m_CubeTransforms;
	Transform[] m_HighlightCubeTransforms;

	protected override void Start()
	{
		m_CubeTransforms = new Transform[m_TargetTransforms.Length];
		for (var i = 0; i < m_CubeTransforms.Length; i++)
		{
			var cube = Instantiate(m_CubePrefab).transform;
			cube.SetParent(m_TargetTransforms[i], false);
			m_CubeTransforms[i] = cube;
		}

		m_HighlightCubeTransforms = new Transform[m_TargetHighlightTransforms.Length];
		for (var i = 0; i < m_TargetHighlightTransforms.Length; i++)
		{
			var cube = Instantiate(m_HighlightCubePrefab).transform;
			cube.SetParent(m_TargetHighlightTransforms[i], false);
			m_HighlightCubeTransforms[i] = cube;
		}

		UpdateCubes();
	}

	protected override void OnRectTransformDimensionsChange()
	{
		UpdateCubes();
	}

	/// <summary>
	/// Set a new material on all backing cubes (used for instanced version of the material)
	/// </summary>
	/// <param name="backingCubeMaterial">New material to use</param>
	public void SetMaterials(Material backingCubeMaterial)
	{
		foreach (var cube in m_CubeTransforms)
			cube.GetComponent<Renderer>().sharedMaterial = backingCubeMaterial;
	}

	public void UpdateCubes()
	{
		if (m_CubeTransforms == null)
			return;

		// Update standard cubes
		const float kStandardCubeSideScalePadding = 0.01f;
		const float kStandardCubeSidePositionPadding = 0.005f;
		for (var i = 0; i < m_CubeTransforms.Length; i++)
		{
			var rectSize = m_TargetTransforms[i].rect.size.Abs();
			// Scale pivot by rect size to get correct xy local position
			var pivotOffset =  Vector2.Scale(rectSize, kCuboidPivot - m_TargetTransforms[i].pivot);

			// Add space for cuboid
			var localPosition = m_TargetTransforms[i].localPosition;
			m_TargetTransforms[i].localPosition = new Vector3(localPosition.x, localPosition.y, -kLayerHeight);

			//Offset by 0.5 * height to account for pivot in center
			const float zOffset = kLayerHeight * 0.5f + kExtraSpace;
			m_CubeTransforms[i].localPosition = new Vector3(pivotOffset.x + kStandardCubeSidePositionPadding, pivotOffset.y, zOffset);
			m_CubeTransforms[i].localScale = new Vector3(rectSize.x + kStandardCubeSideScalePadding, rectSize.y, kLayerHeight);
		}

		// Update highlight cubes
		for (var i = 0; i < m_HighlightCubeTransforms.Length; i++)
		{
			var rectSize = m_TargetHighlightTransforms[i].rect.size.Abs();
			// Scale pivot by rect size to get correct xy local position
			var pivotOffset = Vector2.Scale(rectSize, kCuboidPivot - m_TargetHighlightTransforms[i].pivot);

			// Add space for cuboid
			var localPosition = m_TargetHighlightTransforms[i].localPosition;
			m_TargetHighlightTransforms[i].localPosition = new Vector3(localPosition.x, localPosition.y, -kLayerHeight);

			//Offset by 0.5 * height to account for pivot in center
			const float zOffset = kLayerHeight * 0.5f + kExtraSpace;
			m_HighlightCubeTransforms[i].localPosition = new Vector3(pivotOffset.x, pivotOffset.y, zOffset);
			m_HighlightCubeTransforms[i].localScale = new Vector3(rectSize.x, rectSize.y, kLayerHeight);
		}
	}
}