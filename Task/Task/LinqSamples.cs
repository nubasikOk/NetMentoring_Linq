// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
	[Title("LINQ Module")]
	[Prefix("Linq")]
	public class LinqSamples : SampleHarness
	{

		private DataSource dataSource = new DataSource();

		[Category("Restriction Operators")]
        [Title("Where - Task 3")]
        [Description("Выдайте список всех клиентов, чей суммарный оборот (сумма всех заказов) " +
            " превосходит некоторую величину X. Продемонстрируйте выполнение запроса с различными X " +
            "(подумайте, можно ли обойтись без копирования запроса несколько раз)")]

        public void Linq1()
        {
            var products =
                from p in dataSource.customerList
                select p;

            foreach (var p in products)
            {
                ObjectDumper.Write(p);
            }
        }

    }
}
