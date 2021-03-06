﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace EFDesignerTestInfrastructure.VS
{
    using System;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.VsIdeTesting;

    public static class UITestRunner
    {
        private static readonly Exception caughtException;

        static UITestRunner()
        {
            try
            {
                UIThreadInvoker.Initialize();
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }
        }

        public static void Execute(Action action)
        {
            if (caughtException != null)
            {
                throw new InvalidOperationException("UITestRunner Initialize failed", caughtException);
            }

            var resetEvent = new ManualResetEvent(false);

            UIThreadInvoker.Invoke(
                new Action(
                    () =>
                        {
                            try
                            {
                                action();
                            }
                            catch (Exception)
                            {
                                // TODO: Screen capture ?
                                throw;
                            }
                            finally
                            {
                                resetEvent.Set();
                            }
                        }));

            resetEvent.WaitOne();
        }
    }
}
