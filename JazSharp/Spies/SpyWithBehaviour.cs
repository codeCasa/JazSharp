﻿using JazSharp.SpyLogic.Behaviour;
using System;

namespace JazSharp.Spies
{
    public class SpyWithBehaviour : ISpy
    {
        private readonly Spy _spy;
        private readonly SpyBehaviourBase _behaviour;

        /// <summary>
        /// A transitionary property used before specifying additional behaviours for the spy.
        /// </summary>
        public SpyThen Then => new SpyThen(_spy);

        Spy ISpy.Spy => _spy;

        internal SpyWithBehaviour(Spy spy, SpyBehaviourBase behaviour)
        {
            _spy = spy;
            _behaviour = behaviour;
        }

        /// <summary>
        /// Configures the spy to modify the parameter values for ref or out parameters after
        /// after the main logic of the spy is complete. Use this to specify return values for
        /// out and ref parameters.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to set.</param>
        /// <param name="value">The value to update the parameter to.</param>
        /// <returns>The current spy configuration object.</returns>
        public SpyWithBehaviour ThenChangeParameter(string parameterName, object value)
        {
            var parameters = _spy.Method.GetParameters();

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                if (parameter.Name == parameterName)
                {
                    if (!parameter.ParameterType.IsByRef)
                    {
                        throw new InvalidOperationException(
                            $"Changing a parameter's value after spy execution only makes sense for ref or out parameters. " +
                            $"If you intended to change a parameter value before call-through, call {nameof(ThenChangeParameter)} " +
                            $"immediately after {nameof(Spy.And)}.");
                    }

                    _behaviour.ParameterChangesAfterExecution.Add(i, value);
                    return this;
                }
            }

            throw new ArgumentException("parameterName does not match a parameter on the method.");
        }

        public SpyWithQuantifiedBehaviour Once()
        {
            _behaviour.UpdateLifetime(1);
            return new SpyWithQuantifiedBehaviour(_spy);
        }

        public SpyWithQuantifiedBehaviour Twice()
        {
            _behaviour.UpdateLifetime(2);
            return new SpyWithQuantifiedBehaviour(_spy);
        }

        public SpyWithQuantifiedBehaviour Times(int repetitions)
        {
            _behaviour.UpdateLifetime(repetitions);
            return new SpyWithQuantifiedBehaviour(_spy);
        }

        public SpyWithQuantifiedBehaviour Forever()
        {
            _behaviour.UpdateLifetime(int.MaxValue);
            return new SpyWithQuantifiedBehaviour(_spy);
        }
    }
}
