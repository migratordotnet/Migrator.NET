#region License
//The contents of this file are subject to the Mozilla Public License
//Version 1.1 (the "License"); you may not use this file except in
//compliance with the License. You may obtain a copy of the License at
//http://www.mozilla.org/MPL/
//Software distributed under the License is distributed on an "AS IS"
//basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
//License for the specific language governing rights and limitations
//under the License.
#endregion
using System;

namespace Migrator
{
	/// <summary>
	/// Represents a table column properties.
	/// </summary>
	[Flags]
	public enum ColumnProperties
	{
		/// <summary>
		/// Null is allowable
		/// </summary>
		Null = 1,
		/// <summary>
		/// Null is not allowable
		/// </summary>
		NotNull = 2,
		/// <summary>
		/// Identity column, autoinc
		/// </summary>
		Identity = 4,
		/// <summary>
		/// Unique Column
		/// </summary>
		Unique = 8,
		/// <summary>
		/// Indexed Column
		/// </summary>
		Indexed = 16,
		/// <summary>
		/// Unsigned Column
		/// </summary>
		Unsigned = 32,
		/// <summary>
		/// Foreign Key
		/// </summary>
		ForeignKey = Unsigned | Null,
		/// <summary>
		/// Primary Key
		/// </summary>
		PrimaryKey = 64 | Unsigned | NotNull,
		/// <summary>
		/// Primary key. Make the column a PrimaryKey and unsigned
		/// </summary>
		PrimaryKeyWithIdentity = PrimaryKey | Identity

	}
	
	/// <summary>
	/// Represents a table column.
	/// </summary>
	public class Column
	{
		private string _name;
		private Type _type;
		private int _size;
		private ColumnProperties _property;
		private object _defaultValue;
		
		public Column(string name, Type type)
		{
			this._name = name;
			this._type = type;
		}
		
		public Column(string name, Type type, int size)
		{
			this._name = name;
			this._type = type;
			this._size = size;
		}
		
		public Column(string name, Type type, ColumnProperties property)
		{
			this._name = name;
			this._type = type;
			this._property = property;
		}
		
		public Column(string name, Type type, int size, ColumnProperties property)
		{
			this._name = name;
			this._type = type;
			this._size = size;
			this._property = property;
		}
		
		public Column(string name, Type type, int size, ColumnProperties property, object defaultValue)
		{
			this._name = name;
			this._type = type;
			this._size = size;
			this._property = property;
			this._defaultValue = defaultValue;
		}
		
		public Column(string name, Type type, ColumnProperties property, object defaultValue)
		{
			this._name = name;
			this._type = type;
			this._property = property;
			this._defaultValue = defaultValue;
		}
		
		public string Name {
			get {
				return _name;
			}
			set {
				_name = value;
			}
		}
		
		public Type Type {
			get {
				return _type;
			}
			set {
				_type = value;
			}
		}
		
		public int Size {
			get {
				return _size;
			}
			set {
				_size = value;
			}
		}
		
		public ColumnProperties ColumnProperty {
			get {
				return _property;
			}
			set {
				_property = value;
			}
		}
		
		public object DefaultValue {
			get {
				return _defaultValue;
			}
			set {
				_defaultValue = value;
			}
		}
				
	}
}
