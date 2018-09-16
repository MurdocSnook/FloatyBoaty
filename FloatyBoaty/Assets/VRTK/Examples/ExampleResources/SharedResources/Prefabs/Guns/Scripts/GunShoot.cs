﻿namespace VRTK.Examples
{
    using UnityEngine;

    [RequireComponent(typeof(AudioSource))]
    public class GunShoot : MonoBehaviour
    {
        public VRTK_InteractableObject linkedObject;
        public GameObject projectile;
        public Transform projectileSpawnPoint;
        public float projectileSpeed = 1000f;
        public float projectileLife = 5f;

        public bool testFire = false;
        private float testFireTimer = 0;

        private void Update() {
            testFireTimer += Time.deltaTime;
            if(testFireTimer > 1f) {
                testFireTimer = 0;
                if(testFire) {
                    FireProjectile();
                }
            }
        }

        protected virtual void OnEnable()
        {
            linkedObject = (linkedObject == null ? GetComponent<VRTK_InteractableObject>() : linkedObject);

            if (linkedObject != null)
            {
                linkedObject.InteractableObjectUsed += InteractableObjectUsed;
            }
        }

        protected virtual void OnDisable()
        {
            if (linkedObject != null)
            {
                linkedObject.InteractableObjectUsed -= InteractableObjectUsed;
            }
        }

        protected virtual void InteractableObjectUsed(object sender, InteractableObjectEventArgs e)
        {
            GetComponent<AudioSource>().Play();
            FireProjectile();
        }

        protected virtual void FireProjectile()
        {
            if (projectile != null && projectileSpawnPoint != null)
            {
                GameObject clonedProjectile = Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                Rigidbody projectileRigidbody = clonedProjectile.GetComponent<Rigidbody>();
                float destroyTime = 0f;
                if (projectileRigidbody != null)
                {
                    projectileRigidbody.AddForce(clonedProjectile.transform.forward * projectileSpeed);
                    destroyTime = projectileLife;
                }
                Destroy(clonedProjectile, destroyTime);
            }
        }
    }
}