using UnityEngine;

namespace Scripts.PlayerController
{
    public class SpaceShipController : MonoBehaviour
    {
        #region Variables
        
        // Movement
        [SerializeField] private float forwardSpeed;
        [SerializeField] private float hoverSpeed;
        private float activeForwardSpeed, activeHoverSpeed;
        private const float ForwardAcceleration = 2.5f;
        private const float HoverAcceleration = 2;

        // Looking
        [SerializeField] private float lookRateSpeed = 90f;
        private Vector2 lookInput, screenCenter, mouseDistance;

        private float rollInput;
        private const float RollSpeed = 90f;
        private const float RollAcceleration = 3.5f;
        
        // Shooting
        public ParticleSystem[] muzzleFlashes;
        public Transform[] raycastOrigin;
        public ParticleSystem hitEffect;
        public TrailRenderer[] tracerEffects;
        public ParticleSystem explosionEffect;
        
        private bool isFiring;
        public int fireRate = 25;
        private float accumulatedTime;
        private Ray ray;
        private RaycastHit hitInfo;
        #endregion
        private void Start()
        {
            screenCenter.x = Screen.width * .5f;
            screenCenter.y = Screen.height * .5f;

        }

        private void Update()
        {
            CalculateMovement();
            ToggleFiring();
            CalculateShooting();
        }

        private void ToggleFiring()
        {
            if (Input.GetButtonDown("Fire1")) isFiring = true;
            if (Input.GetButtonUp("Fire1")) isFiring = false;
        }

        private void CalculateShooting()
        {
            if (!isFiring) return;
            
            accumulatedTime += Time.deltaTime;
            if (accumulatedTime < 1.0f / fireRate) return;
            accumulatedTime = 0;
            
            FireBullet();
        }
        

        private void FireBullet()
        {
            // Muzzle Flash Stuff
            foreach (var particle in muzzleFlashes) particle.Emit(1);
            
            // Pew Pew
            foreach (var origin in raycastOrigin)
            {
                ray.origin = origin.position;
                ray.direction = origin.forward;
                
                // If it hits, make the laser go there
                if (Physics.Raycast(ray, out hitInfo, 100f))
                {
                    var transform1 = hitEffect.transform;
                    transform1.position = hitInfo.point;
                    transform1.forward = hitInfo.normal;
                    hitEffect.Emit(1);
                    
                    foreach (var tracerEffect in tracerEffects)
                    {
                        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
                        tracer.AddPosition(ray.origin);
                        tracer.transform.position = hitInfo.point;
                    }
                    
                    // Destroy the object
                    // Health, make it disappear, bla bla bla

                    hitInfo.transform.gameObject.SetActive(false);
                    var explosion = Instantiate(explosionEffect, hitInfo.point, Quaternion.identity);
                    explosion.Emit(1);


                }
                // If it doesn't, just make it go forward I guess idk
                else
                {
                    foreach (var tracerEffect in tracerEffects)
                    {
                        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
                        tracer.AddPosition(ray.origin);
                        tracer.AddPosition(ray.GetPoint(100f));
                    }
                }
            }
        }


        private void CalculateMovement()
        {
            lookInput.x = Input.mousePosition.x;
            lookInput.y = Input.mousePosition.y;

            mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
            mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;
            mouseDistance = Vector2.ClampMagnitude(mouseDistance, 0.8f);

            rollInput = Mathf.Lerp(rollInput, -Input.GetAxis("Horizontal"), RollAcceleration * Time.deltaTime);

            transform.Rotate(-mouseDistance.y * lookRateSpeed * Time.deltaTime, mouseDistance.x * lookRateSpeed * Time.deltaTime, rollInput * RollSpeed * Time.deltaTime, Space.Self);

            activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxis("Vertical") * forwardSpeed, ForwardAcceleration * Time.deltaTime);
            
            activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxis("Hover") * hoverSpeed, HoverAcceleration);

            var transform1 = transform;
            transform1.position += activeForwardSpeed * Time.deltaTime * transform1.forward +
                                   activeHoverSpeed * Time.deltaTime * transform1.up;
        }


      
    }
}
