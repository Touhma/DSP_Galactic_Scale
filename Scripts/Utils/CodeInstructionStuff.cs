using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    /// As we cannot update Harmony, the following code is borrowed
    /// https://github.com/pardeike/Harmony/blame/master/Harmony/Public/CodeInstruction.cs
    /// 
    ///  MIT License

    //  Copyright (c) 2017 Andreas Pardeike
    //
    //  Permission is hereby granted, free of charge, to any person obtaining a copy
    // 	of this software and associated documentation files (the "Software"), to deal
    // 	in the Software without restriction, including without limitation the rights
    //  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    //  copies of the Software, and to permit persons to whom the Software is
    // 	furnished to do so, subject to the following conditions:
    //
    //  The above copyright notice and this permission notice shall be included in all
    // 	copies or substantial portions of the Software.
    //
    // 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    // 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    // 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    // 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    // 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    //  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    //  SOFTWARE.
    public static partial class Utils
    {
        // --- CALLING

        /// <summary>Creates a CodeInstruction calling a method (CALL)</summary>
        /// <param name="type">The class/type where the method is declared</param>
        /// <param name="name">The name of the method (case sensitive)</param>
        /// <param name="parameters">Optional parameters to target a specific overload of the method</param>
        /// <param name="generics">Optional list of types that define the generic version of the method</param>
        /// <returns>A code instruction that calls the method matching the arguments</returns>
        ///
        public static CodeInstruction Call(Type type, string name, Type[] parameters = null, Type[] generics = null)
        {
            var method = AccessTools.Method(type, name, parameters, generics);
            if (method is null)
                throw new ArgumentException(
                    $"No method found for type={type}, name={name}, parameters={parameters.Description()}, generics={generics.Description()}");
            return new CodeInstruction(OpCodes.Call, method);
        }

        /// <summary>Creates a CodeInstruction calling a method (CALL)</summary>
        /// <param name="typeColonMethodname">The target method in the form <c>TypeFullName:MethodName</c>, where the type name matches a form recognized by <a href="https://docs.microsoft.com/en-us/dotnet/api/system.type.gettype">Type.GetType</a> like <c>Some.Namespace.Type</c>.</param>
        /// <param name="parameters">Optional parameters to target a specific overload of the method</param>
        /// <param name="generics">Optional list of types that define the generic version of the method</param>
        /// <returns>A code instruction that calls the method matching the arguments</returns>
        ///
        public static CodeInstruction Call(string typeColonMethodname, Type[] parameters = null, Type[] generics = null)
        {
            var method = AccessTools.Method(typeColonMethodname, parameters, generics);
            if (method is null)
                throw new ArgumentException(
                    $"No method found for {typeColonMethodname}, parameters={parameters.Description()}, generics={generics.Description()}");
            return new CodeInstruction(OpCodes.Call, method);
        }

        /// <summary>Creates a CodeInstruction calling a method (CALL)</summary>
        /// <param name="expression">The lambda expression using the method</param>
        /// <returns></returns>
        ///
        public static CodeInstruction Call(Expression<Action> expression)
        {
            return new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(expression));
        }

        /// <summary>Creates a CodeInstruction calling a method (CALL)</summary>
        /// <param name="expression">The lambda expression using the method</param>
        /// <returns></returns>
        ///
        public static CodeInstruction Call<T>(Expression<Action<T>> expression)
        {
            return new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(expression));
        }

        /// <summary>Creates a CodeInstruction calling a method (CALL)</summary>
        /// <param name="expression">The lambda expression using the method</param>
        /// <returns></returns>
        ///
        public static CodeInstruction Call<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            return new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(expression));
        }

        /// <summary>Creates a CodeInstruction calling a method (CALL)</summary>
        /// <param name="expression">The lambda expression using the method</param>
        /// <returns></returns>
        ///
        public static CodeInstruction Call(LambdaExpression expression)
        {
            return new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(expression));
        }


        // --- FIELDS

        /// <summary>Creates a CodeInstruction loading a field (LD[S]FLD[A])</summary>
        /// <param name="type">The class/type where the field is defined</param>
        /// <param name="name">The name of the field (case sensitive)</param>
        /// <param name="useAddress">Use address of field</param>
        /// <returns></returns>
        public static CodeInstruction LoadField(Type type, string name, bool useAddress = false)
        {
            var field = AccessTools.Field(type, name);
            if (field is null) throw new ArgumentException($"No field found for {type} and {name}");
            return new CodeInstruction(
                useAddress
                    ? (field.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda)
                    : (field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld), field);
        }

        /// <summary>Creates a CodeInstruction storing to a field (ST[S]FLD)</summary>
        /// <param name="type">The class/type where the field is defined</param>
        /// <param name="name">The name of the field (case sensitive)</param>
        /// <returns></returns>
        public static CodeInstruction StoreField(Type type, string name)
        {
            var field = AccessTools.Field(type, name);
            if (field is null) throw new ArgumentException($"No field found for {type} and {name}");
            return new CodeInstruction(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field);
        }

        // --- LOCALS

        /// <summary>Creates a CodeInstruction loading a local with the given index, using the shorter forms when possible</summary>
        /// <param name="index">The index where the local is stored</param>
        /// <param name="useAddress">Use address of local</param>
        /// <returns></returns>
        /// <seealso cref="CodeInstructionExtensions.LocalIndex(CodeInstruction)"/>
        public static CodeInstruction LoadLocal(int index, bool useAddress = false)
        {
            if (useAddress)
            {
                if (index < 256) return new CodeInstruction(OpCodes.Ldloca_S, Convert.ToByte(index));
                else return new CodeInstruction(OpCodes.Ldloca, index);
            }
            else
            {
                if (index == 0) return new CodeInstruction(OpCodes.Ldloc_0);
                else if (index == 1) return new CodeInstruction(OpCodes.Ldloc_1);
                else if (index == 2) return new CodeInstruction(OpCodes.Ldloc_2);
                else if (index == 3) return new CodeInstruction(OpCodes.Ldloc_3);
                else if (index < 256) return new CodeInstruction(OpCodes.Ldloc_S, Convert.ToByte(index));
                else return new CodeInstruction(OpCodes.Ldloc, index);
            }
        }

        /// <summary>Creates a CodeInstruction storing to a local with the given index, using the shorter forms when possible</summary>
        /// <param name="index">The index where the local is stored</param>
        /// <returns></returns>
        /// <seealso cref="CodeInstructionExtensions.LocalIndex(CodeInstruction)"/>
        public static CodeInstruction StoreLocal(int index)
        {
            if (index == 0) return new CodeInstruction(OpCodes.Stloc_0);
            else if (index == 1) return new CodeInstruction(OpCodes.Stloc_1);
            else if (index == 2) return new CodeInstruction(OpCodes.Stloc_2);
            else if (index == 3) return new CodeInstruction(OpCodes.Stloc_3);
            else if (index < 256) return new CodeInstruction(OpCodes.Stloc_S, Convert.ToByte(index));
            else return new CodeInstruction(OpCodes.Stloc, index);
        }

        // --- ARGUMENTS

        /// <summary>Creates a CodeInstruction loading an argument with the given index, using the shorter forms when possible</summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="useAddress">Use address of argument</param>
        /// <returns></returns>
        /// <seealso cref="CodeInstructionExtensions.ArgumentIndex(CodeInstruction)"/>
        public static CodeInstruction LoadArgument(int index, bool useAddress = false)
        {
            if (useAddress)
            {
                if (index < 256) return new CodeInstruction(OpCodes.Ldarga_S, Convert.ToByte(index));
                else return new CodeInstruction(OpCodes.Ldarga, index);
            }
            else
            {
                if (index == 0) return new CodeInstruction(OpCodes.Ldarg_0);
                else if (index == 1) return new CodeInstruction(OpCodes.Ldarg_1);
                else if (index == 2) return new CodeInstruction(OpCodes.Ldarg_2);
                else if (index == 3) return new CodeInstruction(OpCodes.Ldarg_3);
                else if (index < 256) return new CodeInstruction(OpCodes.Ldarg_S, Convert.ToByte(index));
                else return new CodeInstruction(OpCodes.Ldarg, index);
            }
        }

        /// <summary>Creates a CodeInstruction storing to an argument with the given index, using the shorter forms when possible</summary>
        /// <param name="index">The index of the argument</param>
        /// <returns></returns>
        /// <seealso cref="CodeInstructionExtensions.ArgumentIndex(CodeInstruction)"/>
        public static CodeInstruction StoreArgument(int index)
        {
            if (index < 256) return new CodeInstruction(OpCodes.Starg_S, Convert.ToByte(index));
            else return new CodeInstruction(OpCodes.Starg, index);
        }



    }
}