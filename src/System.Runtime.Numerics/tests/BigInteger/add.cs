// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using Tools;
using Xunit;

namespace System.Numerics.Tests
{
    public class addTest
    {
        private static int s_samples = 10;
        private static Random s_random = new Random(100);

        [Fact]
        public static void RunAddTests()
        {
            byte[] tempByteArray1 = new byte[0];
            byte[] tempByteArray2 = new byte[0];

            // Add Method - Two Large BigIntegers
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random);
                tempByteArray2 = GetRandomByteArray(s_random);
                Assert.True(VerifyAdditionString(Print(tempByteArray1) + Print(tempByteArray2) + "bAdd"), " Verification Failed");
            }

            // Add Method - Two Small BigIntegers
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random, 2);
                tempByteArray2 = GetRandomByteArray(s_random, 2);
                Assert.True(VerifyAdditionString(Print(tempByteArray1) + Print(tempByteArray2) + "bAdd"), " Verification Failed");
            }

            // Add Method - One large and one small BigIntegers
            for (int i = 0; i < s_samples; i++)
            {
                try
                {
                    tempByteArray1 = GetRandomByteArray(s_random);
                    tempByteArray2 = GetRandomByteArray(s_random, 2);
                    Assert.True(VerifyAdditionString(Print(tempByteArray1) + Print(tempByteArray2) + "bAdd"), " Verification Failed");

                    tempByteArray1 = GetRandomByteArray(s_random, 2);
                    tempByteArray2 = GetRandomByteArray(s_random);
                    Assert.True(VerifyAdditionString(Print(tempByteArray1) + Print(tempByteArray2) + "bAdd"), " Verification Failed");
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Array1: " + Print(tempByteArray1));
                    Console.WriteLine("Array2: " + Print(tempByteArray2));
                    throw;
                }
            }

            // Add Method - One large BigIntegers and zero
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random);
                tempByteArray2 = new byte[] { 0 };
                Assert.True(VerifyAdditionString(Print(tempByteArray1) + Print(tempByteArray2) + "bAdd"), " Verification Failed");

                tempByteArray1 = new byte[] { 0 };
                tempByteArray2 = GetRandomByteArray(s_random);
                Assert.True(VerifyAdditionString(Print(tempByteArray1) + Print(tempByteArray2) + "bAdd"), " Verification Failed");
            }

            // Add Method - One small BigIntegers and zero
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random, 2);
                tempByteArray2 = new byte[] { 0 };
                Assert.True(VerifyAdditionString(Print(tempByteArray1) + Print(tempByteArray2) + "bAdd"), " Verification Failed");

                tempByteArray1 = new byte[] { 0 };
                tempByteArray2 = GetRandomByteArray(s_random, 2);
                Assert.True(VerifyAdditionString(Print(tempByteArray1) + Print(tempByteArray2) + "bAdd"), " Verification Failed");
            }

            // 32 bit boundary n1=0 n2=0 c=0
            Assert.True(VerifyAdditionString("0 0 bAdd"), " Verification Failed");

            // 32 bit boundary n1=0 n2=0 c=1
            Assert.True(VerifyAdditionString((Math.Pow(2, 31) + Math.Pow(2, 30)) + " " + (Math.Pow(2, 31) + Math.Pow(2, 30)) + " bAdd"), " Verification Failed");

            // 32 bit boundary n1=0 n2=1 c=0
            Assert.True(VerifyAdditionString("0" + " " + Math.Pow(2, 32) + " bAdd"), " Verification Failed");

            // 32 bit boundary n1=0 n2=1 c=1
            Assert.True(VerifyAdditionString(Math.Pow(2, 31) + " " + (Math.Pow(2, 32) + Math.Pow(2, 31)) + " bAdd"), " Verification Failed");

            // 32 bit boundary n1=1 n2=0 c=0
            Assert.True(VerifyAdditionString(Math.Pow(2, 32) + " " + "0" + " bAdd"), " Verification Failed");

            // 32 bit boundary n1=1 n2=0 c=1
            Assert.True(VerifyAdditionString((Math.Pow(2, 32) + Math.Pow(2, 31)) + " " + Math.Pow(2, 31) + " bAdd"), " Verification Failed");

            // 32 bit boundary n1=0 n2=1 c=0
            Assert.True(VerifyAdditionString(Math.Pow(2, 32) + " " + Math.Pow(2, 32) + " bAdd"), " Verification Failed");

            // 32 bit boundary n1=0 n2=1 c=1
            Assert.True(VerifyAdditionString((Math.Pow(2, 32) + Math.Pow(2, 31)) + " " + (Math.Pow(2, 32) + Math.Pow(2, 31)) + " bAdd"), " Verification Failed");

            // Identity (x+y)+z == (y+z)+x
            //Check some identities
            // (x+y)+z = (y+z)+x

            Assert.True(VerifyIdentityString(
                    Int64.MaxValue.ToString() + " " + Int32.MaxValue.ToString() + " bAdd " + Int16.MaxValue.ToString() + " bAdd",
                    Int32.MaxValue.ToString() + " " + Int16.MaxValue.ToString() + " bAdd " + Int64.MaxValue.ToString() + " bAdd"
            ), "Test failed");

            byte[] x = GetRandomByteArray(s_random);
            byte[] y = GetRandomByteArray(s_random);
            byte[] z = GetRandomByteArray(s_random);

            Assert.True(VerifyIdentityString(Print(x) + Print(y) + Print(z) + "bAdd bAdd", Print(y) + Print(z) + Print(x) + "bAdd bAdd"), " Verification Failed");
        }

        private static bool VerifyAdditionString(string opstring)
        {
            bool ret = true;
            StackCalc sc = new StackCalc(opstring);
            while (sc.DoNextOperation())
            {
                ret &= Eval(sc.snCalc.Peek().ToString(), sc.myCalc.Peek().ToString(), String.Format("Out of Sync stacks found.  BigInteger {0} Mine {1}", sc.snCalc.Peek(), sc.myCalc.Peek()));
            }
            return ret;
        }
        private static bool VerifyIdentityString(string opstring1, string opstring2)
        {
            bool ret = true;

            StackCalc sc1 = new StackCalc(opstring1);
            while (sc1.DoNextOperation())
            {	//Run the full calculation
                sc1.DoNextOperation();
            }

            StackCalc sc2 = new StackCalc(opstring2);
            while (sc2.DoNextOperation())
            {	//Run the full calculation
                sc2.DoNextOperation();
            }

            ret &= Eval(sc1.snCalc.Peek().ToString(), sc2.snCalc.Peek().ToString(), String.Format("Out of Sync stacks found.  BigInteger1: {0} BigInteger2: {1}", sc1.snCalc.Peek(), sc2.snCalc.Peek()));

            return ret;
        }

        private static Byte[] GetRandomByteArray(Random random)
        {
            return GetRandomByteArray(random, random.Next(0, 1024));
        }
        private static Byte[] GetRandomByteArray(Random random, int size)
        {
            byte[] value = new byte[size];

            for (int i = 0; i < value.Length; ++i)
            {
                value[i] = (byte)random.Next(0, 256);
            }

            return value;
        }

        private static String Print(byte[] bytes)
        {
            String ret = "make ";

            for (int i = 0; i < bytes.Length; i++)
            {
                ret += bytes[i] + " ";
            }
            ret += "endmake ";

            return ret;
        }

        public static bool Eval<T>(T expected, T actual, String errorMsg)
        {
            bool retValue = expected == null ? actual == null : expected.Equals(actual);

            if (!retValue)
                return Eval(retValue, errorMsg +
                " Expected:" + (null == expected ? "<null>" : expected.ToString()) +
                " Actual:" + (null == actual ? "<null>" : actual.ToString()));

            return true;
        }
        public static bool Eval(bool expression, string message)
        {
            if (!expression)
            {
                Console.WriteLine(message);
            }

            return expression;
        }
    }
}
