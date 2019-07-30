using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Typecasting.Pages;

namespace Typecasting
{
    public static class JSInterop
    {
        public static Task<T> Init<T>(IJSRuntime jsRuntime, DotNetObjectRef<object> helper)
        {
            
                // Implemented in exampleJsInterop.js
                return jsRuntime.InvokeAsync<T>(
                    "comp.init",
                     helper);
                     
        }
    }
}
