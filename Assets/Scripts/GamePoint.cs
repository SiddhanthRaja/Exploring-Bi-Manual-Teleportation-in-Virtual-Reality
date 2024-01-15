
using UnityEditor;
using UnityEngine;

namespace PointSystem
{
    [RequireComponent(typeof(Collider))]
    public class GamePoint : MonoBehaviour
    {
        public float points = 1;

        public System.Action<GamePoint,float,float> OnTriggerEnterAction;
        public System.Action<GamePoint> OnVisibleInFov;
        public System.Action<GamePoint> OnSkipPoint;
        public GameObject MRPS;

        void Start()
        {
            // Make sure none of the colliders in child objects are active
            //foreach (Collider collider in GetComponentsInChildren<Collider>())
            //{
            //    collider.enabled = false;
            //}

            // Make sure the root collider is a trigger and enabled
            Collider rootCollider = GetComponent<Collider>();
            rootCollider.enabled = true;
            rootCollider.isTrigger = true;


            Vector3 tmp = transform.position;
            tmp.y += 10;
            //DrawLine(transform.position, tmp, Color.white);

        }


        private void Update()
        {
            Vector3 pointOnScreen = Camera.main.WorldToScreenPoint(gameObject.GetComponentInChildren<Renderer>().bounds.center);

            
            int centerFOVWidthMax = Camera.main.scaledPixelWidth - Camera.main.scaledPixelWidth / 4;
            int centerFOVWidthMin = Camera.main.scaledPixelWidth / 4;

            int centerFOVHeightMax = Camera.main.scaledPixelHeight - Camera.main.scaledPixelHeight / 4;
            int centerFOVheightMin = Camera.main.scaledPixelHeight / 4;

            //Is in FOV
            if ((pointOnScreen.x < centerFOVWidthMin) || (pointOnScreen.x > centerFOVWidthMax) 
                || (pointOnScreen.y < centerFOVheightMin) || (pointOnScreen.y > centerFOVHeightMax))
            {
                //Debug.Log("OutOfBounds");
            }
            else
            {
                OnVisibleInFov?.Invoke(this);
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.transform.IsChildOf(MRPS.transform))
            {
                Debug.Log(collider.transform.gameObject.name + " x " + transform.gameObject.name );
                OnTriggerEnterAction?.Invoke(this, 0.0f, 0.0f);
            }
        }

        public void manuallyTrigger(float distance, float distanceWithSpike)
        {
            //OnTriggerEnterAction?.Invoke(this, distance, distanceWithSpike);
        }

        public void skipPoint()
        {
            //OnSkipPoint?.Invoke(this);
        }
    }
}
