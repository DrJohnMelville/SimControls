using System;
using System.Collections.Generic;

namespace SimControls.Model
{
    public interface ISimVariableBinder
    {
        void BindVariableToSimulator<T>(string name, string unit, string simType, ReadOnlyDataItem<T> variable);
        void BindEventToSimulator(SimEventTrigger simEvent);
    }

    public interface IVariableCache
    {
        public TResult GetVariable<T, TResult>(string name, string units, string dataType)
            where TResult : ReadOnlyDataItem<T>, new();

        public SimEventTrigger GetEvent(string simEventName);
    }
    public class VariableCache:IVariableCache
    {
      private readonly Dictionary<string, DataItem> data = new();
      private readonly Dictionary<string, SimEventTrigger> events = new();
      private readonly ISimVariableBinder binder;

      public VariableCache(ISimVariableBinder binder)
      {
          this.binder = binder;
      }

      private T Get<T>(string name, Func<T> create) where T:DataItem =>
          data.GetOrCreate(name, create) as T ??
          throw new InvalidOperationException("Type Mismatch: " + name);

      public TResult GetVariable<T, TResult>(string name, string units, string dataType)
          where TResult : ReadOnlyDataItem<T>, new()
      {
          return Get(name, Create);
          TResult Create()
          {
              var ret = new TResult();
              binder.BindVariableToSimulator<T>(name, units, dataType, ret);
              return ret;
          }
      }

      public SimEventTrigger GetEvent(string simEventName)
      {
          return events.GetOrCreate(simEventName, CreateEvent);

          SimEventTrigger CreateEvent()
          {
              var ret = new SimEventTrigger(simEventName);
              binder.BindEventToSimulator(ret);
              return ret;
          }
      }
    }
}