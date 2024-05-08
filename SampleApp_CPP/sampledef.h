/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#include <oleauto.h>
#pragma once

//---- Scanner types ------//
#define SCANNER_TYPES_ALL       1
#define SCANNER_TYPES_SNAPI     2
#define SCANNER_TYPES_SSI       3
#define SCANNER_TYPES_IBMHID    6
#define SCANNER_TYPES_NIXMODB   7
#define SCANNER_TYPES_HIDKB	    8
#define SCANNER_TYPES_IBMTT     9
#define SCANNER_TYPES_IBM      10
#define SCANNER_TYPES_SSI_BT   11
#define SCANNER_TYPES_OPOS     13
#define SCANNER_TYPES_SSI_IP   15


//---- Total number of scanner types ----//
#define TOTAL_SCANNER_TYPES         (SCANNER_TYPES_OPOS)

//--------- Command opcodes --------//
typedef enum 
{
    GET_VERSION                         = 0x3E8,    //1000
    REGISTER_FOR_EVENTS                 = 0x3E9,    //1001
    UNREGISTER_FOR_EVENTS               = 0x3EA,    //1002
   
    CLAIM_DEVICE                        = 0x5DC,    //1500
    RELEASE_DEVICE                      = 0x5DD,    //1501
    
    DEVICE_ABORT_MACROPDF               = 0x7D0,    //2000
    DEVICE_ABORT_UPDATE_FIRMWARE        = 0x7D1,    //2001
    DEVICE_AIM_OFF                      = 0x7D2,    //2002
    DEVICE_AIM_ON                       = 0x7D3,    //2003
    DEVICE_ENTER_LOW_POWER_MODE         = 0x7D4,    //2004
    DEVICE_FLUSH_MACROPDF               = 0x7D5,    //2005
    DEVICE_GET_PARAMETERS               = 0x7D7,    //2007
    DEVICE_GET_SCANNER_CAPABILITIES     = 0x7D8,    //2008
    DEVICE_LED_OFF                      = 0x7D9,    //2009
    DEVICE_LED_ON                       = 0x7DA,    //2010
    DEVICE_PULL_TRIGGER                 = 0x7DB,    //2011
    DEVICE_RELEASE_TRIGGER              = 0x7DC,    //2012
    DEVICE_SCAN_DISABLE                 = 0x7DD,    //2013
    DEVICE_SCAN_ENABLE                  = 0x7DE,    //2014
    DEVICE_SET_PARAMETER_DEFAULTS       = 0x7DF,    //2015
    DEVICE_SET_PARAMETERS               = 0x7E0,    //2016
    DEVICE_SET_PARAMETER_PERSISTANCE    = 0x7E1,    //2017
    DEVICE_BEEP_CONTROL                 = 0x7E2,    //2018
    REBOOT_SCANNER                      = 0x7E3,    //2019
    DISCONNECT_BT_SCANNER               = 0x7E7,    //2023
    DEVICE_CAPTURE_IMAGE                = 0xBB8,    //3000
    DEVICE_ABORT_IMAGE_XFER             = 0xBB9,    //3001
    DEVICE_CAPTURE_BARCODE              = 0xDAC,    //3500
    DEVICE_CAPTURE_VIDEO                = 0xFA0,    //4000
    
    GET_PAIRING_BARCODE                 = 0x3ED,    //1005
    
    RSM_ATTR_GETALL                     = 0x1388,   //5000
    RSM_ATTR_GET                        = 0x1389,   //5001
    RSM_ATTR_GETNEXT                    = 0x138A,   //5002
    RSM_ATTR_GETOFFSET                  = 0x138B,   //5003
    RSM_ATTR_SET                        = 0x138C,   //5004
    RSM_ATTR_STORE                      = 0x138D,   //5005
    GET_DEVICE_TOPOLOGY                 = 0x138E,   //5006
    REFRESH_TOPOLOGY                    = 0x138F,   //5007
    UPDATE_DECODE_TONE                  = 0x13BA,   //5050
    ERASE_DECODE_TONE                   = 0x13BB,   //5051
    UPDATE_ELECTRIC_FENCE_CUSTOM_TONE   = 0x13BC,   //5052
    ERASE_ELECTRIC_FENCE_CUSTOM_TONE    = 0x13BD,   //5053
    
    START_NEW_FIRMWARE                  = 0x1396,   //5014
    UPDATE_ATTRIB_META_FILE             = 0x1397,   //5015
    DEVICE_UPDATE_FIRMWARE              = 0x1398,   //5016
    DEVICE_UPDATE_FIRMWARE_FROM_PLUGIN  = 0x1399,   //5017
    
   
    DEVICE_SET_SERIAL_PORT_SETTINGS     = 0x17D5,   //6101
    SET_ACTION                          = 0x1770,   //6000
    DEVICE_SWITCH_HOST_MODE             = 0x1838,   //6200
    SWITCH_CDC_DEVICES                  = 0x1839,   //6201
    PAGER_MOTOR_ACTION                  = 0x1791,   //6033
    
    //HID keyboard emulator opcodes
    KEYBOARD_EMULATOR_ENABLE            = 0x189C,   //6300
    KEYBOARD_EMULATOR_SET_LOCALE        = 0x189D,   //6301
    KEYBOARD_EMULATOR_GET_CONFIG        = 0x189E,   //6302
   
    //DRIVER ADF COMMANDS
    CONFIGURE_DADF                      = 0x1900,   //6400
    RESET_DADF                          = 0x1901,   //6401
    
    
    //Scale opcodes
    SCALE_READ_WEIGHT                   = 0x1b58,   //7000
    SCALE_ZERO_SCALE                    = 0X1B5A,   //7002
    SCALE_SYSTEM_RESET                  = 0X1B67,   //7015
    
    ERROR_OPCODE                        = -1,		
    
} CMD_OPCODE;

//---------- Beep Codes for SoundBeeper() function -----------//
#define ONESHORTHI      0x00
#define TWOSHORTHI      0x01
#define THREESHORTHI    0x02
#define FOURSHORTHI     0x03
#define FIVESHORTHI     0x04

#define ONESHORTLO      0x05
#define TWOSHORTLO      0x06
#define THREESHORTLO    0x07
#define FOURSHORTLO     0x08
#define FIVESHORTLO     0x09

#define ONELONGHI       0x0A
#define TWOLONGHI       0x0B
#define THREELONGHI     0x0C
#define FOURLONGHI      0x0D
#define FIVELONGHI      0x0E

#define ONELONGLO       0x0F
#define TWOLONGLO       0x10
#define THREELONGLO     0x11
#define FOURLONGLO      0x12
#define FIVELONGLO      0x13

#define FASTHILOHILO    0x14
#define SLOWHILOHILO    0x15
#define HILO            0x16
#define LOHI            0x17
#define HILOHI          0x18
#define LOHILO          0x19

//----- Firmware download events ------//
#define SCANNER_UF_SESS_START       11 // Triggered when flash download session starts 
#define SCANNER_UF_DL_START         12 // Triggered when component download starts 
#define SCANNER_UF_DL_PROGRESS      13 // Triggered when block(s) of flash completed 
#define SCANNER_UF_DL_END           14 // Triggered when component download ends 
#define SCANNER_UF_SESS_END         15 // Triggered when flash download session ends 
#define SCANNER_UF_STATUS           16 // Triggered when update error or status

//------- Scanner Notification Event Types ----//
#define BARCODE_MODE    1
#define IMAGE_MODE      2
#define VIDEO_MODE      3
#define DEVICE_ENABLED  13
#define DEVICE_DISABLED 14

//----- Symbology Types ---------------//
#define   ST_NOT_APP                0x00  
#define   ST_CODE_39                0x01  
#define   ST_CODABAR                0x02  
#define   ST_CODE_128               0x03  
#define   ST_D2OF5                  0x04  
#define   ST_IATA                   0x05  
#define   ST_I2OF5                  0x06  
#define   ST_CODE93                 0x07  
#define   ST_UPCA                   0x08  
#define   ST_UPCE0                  0x09  
#define   ST_EAN8                   0x0a  
#define   ST_EAN13                  0x0b  
#define   ST_CODE11                 0x0c  
#define   ST_CODE49                 0x0d  
#define   ST_MSI                    0x0e  
#define   ST_EAN128                 0x0f  
#define   ST_UPCE1                  0x10  
#define   ST_PDF417                 0x11  
#define   ST_CODE16K                0x12  
#define   ST_C39FULL                0x13  
#define   ST_UPCD                   0x14  
#define   ST_TRIOPTIC               0x15  
#define   ST_BOOKLAND               0x16  
#define   ST_UPCA_W_CODE128         0x17 // For Old Style Coupon Code
#define   ST_JAN13_W_CODE128        0x78 // For EAN/JAN-13 w/Code 128 Supplemental
#define   ST_NW7                    0x18  
#define   ST_ISBT128                0x19  
#define   ST_MICRO_PDF              0x1a  
#define   ST_DATAMATRIX             0x1b  
#define   ST_QR_CODE                0x1c  
#define   ST_MICRO_PDF_CCA          0x1d  
#define   ST_POSTNET_US             0x1e  
#define   ST_PLANET_CODE            0x1f  
#define   ST_CODE_32                0x20  
#define   ST_ISBT128_CON            0x21  
#define   ST_JAPAN_POSTAL           0x22  
#define   ST_AUS_POSTAL             0x23  
#define   ST_DUTCH_POSTAL           0x24  
#define   ST_MAXICODE               0x25  
#define   ST_CANADIN_POSTAL         0x26  
#define   ST_UK_POSTAL              0x27  
#define   ST_MACRO_PDF              0x28  
#define   ST_MACRO_QR_CODE          0x29  
#define   ST_MICRO_QR_CODE          0x2c  
#define   ST_AZTEC                  0x2d  
#define   ST_AZTEC_RUNE             0x2e  
#define   ST_DISTANCE               0x2f  
#define   ST_RSS14                  0x30  
#define   ST_RSS_LIMITET            0x31  
#define   ST_RSS_EXPANDED           0x32  
#define   ST_PARAMETER              0x33  
#define   ST_USPS_4CB               0x34  
#define   ST_UPU_FICS_POSTAL        0x35  
#define   ST_ISSN                   0x36  
#define   ST_SCANLET                0x37  
#define   ST_CUECODE                0x38  
#define   ST_MATRIX2OF5             0x39  
#define   ST_UPCA_2                 0x48  
#define   ST_UPCE0_2                0x49  
#define   ST_EAN8_2                 0x4a  
#define   ST_EAN13_2                0x4b  
#define   ST_UPCE1_2                0x50  
#define   ST_CCA_EAN128             0x51  
#define   ST_CCA_EAN13              0x52  
#define   ST_CCA_EAN8               0x53  
#define   ST_CCA_RSS_EXPANDED       0x54  
#define   ST_CCA_RSS_LIMITED        0x55  
#define   ST_CCA_RSS14              0x56  
#define   ST_CCA_UPCA               0x57  
#define   ST_CCA_UPCE               0x58  
#define   ST_CCC_EAN128             0x59  
#define   ST_TLC39                  0x5A  
#define   ST_CCB_EAN128             0x61  
#define   ST_CCB_EAN13              0x62  
#define   ST_CCB_EAN8               0x63  
#define   ST_CCB_RSS_EXPANDED       0x64  
#define   ST_CCB_RSS_LIMITED        0x65  
#define   ST_CCB_RSS14              0x66  
#define   ST_CCB_UPCA               0x67  
#define   ST_CCB_UPCE               0x68  
#define   ST_SIGNATURE_CAPTURE      0x69  
#define   ST_MOA                    0x6A  
#define   ST_PDF417_PARAMETER       0x70  
#define   ST_CHINESE2OF5            0x72  
#define   ST_KOREAN_3_OF_5          0x73  
#define   ST_DATAMATRIX_PARAM       0x74  
#define   ST_CODE_Z                 0x75  
#define   ST_UPCA_5                 0x88  
#define   ST_UPCE0_5                0x89  
#define   ST_EAN8_5                 0x8a  
#define   ST_EAN13_5                0x8b  
#define   ST_UPCE1_5                0x90  
#define   ST_MACRO_MICRO_PDF        0x9A  
#define   ST_OCRB                   0xA0  
#define   ST_OCRA                   0xA1  
#define   ST_PARSED_DRIVER_LICENSE  0xB1  
#define   ST_PARSED_UID             0xB2  
#define   ST_PARSED_NDC             0xB3  
#define   ST_DATABAR_COUPON         0xB4  
#define   ST_PARSED_XML             0xB6  
#define   ST_HAN_XIN_CODE           0xB7  
#define   ST_CALIBRATION            0xC0  
#define   ST_GS1_DATAMATRIX         0xC1  
#define   ST_GS1_QR                 0xC2
#define	  BT_MAINMARK               0xC3
#define   BT_DOTCODE                0xC4
#define	  BT_GRID_MATRIX            0xC8
#define   BT_UDI_CODE               0xCC
#define   STATUS_SUCCESS            0


//Language definition Enum - Add new Languages here - VRQW74
#ifndef HID_PUMP_LANGUAGE_CODES
#define HID_PUMP_LANGUAGE_CODES
 enum LANGUAGE_CODES
{
    STARTCODE = -1,
    DEFAULT = 0,
    FRENCH  = 1,
    ENGLISH = 2,
    ENDCODE = ENGLISH + 1 //Allways one more than the last lang entry
};
#endif