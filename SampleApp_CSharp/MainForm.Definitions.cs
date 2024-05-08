namespace Scanner_SDK_Sample_Application
{
    partial class frmScannerApp
    {
        /*****************************************************************************************************
         * Constants
         *****************************************************************************************************/

        const string APP_TITLE = "Scanner Multi-Interface Test Utility";
        const string STR_OPEN = "Start";
        const string STR_CLOSE = "Stop";
        const string STR_REFRESH = "Rediscover Scanners";
        const string STR_FIND = "Discover Scanners";
        const int NUM_SCANNER_EVENTS = 6;


        // Scanner types
        public const short SCANNER_TYPES_ALL = 1;
        public const short SCANNER_TYPES_SNAPI = 2;
        public const short SCANNER_TYPES_SSI = 3;
        public const short SCANNER_TYPES_RSM = 4;
        public const short SCANNER_TYPES_IMAGING = 5;
        public const short SCANNER_TYPES_IBMHID	= 6;
        public const short SCANNER_TYPES_NIXMODB = 7;
        public const short SCANNER_TYPES_HIDKB = 8;
        public const short SCANNER_TYPES_IBMTT = 9;
        public const short SCALE_TYPES_IBM = 10;
        public const short SCALE_TYPES_SSI_BT = 11;
        public const short CAMERA_TYPES_UVC = 14;

        // Total number of scanner types
        public const short TOTAL_SCANNER_TYPES = CAMERA_TYPES_UVC;

        //as in RMD_CMD_OPCODE_T of FUD_RMDTypes.h //
        const int ATTR_GETALL = 0x01;
        const int ATTR_GET = 0x02;
        const int ATTR_GETNEXT = 0x03;
        const int ATTR_GETOFFSET = 0x04;
        const int ATTR_SET = 0x05;
        const int ATTR_STORE = 0x06;
        // end of RMD_CMD_OPCODE_T //

        const int SUBSCRIBE_BARCODE = 1;
        const int SUBSCRIBE_IMAGE = 2;
        const int SUBSCRIBE_VIDEO = 4;
        const int SUBSCRIBE_RMD = 8;
        const int SUBSCRIBE_PNP = 16;
        const int SUBSCRIBE_OTHER = 32;

        // available values for 'status' //
        const int STATUS_SUCCESS = 0;
        const int STATUS_FALSE = 1;
        const int STATUS_LOCKED = 10;
        const int ERROR_CDC_SCANNERS_NOT_FOUND = 150;
        const int ERROR_UNABLE_TO_OPEN_CDC_COM_PORT = 151;

        // Barcode, Image & Video event types //
        const int SCANNER_DECODE_GOOD = 1;
        const int IMAGE_COMPLETE = 1;
        const int IMAGE_TRAN_STATUS = 2;
        const int VIDEO_FRAME_COMPLETE = 1;

        //for WM_DEVICE_NOTIFICATION//
        const int SCANNER_ATTACHED = 0;
        const int SCANNER_DETTACHED = 1;

        // Scanner Notification Event Types

        const int BARCODE_MODE = 1;
        const int IMAGE_MODE = 2;
        const int VIDEO_MODE = 3;
        const int DEVICE_ENABLED = 13;
        const int DEVICE_DISABLED = 14;

        // Firmware download events //
        const int SCANNER_UF_SESS_START	= 11; // Triggered when flash download session starts 
        const int SCANNER_UF_DL_START	= 12; // Triggered when component download starts 
        const int SCANNER_UF_DL_PROGRESS = 13; // Triggered when block(s) of flash completed 
        const int SCANNER_UF_DL_END		= 14; // Triggered when component download ends 
        const int SCANNER_UF_SESS_END = 15; // Triggered when flash download session ends 

        const int SCANNER_UF_STATUS = 16; // Triggered when update error or status
        //------FW------------------//


        const int MAX_NUM_DEVICES = 255; /* Maximum number of scanners to be connected*/

        const int MAX_BUFF_SIZE = 1024;
        const int MAX_PARAM_LEN = 2; /* Maximum number of bytes per parameter     */
        const uint MAX_SERIALNO_LEN = 255; /* Maximum number of bytes for serail number */

        const ushort PARAM_USE_HID = 1004;
        const ushort PARAM_USE_HID_OLD = 122;
        const ushort PARAM_CLEAR_MEMORY = 806;
        const ushort BMP_FILE_SELECTION = 0x0003; /* models. Please refer scanner PRGs for     */
        const ushort TIFF_FILE_SELECTION = 0x0004; /* more information on scanner parameters.   */
        const ushort JPEG_FILE_SELECTION = 0x0001;
        const int PARAM_PERSISTANCE_ON = 0x0001; /* Parameters persistance on */
        const int PARAM_PERSISTANCE_OFF = 0x0000; /* Parameters persistance off */
      
        //****** CORESCANNER PROTOCOL ******//
        const int GET_VERSION = 1000;
        const int REGISTER_FOR_EVENTS = 1001;
        const int UNREGISTER_FOR_EVENTS = 1002;
        const int GET_PAIRING_BARCODE = 1005; // Get  Blue tooth scanner pairing bar code
        const int CLAIM_DEVICE = 1500;
        const int RELEASE_DEVICE = 1501;
        const int ABORT_MACROPDF = 2000;
        const int ABORT_UPDATE_FIRMWARE = 2001;
        const int DEVICE_AIM_OFF = 2002;
        const int DEVICE_AIM_ON = 2003;
        const int FLUSH_MACROPDF = 2005;
        const int GET_ALL_PARAMETERS = 2006;
        const int GET_PARAMETERS = 2007;
        const int DEVICE_GET_SCANNER_CAPABILITIES = 2008;
        const int DEVICE_PULL_TRIGGER = 2011;
        const int DEVICE_RELEASE_TRIGGER = 2012;
        const int SET_PARAMETER_DEFAULTS = 2015;
        const int DEVICE_SET_PARAMETERS = 2016;
        const int SET_PARAMETER_PERSISTANCE = 2017;
        
        public const int RSM_ATTR_GETALL = 5000;
        public const int RSM_ATTR_GET = 5001;
        public const int RSM_ATTR_GETNEXT = 5002;
        public const int RSM_ATTR_SET = 5004;
        public const int RSM_ATTR_STORE = 5005;
        const int GET_DEVICE_TOPOLOGY = 5006;
        const int START_NEW_FIRMWARE = 5014;
        const int UPDATE_ATTRIB_META_FILE = 5015;
        const int UPDATE_FIRMWARE = 5016;
        const int UPDATE_FIRMWARE_FROM_PLUGIN = 5017;
        const int UPDATE_DECODE_TONE = 5050;
        const int ERASE_DECODE_TONE = 5051;
        const int UPDATE_ELECTRIC_FENCE_CUSTOM_TONE = 5052;
        const int ERASE_ELECTRIC_FENCE_CUSTOM_TONE = 5053;
        const int SET_ACTION = 6000;
        
        const int KEYBOARD_EMULATOR_ENABLE = 6300; //6300
        const int KEYBOARD_EMULATOR_SET_LOCALE = 6301; //6301
        const int KEYBOARD_EMULATOR_GET_CONFIG = 6302; //6302

        const int CONFIGURE_DADF = 6400;
        const int RESET_DADF = 6401;

        // Serial //
        const int DEVICE_SET_SERIAL_PORT_SETTINGS = 6101;
        // Serial - end //



        //Scale Commands //
        const int SCALE_READ_WEIGHT = 0x1b58; //7000
	    const int SCALE_ZERO_SCALE = 0X1B5A; //7002
	    const int SCALE_SYSTEM_RESET = 0X1B67; //7015
        //Scale Commands //

        //Wave file Buffer Size (Default File Size is 10KB)//
        const int WAV_FILE_MAX_SIZE = 10240;

        //****** END OF CORESCANNER PROTOCOL *********//
        /**************************************************/

        public enum CodeTypes
        {
            ///<summary>An unknown, with respect to this enumeration, code type</summary>
            Unknown = 0,
            ///<summary>Code 39 symbology</summary>
            Code39 = 1,
            ///<summary>Codabar symbology</summary>
            Codabar = 2,
            ///<summary>Code 128 symbology</summary>
            Code128 = 3,
            ///<summary>Dinscrete 2 of 5 symbology</summary>
            Discrete2of5 = 4,
            ///<summary>IATA symbology</summary>
            Iata = 5,
            ///<summary>Interleaved 2 of 5 symbology</summary>
            Interleaved2of5 = 6,
            ///<summary>Code 93 symbology</summary>
            Code93 = 7,
            ///<summary>UPC-A symbology</summary>
            UpcA = 8,
            ///<summary>UPC-E0 symbology</summary>
            UpcE0 = 9,
            ///<summary>EAN-8 symbology</summary>
            Ean8 = 10,
            ///<summary>EAN-13 symbology</summary>
            Ean13 = 11,
            ///<summary>Code 11symbology</summary>
            Code11 = 12,
            ///<summary>Code 49symbology</summary>
            Code49 = 13,
            ///<summary>MSI Plessey symbology</summary>
            Msi = 14,
            ///<summary>EAN-128 symbology</summary>
            Ean128 = 15,
            ///<summary>UPC-E1 symbology</summary>
            UpcE1 = 16,
            ///<summary>PDF-417 symbology</summary>
            Pdf417 = 17,
            ///<summary>Code 16K symbology</summary>
            Code16K = 18,
            ///<summary>Code 39 symbology, with full-ASCII expansion applied</summary>
            Code39FullAscii = 19,
            ///<summary>UPC-D symbology</summary>
            UpcD = 20,
            ///<summary>symbology</summary>
            Code39Trioptic = 21,
            ///<summary>symbology</summary>
            Bookland = 22,
            ///<summary>symbology</summary>
            CouponCode = 23,
            ///<summary>symbology</summary>
            Nw7 = 24,
            ///<summary>symbology</summary>
            Isbt128 = 25,
            ///<summary>symbology</summary>
            Micro_Pdf = 26,
            ///<summary>symbology</summary>
            DataMatrix = 27,
            ///<summary>symbology</summary>
            QrCode = 28,
            ///<summary>symbology</summary>
            MicroPdfCca = 29,
            ///<summary>symbology</summary>
            PostNetUS = 30,
            ///<summary>symbology</summary>
            PlanetCode = 31,
            ///<summary>symbology</summary>
            Code32 = 32,
            ///<summary>symbology</summary>
            Isbt128Con = 33,
            ///<summary>symbology</summary>
            JapanPostal = 34,
            ///<summary>symbology</summary>
            AustralianPostal = 35,
            ///<summary>symbology</summary>
            DutchPostal = 36,
            ///<summary>symbology</summary>
            MaxiCode = 37,
            ///<summary>symbology</summary>
            CanadianPostal = 38,
            ///<summary>symbology</summary>
            UkPostal = 39,
            ///<summary>symbology</summary>
            MacroPdf = 40,
            ///<summary>symbology</summary>
            Aztec = 45,
            ///<summary>symbology</summary>
            Rss14 = 48,
            ///<summary>symbology</summary>
            RssLimited = 49,
            ///<summary>symbology</summary>
            RssExpanded = 50,
            ///<summary>symbology</summary>
            Scanlet = 55,
            ///<summary>symbology</summary>
            UpcAPlus2 = 72,
            ///<summary>symbology</summary>
            UpcE0Plus2 = 73,
            ///<summary>symbology</summary>
            Ean8Plus2 = 74,
            ///<summary>symbology</summary>
            Ean13Plus2 = 75,
            ///<summary>symbology</summary>
            UpcE1Plus2 = 80,
            ///<summary>symbology</summary>
            CcaEAN128 = 81,
            ///<summary>symbology</summary>
            CcaEAN13 = 82,
            ///<summary>symbology</summary>
            CcaEAN8 = 83,
            ///<summary>symbology</summary>
            CcaRssExpanded = 84,
            ///<summary>symbology</summary>
            CcaRssLimited = 85,
            ///<summary>symbology</summary>
            CcaRss14 = 86,
            ///<summary>symbology</summary>
            CcaUpcA = 87,
            ///<summary>symbology</summary>
            CcaUpcE = 88,
            ///<summary>symbology</summary>
            CccEAN128 = 89,
            ///<summary>symbology</summary>
            Tlc39 = 90,
            ///<summary>symbology</summary>
            CcbEan128 = 97,
            ///<summary>symbology</summary>
            CcbEan13 = 98,
            ///<summary>symbology</summary>
            CcbEan8 = 99,
            ///<summary>symbology</summary>
            CcbRssExpanded = 100,
            ///<summary>symbology</summary>
            CcbRssLimited = 101,
            ///<summary>symbology</summary>
            CcbRss14 = 102,
            ///<summary>symbology</summary>
            CcbUpcA = 103,
            ///<summary>symbology</summary>
            CcbUpcE = 104,
            ///<summary>symbology</summary>
            SignatureCapture = 105,
            ///<summary>symbology</summary>
            Matrix2of5 = 113,
            ///<summary>symbology</summary>
            Chinese2of5 = 114,
            ///<summary>symbology</summary>
            UpcAPlus5 = 136,
            ///<summary>symbology</summary>
            UpcE0Plus5 = 137,
            ///<summary>symbology</summary>
            Ean8Plus5 = 138,
            ///<summary>symbology</summary>
            Ean13Plus5 = 139,
            ///<summary>symbology</summary>
            UpcE1Plus5 = 144,
            ///<summary>symbology</summary>
            MacroMicroPDF = 154,
            ///<summary>symbology</summary>
            MailMark = 195,
            ///<summary>symbology</summary>
            DotCode = 196,
            ///<summary>symbology</summary>
            UDICode = 204,
            ///<summary>The no-decode symbol</summary>
            NoDecode = 255,
        }
        public enum RSMDataTypes
        {
            /// <summary>
            /// Byte – unsigned char
            /// </summary>
            B,
            /// <summary>
            /// Char – signed byte
            /// </summary>
            C,
            /// <summary>
            /// Bit Flags
            /// </summary>
            F,
            /// <summary>
            /// WORD – short unsigned integer (16 bits)
            /// </summary>
            W,
            /// <summary>
            /// SWORD – short signed integer (16 bits)
            /// </summary>
            I,
            /// <summary>
            /// DWORD – long unsigned integer (32 bits)
            /// </summary>
            D,
            /// <summary>
            /// SDWORD – long signed integer (32 bits)
            /// </summary>
            L,
            /// <summary>
            /// Array
            /// </summary>
            A,
            /// <summary>
            /// String
            /// </summary>
            S,
            /// <summary>
            /// Action
            /// </summary>
            X
        }
    }
}

