﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public abstract class Matrix<T> : SymbolicExpression
	{
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
		/// <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public abstract T this[int rowIndex, int columnIndex]
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the rowIndex size of elements.
		/// </summary>
		public int RowCount
		{
			get
			{
				return NativeMethods.Rf_nrows(this.handle);
			}
		}

		/// <summary>
		/// Gets the columnIndex size of elements.
		/// </summary>
		public int ColumnCount
		{
			get
			{
				return NativeMethods.Rf_ncols(this.handle);
			}
		}

		/// <summary>
		/// Gets the pointer for the first element.
		/// </summary>
		protected IntPtr DataPointer
		{
			get
			{
				return IntPtr.Add(this.handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
			}
		}

		/// <summary>
		/// Gets the size of an element in byte.
		/// </summary>
		protected abstract int DataSize
		{
			get;
		}

		protected Matrix(REngine engine, SymbolicExpressionType type, int rowCount, int columnCount)
			: base(engine, NativeMethods.Rf_allocMatrix(type, rowCount, columnCount))
		{
			if (rowCount <= 0)
			{
				throw new ArgumentOutOfRangeException("rowCount");
			}
			if (columnCount <= 0)
			{
				throw new ArgumentOutOfRangeException("columnCount");
			}
		}

		/// <summary>
		/// Creates a new IntegerVector with the specified values.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="length">The values.</param>
		public Matrix(REngine engine, SymbolicExpressionType type, T[, ] matrix)
			: base(engine, NativeMethods.Rf_allocMatrix(type, matrix.GetLength(0), matrix.GetLength(1)))
		{
			int rowCount = RowCount;
			int columnCount = ColumnCount;
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
			{
				for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
				{
					this[rowIndex, columnIndex] = matrix[rowIndex, columnIndex];
				}
			}
		}

		protected Matrix(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{
		}

		protected int GetOffset(int rowIndex, int columnIndex)
		{
			return DataSize * (columnIndex * RowCount + rowIndex);
		}
	}
}