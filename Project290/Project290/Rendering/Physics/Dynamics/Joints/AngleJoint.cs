//////__     __               _                 _     _               _   
//////\ \   / /              | |               | |   | |             | |  
////// \ \_/ /__  _   _   ___| |__   ___  _   _| | __| |  _ __   ___ | |_ 
//////  \   / _ \| | | | / __| '_ \ / _ \| | | | |/ _` | | '_ \ / _ \| __|
//////   | | (_) | |_| | \__ \ | | | (_) | |_| | | (_| | | | | | (_) | |_ 
//////   |_|\___/ \__,_| |___/_| |_|\___/ \__,_|_|\__,_| |_| |_|\___/ \__|
//////      _                              _   _     _     _ 
//////     | |                            | | | |   (_)   | |
//////  ___| |__   __ _ _ __   __ _  ___  | |_| |__  _ ___| |
////// / __| '_ \ / _` | '_ \ / _` |/ _ \ | __| '_ \| / __| |
//////| (__| | | | (_| | | | | (_| |  __/ | |_| | | | \__ \_|
////// \___|_| |_|\__,_|_| |_|\__, |\___|  \__|_| |_|_|___(_)
//////                         __/ |                         
//////                        |___/                          

using System;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints
{
    /// <summary>
    /// Maintains a fixed angle between two bodies
    /// </summary>
    public class AngleJoint : Joint
    {
        public float BiasFactor;
        public float MaxImpulse;
        public float Softness;
        private float _bias;
        private float _jointError;
        private float _massFactor;
        private float _targetAngle;

        public AngleJoint(Body bodyA, Body bodyB)
            : base(bodyA, bodyB)
        {
            JointType = JointType.Angle;
            TargetAngle = 0;
            BiasFactor = .2f;
            Softness = 0f;
            MaxImpulse = float.MaxValue;
        }

        public float TargetAngle
        {
            get { return _targetAngle; }
            set
            {
                if (value != _targetAngle)
                {
                    _targetAngle = value;
                    WakeBodies();
                }
            }
        }

        public override Vector2 WorldAnchorA
        {
            get { return BodyA.Position; }
        }

        public override Vector2 WorldAnchorB
        {
            get { return BodyB.Position; }
        }

        public override Vector2 GetReactionForce(float inv_dt)
        {
            // TODO
            // return _inv_dt * _impulse;
            return Vector2.Zero;
        }

        public override float GetReactionTorque(float inv_dt)
        {
            return 0;
        }

        internal override void InitVelocityConstraints(ref TimeStep step)
        {
            _jointError = (BodyB.Sweep.a - BodyA.Sweep.a - TargetAngle);

            _bias = -BiasFactor * step.inv_dt * _jointError;

            _massFactor = (1 - Softness) / (BodyA.InvI + BodyB.InvI);
        }

        internal override void SolveVelocityConstraints(ref TimeStep step)
        {
            float p = (_bias - BodyB.AngularVelocity + BodyA.AngularVelocity) * _massFactor;
            BodyA.AngularVelocity -= BodyA.InvI * Math.Sign(p) * Math.Min(Math.Abs(p), MaxImpulse);
            BodyB.AngularVelocity += BodyB.InvI * Math.Sign(p) * Math.Min(Math.Abs(p), MaxImpulse);
        }

        internal override bool SolvePositionConstraints()
        {
            // no position solving for this joint
            return true;
        }
    }
}