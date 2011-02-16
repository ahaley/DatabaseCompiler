using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Pyrite.DataLayer
{
	// collection of extension methods used to retrieve data from a table
	public class DatabaseAdapter : IDatabaseAdapter
	{
		private IDataReader reader;
		private readonly ISqlGenerator sqlGenerator;
		private string tableName;
		private string[] fields;

		public DatabaseAdapter(IDbConnection conn) :
			this(conn, new SqlGenerator(conn))
		{
		}

		public DatabaseAdapter(IDbConnection conn, ISqlGenerator sqlGenerator)
		{
			this.DbConnection = conn;
			this.sqlGenerator = sqlGenerator;
		}

		public IDbConnection DbConnection { get; private set; }

		/// <summary>
		/// Retrieves a record returned from query and returns it as a associative
		/// array of field names found in fields to the objects that contain the value.
		/// Each subsequent call will return the next record in the query.
		/// </summary>
		/// <param name="tableName">The table to retrieve the record from.</param>
		/// <returns>Associative array of field names to values</returns>
		public Dictionary<string, object> ExtractRecord(string tableName)
		{
			if (tableName != this.tableName) {
				this.tableName = tableName;
				if (null != this.reader && !this.reader.IsClosed)
					this.reader.Close();
				this.reader = null;
				this.fields = null;
			}

			if (null == this.reader)
				this.reader = ExecuteReader(this.sqlGenerator.GenerateSelect(this.tableName));

			if (null == this.fields)
				this.fields = this.sqlGenerator.GetFields(this.tableName);

			var record = ExtractRecord(this.reader, this.fields);
			if (null == record)
				this.reader = null;
			return record;
		}

		/// <summary>
		/// Returns a mapping of field names to value objects from an opened data reader.
		/// </summary>
		/// <param name="reader">Data reader executed against a query that contains the requested fields.</param>
		/// <param name="fields">The list of fields to be returned.</param>
		/// <returns>Mapping of field names to value objects from the data reader, null otherwise.</returns>
		public static Dictionary<string, object> ExtractRecord(IDataReader reader, string[] fields)
		{
			if (null == fields)
				return null;

			if (reader.IsClosed)
				return null;

			if (!reader.Read()) {
				if (!reader.IsClosed)
					reader.Close();
				return null;
			}

			var record = new Dictionary<string, object>();

			foreach (var field in fields) {
				if (!(reader[field] is DBNull))
					record.Add(field, reader[field]);
			}
			return record;
		}

		/// <summary>
		/// Retrieves the distinct values from the specified column.
		/// </summary>
		/// <param name="table">The table name containing the column.</param>
		/// <param name="column">The column name containing the values.</param>
		/// <returns>Array of the object values of the specified column.</returns>
		public object[] ExtractColumnDistinct(string table, string column)
		{
			var command = this.DbConnection.CreateCommand();
			command.CommandText = string.Format("SELECT DISTINCT {0} FROM {1}", column, table);
			var reader = command.ExecuteReader();
			var distinctFields = new List<object>();
			while (reader.Read())
				distinctFields.Add(reader[column]);
			return distinctFields.ToArray();
		}
		
		public object[] ExtractMultiColumnDistinct(string table, string[] columns)
		{
			var select = "";
			foreach (var column in columns) {
				if ("" != select)
					select += " UNION ";
				select += string.Format("SELECT {0} FROM {1}", column, table);
			}
			var reader = ExecuteReader(select);
			var distinctMultiColumns = new List<string>();
			while (reader.Read())
				distinctMultiColumns.Add(reader[0].ToString());
			return distinctMultiColumns.ToArray();
		}

		public int ExecuteNonQuery(string nonQuery)
		{
			var command = this.DbConnection.CreateCommand();
			command.CommandText = nonQuery;
			return command.ExecuteNonQuery();
		}

		public int GetIdLastInsert()
		{
			var command = this.DbConnection.CreateCommand();
			command.CommandText = "SELECT @@identity";
			return decimal.ToInt32((decimal)command.ExecuteScalar());
		}

		public int InsertRecord(string table, Dictionary<string, object> record)
		{
			return ExecuteNonQuery(sqlGenerator.GenerateInsert(table, record));
		}

		public string[] GetSchema(string table)
		{
			var restrictions = new string[] { null, null, table, null };
			var dataTable = (this.DbConnection as DbConnection).GetSchema("Columns", restrictions);
			var fields = new List<string>();
			foreach (DataRow row in dataTable.Rows)
				fields.Add(row["COLUMN_NAME"].ToString());
			return fields.ToArray();
		}

		private IDataReader ExecuteReader(string query)
		{
			var command = this.DbConnection.CreateCommand();
			command.CommandText = query;
			return command.ExecuteReader();
		}
		
	}
}
