#if !NETCOREAPP
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics.CodeAnalysis
{
	/// <summary>Specifies that an output may be null even if the corresponding type disallows it.</summary>
	[AttributeUsage (AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
	internal sealed class MaybeNullAttribute : Attribute
	{ }
}
#endif
