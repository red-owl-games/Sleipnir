using System;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.HideMonoScript]
#endif
    public class GraphController : MonoBehaviour
    {
        [Flags]
        public enum Stages
        {
            None = 0b_00000,
            Awake = 0b_00001,
            Start = 0b_00010,
            Update = 0b_00100,
            FixedUpdate = 0b_01000,
            LateUpdate = 0b_10000,
            All = 0b_11111
        }
        
        public GraphAsset asset;
        public Stages stage;
        
        private IFlow _awakeFlow;
        private IFlow _startFlow;
        private IFlow _updateFlow;
        private IFlow _fixedUpdateFlow;
        private IFlow _lateUpdateFlow;
        
        private void Awake()
        {
            _awakeFlow = new Flow<StartNode>(asset.Graph);
            _startFlow = new Flow<StartNode>(asset.Graph);
            _updateFlow = new Flow<UpdateNode>(asset.Graph);
            _fixedUpdateFlow = new Flow<FixedUpdateNode>(asset.Graph);
            _lateUpdateFlow = new Flow<LateUpdateNode>(asset.Graph);
            
            if (stage.HasFlag(Stages.Awake)) _awakeFlow.Execute();
        }

        private void Start()
        {
            if (stage.HasFlag(Stages.Start)) _startFlow.Execute();
        }

        private void Update()
        {
            if (stage.HasFlag(Stages.Update)) _updateFlow.Execute();
        }

        private void FixedUpdate()
        {
            if (stage.HasFlag(Stages.FixedUpdate)) _fixedUpdateFlow.Execute();
        }

        private void LateUpdate()
        {
            if (stage.HasFlag(Stages.LateUpdate)) _lateUpdateFlow.Execute();
        }
    }
}