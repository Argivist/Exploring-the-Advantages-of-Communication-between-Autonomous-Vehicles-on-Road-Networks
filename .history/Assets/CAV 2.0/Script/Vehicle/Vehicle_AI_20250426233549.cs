

// Traffic Simulation
// https://github.com/mchrbn/unity-traffic-simulation

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSimulation
{

    /*
        [-] Check prefab #6 issue
        [-] Deaccelerate when see stop in front
        [-] Smooth sharp turns when two segments are linked
        
    */

    // public struct Target{
    //     public int segment;
    //     public int waypoint;
    // }

    public enum Status_
    {
        GO,
        STOP,
        SLOW_DOWN,

        END
    }

    public class Vehicle_AI : MonoBehaviour
    {
        [Header("Traffic System")]
        [Tooltip("Current active traffic system")]
        public TrafficSystem trafficSystem;

        [Tooltip("Determine when the vehicle has reached its target. Can be used to \"anticipate\" earlier the next waypoint (the higher this number his, the earlier it will anticipate the next waypoint)")]
        public float waypointThresh = 6;


        [Header("Radar")]

        [Tooltip("Empty gameobject from where the rays will be casted")]
        public Transform raycastAnchor;

        [Tooltip("Length of the casted rays")]
        public float raycastLength = 5;

        [Tooltip("Spacing between each rays")]
        public int raySpacing = 2;

        [Tooltip("Number of rays to be casted")]
        public int raysNumber = 6;

        [Tooltip("If detected vehicle is below this distance, ego vehicle will stop")]
        public float emergencyBrakeThresh = 2f;

        [Tooltip("If detected vehicle is below this distance (and above, above distance), ego vehicle will slow down")]
        public float slowDownThresh = 4f;

        [HideInInspector] public Status_ vehicleStatus = Status_.GO;

        private WheelDrive wheelDrive;
        private float initMaxSpeed = 0;
        private int pastTargetSegment = -1;
        private Target currentTarget;
        private Target futureTarget;

        // For debugging
        [Header("Target Information")]
        public int currentSegment;
        public int currentWaypoint;
        public int futureSegment;
        public int futureWaypoint;

        [Header("Navigation")]
        public Navigation NavigationComponent;

        [Header("DataGathering")]
        public StopWatch StopWatch;

        [Header("Debug")]
        bool changedSegment;
        int prevSegment = -1;


        void Start()
        {
            // NavigationComponent.UpdateCurrentSegment();
            // NavigationComponent.UpdatePath();
            wheelDrive = this.GetComponent<WheelDrive>();
            StopWatch = this.GetComponent<StopWatch>();
            StopWatch.startTimer();

            if (trafficSystem == null)
                return;

            initMaxSpeed = wheelDrive.maxSpeed;
            SetWaypointVehicleIsOn();
            prevSegment = currentTarget.segment;
            changedSegment = false;
        }

        void Update()
        {
            if (NavigationComponent.isReady == false)
            {
                return;
            }


            currentSegment = currentTarget.segment;
            currentWaypoint = currentTarget.waypoint;
            futureSegment = futureTarget.segment;
            futureWaypoint = futureTarget.waypoint;
            if (prevSegment != currentSegment)
            {
                NavigationComponent.EnterSegment();// indicate entry for comsys
                Debug.Log("Changed segment: " + currentSegment + " Vehicle: " + this.gameObject.name);
                prevSegment = currentTarget.segment;
            }




            if (trafficSystem == null)
                return;

            WaypointChecker();
            MoveVehicle();

            if (NavigationComponent.destinationReached(this.transform.position))
            {
                DestroyVehicle_();
            }

        }

    
        public void DestroyVehicle()
        {
            float time = StopWatch.getTime();
            StopWatch.stopTimer();
            SimulationMaster sm = GameObject.FindObjectOfType<SimulationMaster>();
            sm.VehicleDestroyed(this.gameObject.name, time);
            Destroy(this.gameObject);
        }
        public void DestroyVehicle_()
        {
            float time = StopWatch.getTime();
            StopWatch.stopTimer();
            SimulationMaster_ sm = GameObject.FindObjectOfType<SimulationMaster_>();
            sm.VehicleDestroyed(GetComponent<Description>().id, time);
            Destroy(this.gameObject);
        }


        // void WaypointChecker(){
        //     GameObject waypoint = trafficSystem.segments[currentTarget.segment].waypoints[currentTarget.waypoint].gameObject;

        //     //Position of next waypoint relative to the car
        //     Vector3 wpDist = this.transform.InverseTransformPoint(new Vector3(waypoint.transform.position.x, this.transform.position.y, waypoint.transform.position.z));

        //     //Go to next waypoint if arrived to current
        //     if(wpDist.magnitude < waypointThresh){
        //         //Get next target
        //         currentTarget.waypoint++;
        //         if(currentTarget.waypoint >= trafficSystem.segments[currentTarget.segment].waypoints.Count){
        //             pastTargetSegment = currentTarget.segment;
        //             // currentTarget.segment = futureTarget.segment;
        //             currentTarget.segment = NavigationComponent.GetNextSegmentId();
        //             currentTarget.waypoint = 0;
        //         }

        //         //Get future target
        //         futureTarget.waypoint = currentTarget.waypoint + 1;
        //         if(futureTarget.waypoint >= trafficSystem.segments[currentTarget.segment].waypoints.Count){
        //             futureTarget.waypoint = 0;
        //             if(NavigationComponent.path.Count>1){
        //             futureTarget.segment = NavigationComponent.path[1];//GetNextSegmentId(); Not necessary
        //             }
        //         }
        //     }
        // }
        void SetWaypointVehicleIsOn()
        {
            // if (currentTarget.segment < 0 || currentTarget.segment >= trafficSystem.segments.Count)
            // {
            //     Debug.LogError($"Invalid segment index: {currentTarget.segment}. Segment count: {trafficSystem.segments.Count}");
            //     return;
            // }
            // if (currentTarget.waypoint < 0 || currentTarget.waypoint >= trafficSystem.segments[currentTarget.segment].waypoints.Count)
            // {
            //     Debug.LogError($"Invalid waypoint index: {currentTarget.waypoint}. Waypoint count: {trafficSystem.segments[currentTarget.segment].waypoints.Count}");
            //     return;
            // }

            // NavigationComponent.UpdateCurrentSegment();
            //Find current target
            // NavigationComponent.ExitSegment();// indicate exit for comsys
            foreach (Segment segment in trafficSystem.segments)
            {
                if (segment.IsOnSegment(this.transform.position))
                {
                    currentTarget.segment = segment.id;
                    pastTargetSegment = currentTarget.segment;

                    //Find nearest waypoint to start within the segment
                    float minDist = float.MaxValue;
                    for (int j = 0; j < trafficSystem.segments[currentTarget.segment].waypoints.Count; j++)
                    {

                        float d = Vector3.Distance(this.transform.position, trafficSystem.segments[currentTarget.segment].waypoints[j].transform.position);

                        //Only take in front points
                        Vector3 lSpace = this.transform.InverseTransformPoint(trafficSystem.segments[currentTarget.segment].waypoints[j].transform.position);

                        if (d < minDist && lSpace.z > 0)
                        {
                            minDist = d;
                            currentTarget.waypoint = j;
                            string s = "";
                            //start segment, waypoint and next segment
                            s = "Start segment: " + currentTarget.segment + "\n Start waypoint: " + currentTarget.waypoint + "\n Start Vehicle: " + this.gameObject.name;
                            // Debug.LogWarning(s);


                        }
                    }
                    break;
                }
            }
            NavigationComponent.CurSegSet();
            NavigationComponent.EnterSegment();// indicate entry for comsys
            //                                    // NavigationComponent.ExitEnterSegment();// indicate entry and exit for comsys

            //Get future target
            futureTarget.waypoint = currentTarget.waypoint + 1;
            futureTarget.segment = currentTarget.segment;

            if (futureTarget.waypoint >= trafficSystem.segments[currentTarget.segment].waypoints.Count)
            {
                futureTarget.waypoint = 0;
                futureTarget.segment = NavigationComponent.GetNextSegmentId();
                // Debug.LogWarning("Future target segment: " + futureTarget.segment);
                //NavigationComponent.path[1];// GetNextSegmentId();// TODO: USE GET SEGMENT HERE SO THE NEXT SEGMENT IS SET AND THEN CORRECT THE WAYPOINT CHECKER
                // Debug.Log("Future target segment: " + futureTarget.segment);
            }
        }
        void WaypointChecker()
        {

            // Validate segment index
            if (currentTarget.segment < 0 || currentTarget.segment >= trafficSystem.segments.Count)
            {
                Debug.LogWarning($"Invalid segment index: {currentTarget.segment}. Segment count: {trafficSystem.segments.Count}");
                return;
            }

            // Validate waypoint index
            if (currentTarget.waypoint < 0 || currentTarget.waypoint >= trafficSystem.segments[currentTarget.segment].waypoints.Count)
            {
                Debug.LogWarning($"Invalid waypoint index: {currentTarget.waypoint}. Waypoint count: {trafficSystem.segments[currentTarget.segment].waypoints.Count}");
                return;
            }

            GameObject waypoint = trafficSystem.segments[currentTarget.segment].waypoints[currentTarget.waypoint].gameObject;

            // Position of next waypoint relative to the car
            Vector3 wpDist = this.transform.InverseTransformPoint(new Vector3(waypoint.transform.position.x, this.transform.position.y, waypoint.transform.position.z));

            // Go to next waypoint if arrived at current
            if (wpDist.magnitude < waypointThresh)
            {
                currentTarget.waypoint++;

                // if at the end of the segment waypoint reached set next segment
                if (currentTarget.segment == -1)
                {
                    DestroyVehicle_();
                }
                // if the vehicle is at the last waypoint of the segment
                if (currentTarget.waypoint >= trafficSystem.segments[currentTarget.segment].waypoints.Count)
                {
                    NavigationComponent.ExitSegment();// indicate exit for comsys
                    // if(NavigationComponent.vehicleType==Navigation.VehicleType.CAV){
                    //     futureTarget.segment = NavigationComponent.GetNextSegmentId();
                    //     futureTarget.waypoint = 0;
                    // }
                    pastTargetSegment = currentTarget.segment;
                    currentTarget.segment = futureTarget.segment;//NavigationComponent.GetNextSegmentId();
                    currentTarget.waypoint = 0;
                    // NavigationComponent.EnterSegment();// indicate entry for comsys
                }

                // Get future target
                futureTarget.waypoint = currentTarget.waypoint + 1;

                if (futureTarget.waypoint >= trafficSystem.segments[currentTarget.segment].waypoints.Count)// && NavigationComponent.vehicleType==Navigation.VehicleType.NonCAV)
                {
                    futureTarget.waypoint = 0;
                    // try{
                    futureTarget.segment = NavigationComponent.GetNextSegmentId();
                    // }catch{
                    // Debug.LogError("line 287");
                    // }
                }
            }
        }


        void MoveVehicle()
        {
            // Ensure segment and waypoint indices are valid
            if (currentTarget.segment < 0 || currentTarget.segment >= trafficSystem.segments.Count)
            {
                Debug.LogWarning($"MoveVehicle Error: Invalid segment index {currentTarget.segment}. Segment count: {trafficSystem.segments.Count} Vehicle: {this.gameObject.name}");
                return;
            }

            if (currentTarget.waypoint < 0 || currentTarget.waypoint >= trafficSystem.segments[currentTarget.segment].waypoints.Count)
            {
                Debug.LogWarning($"MoveVehicle Error: Invalid waypoint index {currentTarget.waypoint}. Waypoint count: {trafficSystem.segments[currentTarget.segment].waypoints.Count} vehicle: {this.gameObject.name}");
                return;
            }

            // if (futureTarget.segment < 0 || futureTarget.segment >= trafficSystem.segments.Count)
            // {
            //     // if(currentTarget.segment == NavigationComponent.DestinationSegment.id){
            //     //     DestroyVehicle_();
            //     // }
            //     Debug.LogWarning($"MoveVehicle Error: Invalid future segment index {futureTarget.segment}. Segment count: {trafficSystem.segments.Count} vehicle: {this.gameObject.name}");
            //     return;
            // }

            // if (futureTarget.waypoint < 0 || futureTarget.waypoint >= trafficSystem.segments[futureTarget.segment].waypoints.Count)
            // {
            //     Debug.LogWarning($"MoveVehicle Error: Invalid future waypoint index {futureTarget.waypoint}. Waypoint count: {trafficSystem.segments[futureTarget.segment].waypoints.Count} vehicle: {this.gameObject.name}");
            //     return;
            // }
            //Default, full acceleration, no break and no steering
            float acc = 1;
            float brake = 0;
            float steering = 0;
            wheelDrive.maxSpeed = initMaxSpeed;

            //Calculate if there is a planned turn
            Transform targetTransform = trafficSystem.segments[currentTarget.segment].waypoints[currentTarget.waypoint].transform;
            Transform futureTargetTransform = null;
            if (futureTarget.segment >= 0 && futureTarget.segment < trafficSystem.segments.Count && futureTarget.waypoint >= 0 && futureTarget.waypoint < trafficSystem.segments[futureTarget.segment].waypoints.Count)
            {
                futureTargetTransform = trafficSystem.segments[futureTarget.segment].waypoints[futureTarget.waypoint].transform;
            }
            else
            {
                // DestroyVehicle_();//TODO - temp
                futureTargetTransform = trafficSystem.segments[currentTarget.segment].nextSegments[0].waypoints[0].transform;
                Debug.LogWarning("Future segment or waypoint is invalid at turn");
            }
            Vector3 futureVel = futureTargetTransform.position - targetTransform.position;
            float futureSteering = Mathf.Clamp(this.transform.InverseTransformDirection(futureVel.normalized).x, -1, 1);

            //Check if the car has to stop
            if (vehicleStatus == Status_.STOP)
            {
                acc = 0;
                brake = 1;
                wheelDrive.maxSpeed = Mathf.Min(wheelDrive.maxSpeed / 2f, 5f);
            }
            else
            {

                //Not full acceleration if have to slow down
                if (vehicleStatus == Status_.SLOW_DOWN)
                {
                    acc = .3f;
                    brake = 0f;
                }

                //If planned to steer, decrease the speed
                if (futureSteering > .3f || futureSteering < -.3f)
                {
                    wheelDrive.maxSpeed = Mathf.Min(wheelDrive.maxSpeed, wheelDrive.steeringSpeedMax);
                }

                //2. Check if there are obstacles which are detected by the radar
                float hitDist;
                GameObject obstacle = GetDetectedObstacles(out hitDist);

                //Check if we hit something
                if (obstacle != null)
                {

                    WheelDrive otherVehicle = null;
                    otherVehicle = obstacle.GetComponent<WheelDrive>();

                    ///////////////////////////////////////////////////////////////
                    //Differenciate between other vehicles AI and generic obstacles (including controlled vehicle, if any)
                    if (otherVehicle != null)
                    {
                        //Check if it's front vehicle
                        float dotFront = Vector3.Dot(this.transform.forward, otherVehicle.transform.forward);

                        //If detected front vehicle max speed is lower than ego vehicle, then decrease ego vehicle max speed
                        if (otherVehicle.maxSpeed < wheelDrive.maxSpeed && dotFront > .8f)
                        {
                            float ms = Mathf.Max(wheelDrive.GetSpeedMS(otherVehicle.maxSpeed) - .5f, .1f);
                            wheelDrive.maxSpeed = wheelDrive.GetSpeedUnit(ms);
                        }

                        //If the two vehicles are too close, and facing the same direction, brake the ego vehicle
                        if (hitDist < emergencyBrakeThresh && dotFront > .8f)
                        {
                            acc = 0;
                            brake = 1;
                            wheelDrive.maxSpeed = Mathf.Max(wheelDrive.maxSpeed / 2f, wheelDrive.minSpeed);
                        }

                        //If the two vehicles are too close, and not facing same direction, slight make the ego vehicle go backward
                        else if (hitDist < emergencyBrakeThresh && dotFront <= .8f)
                        {
                            acc = -.3f;
                            brake = 0f;
                            wheelDrive.maxSpeed = Mathf.Max(wheelDrive.maxSpeed / 2f, wheelDrive.minSpeed);

                            //Check if the vehicle we are close to is located on the right or left then apply according steering to try to make it move
                            float dotRight = Vector3.Dot(this.transform.forward, otherVehicle.transform.right);
                            //Right
                            if (dotRight > 0.1f) steering = -.3f;
                            //Left
                            else if (dotRight < -0.1f) steering = .3f;
                            //Middle
                            else steering = -.7f;
                        }

                        //If the two vehicles are getting close, slow down their speed
                        else if (hitDist < slowDownThresh)
                        {
                            acc = .5f;
                            brake = 0f;
                            //wheelDrive.maxSpeed = Mathf.Max(wheelDrive.maxSpeed / 1.5f, wheelDrive.minSpeed);
                        }
                    }

                    ///////////////////////////////////////////////////////////////////
                    // Generic obstacles
                    else
                    {
                        //Emergency brake if getting too close
                        if (hitDist < emergencyBrakeThresh)
                        {
                            acc = 0;
                            brake = 1;
                            wheelDrive.maxSpeed = Mathf.Max(wheelDrive.maxSpeed / 2f, wheelDrive.minSpeed);
                        }

                        //Otherwise if getting relatively close decrease speed
                        else if (hitDist < slowDownThresh)
                        {
                            acc = .5f;
                            brake = 0f;
                        }


                    }
                }

                //Check if we need to steer to follow path
                if (acc > 0f)
                {
                    Vector3 desiredVel = trafficSystem.segments[currentTarget.segment].waypoints[currentTarget.waypoint].transform.position - this.transform.position;
                    steering = Mathf.Clamp(this.transform.InverseTransformDirection(desiredVel.normalized).x, -1f, 1f);
                }

            }

            //Move the car
            wheelDrive.Move(acc, steering, brake);
        }

        public GameObject GetDetectedObstacles()
        {
            float hitDist = 0f;
            return GetDetectedObstacles(out hitDist);
        }

        GameObject GetDetectedObstacles(out float _hitDist)
        {
            GameObject detectedObstacle = null;
            float minDist = 1000f;

            float initRay = (raysNumber / 2f) * raySpacing;
            float hitDist = -1f;
            for (float a = -initRay; a <= initRay; a += raySpacing)
            {
                CastRay(raycastAnchor.transform.position, a, this.transform.forward, raycastLength, out detectedObstacle, out hitDist);

                if (detectedObstacle == null) continue;

                float dist = Vector3.Distance(this.transform.position, detectedObstacle.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    break;
                }
            }

            _hitDist = hitDist;
            return detectedObstacle;
        }


        void CastRay(Vector3 _anchor, float _angle, Vector3 _dir, float _length, out GameObject _outObstacle, out float _outHitDistance)
        {
            _outObstacle = null;
            _outHitDistance = -1f;

            //Draw raycast
            Debug.DrawRay(_anchor, Quaternion.Euler(0, _angle, 0) * _dir * _length, new Color(1, 0, 0, 0.5f));

            //Detect hit only on the autonomous vehicle layer
            int layer = 1 << LayerMask.NameToLayer("AutonomousVehicle");
            int finalMask = layer;

            foreach (string layerName in trafficSystem.collisionLayers)
            {
                int id = 1 << LayerMask.NameToLayer(layerName);
                finalMask = finalMask | id;
            }

            RaycastHit hit;
            if (Physics.Raycast(_anchor, Quaternion.Euler(0, _angle, 0) * _dir, out hit, _length, finalMask))
            {
                _outObstacle = hit.collider.gameObject;
                _outHitDistance = hit.distance;
            }
        }

        //Next Segment to go from traffic system
        int GetNextSegmentId()
        {
            // NavigationComponent.UpdateCurrentSegment();
            // if(trafficSystem.segments[currentTarget.segment].nextSegments.Count == 0)
            //     return 0;
            // int c = Random.Range(0, trafficSystem.segments[currentTarget.segment].nextSegments.Count);//segment selection
            // return trafficSystem.segments[currentTarget.segment].nextSegments[c].id;
            return NavigationComponent.GetNextSegmentId();
        }



        // public int GetSegmentVehicleIsIn()
        // {
        //     int vehicleSegment = currentTarget.segment;
        //     bool isOnSegment = trafficSystem.segments[vehicleSegment].IsOnSegment(this.transform.position);
        //     if (!isOnSegment)
        //     {
        //         bool isOnPSegement = trafficSystem.segments[pastTargetSegment].IsOnSegment(this.transform.position);
        //         if (isOnPSegement)
        //             vehicleSegment = pastTargetSegment;
        //     }
        //     return vehicleSegment;
        // }

        public int GetSegmentVehicleIsIn()
        {
            int vehicleSegment = currentTarget.segment;

            if (trafficSystem.segments[vehicleSegment].IsOnSegment(this.transform.position))
                return vehicleSegment;

            if (trafficSystem.segments[pastTargetSegment].IsOnSegment(this.transform.position))
                return pastTargetSegment;

            Debug.LogWarning("get segment return -1");
            // Fallback: Vehicle is not in the expected or past segment.
            // return -1; // Indicates an invalid segment (or handle differently)
            return currentTarget.segment;
        }

        public Target getCurrentTarget()
        {
            return currentTarget;
        }

        public Target getNextTarget()
        {
            return futureTarget;
        }
        public bool VIsOnSegment(Vector3 pos, int segment)
        {
            if (trafficSystem.segments[segment].IsOnSegment(pos))
            {
                return true;
            }
            return false;
        }


    }


}













