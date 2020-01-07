using System;
using XRTK.Providers.Controllers.Simulation.Hands;

namespace XRTK.Extensions
{
    public static class SimulatedHandControllerPoseExtensions
    {
        /// <summary>
        /// Copy data from another simulated hand pose.
        /// </summary>
        public static void Copy(this SimulatedHandControllerPose pose, SimulatedHandControllerPose other)
        {
            Array.Copy(other.LocalJointPoses, pose.LocalJointPoses, SimulatedHandControllerPose.JointCount);
        }
    }
}