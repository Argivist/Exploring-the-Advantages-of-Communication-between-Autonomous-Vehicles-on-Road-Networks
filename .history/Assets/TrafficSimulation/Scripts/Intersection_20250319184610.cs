// Traffic Simulation
// https://github.com/mchrbn/unity-traffic-simulation

using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;
[System.Serializable]

public class VehicleDebug{

            public string vehicleId;
            public GameObject vehicle;
            public Status_ status;
            public bool isInQueue;
            public bool isAlreadyInIntersection;

            public bool isPriority;
            public int segmentVehicleIsIn;


            public int nextSegment;

            public VehicleDebug(GameObject _vehicle, Status_ _status, bool _isAlreadyInIntersection,bool isInQueue,bool priority, int _segmentVehicleIsIn, int _nextSegment){
                vehicle = _vehicle;
                status = _status;
                isAlreadyInIntersection = _isAlreadyInIntersection;
                segmentVehicleIsIn = _segmentVehicleIsIn;
                nextSegment = _nextSegment;
                this.isInQueue = isInQueue;
                isPriority = priority;
                vehicleId = _vehicle.name;
            }

        }

namespace TrafficSimulation{
    public enum IntersectionType{
        STOP,
        TRAFFIC_LIGHT
    }

    public class Intersection : MonoBehaviour
    {   
        public IntersectionType intersectionType;
        public int id;  

        //For stop only
        public List<Segment> prioritySegments;

        //For traffic lights only
        public float lightsDuration = 8;
        public float orangeLightDuration = 2;
        public List<Segment> lightsNbr1;
        public List<Segment> lightsNbr2;
        public List<VehicleDebug> vehiclesList= new List<VehicleDebug>(); 
    


        



        private List<GameObject> vehiclesQueue;
        private List<GameObject> vehiclesInIntersection;
        private TrafficSystem trafficSystem;
        
        [HideInInspector] public int currentRedLightsGroup = 1;

        // [Header("Debug")]
        // [SerializeReference]
        // public List<VehicleDebug> vehiclesList= new List<VehicleDebug>(); 


        void Start(){
            gameObject.AddComponent<IntersectionDebug>();
            vehiclesList = new List<VehicleDebug>();
            vehiclesQueue = new List<GameObject>();
            vehiclesInIntersection = new List<GameObject>();
            if(intersectionType == IntersectionType.TRAFFIC_LIGHT)
                InvokeRepeating("SwitchLights", lightsDuration, lightsDuration);
        }

        void Update(){
            //if the vehicle inn the current segment is missing a gameobjet, reomove from the list by calling the exit functio
            foreach(GameObject vehicle in vehiclesInIntersection){
                if(vehicle == null){
                    ExitStop(vehicle);
                    //
                }
            }
        }
        void SwitchLights(){

            if(currentRedLightsGroup == 1) currentRedLightsGroup = 2;
            else if(currentRedLightsGroup == 2) currentRedLightsGroup = 1;            
            
            //Wait few seconds after light transition before making the other car move (= orange light)
            Invoke("MoveVehiclesQueue", orangeLightDuration);
        }

        void OnTriggerEnter(Collider _other) {
            //Check if vehicle is already in the list if yes abort
            //Also abort if we just started the scene (if vehicles inside colliders at start)
            if(IsAlreadyInIntersection(_other.gameObject) || Time.timeSinceLevelLoad < .5f) return;

            if(_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.STOP)
                TriggerStop(_other.gameObject);
            else if(_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.TRAFFIC_LIGHT)
                TriggerLight(_other.gameObject);
        }

        void OnTriggerExit(Collider _other) {
            if(_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.STOP)
                ExitStop(_other.gameObject);
            else if(_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.TRAFFIC_LIGHT)
                ExitLight(_other.gameObject);
        }


        // TriggerStop: When a vehicle enters the intersection, it will be stopped if there are other vehicles in the intersection or in the queue
        void TriggerStop(GameObject _vehicle){
            Vehicle_AI Vehicle_AI = _vehicle.GetComponent<Vehicle_AI>();
            
            //Depending on the waypoint threshold, the car can be either on the target segment or on the past segment
            int vehicleSegment = Vehicle_AI.GetSegmentVehicleIsIn();

            if(!IsPrioritySegment(vehicleSegment)){
                if(vehiclesQueue.Count > 0 || vehiclesInIntersection.Count > 0){
                    Vehicle_AI.vehicleStatus = Status_.STOP;
                    vehiclesQueue.Add(_vehicle);
                    // Debug
                    vehiclesList.Add(new VehicleDebug(_vehicle, Status_.STOP, false,true,false, vehicleSegment, Vehicle_AI.futureSegment));
                    
                }
                else{
                    vehiclesInIntersection.Add(_vehicle);
                    Vehicle_AI.vehicleStatus = Status_.SLOW_DOWN;
                    vehiclesList.Add(new VehicleDebug(_vehicle, Status_.STOP, true, false,false,vehicleSegment, Vehicle_AI.futureSegment));
                }
            }
            else{
                Vehicle_AI.vehicleStatus = Status_.SLOW_DOWN;
                vehiclesInIntersection.Add(_vehicle);
                vehiclesList.Add(new VehicleDebug(_vehicle, Status_.STOP, true, false,true,vehicleSegment, Vehicle_AI.futureSegment));
            }
        }

        // ExitStop: When a vehicle exits the intersection, it will be set to GO and removed from the intersection list
        void ExitStop(GameObject _vehicle){
            try{
            _vehicle.GetComponent<Vehicle_AI>().vehicleStatus = Status_.GO;
            vehiclesInIntersection.Remove(_vehicle);
            }catch{
                // _vehicle.GetComponent<Vehicle_AI>().vehicleStatus = Status_.GO;
                Debug.LogError("Vehicle  destroyed before being able to set status to GO");
                Debug.LogWarning("Vehicle destroyed before being able to set status to GO");
            }
            try{
            
            }catch{
                Debug.LogError("Vehicle  destroyed before being able to remove from intersection");
                Debug.LogWarning("Vehicle destroyed before being able to remove from intersection");
        
            }
            try{
            vehiclesQueue.Remove(_vehicle);
            }catch{
                Debug.LogError("Vehicle  destroyed before being able to remove from queue");
                Debug.LogWarning("Vehicle destroyed before being able to remove from queue");
            }
            try{
            vehiclesList.Remove(vehiclesList.Find(x => x.vehicle == _vehicle));
            }catch{
                Debug.LogError("Vehicle  destroyed before being able to remove from list");
                Debug.LogWarning("Vehicle destroyed before being able to remove from list");
            }

            if(vehiclesQueue.Count > 0 && vehiclesInIntersection.Count == 0){
                while(vehiclesQueue[0]==null){
                    vehiclesQueue.RemoveAt(0);
                }
                vehiclesQueue[0].GetComponent<Vehicle_AI>().vehicleStatus = Status_.GO;
            }
        }

        void TriggerLight(GameObject _vehicle){
            Vehicle_AI Vehicle_AI = _vehicle.GetComponent<Vehicle_AI>();
            int vehicleSegment = Vehicle_AI.GetSegmentVehicleIsIn();

            if(IsRedLightSegment(vehicleSegment)){
                Vehicle_AI.vehicleStatus = Status_.STOP;
                vehiclesQueue.Add(_vehicle);
            }
            else{
                Vehicle_AI.vehicleStatus = Status_.GO;
            }
        }

        void ExitLight(GameObject _vehicle){
            _vehicle.GetComponent<Vehicle_AI>().vehicleStatus = Status_.GO;
        }

        bool IsRedLightSegment(int _vehicleSegment){
            if(currentRedLightsGroup == 1){
                foreach(Segment segment in lightsNbr1){
                    if(segment.id == _vehicleSegment)
                        return true;
                }
            }
            else{
                foreach(Segment segment in lightsNbr2){
                    if(segment.id == _vehicleSegment)
                        return true;
                }
            }
            return false;
        }

        void MoveVehiclesQueue(){
            //Move all vehicles in queue
            List<GameObject> nVehiclesQueue = new List<GameObject>(vehiclesQueue);
            foreach(GameObject vehicle in vehiclesQueue){
                int vehicleSegment = vehicle.GetComponent<Vehicle_AI>().GetSegmentVehicleIsIn();
                if(!IsRedLightSegment(vehicleSegment)){
                    vehicle.GetComponent<Vehicle_AI>().vehicleStatus = Status_.GO;
                    nVehiclesQueue.Remove(vehicle);
                }
            }
            vehiclesQueue = nVehiclesQueue;
        }

        bool IsPrioritySegment(int _vehicleSegment){
            foreach(Segment s in prioritySegments){
                if(_vehicleSegment == s.id)
                    return true;
            }
            return false;
        }

        bool IsAlreadyInIntersection(GameObject _target){
            foreach(GameObject vehicle in vehiclesInIntersection){
                if(vehicle.GetInstanceID() == _target.GetInstanceID()) return true;
            }
            foreach(GameObject vehicle in vehiclesQueue){
                if(vehicle.GetInstanceID() == _target.GetInstanceID()) return true;
            }

            return false;
        } 


        private List<GameObject> memVehiclesQueue = new List<GameObject>();
        private List<GameObject> memVehiclesInIntersection = new List<GameObject>();

        public void SaveIntersectionStatus(){
            memVehiclesQueue = vehiclesQueue;
            memVehiclesInIntersection = vehiclesInIntersection;
        }

        public void ResumeIntersectionStatus(){
            foreach(GameObject v in vehiclesInIntersection){
                foreach(GameObject v2 in memVehiclesInIntersection){
                    if(v.GetInstanceID() == v2.GetInstanceID()){
                        v.GetComponent<Vehicle_AI>().vehicleStatus = v2.GetComponent<Vehicle_AI>().vehicleStatus;
                        break;
                    }
                }
            }
            foreach(GameObject v in vehiclesQueue){
                foreach(GameObject v2 in memVehiclesQueue){
                    if(v.GetInstanceID() == v2.GetInstanceID()){
                        v.GetComponent<Vehicle_AI>().vehicleStatus = v2.GetComponent<Vehicle_AI>().vehicleStatus;
                        break;
                    }
                }
            }
        }
    }
}

// // Traffic Simulation
// // https://github.com/mchrbn/unity-traffic-simulation

// using System.Collections.Generic;
// using UnityEngine;

// namespace TrafficSimulation{
//     public enum IntersectionType{
//         STOP,
//         TRAFFIC_LIGHT
//     }

//     public class Intersection : MonoBehaviour
//     {   
//         bool _vai=true;
//         public IntersectionType intersectionType;
//         public int id;  

//         //For stop only
//         public List<Segment> prioritySegments;

//         //For traffic lights only
//         public float lightsDuration = 8;
//         public float orangeLightDuration = 2;
//         public List<Segment> lightsNbr1;
//         public List<Segment> lightsNbr2;

//         private List<GameObject> vehiclesQueue;
//         private List<GameObject> vehiclesInIntersection;
//         private TrafficSystem trafficSystem;
        
//         [HideInInspector] public int currentRedLightsGroup = 1;

//         void Start(){
//             vehiclesQueue = new List<GameObject>();
//             vehiclesInIntersection = new List<GameObject>();
//             if(intersectionType == IntersectionType.TRAFFIC_LIGHT)
//                 InvokeRepeating("SwitchLights", lightsDuration, lightsDuration);
//         }

//         void SwitchLights(){

//             if(currentRedLightsGroup == 1) currentRedLightsGroup = 2;
//             else if(currentRedLightsGroup == 2) currentRedLightsGroup = 1;            
            
//             //Wait few seconds after light transition before making the other car move (= orange light)
//             Invoke("MoveVehiclesQueue", orangeLightDuration);
//         }

//         void OnTriggerEnter(Collider _other) {
//             //Check if vehicle is already in the list if yes abort
//             //Also abort if we just started the scene (if vehicles inside colliders at start)
//             if(IsAlreadyInIntersection(_other.gameObject) || Time.timeSinceLevelLoad < .5f) return;

//             if(_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.STOP)
//                 TriggerStop(_other.gameObject);
//             else if(_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.TRAFFIC_LIGHT)
//                 TriggerLight(_other.gameObject);
//         }

//         void OnTriggerExit(Collider _other) {
//             if(_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.STOP)
//                 ExitStop(_other.gameObject);
//             else if(_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.TRAFFIC_LIGHT)
//                 ExitLight(_other.gameObject);
//         }

//         void TriggerStop(GameObject _vehicle){
//             if(_vehicle==null) return;
//             Vehicle_AI Vehicle_AI = _vehicle.GetComponent<Vehicle_AI>();
//             Vehicle_AI vehicle_AI = _vehicle.GetComponent<Vehicle_AI>();
//             int vehicleSegment;
//             //Depending on the waypoint threshold, the car can be either on the target segment or on the past segment
//             if(_vai){
//             vehicleSegment = vehicle_AI.GetSegmentVehicleIsIn();
//             }else{
//             vehicleSegment = Vehicle_AI.GetSegmentVehicleIsIn();
//             }
//             if(!IsPrioritySegment(vehicleSegment)){
//                 if(vehiclesQueue.Count > 0 || vehiclesInIntersection.Count > 0){
//                     Vehicle_AI.vehicleStatus = Status_.STOP;
//                     vehicle_AI.vehicleStatus = Status__.STOP;
//                     vehiclesQueue.Add(_vehicle);
//                 }
//                 else{
//                     vehiclesInIntersection.Add(_vehicle);
//                     Vehicle_AI.vehicleStatus = Status_.SLOW_DOWN;
//                     vehicle_AI.vehicleStatus = Status__.SLOW_DOWN;
//                 }
//             }
//             else{
//                 Vehicle_AI.vehicleStatus = Status_.SLOW_DOWN;
//                 vehicle_AI.vehicleStatus = Status__.SLOW_DOWN;
//                 vehiclesInIntersection.Add(_vehicle);
//             }
//         }

//         void ExitStop(GameObject _vehicle){
//                     if(_vehicle==null) {
//                 return;
//                     }

//             _vehicle.GetComponent<Vehicle_AI>().vehicleStatus = Status_.GO;
//             _vehicle.GetComponent<Vehicle_AI>().vehicleStatus = Status__.GO;
//             vehiclesInIntersection.Remove(_vehicle);
//             vehiclesQueue.Remove(_vehicle);

//             if(vehiclesQueue.Count > 0 && vehiclesInIntersection.Count == 0){
//                 vehiclesQueue[0].GetComponent<Vehicle_AI>().vehicleStatus = Status_.GO;
//                 vehiclesQueue[0].GetComponent<Vehicle_AI>().vehicleStatus = Status__.GO;
//             }
//         }

//         void TriggerLight(GameObject _vehicle){
//                     if(_vehicle==null) return;
//             Vehicle_AI Vehicle_AI = _vehicle.GetComponent<Vehicle_AI>();
//             Vehicle_AI vehicle_AI = _vehicle.GetComponent<Vehicle_AI>();
//             int vehicleSegment;
//             if(_vai){
//             vehicleSegment = vehicle_AI.GetSegmentVehicleIsIn();
//             }else{
//             vehicleSegment = Vehicle_AI.GetSegmentVehicleIsIn();
//             }

//             if(IsRedLightSegment(vehicleSegment)){
//                 Vehicle_AI.vehicleStatus = Status_.STOP;
//                 vehicle_AI.vehicleStatus = Status__.STOP;
//                 vehiclesQueue.Add(_vehicle);
//             }
//             else{
//                 Vehicle_AI.vehicleStatus = Status_.GO;
//                 vehicle_AI.vehicleStatus = Status__.GO;
//             }
//         }

//         void ExitLight(GameObject _vehicle){
//                     if(_vehicle==null) return;
//             _vehicle.GetComponent<Vehicle_AI>().vehicleStatus = Status_.GO;
//             _vehicle.GetComponent<Vehicle_AI>().vehicleStatus = Status__.GO;
//         }

//         bool IsRedLightSegment(int _vehicleSegment){
//             if(currentRedLightsGroup == 1){
//                 foreach(Segment segment in lightsNbr1){
//                     if(segment.id == _vehicleSegment)
//                         return true;
//                 }
//             }
//             else{
//                 foreach(Segment segment in lightsNbr2){
//                     if(segment.id == _vehicleSegment)
//                         return true;
//                 }
//             }
//             return false;
//         }

//         void MoveVehiclesQueue(){

//             //Move all vehicles in queue
//             List<GameObject> nVehiclesQueue = new List<GameObject>(vehiclesQueue);
//             foreach(GameObject vehicle in vehiclesQueue){
//                 if (vehicle == null){
//                     nVehiclesQueue.Remove(vehicle);
//                     continue;
//                 }
//                 int vehicleSegment;
//                 if(_vai){
//                 vehicleSegment = vehicle.GetComponent<Vehicle_AI>().GetSegmentVehicleIsIn();
//                 }else{
//                 vehicleSegment = vehicle.GetComponent<Vehicle_AI>().GetSegmentVehicleIsIn();
//                 }
//                 if(!IsRedLightSegment(vehicleSegment)){
//                     vehicle.GetComponent<Vehicle_AI>().vehicleStatus = Status_.GO;
//                     vehicle.GetComponent<Vehicle_AI>().vehicleStatus = Status__.GO;
//                     nVehiclesQueue.Remove(vehicle);
//                 }
//             }
//             vehiclesQueue = nVehiclesQueue;
//         }

//         bool IsPrioritySegment(int _vehicleSegment){
//             foreach(Segment s in prioritySegments){
//                 if(_vehicleSegment == s.id)
//                     return true;
//             }
//             return false;
//         }

//         bool IsAlreadyInIntersection(GameObject _target){
//             foreach(GameObject vehicle in vehiclesInIntersection){
//                 if(vehicle.GetInstanceID() == _target.GetInstanceID()) return true;
//             }
//             foreach(GameObject vehicle in vehiclesQueue){
//                 if(vehicle.GetInstanceID() == _target.GetInstanceID()) return true;
//             }

//             return false;
//         } 


//         private List<GameObject> memVehiclesQueue = new List<GameObject>();
//         private List<GameObject> memVehiclesInIntersection = new List<GameObject>();

//         public void SaveIntersectionStatus_(){
//             memVehiclesQueue = vehiclesQueue;
//             memVehiclesInIntersection = vehiclesInIntersection;
//         }

//         public void ResumeIntersectionStatus_(){
//             foreach(GameObject v in vehiclesInIntersection){
//                 foreach(GameObject v2 in memVehiclesInIntersection){
//                     if(v.GetInstanceID() == v2.GetInstanceID()){
//                         v.GetComponent<Vehicle_AI>().vehicleStatus = v2.GetComponent<Vehicle_AI>().vehicleStatus;
//                         v.GetComponent<Vehicle_AI>().vehicleStatus = v2.GetComponent<Vehicle_AI>().vehicleStatus;
//                         break;
//                     }
//                 }
//             }
//             foreach(GameObject v in vehiclesQueue){
//                 foreach(GameObject v2 in memVehiclesQueue){
//                     if(v.GetInstanceID() == v2.GetInstanceID()){
//                         v.GetComponent<Vehicle_AI>().vehicleStatus = v2.GetComponent<Vehicle_AI>().vehicleStatus;
//                         v.GetComponent<Vehicle_AI>().vehicleStatus = v2.GetComponent<Vehicle_AI>().vehicleStatus;
//                         break;
//                     }
//                 }
//             }
//         }
//     }
// }
