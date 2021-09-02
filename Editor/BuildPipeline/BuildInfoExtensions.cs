﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XRTK.Editor.BuildPipeline
{
    public static class BuildInfoExtensions
    {
        /// <summary>
        /// Append symbols to the end of the <see cref="IBuildInfo"/>'s<see cref="IBuildInfo.BuildSymbols"/>.
        /// </summary>
        /// <param name="buildInfo"></param>
        /// <param name="symbols">The string array to append.</param>
        public static void AppendSymbols(this IBuildInfo buildInfo, params string[] symbols)
        {
            buildInfo.AppendSymbols((IEnumerable<string>)symbols);
        }

        /// <summary>
        /// Append symbols to the end of the <see cref="IBuildInfo"/>'s <see cref="IBuildInfo.BuildSymbols"/>.
        /// </summary>
        /// <param name="buildInfo"></param>
        /// <param name="symbols">The string collection to append.</param>
        public static void AppendSymbols(this IBuildInfo buildInfo, IEnumerable<string> symbols)
        {
            var toAdd = symbols
                .Where(symbol => !string.IsNullOrEmpty(symbol))
                .Except(buildInfo.BuildSymbols.Split(';'))
                .ToArray();

            if (!toAdd.Any())
            {
                return;
            }

            if (!string.IsNullOrEmpty(buildInfo.BuildSymbols))
            {
                buildInfo.BuildSymbols += ";";
            }

            var newSymbols = string.Join(";", toAdd);

            buildInfo.BuildSymbols += newSymbols;
        }

        /// <summary>
        /// Remove symbols from the <see cref="IBuildInfo"/>'s <see cref="IBuildInfo.BuildSymbols"/>.
        /// </summary>
        /// <param name="buildInfo"></param>
        /// <param name="symbolsToRemove">The string collection to remove.</param>
        public static void RemoveSymbols(this IBuildInfo buildInfo, IEnumerable<string> symbolsToRemove)
        {
            var toKeep = buildInfo.BuildSymbols
                .Split(';')
                .Where(symbol => !string.IsNullOrEmpty(symbol))
                .Except(symbolsToRemove)
                .ToArray();

            if (!toKeep.Any())
            {
                return;
            }

            if (!string.IsNullOrEmpty(buildInfo.BuildSymbols))
            {
                buildInfo.BuildSymbols = string.Empty;
            }

            var newSymbols = string.Join(";", toKeep);

            buildInfo.BuildSymbols += newSymbols;
        }

        /// <summary>
        /// Does the <see cref="IBuildInfo"/> contain any of the provided symbols in the <see cref="IBuildInfo.BuildSymbols"/>?
        /// </summary>
        /// <param name="buildInfo"></param>
        /// <param name="symbols">The string array of symbols to match.</param>
        /// <returns>True, if any of the provided symbols are in the <see cref="IBuildInfo.BuildSymbols"/></returns>
        public static bool HasAnySymbols(this IBuildInfo buildInfo, params string[] symbols)
            => !string.IsNullOrEmpty(buildInfo.BuildSymbols) &&
               buildInfo.BuildSymbols.Split(';').Intersect(symbols).Any();

        /// <summary>
        /// Does the <see cref="IBuildInfo"/> contain any of the provided symbols in the <see cref="IBuildInfo.BuildSymbols"/>?
        /// </summary>
        /// <param name="buildInfo"></param>
        /// <param name="symbols">The string collection of symbols to match.</param>
        /// <returns>True, if any of the provided symbols are in the <see cref="IBuildInfo.BuildSymbols"/></returns>
        public static bool HasAnySymbols(this IBuildInfo buildInfo, IEnumerable<string> symbols)
            => !string.IsNullOrEmpty(buildInfo.BuildSymbols) &&
               buildInfo.BuildSymbols.Split(';').Intersect(symbols).Any();

        /// <summary>
        /// Checks if the <see cref="IBuildInfo"/> has any configuration symbols (i.e. debug, release, or master).
        /// </summary>
        /// <param name="buildInfo"></param>
        /// <returns>True, if the <see cref="IBuildInfo.BuildSymbols"/> contains debug, release, or master.</returns>
        public static bool HasConfigurationSymbol(this IBuildInfo buildInfo)
            => buildInfo.HasAnySymbols(
                UnityPlayerBuildTools.BuildSymbolDebug,
                UnityPlayerBuildTools.BuildSymbolRelease,
                UnityPlayerBuildTools.BuildSymbolMaster);

        /// <summary>
        /// Appends the <see cref="IBuildInfo"/>'s <see cref="IBuildInfo.BuildSymbols"/> without including debug, release or master.
        /// </summary>
        /// <param name="buildInfo"></param>
        /// <param name="symbols">Symbols to append.</param>
        public static void AppendWithoutConfigurationSymbols(this IBuildInfo buildInfo, string symbols)
        {
            buildInfo.AppendSymbols(symbols.Split(';').Except(new[]
            {
                UnityPlayerBuildTools.BuildSymbolDebug,
                UnityPlayerBuildTools.BuildSymbolRelease,
                UnityPlayerBuildTools.BuildSymbolMaster
            }).ToString());
        }
    }
}
