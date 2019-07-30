# Typecasting
This is the sample to disclose the issue in type casting.

## issue details

1. Having a method as intermediatory between JS and Blazor `EventCallback<T>` method. which helps to cast the argument and send the argument with proper type to user application mapped event handler. 

2. So, we have used `makeGenericMethod` option in Asp.net, to convert the normal method to Generic method with its proper type. but here, possible way is `invoke` not an `invokeAsync` since it is `Action<T>`. 

3. Due to this, we unable to process the data in event handler and return back to JS end appropriately since a sync action is happend intermediately. 

### Question

1. How to cast the `EventCallback<T>` without knowing it T type statically but having it as variable storage and then invoke the function handler. 

2. We used Task run to trigger the Generic invoke method. but error throws from blazor stated that `Current thread is differ than ui-rendered thread` from `stateHasChanged` method. 


### Expected

1. I expect the textbox value to be changed when a change event triggered from textbox as `this is after async method call` but it is as `this is before async method call`.


