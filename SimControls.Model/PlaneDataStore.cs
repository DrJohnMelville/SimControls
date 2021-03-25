using System;
using System.Collections.Generic;
using SimControls.Model.VariableBinders;

namespace SimControls.Model
{
    public interface IVariableCache
    {
        public TResult GetVariable<T, TResult>(string name, string units, string dataType, ushort uniqueId)
            where TResult : ReadOnlyDataItem<T>, new();

        public SimEventTrigger GetEvent(string simEventName);
    }
    
    public class VariableCache:IVariableCache
    {
      private readonly Dictionary<int, DataItem> data = new();
      private readonly Dictionary<string, SimEventTrigger> events = new();
      private readonly ISimVariableBinder binder;

      public VariableCache(ISimVariableBinder binder)
      {
          this.binder = binder;
      }

      private T Get<T>(ushort uniqueId, Func<T> create) where T:DataItem =>
          data.GetOrCreate(uniqueId, create) as T ??
          throw new InvalidOperationException("Type Mismatch: " + uniqueId);

      public TResult GetVariable<T, TResult>(string name, string units, string dataType, ushort uniqueId)
          where TResult : ReadOnlyDataItem<T>, new()
      {
          return Get(uniqueId, Create);
          TResult Create()
          {
              var ret = new TResult() {UniqueIndex = uniqueId};
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