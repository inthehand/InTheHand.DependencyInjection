//-----------------------------------------------------------------------
// <copyright file="DependencyFetchTarget.cs" company="In The Hand Ltd">
//   Copyright (c) 2023 In The Hand Ltd, All rights reserved.
//   This source code is licensed under the MIT License
// </copyright>
//-----------------------------------------------------------------------

namespace InTheHand.DependencyInjection
{
    /// <summary>
    /// Enumeration specifying whether <see cref="DependencyService.Get{T}(DependencyFetchTarget)"/> should return a reference to a global or new instance.
    /// </summary>
    /// <remarks>
    /// The following example shows how DependencyFetchTarget can be used to specify a new instance:
    /// <code lang="C#">
    /// var secondFetch = DependencyService.Get<IDependencyTest> (DependencyFetchTarget.NewInstance);
    /// </code>
    /// </remarks>
    public enum DependencyFetchTarget
    {
        /// <summary>
        /// Return a global instance.
        /// </summary>
        GlobalInstance,
        /// <summary>
        /// Return a new instance.
        /// </summary>
        NewInstance
    }
}