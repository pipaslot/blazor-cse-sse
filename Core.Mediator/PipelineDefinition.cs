using System;

namespace Core.Mediator
{
    internal class PipelineDefinition
    {
        public PipelineDefinition(Type pipelineType, Type? markerType = null)
        {
            PipelineType = pipelineType;
            MarkerType = markerType;
        }

        public Type PipelineType { get;  }
        /// <summary>
        /// Class or interface which needs to be implemented by Request object to be apply the pipeline for
        /// </summary>
        public Type? MarkerType { get;  }
    }
}
