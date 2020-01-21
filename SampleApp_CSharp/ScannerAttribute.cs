using System;
using System.Collections.Generic;
using System.Text;

namespace Scanner_SDK_Sample_Application
{
    /// <summary>
    /// Scanner Attribute of a Zebra Scanner
    /// </summary>
    public class ScannerAttribute
    {
        #region Members of a Scanner
        int id;
        /// <summary>
        /// Get or Set the Attribute ID
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        string type;

        /// <summary>
        /// Get or Set the Attribute Type
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        object value;

        /// <summary>
        /// Get or Set the Attribute Value
        /// </summary>
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }
        string permission;

        /// <summary>
        /// Get or Set the Attribute Permission
        /// </summary>
        public string Permission
        {
            get { return permission; }
            set { permission = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Scanner Attribute
        /// </summary>
        public ScannerAttribute()
        {

        }

        /// <summary>
        /// Creates a new Scanner Attribute  
        /// </summary>
        /// <param name="id">Attribute ID</param>
        /// <param name="type">Attribute Type</param>
        /// <param name="value">Attribute Value</param>
        public ScannerAttribute(int id, string type, object value)
        {
            this.Id = id;
            this.Type = type;
            this.Value = value;
        }
        #endregion

    }
}
