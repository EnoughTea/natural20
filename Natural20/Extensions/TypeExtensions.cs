// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Natural20 {
    /// <summary> Extension methods for the <see cref="Type"/>. </summary>
    internal static class TypeExtensions {
        /// <summary> Gets all assignable types for a specified type which are in a given assembly. </summary>
        /// <param name="t">The target type.</param>
        /// <param name="searchTarget">The assembly to search. 
        /// If null, all assemblies in a current domain will be searched.</param>
        /// <returns> Sequence of assignable classes for a specified type. </returns>
        public static IEnumerable<Type> GetAssignableTypes(this Type t, Assembly searchTarget = null) {
            Contract.Requires(t != null);

            var typesToSearch = (searchTarget != null)
                ? searchTarget.GetTypes()
                : AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());

            return typesToSearch.Where(t.IsAssignableFrom);
        }
    }
}