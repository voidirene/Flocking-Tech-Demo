﻿using UnityEngine;

public class Wander : SteeringBehaviour
{
	/// <summary>
	/// Controls how large the imaginary circle is
	/// NOTE: [SerializeField] exposes a C# variable to Unity's inspector without making it public. Useful for encapsulating code
	/// while still giving access to the Unity inspector
	/// </summary>
	[SerializeField]
    protected float circleRadius = 150.0f;

	/// <summary>
	/// Controls how far from the agent position should the centre of the circle be
	/// NOTE: [SerializeField] exposes a C# variable to Unity's inspector without making it public. Useful for encapsulating code
	/// while still giving access to the Unity inspector
	/// </summary>
	[SerializeField]
    protected float circleDistance = 250.0f;

	/// <summary>
	/// Controls how large the random displacement can be per game frame
	/// NOTE: [SerializeField] exposes a C# variable to Unity's inspector without making it public. Useful for encapsulating code
	/// while still giving access to the Unity inspector
	/// </summary>
	[SerializeField]
	protected float maxRandomDisplacement = 25.0f;

	/// <summary>
	/// Stores the previous target position of the behaviour so it can be used in future calculations
	/// </summary>
	private Vector3 previousTargetPosition;

	protected override void Start()
	{
		base.Start();

		// Start with a random direction to make it interesting
		transform.up = RandomPointOnUnitCircleCircumference();

		// Set the intial previous target position on the circle as the furtherest point from the agent
		previousTargetPosition = transform.up * (circleDistance + circleRadius);
	}

	public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
	{
		// First get a random position on the circumference of a circle which can be used as a direction
		Vector3 targetPosition = RandomPointOnUnitCircleCircumference();

		// Scale the direction by the maximum amount of displacement to get the "small circle"
		targetPosition *= maxRandomDisplacement;

		// Add the previous target position to get a displacement from the last target position
		targetPosition += previousTargetPosition;

		// These 2 lines effectly remap the target position to a point on the circumference of the imaginary circle
		// Get the centre position of the imaginary circle and then calculate the direction to the target position
		// Normalise to a unit vector so it can be scaled to the size of the imaginary circle and then add on
		// the position of the circle to get the point back on to the circumference of the circle
		Vector3 circlePosition = transform.position + (transform.up * circleDistance);
		targetPosition = circlePosition + (Vector3.Normalize(targetPosition - circlePosition) * circleRadius);

		// Update the previous target position with the new position
		previousTargetPosition = targetPosition;

		// Get the desired velocity just like seek and limit to maxSpeed
		desiredVelocity = Vector3.Normalize(targetPosition - transform.position) * steeringAgent.MaxSpeed;

		// Calculate steering velocity
		steeringVelocity = desiredVelocity - steeringAgent.CurrentVelocity;
		return steeringVelocity;
	}

	protected static Vector3 RandomPointOnUnitCircleCircumference()
	{
		float randomAngle = Random.value * (2.0f * Mathf.PI);
		return new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0.0f);

		// Could also use Unity and do the following:
		//Vector3 circumferencePosition = Random.onUnitSphere;
		//circumferencePosition.z = 0.0f;
		//circumferencePosition.Normalize();
		//return circumferencePosition;
	}

	public override void DebugDraw(SteeringAgent steeringAgent)
	{
		base.DebugDraw(steeringAgent);

		Vector3 circlePosition = transform.position + (transform.up * circleDistance);
		DebugDrawCircle(circlePosition, circleRadius);
	}
}
