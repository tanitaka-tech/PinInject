using System;
using System.Collections;
using System.Collections.Generic;

namespace Cathei.PinInject
{
    public class InjectContainer
    {
        // type -> constructor
        public Dictionary<Type, Func<object>> _builders = new Dictionary<Type, Func<object>>();

        // type -> instance
        public Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        // direct parent to current container
        private InjectContainer _parent;

        internal void SetParent(InjectContainer parent)
        {
            _parent = parent;
        }

        internal void Reset()
        {
            _parent = null;
            _builders.Clear();
            _instances.Clear();
        }

        internal T Reslove<T>()
        {
            Type type = typeof(T);

            if (_instances.TryGetValue(type, out var instance))
                return (T)instance;

            if (_builders.TryGetValue(type, out var builder))
            {
                instance = builder();
                _instances.Add(type, instance);
                return (T)instance;
            }

            if (_parent == null)
                throw new InjectException($"Type {type} cannot be resolved");

            // tail call would be optimized
            return _parent.Reslove<T>();
        }

        public void Bind<T>() where T : new()
        {
            _builders.Add(typeof(T), () => new T());
        }

        public void Bind<T>(T instance)
        {
            _instances.Add(typeof(T), instance);
        }

        public void Bind<T, TImpl>() where TImpl : T, new()
        {
            _builders.Add(typeof(T), () => new TImpl());
        }
    }
}