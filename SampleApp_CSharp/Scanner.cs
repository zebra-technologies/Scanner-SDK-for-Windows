using System;
using System.Collections.Generic;
using System.Text;

namespace Scanner_SDK_Sample_Application
{
    public class Scanner
    {
        #region constants
        public const string SCANNER_SNAPI = "SNAPI";
        public const string SCANNER_SSI = "SSI";
        public const string SCANNER_NIXMODB = "NIXMODB";
        public const string SCANNER_IBMHID = "USBIBMHID";
        public const string SCANNER_IBMTT = "USBIBMTT";
        public const string SCALE_IBM = "USBIBMSCALE";
        public const string SCANNER_SSI_BT = "SSI_BT";
        public const string SCANNER_SSI_IP = "SSI_IP";
        public const string SCANNER_OPOS = "USBOPOS";
        public const string SCANNER_HIDKB =	"USBHIDKB";
        public const string CAMERA_UVC = "UVC_CAMERA";

        public const int MAX_ATTRIBUTE_COUNT = 2000;
        public const int MAX_ATTRIBUTE_ITEMS = 5;//att-id, att-type, att-prop, att-value, att-name// ( is not used) 
        public const int POS_ATTR_ID = 0;
        public const int POS_ATTR_TYPE = 1;
        public const int POS_ATTR_PROPERTY = 2;
        public const int POS_ATTR_VALUE = 3;
        public const int POS_ATTR_NAME = 4;

        public const string TAG_OUTARGS = "outArgs";
        public const string TAG_ARG_XML = "arg-xml";
        public const string TAG_DISCOVERY = "discovery";
        public const string TAG_STATUS = "status";
        public const string TAG_OPCODE = "opcode";

        public const string TAG_SCANNER = "scanner";
        public const string TAG_SCANNER_SNAPI = SCANNER_SNAPI;
        public const string TAG_SCANNER_SSI = SCANNER_SSI;
        public const string TAG_SCANNER_NIXMODB = SCANNER_NIXMODB;
        public const string TAG_SCANNER_IBMHID = SCANNER_IBMHID;
        public const string TAG_SCANNER_OPOS = SCANNER_OPOS;
        public const string TAG_SCANNER_HIDKB = SCANNER_HIDKB;
        public const string TAG_SCANNER_IMBTT = SCANNER_IBMTT;
        public const string TAG_SCALE_IBM = SCALE_IBM;
        public const string TAG_SCANNER_SSI_BT = SCANNER_SSI_BT;
        public const string TAG_SCANNER_SSI_IP = SCANNER_SSI_IP;

        public const string TAG_SCANNER_ID = "scannerID";
        public const string TAG_SCANNER_TYPE = "type";
        public const string TAG_SCANNER_SERIALNUMBER = "serialnumber";
        public const string TAG_SCANNER_MODELNUMBER = "modelnumber";
        public const string TAG_SCANNER_GUID = "GUID";
        public const string TAG_SCANNER_PORT = "port";
        public const string TAG_SCANNER_VID = "VID";
        public const string TAG_SCANNER_PID = "PID";
        public const string TAG_SCANNER_DOM = "DoM";
        public const string TAG_SCANNER_FW = "firmware";
        public const string TAG_SCANNER_CN = "configname";

        public const string TAG_ATTRIBUTE = "attribute";
        public const string TAG_ATTR_ID = "id";
        public const string TAG_ATTR_NAME = "name";
        public const string TAG_ATTR_TYPE = "datatype";
        public const string TAG_ATTR_PROPERTY = "permission";
        public const string TAG_ATTR_VALUE = "value";

        public const string TAG_SCALE_WEIGHT = "weight";
        public const string TAG_SCALE_WEIGHT_MODE = "weight_mode";
        public const string TAG_SCALE_STATUS = "status";
        public const string TAG_SCALE_RAWDATA = "rawdata";

        public const string TAG_SNAPI_PARAM_VAL = "param_value";
        #endregion

        public class RSMAttribute
        {
            public string ID;
            public string Type;
            public string property;
            public string value;
            public string name;
        }

        /// <summary>
        /// 5 Diamentional string array to store data retrieved from RSM_GET_ATTR call
        /// </summary>
        public Array m_arAttributes;  //string[,] m_arAttributes;
        public RSMAttribute m_rsmAttribute;
        #region Private Members
        private int handle;
        private string scannerName;// now scannerName = scannerID
        private string scannerID;// a unique id
        private string scannerType;//SCANNER_SNAPI, SCANNER_SSI
        private string serialNo;
        private string modelNo;
        private string guid;
        private string port;
        private string firmware;
        private string configname;
        private string mnfdate; //manufacture date
        private bool claimed;//scanner is claimed by this client-app
        private bool useHID; // Scanner is using HID channel for Binary Data transfer
        #endregion

        public Scanner()
        {
            m_arAttributes = Array.CreateInstance(typeof(String), MAX_ATTRIBUTE_COUNT, MAX_ATTRIBUTE_ITEMS);
            m_rsmAttribute = new RSMAttribute();
            ClearValues();
        }

        //public RSMAttribute GetAttribute(int attributeID)
        //{
        //    RSMAttribute tempAttr;
        //    return tempAttr;
        //}
        //public void SetAttribute(RSMAttribute attribute)
        //{

        //}

        /// <summary>
        /// Clear the public properties of the object
        /// </summary>
        public void ClearValues()
        {
            CLAIMED = false;
            useHID = false;
            SCANNERNAME = "";
            SCANNERID = "";
            SERIALNO = "";
            MODELNO = "";
            GUID = "";
            SCANNERTYPE = "";
            SCANNERMNFDATE = "";
            SCANNERFIRMWARE = "";
            SCANNERCONFIG= "";
        }

        #region Public Getters and Setters
        public string SCANNERMNFDATE
        {
            get { return mnfdate; }
            set { mnfdate = value; }
        }
        public string SCANNERFIRMWARE
        {
            get { return firmware; }
            set { firmware = value; }
        }
        public string SCANNERCONFIG
        {
            get { return configname; }
            set { configname = value; }
        }
        public string SCANNERNAME
        {
            get { return scannerName; }
            set { scannerName = value; }
        }
        public string SCANNERTYPE
        {
            get { return scannerType; }
            set { scannerType = value; }
        }
        public int HANDLE
        {
            get { return handle; }
            set { handle = value; }
        }
        public string SCANNERID
        {
            get { return scannerID; }
            set { scannerID = value; }
        }
        public string SERIALNO
        {
            get { return serialNo; }
            set { serialNo = value; }
        }
        public string MODELNO
        {
            get { return modelNo; }
            set { modelNo = value; }
        }
        public string GUID
        {
            get { return guid; }
            set { guid = value; }
        }
        public string PORT
        {
            get { return port; }
            set { port = value; }
        }
        public bool CLAIMED
        {
            get { return claimed;  }
            set { claimed = value; }
        }
        public bool UseHID
        {
            get { return useHID; }
            set { useHID = value; }
        }
        #endregion
    }
}
