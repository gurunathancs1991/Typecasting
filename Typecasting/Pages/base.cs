using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Dynamic;

namespace Typecasting
{
    public class Base: ComponentBase
    {
     
        public Boolean isRendered { get; set; }

        public Dictionary<string, EventData> DelegateList = new Dictionary<string, EventData>();
        [JSInvokable]
        public object Trigger(string eventName, string arg)
        {
            // getting the event data based on event event name.
            EventData data = this.DelegateList[eventName];
            // invoke the generic method as synchronous action. so eventcallback doesn't wait.
            // since here, sync action is happening henceforth, data returning back to js end before action done in sample event handler
            return this.InvokeGenericMethod("TriggerHandler", data.ArgumentType, this.GetType(), eventName, arg) as string;
        }
        public object InvokeGenericMethod(string name, Type type, Type parentType, params object[] arguments)
        {
            //collecting the method details.
            MethodInfo method = parentType.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
            // invoke the generic method.
            return method.MakeGenericMethod(type).Invoke(this, arguments);
        }


        internal string TriggerHandler<T>(string eventName, string arg)
        {
            // cast the event method handler function to proper T type
            EventCallback<T> fn = (EventCallback<T>)this.DelegateList[eventName].Handler;

            // Deserialize the event arguments with generic type T
            var Settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            T eventArgs = JsonConvert.DeserializeObject<T>(arg, Settings);

            // Invoke the event handler method
            fn.InvokeAsync(eventArgs);

            // Return the event argument changes to client side
            return JsonConvert.SerializeObject(eventArgs);
        }
        internal virtual void SetEvent<T>(string name, EventCallback<T> eventCallback)
        {
            if (this.DelegateList.ContainsKey(name))
            {
                this.DelegateList[name] = new EventData().Set<T>(eventCallback, typeof(T));
            }
            else
            {
                this.DelegateList.Add(name, new EventData().Set<T>(eventCallback, typeof(T)));
            }
        }

        internal virtual object GetEvent(string name)
        {
            if (this.DelegateList.ContainsKey(name) == false)
            {
                return null;
            }
            return this.DelegateList[name].Handler;
        }

        protected override void OnAfterRender()
        {
            if (!this.isRendered)
            {
                this.dotNetObjectRef = this.CreateDotNetObjectRef<object>(this);
                JSInterop.Init<object>(this.jsRuntime, this.dotNetObjectRef);
                this.isRendered = true;
            }         

        }
        protected DotNetObjectRef<object> dotNetObjectRef { get; set; }
        protected DotNetObjectRef<T> CreateDotNetObjectRef<T>(T value) where T : class
        {
            lock (CreateDotNetObjectRefSyncObj)
            {
                JSRuntime.SetCurrentJSRuntime(this.jsRuntime);
                return DotNetObjectRef.Create(value);
            }
        }
        [Inject]
        internal IJSRuntime jsRuntime { get; set; }
        internal static object CreateDotNetObjectRefSyncObj = new object();

        protected void DisposeDotNetObjectRef<T>(DotNetObjectRef<T> value) where T : class
        {
            if (value != null)
            {
                lock (CreateDotNetObjectRefSyncObj)
                {
                    JSRuntime.SetCurrentJSRuntime(this.jsRuntime);
                    value.Dispose();
                }
            }
        }
    }

    public class EventData
    {
        public object Handler { get; set; }

        public Type ArgumentType { get; set; }

        public EventData Set<T>(EventCallback<T> action, Type type)
        {
            this.Handler = action;
            this.ArgumentType = type;
            return this;
        }
      
    }
    
}
