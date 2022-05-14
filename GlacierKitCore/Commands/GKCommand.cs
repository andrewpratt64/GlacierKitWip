using GlacierKitCore.Attributes;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Commands
{
    /// <summary>
    /// Interface for types that wrap a GlacierKit-specific command
    /// </summary>
    public interface IGKCommand
    {
        /// <summary>
        /// Unique identifier of the command
        /// </summary>
        /// <example>"NewProject"</example>
        public string GKCommandId
        { get; }

        /// <summary>
        /// Print-friendly name to display in the editor
        /// </summary>
        /// <example>New Project</example>
        public string? DisplayName
        { get; }

        /// <summary>
        /// The value of TParam in ReactiveUI.ReactiveCommandBase
        /// </summary>
        public Type TParamValue
        { get; }

        /// <summary>
        /// The value of TResult in ReactiveUI.ReactiveCommandBase
        /// </summary>
        public Type TResultValue
        { get; }
    }


    /// <summary>
    /// Base class that wraps a GlacierKit-specific command
    /// </summary>
    /// <typeparam name="TParam">See: ReactiveUI.ReactiveCommandBase</typeparam>
    /// <typeparam name="TResult">ReactiveUI.ReactiveCommandBase</typeparam>
    public class GKCommand<TParam, TResult> : IGKCommand
    {
        public string GKCommandId
        { get; }

        public string? DisplayName
        { get; }

        public Type TParamValue => typeof(TParam);
        public Type TResultValue => typeof(TResult);


        /// <summary>
        /// The command itself
        /// </summary>
        public ReactiveCommand<TParam, TResult> Command
        { get; }


        public GKCommand(string commandId, string? displayName, ReactiveCommand<TParam, TResult> command)
        {
            Debug.Assert(commandId.Length > 0, "Commands can't have a blank id");

            GKCommandId = commandId;
            Command = command;
            DisplayName = displayName ?? commandId;
        }
    }
}
