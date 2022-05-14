using GlacierKitCore.Attributes;
using GlacierKitCore.Commands;
using GlacierKitTestShared;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.Commands
{
    public class GKCommandTest
    {
        private static readonly string _DATA_CommandId = "GKCommandTest_Foo";
        private static readonly string _DATA_CommandDisplayName = "Foo";


        private static IGKCommand NewGKCommandWith<TParam, TResult>(
            TParam? TParamValue,
            TResult? TResultValue,
            string commandId,
            string? displayName)
        {
            if (TParamValue == null)
            {
                if (TResultValue == null)
                {
                    return new GKCommand<Unit, Unit>(
                        commandId,
                        displayName,
                        ReactiveCommand.Create<Unit, Unit>(_ =>
                        {
                            return Unit.Default;
                        })
                    );
                }
                else
                {
                    return new GKCommand<Unit, TResult>(
                        commandId,
                        displayName,
                        ReactiveCommand.Create<Unit, TResult>(_ =>
                        {
                            return TResultValue;
                        })
                    );
                }
            }
            else if (TResultValue == null)
            {
                return new GKCommand<TParam, Unit>(
                    commandId,
                    displayName,
                    ReactiveCommand.Create<TParam, Unit>(_ =>
                    {
                        return Unit.Default;
                    })
                );
            }
            else
            {
                return new GKCommand<TParam, TResult>(
                    commandId,
                    displayName,
                    ReactiveCommand.Create<TParam, TResult>(_ =>
                    {
                        return TResultValue;
                    })
                );
            }
        }


        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "Return this")]
        [InlineData("Use this", null)]
        [InlineData("Use this", "Return this")]
        public static void Ctor_with_type_args_does_not_throw<TParam, TResult>(TParam? TParamValue, TResult? TResultValue)
        {
            // Arrange
            IGKCommand cmd;

            // Act/Assert
            Util.AssertCodeDoesNotThrowException(() =>
            {
                cmd = NewGKCommandWith(
                    TParamValue,
                    TResultValue,
                    _DATA_CommandId,
                    _DATA_CommandDisplayName
                );
            });
        }

        [Fact]
        public static void Ctor_with_blank_id_throws()
        {
            // Arrange
            GKCommand<Unit, Unit> gkCommand;
            string commandId = "";
            ReactiveCommand<Unit, Unit> cmd;

            // Act
            cmd = ReactiveCommand.Create<Unit, Unit>(_ =>
            {
                return Unit.Default;
            });

            // Assert
            Util.AssertCodeThrowsException(() =>
            {
                gkCommand = new GKCommand<Unit, Unit>(
                    commandId,
                    _DATA_CommandDisplayName,
                    cmd
                );
            });
        }

        [Fact]
        public static void Ctor_with_null_display_name_does_not_throw()
        {
            // Arrange
            GKCommand<Unit, Unit> gkCommand;
            string? displayName = null;

            // Assert
            Util.AssertCodeDoesNotThrowException(() =>
            {
                gkCommand = new(
                    _DATA_CommandId,
                    displayName,
                    ReactiveCommand.Create<Unit, Unit>(_ =>
                    {
                        return Unit.Default;
                    })
                );
            });
        }

        [Fact]
        public static void GKCommandId_is_constant()
        {
            // Arrange
            Type GKCommandType;
            string GKCommandIdPropertyName = "GKCommandId";
            PropertyInfo? GKCommandIdPropertyInfo;

            // Act
            GKCommandType = typeof(GKCommand<Unit, Unit>);
            GKCommandIdPropertyInfo = GKCommandType.GetProperty(GKCommandIdPropertyName);

            // Assert
            Assert.NotNull(GKCommandIdPropertyInfo);
            Assert.False(GKCommandIdPropertyInfo!.CanWrite);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "Return this")]
        [InlineData("Use this", null)]
        [InlineData("Use this", "Return this")]
        public static void TParamValue_and_TResultValue_have_correct_values<TParam, TResult>(TParam? TParamValue, TResult? TResultValue)
        {
            // Arrange
            IGKCommand cmd;
            Type expectedTParamValue;
            Type actualTParamValue;
            Type expectedTResultValue;
            Type actualTResultValue;

            // Act
            cmd = NewGKCommandWith(
                TParamValue,
                TResultValue,
                _DATA_CommandId,
                _DATA_CommandDisplayName
            );

            if (TParamValue == null)
                expectedTParamValue = typeof(Unit);
            else
                expectedTParamValue= typeof(TParam);
            
            if (TResultValue == null)
                expectedTResultValue = typeof(Unit);
            else
                expectedTResultValue = typeof(TParam);

            actualTParamValue = cmd.TParamValue;
            actualTResultValue = cmd.TResultValue;

            // Assert
            Assert.Equal(expectedTParamValue, actualTParamValue);
            Assert.Equal(expectedTResultValue, actualTResultValue);
        }
    }
}
