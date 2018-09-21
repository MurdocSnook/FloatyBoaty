using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	private Vector3 lastPosisiton;

	public int damage = 5;
	public bool isPiercing = false;
	public float inactiveDistance = .5f;
	public string[] interactLayers = new string[] {"Default", "Hitbox"};

	private Vector3 initialPosition;

	private void Start() {
		initialPosition = transform.position;
		lastPosisiton = transform.position;
		UpdateLineRenderer();
	}

	private void Update()
    {
        RaycastHit hit;
        Vector3 dif = transform.position - lastPosisiton;
        bool success = Physics.Raycast(lastPosisiton, dif.normalized, out hit, dif.magnitude, LayerMask.GetMask(interactLayers));

        if (success)
        {
            if (Vector3.Distance(initialPosition, hit.point) >= inactiveDistance)
            {
                Creature creature = hit.collider.gameObject.GetComponentInParent<Creature>();
                DestructibleObject destructibleObject = hit.collider.gameObject.GetComponentInParent<DestructibleObject>();
				Hitbox hitbox = hit.collider.gameObject.GetComponent<Hitbox>();

                if (creature != null)
                {
					int modifiedDamage = damage;
					if(hitbox != null) {
						modifiedDamage = Mathf.RoundToInt(modifiedDamage * hitbox.damageMultiplier);
					}
                    creature.DealDamage(modifiedDamage);
                }
                else if(destructibleObject != null) {
                    destructibleObject.DealDamage(damage);
                }

                if (!isPiercing)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        UpdateLineRenderer();

        lastPosisiton = transform.position;
    }

    private void UpdateLineRenderer()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.SetPosition(0, initialPosition);
            lr.SetPosition(1, transform.position);
        }
    }
}
