using System.Collections.Generic;

namespace Pyrite.XmlConversionRules
{
	public class AccountConversionRules : ITableConversionRules
	{
		private readonly Dictionary<string, string> fieldRenames = new Dictionary<string, string> {
			{"tblContactEmpID", "emp_id"},
			{"tblContactLoginID", "username"},
			{"tblContactLoginPW", "password"},
			{"tblContactLoginAdminLevel", "admin_level"},
			{"tblContactNameFirst", "firstname"},
			{"tblContactNameLast", "lastname"},
			{"tblContactNumber", "phone_number"},
			{"tblContactExtension", "extension"},
			{"tblContactCell", "mobile_number"},
			{"tblContactFax", "fax_number"},
			{"tblContactEmail", "email"},
			{"tblComments", "comments"},
			{"tblNotes", "notes"},
			{"tblDeleted", "deleted"},
			{"tblDeletedBy", "deleted_by"},
			{"tblSysDate", "sys_date"},
			{"tblUserID", "user_id"}};

		private readonly Dictionary<string, string> extractedColumns = new Dictionary<string, string> {
			{"tblContactCat", "Title"},
			{"tblContactOffice", "Office"},
			{"tblDepartment", "Department"}};

		private readonly Dictionary<string, string[]> extractedAssociations = new Dictionary<string, string[]> {
			{"Responsibility", new [] { "tblResponsible1", "tblResponsible2", "tblResponsible3", "tblResponsible4",
									  "tblResponsible5", "tblResponsible6", "tblResponsible7", "tblResponsible8",
									  "tblResponsible9", "tblResponsible10" } } };

		public string SrcTableName { get { return "tblContacts"; } }
		public string DestTableName { get { return "Account"; } }

		public Dictionary<string, string> FieldRenames
		{
			get { return this.fieldRenames; }
		}

		public Dictionary<string, string> ExtractedColumns
		{
			get { return this.extractedColumns; }
		}

		public Dictionary<string, string[]> ExtractedAssociations
		{
			get { return this.extractedAssociations; }
		}

		public Dictionary<string, string[]> InferRelations
		{
			get { throw new System.NotImplementedException(); }
		}

	}
}
