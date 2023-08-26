# InTheHand.DependencyInjection

A self-contained implementation of the Xamarin.Forms DependencyService for all .NET 6.0 (and later) platforms. 
Usage is the same as in Xamarin forms with the exception that DependencyAttribute is not currently used to register types automatically.

[Xamarin.Forms DependencyService Introduction - Microsoft Learn](https://learn.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/dependency-service/introduction)

## Usage

Register a type with `DependencyService.Register<YourType>();` or with an interface and a specific implementation using `DependencyService.Register<IYourInterface, YourType>();`

Retrieve an instance with `DependencyService.Get<IYourInterface>();` using the interface or class type your registered.
Optionally specify whether to return a global instance or a new instance using the fetchTarget argument. e.g. for a new instance:
`var newInstance = DependencyService.Get<IYourInterface>(DependencyFetchTarget.NewInstance);`

This class extends the DependencyService with support for parameterized constructors. 
For example when registering a type `MyType` with a default constructor of the form `MyType(IService1 service1, IService2 service2)` 
the DependencyService will look up those types in the container 
and instantiate these using either a singleton or a new instance depending on how they were registered and inject them into the constructor.
The types referenced must have been registered with DependencyService prior to the call to Get which references them.